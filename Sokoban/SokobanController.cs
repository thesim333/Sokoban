using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Filer;
using GameNS;
using Designer;
using System.Windows.Forms;
using System.Windows;

namespace Sokoban
{
    public class SokobanController
    {
        protected IGame Game;
        protected IView View;
        protected IDesign Designer;
        protected IFiler Filer;
        protected IFileManager FM;
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

        public SokobanController(IGame game, IView view, IDesign design, IFiler filer, IFileManager fm)
        {
            Game = game;
            View = view;
            Designer = design;
            Filer = filer;
            FM = fm;
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
                string player = GetPlayerName();
                string fileName = Game.GetName();
                string file = Filer.Load(fileName);
                int thisScore = Game.GetMoveCount();
                Filer.ReplaceFile(fileName, FM.GetFileWithNewStatInserted(fileName, player, thisScore));
                string[] bestScore = FM.GetBestStat(file).Split('-');
                View.FinishGame(bestScore[0], bestScore[1], thisScore);
            }
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
                string file = Filer.Load(fileName);
                string level = FM.GetLevel(file);
                switch (which)
                {
                    case GAME_STRING:
                        PlayLevel(fileName, level);
                        break;
                    case DESIGN_STRING:
                        OpenDesignerLoad(fileName, level);
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
            string file = Filer.Load(Game.GetName());
            string[] states = FM.GetStatesSaved(file);
            string theState;

            if (states.Length > 0)
            {
                theState = GetStateToLoad(file, states);

                if (theState.Length > 0)
                {
                    Game.LoadState(theState);
                    CreateGameView();
                }
            }
        }

        protected string GetStateToLoad(string file, string[] states)
        {
            LoadFileFromListDialog levDia = new LoadFileFromListDialog();
            levDia.SetText("Stats for " + Game.GetName());
            levDia.InsertLevels(states);
            string toReturn = string.Empty;

            if (levDia.ShowDialog() == DialogResult.OK)
            {
                toReturn = FM.GetState(file, levDia.GetSelected());
            }

            levDia.Dispose();
            return toReturn;
        }

        public void SaveGameState()
        {
            FileSaveNameDialog sn = new FileSaveNameDialog();
            sn.SetLabel("Name for state:");
            
            if (sn.ShowDialog() == DialogResult.OK && sn.GetName().Length > 0)
            {
                string file = Filer.Load(Game.GetName());
                string name = sn.GetName();
                string state = Game.SaveState();
                if (FM.StateExists(file, name) &&
                    MessageBox.Show("State Exists", "OK to overwrite?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Filer.ReplaceFile(name, FM.OverwriteSavedState(file, name, state));
                }
                else if (!FM.StateExists(file, name))
                {
                    Filer.AppendState(file, name, state);
                }
            }
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

        private bool CanExitDesignerWithUnsavedChanges()
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

        private bool SaveLevelInDesigner()
        {
            bool returnMe = false;
            FileSaveNameDialog sn = new FileSaveNameDialog();
            sn.SetName(Designer.GetName());

            while (true)
            {
                if (sn.ShowDialog() == DialogResult.OK) //if file doesn't exist or it's ok to overwrite
                {
                    string toFileName = sn.GetName();

                    if (toFileName.Length > 0 &&
                       (!Filer.LevelExists(toFileName) ||
                        MessageBox.Show("Overwite Level?",
                        "Level file already exists.",
                        MessageBoxButtons.YesNo) == DialogResult.Yes))
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
    }
}
