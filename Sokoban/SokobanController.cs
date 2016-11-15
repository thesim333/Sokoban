using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Filer;
using GameNS;
using Designer;
using System.Windows.Forms;

namespace Sokoban
{
    public class SokobanController
    {
        protected IGame Game;
        protected IView View;
        protected IDesign Designer;
        protected IFiler Filer;
        protected const string GAME_STRING = "Game";
        protected const string DESIGN_STRING = "Design";

        public string Game_ST
        {
            get
            {
                return GAME_STRING;
            }
        }

        public string Design_ST
        {
            get
            {
                return DESIGN_STRING;
            }
        }

        public SokobanController(IGame game, IView view, IDesign design, IFiler filer)
        {
            Game = game;
            View = view;
            Designer = design;
            Filer = filer;
        }

        public void Move(Direction d)
        {
            Position[] movePositions = Game.Move(d);
            View.SetMoves(Game.GetMoveCount());
            
            foreach (Position p in movePositions)
            {
                View.SetGamePosition(p.Row, p.Column, Game.GetPartAt(p.Row, p.Column));
            }

            if (Game.IsFinished())
            {
                FinishGame();
            }
        }

        protected void FinishGame()
        {
            string player = GetPlayerName();
            string fileName = Game.GetName();
            int thisScore = Game.GetMoveCount();
            Filer.AppendStat(fileName, player, thisScore);
            View.FinishGame();
            DisplayBestScores();
        }

        protected void DisplayBestScores()
        {
            Stat[] scores = Filer.GetBestX_Stats(Game.GetName(), 10);
            StringBuilder display = new StringBuilder();

            for (int i = 0; i < scores.Length; i++)
            {
                display.Append(i);
                display.Append("\t");
                display.Append(scores[i].Name);
                display.Append("\t");
                display.Append(scores[i].Moves);
                display.Append("\n");
            }

            MessageBox.Show(display.ToString(), "Best Scores");
        }

        protected string GetPlayerName()
        {
            FileSaveNameDialog sn = new FileSaveNameDialog();
            sn.SetLabel("Your name:");
            sn.ShowDialog();
            return sn.GetName();
        }

        /**
         * File name from user select from list
         * Get that file from Filer
         * Get Level string from FileManager
         * Send name and level string to game
         * Start new game
         */
        protected void PlayLevel(string fileName, string level)
        {
            Game.Load(fileName, level);
            NewGame();
        }

        /**
         * Gets filename from dialog
         * Get that file from Filer
         * Call other Load(fileName)
         */
        public void LoadLevel(string which)
        {
            LoadFileFromListDialog levDia = new LoadFileFromListDialog();
            levDia.InsertLevels(Filer.GetAllLevels());

            if (levDia.ShowDialog() == DialogResult.OK)
            {
                string fileName = levDia.GetSelected();
                string grid = Filer.LoadGrid(fileName);
                switch (which)
                {
                    case GAME_STRING:
                        PlayLevel(fileName, grid);
                        break;
                    case DESIGN_STRING:
                        OpenDesignerLoad(fileName, grid);
                        break;
                }
            }
            levDia.Dispose();
        }

        public void NewGame()
        {
            Game.Restart();
            CreateGameView();
        }

        protected void CreateGameView()
        {
            View.GameSetup(Game.GetMoveCount());
            int rows = Game.GetRows();
            int cols = Game.GetCols();
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    View.SetGamePosition(r, c, Game.GetPartAt(r, c));
                }
            }
        }

        public void LoadGameState()
        {
            string fileName = Game.GetName();
            string[] states = Filer.GetAllStates(fileName);
            LoadFileFromListDialog levDia = new LoadFileFromListDialog();
            levDia.SetText("Stats for " + Game.GetName());
            levDia.InsertLevels(states);

            if (states.Length > 0 && levDia.ShowDialog() == DialogResult.OK)
            {
                Game.LoadState(Filer.LoadState(fileName, levDia.GetSelected()));
                CreateGameView();
            }
        }

        public void Undo()
        {
            Position[] thePositions = Game.Undo();
            View.SetMoves(Game.GetMoveCount());

            foreach (Position p in thePositions)
            {
                View.SetGamePosition(p.Row, p.Column, Game.GetPartAt(p.Row, p.Column));
            }
            View.ReestablishKeys();
        }

        protected void OpenDesignerLoad(string fileName, string level)
        {
            Designer.LoadLevel(fileName, level);
            int rows = Designer.GetRowCount();
            int cols = Designer.GetColumnCount();
            View.DesignerLoadLevel();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    View.CreateLevelGridButton(r, c, Designer.WhatsAt(r, c));
                }
            }
        }

        public void OpenDesignerNew()
        {
            SizeDialog sd = new SizeDialog();

            if (sd.ShowDialog() == DialogResult.OK)
            {
                Designer.NewLevel(sd.GetRows(), sd.GetCols());
                View.DesignerNewLevel(sd.GetRows(), sd.GetCols());
            }

            sd.Dispose();
        }

        public void CloseDesigner()
        {
            if (Designer.HasUnsavedChanges() == false ||
                CanExitDesignerWithUnsavedChanges())
            {
                Designer.SetSaved();
                View.DisplayMain();
            }
        }

        public void CloseGame()
        {
            View.DisplayMain();
        }

        public void DesignerSetPartAt(int row, int col, Parts part)
        {
            Designer.SetPart(row, col, part);
        }

        /**
         * Gets the name of the file to save
         * If the file does not exist - save
         * If exists question overwite Y Save N return unsaved
         */
        public void SaveLevelDesigner()
        {
            if (SaveLevelInDesigner())
            {
                Designer.SetSaved();
            }
        }

        protected bool CanExitDesignerWithUnsavedChanges()
        {
            DialogResult result = MessageBox.Show("Do you wish to save before closing?", "Level has unsaved changes", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                return SaveLevelInDesigner();
            }
            else if (result == DialogResult.Cancel)
            {
                return false;
            }

            return true;
        }

        protected bool SaveLevelInDesigner()
        {
            bool returnMe = false;
            FileSaveNameDialog sn = new FileSaveNameDialog();
            sn.SetName(Designer.GetName());

            while (true)
            {
                if (sn.ShowDialog() == DialogResult.OK) 
                {
                    string toFileName = sn.GetName();
                    //if file doesn't exist or it's ok to overwrite
                    if (CanSaveLevel(toFileName))  
                    {
                        Filer.Save(toFileName, Designer);
                        returnMe = true;
                        MessageBox.Show("Level " + sn.GetName() + " saved successfully.");
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            sn.Dispose();
            return returnMe;
        }

        protected bool CanSaveLevel(string fileName)
        {
            if (!Filer.LevelExists(fileName))
            {
                return true;
            }
            else if (MessageBox.Show("Overwite Level?", "Level file already exists.", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public void SaveGameState()
        {
            FileSaveNameDialog sn = new FileSaveNameDialog();
            sn.SetLabel("Name for state:");

            while (true)
            {
                if (sn.ShowDialog() == DialogResult.OK)
                {
                    if (DidSave(sn.GetName()))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        protected bool DidSave(string stateName)
        {
            State state = Game.MakeState();
            string fileName = Game.GetName();

            if (Filer.StateExists(fileName, stateName))
            {
                if (MessageBox.Show("This state exists, ok to replace?", "Replace?", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return false;
                }
                else
                {
                    Filer.ReplaceState(fileName, stateName, state);
                }
            }
            else
            {
                Filer.AppendState(fileName, stateName, state);
            }

            return true;
        }
    }
}
