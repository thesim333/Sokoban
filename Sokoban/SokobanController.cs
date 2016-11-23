using System.Text;
using Filer;
using GameNS;
using Designer;
using System.Windows.Forms;
using GameGlobals;

namespace Sokoban
{
    public class SokobanController
    {
        protected IGame Game;
        protected IView View;
        protected IDesign Designer;
        protected IFiler Filer;
        protected IChecker Check;
        private const string GAME_STRING = "Game";
        private const string DESIGN_STRING = "Design";

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

        /// <summary>
        /// Initializes a new instance of the <see cref="SokobanController"/> class.
        /// </summary>
        /// <param name="game">The game object.</param>
        /// <param name="view">The view object.</param>
        /// <param name="design">The designer object.</param>
        /// <param name="filer">The filer object.</param>
        /// <param name="check">The checker object.</param>
        public SokobanController(IGame game, IView view, IDesign design, IFiler filer, IChecker check)
        {
            Game = game;
            View = view;
            Designer = design;
            Filer = filer;
            Check = check;
        }

        /// <summary>
        /// Moves the player in the game object in the specified direction.
        /// </summary>
        /// <param name="d">The direction. {UP, Down, Left, Right}</param>
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

        /// <summary>
        /// Checks the designer level for reasons the level might not be legal.
        /// Displays a message box explaining the outcomes.
        /// One player check must pass for the player not reaching edge check to not crash the program.
        /// </summary>
        public void CheckDesignerLevel()
        {
            string players;
            string blocks = (Designer.BlocksEqualTargets()) ? "\u221A" : "X";
            string edge;

            if (Designer.HasOnePlayer())
            {
                players = "\u221A";
                edge = (CheckLevelPlayerEdge()) ? "\u221A" : "X";
            }
            else
            {
                players = "X";
                edge = "?";
            }

            MessageBox.Show("Correct amount of players: " + players + "\n" +
                "Blocks and Targets are equal: " + blocks + "\n" +
                "Player cannot reach the edge: " + edge);
        }

        /// <summary>
        /// Checks Player cannot reach the edge of the level.
        /// The player must be ringed by walls
        /// </summary>
        /// <returns><c>true</c> if the player cannot reach the edge; otherwise <c>false</c></returns>
        protected bool CheckLevelPlayerEdge()
        {
            Check.InsertDesigner(Designer);
            return Check.PlayerCannotReachEdge();
        }

        /// <summary>
        /// Finishes the game.
        /// Inserts the score into the stats for the level.
        /// Changes the view so the game cannot be continued.
        /// Displays Scores.
        /// </summary>
        protected void FinishGame()
        {
            string player = GetPlayerName();
            string fileName = Game.GetName();
            int thisScore = Game.GetMoveCount();
            Filer.AppendStat(fileName, player, thisScore);
            View.FinishGame(Game.GetMoveCount());
            DisplayBestScores();
        }

        /// <summary>
        /// Displays the best scores of the level just completed.
        /// </summary>
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

        /// <summary>
        /// Gets the name of the player from user input.
        /// </summary>
        /// <returns>The name</returns>
        protected string GetPlayerName()
        {
            FileSaveNameDialog sn = new FileSaveNameDialog();
            sn.SetLabel("Your name:");
            sn.ShowDialog();
            return sn.GetName();
        }

        /// <summary>
        /// Plays the level.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="level">The level grid as string.</param>
        protected void PlayLevel(string fileName, string level)
        {
            Game.Load(fileName, level);
            NewGame();
        }

        /// <summary>
        /// Loads the level.
        /// </summary>
        /// <param name="which">Which object to load the level for Designer or Game</param>
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

        /// <summary>
        /// Plays a new game of the same level
        /// </summary>
        public void NewGame()
        {
            Game.Restart();
            CreateGameView();
        }

        /// <summary>
        /// Creates the game view from a game that has been loaded and started.
        /// </summary>
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

        /// <summary>
        /// Loads a game state from a saved state.
        /// </summary>
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

        /// <summary>
        /// Undo the last move.
        /// </summary>
        public void Undo()
        {
            Position[] thePositions = Game.Undo();
            View.SetMoves(Game.GetMoveCount());

            foreach (Position p in thePositions)
            {
                View.SetGamePosition(p.Row, p.Column, Game.GetPartAt(p.Row, p.Column));
            }
        }

        /// <summary>
        /// Opens the designer, loads the level to edit.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="level">The level grid as string.</param>
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

        /// <summary>
        /// Opens the designer with a new level.
        /// </summary>
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

        /// <summary>
        /// Closes the designer.
        /// Checks for unsaved changes and asks to save if there are.
        /// View changes to main screen.
        /// </summary>
        public void CloseDesigner()
        {
            if (Designer.HasUnsavedChanges() == false ||
                CanExitDesignerWithUnsavedChanges())
            {
                Designer.SetSaved();
                View.DisplayMain();
            }
        }

        /// <summary>
        /// Closes the game back to main screen.
        /// </summary>
        public void CloseGame()
        {
            View.DisplayMain();
        }

        /// <summary>
        /// Designer: part set in view store this change.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <param name="part">The part.</param>
        public void DesignerSetPartAt(int row, int col, Parts part)
        {
            Designer.SetPart(row, col, part);
        }

        /// <summary>
        /// Saves the level in designer.
        /// </summary>
        public void SaveLevelDesigner()
        {
            if (SaveLevelInDesigner())
            {
                Designer.SetSaved();
            }
        }

        /// <summary>
        /// Determines whether the designer can be closed back to the main menu.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance has no unsaved changes or the user wishes to close and discard changes; otherwise, if the user does not wish to save and close, <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Checks if the level is legal.
        /// If legal, gets the name from the user and attempts to save.
        /// </summary>
        /// <returns></returns>
        protected bool SaveLevelInDesigner()
        {
            if (!Designer.HasOnePlayer())
            {
                MessageBox.Show("Level must have one player");
                return false;
            }

            if (!Designer.BlocksEqualTargets())
            {
                MessageBox.Show("Blocks and Targets must be equal");
                return false;
            }

            if (!CheckLevelPlayerEdge())
            {
                MessageBox.Show("Player must not be able to reach the edge");
                return false;
            }

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
                        Designer.SetSaved();
                        Designer.SetName(toFileName);
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

        /// <summary>
        /// Attempts to save the level as [fileName]. If the level exists, ask to overwrite.
        /// </summary>
        /// <param name="fileName">Name to save the file as.</param>
        /// <returns>
        ///   <c>true</c> If the level saves; otherwise, if the user does not wish to overwrite an existing level, <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Saves the state of the game.
        /// </summary>
        public void SaveGameState()
        {
            FileSaveNameDialog sn = new FileSaveNameDialog();
            sn.SetLabel("Name for state:");

            while (true)
            {
                if (sn.ShowDialog() == DialogResult.OK)
                {
                    if (DidSaveState(sn.GetName()))
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

        /// <summary>
        /// Attempts to save the state in the level file as [stateName].
        /// If the state exists, ask to overwrite.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <returns>
        ///   <c>true</c> If the state saves; otherwise, if the user does not wish to overwrite an existing state, <c>false</c>
        /// </returns>
        protected bool DidSaveState(string stateName)
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
