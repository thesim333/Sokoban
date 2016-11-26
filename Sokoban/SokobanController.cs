using System.Text;
using Filer;
using GameNS;
using Designer;
using GameGlobals;

namespace Sokoban
{
    public class SokobanController
    {
        protected IGame Game;
        protected IView View;
        protected IDesign Designer;
        protected IFiler Filer;
        protected IGridChecker GridCheck;
        protected IFileChecker FileCheck;
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
        public SokobanController(IGame game, IView view, IDesign design, IFiler filer, IGridChecker gridCheck, IFileChecker fileCheck)
        {
            Game = game;
            View = view;
            Designer = design;
            Filer = filer;
            GridCheck = gridCheck;
            FileCheck = fileCheck;
            Filer.InsertApplicationPath(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"levels\"));
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

            View.Display("Correct amount of players: " + players + "\n" +
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
            GridCheck.InsertDesigner(Designer);
            return GridCheck.PlayerCannotReachEdge();
        }

        /// <summary>
        /// Finishes the game.
        /// Inserts the score into the stats for the level.
        /// Changes the view so the game cannot be continued.
        /// Displays Scores.
        /// </summary>
        protected void FinishGame()
        {
            string player = View.GetInput("Player Name:");
            int thisScore = Game.GetMoveCount();
            Filer.AppendStat(player, thisScore);
            View.FinishGame(Game.GetMoveCount());
            DisplayBestScores();
        }

        /// <summary>
        /// Displays the best scores of the level just completed.
        /// </summary>
        protected void DisplayBestScores()
        {
            Stat[] scores = Filer.GetBestX_Stats(10);
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

            View.Display(display.ToString());
        }
        
        /// <summary>
        /// Plays the level.
        /// </summary>
        /// <param name="level">The level grid as string.</param>
        protected void PlayLevel(string level)
        {
            Game.Load(level);
            NewGame();
        }

        /// <summary>
        /// Loads the level.
        /// </summary>
        /// <param name="which">Which object to load the level for Designer or Game</param>
        public void LoadLevel(string which)
        {
            string filePath = View.GetFileToLoad(Filer.GetCurrentPath());
            if (FileCheck.FileChecksOut(filePath))
            {
                string grid = Filer.LoadGrid(filePath);

                switch (which)
                {
                    case GAME_STRING:
                        PlayLevel(grid);
                        break;
                    case DESIGN_STRING:
                        OpenDesignerLoad(grid);
                        break;
                }
            }
            else
            {
                View.Display("There is a problem with that file.");
            }
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
            View.GameSetup(Game.GetMoveCount(), Game.GetCols(), Game.GetRows());
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
            string[] states = Filer.GetAllStates();
            string theState = View.GetSelectedState(states);            

            if (theState.Length > 0)
            {
                Game.LoadState(Filer.LoadState(theState));
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
        protected void OpenDesignerLoad(string level)
        {
            Designer.LoadLevel(level);
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
            int[] myRowCol = View.GetLevelSize();
            Designer.NewLevel(myRowCol[0], myRowCol[1]);
            View.DesignerNewLevel(myRowCol[0], myRowCol[1]);
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
        /// If the level can't be saved displays warning message.
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
            string result = View.GetUserResponse("Do you wish to save before closing?", "Level has unsaved changes");
            if (result == "Y")
            {
                return SaveLevelInDesigner();
            }
            else if (result == "N")
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the level is legal.
        /// If legal, gets the name from the View and attempts to save.
        /// </summary>
        /// <returns></returns>
        protected bool SaveLevelInDesigner()
        {
            if (!Designer.HasOnePlayer())
            {
                View.Display("Level must have one player");
                return false;
            }

            if (!Designer.BlocksEqualTargets())
            {
                View.Display("Blocks and Targets must be equal");
                return false;
            }

            if (!CheckLevelPlayerEdge())
            {
                View.Display("Player must not be able to reach the edge");
                return false;
            }

            bool returnMe = false;
            string fileNamePath = View.SaveMyFile(Filer.GetCurrentPath());

            if (fileNamePath.Length > 0) 
            {
                string fileName = Filer.Save(fileNamePath, Designer);
                Designer.SetSaved();
                returnMe = true;
                View.Display("Level " + fileName + " saved successfully.");
            }
                
            return returnMe;
        }
        
        /// <summary>
        /// Saves the state of the game.
        /// </summary>
        public void SaveGameState()
        {
            while (true)
            {
                string state = View.GetInput("Name for State:");
                bool stateExists = Filer.StateExists(state);
                if (stateExists && View.GetUserResponse("OK to overwrite existing state: " + state, "State Exists") == "Y")
                {
                    Filer.ReplaceState(state, Game.MakeState());
                    View.Display("State " + state + " saved");
                    break;
                }
                else if (!stateExists)
                {
                    Filer.AppendState(state, Game.MakeState());
                    View.Display("State " + state + " saved");
                    break;
                }
            }
        }
    }
}
