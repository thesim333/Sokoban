using GameGlobals;

namespace Designer
{
    public class Design : IDesign
    {
        protected Parts[,] LevelGrid;
        protected string Name;
        protected bool UnSavedChanges = false;

        /// <summary>
        /// Gets the name of the level from when this level is loaded or saved.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return Name;
        }

        /// <summary>
        /// Sets the name of the level.
        /// </summary>
        /// <param name="name">The name.</param>
        public void SetName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Used after the level is saved to register that there are no unsaved changes.
        /// </summary>
        public void SetSaved()
        {
            UnSavedChanges = false;
        }

        /// <summary>
        /// Determines whether the level has unsaved changes.
        /// Called when checking for save.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has unsaved changes]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasUnsavedChanges()
        {
            return UnSavedChanges;
        }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <returns>The length of the second array dimension</returns>
        public int GetColumnCount()
        {
            return LevelGrid.GetLength(1);
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        /// <returns>The length of the first array dimension.</returns>
        public int GetRowCount()
        {
            return LevelGrid.GetLength(0);
        }

        /// <summary>
        /// Creates the level grid
        /// Sets no unsaved changes for this newly loaded level.
        /// </summary>
        /// <param name="name">The level name.</param>
        /// <param name="level">The contents of the level grid as a string.</param>
        public void LoadLevel(string name, string level)
        {
            Name = name;
            string[] rowStrings = level.Split(',');
            int rows = rowStrings.Length;
            int cols = rowStrings[0].Length;
            LevelGrid = new Parts[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    LevelGrid[r, c] = (Parts)rowStrings[r][c];
                }
            }

            UnSavedChanges = false;
        }

        /// <summary>
        /// Creates a new level grid of empty squares.
        /// Sets no unsaved changes for this new empty level.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="cols">The columns.</param>
        public void NewLevel(int rows, int cols)
        {
            LevelGrid = GetNew2DArray(rows, cols, Parts.Empty);
            Name = string.Empty;
            UnSavedChanges = false;
        }

        /// <summary>
        /// Set the part stored in the level grid {row, column} position.
        /// Unsaved changes set to true by this.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <param name="part">The part.</param>
        public void SetPart(int row, int col, Parts part)
        {
            LevelGrid[row, col] = part;
            UnSavedChanges = true;
        }

        /// <summary>
        /// Get the part at {row, col}
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>The part</returns>
        public Parts WhatsAt(int row, int column)
        {
            return LevelGrid[row, column];
        }

        /// <summary>
        /// Gets the new 2d array of size {x, y} with initial value.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="initialValue">The initial value.</param>
        /// <returns>The array</returns>
        protected Parts[,] GetNew2DArray(int x, int y, Parts initialValue)
        {
            Parts[,] parts = new Parts[x, y];
            for (int i = 0; i < x * y; i++) parts[i % x, i / x] = initialValue;
            return parts;
        }

        /// <summary>
        /// Determines whether the level has one player.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has one player]; otherwise, <c>false</c> for 0 or more than 1.
        /// </returns>
        public bool HasOnePlayer()
        {
            int players = 0;

            for (int r = 0; r < LevelGrid.GetLength(0); r++)
            {
                for (int c = 0; c < LevelGrid.GetLength(1); c++)
                {
                    if (LevelGrid[r, c] == Parts.Player
                        || LevelGrid[r, c] == Parts.PlayerOnGoal)
                    {
                        players++;
                    }
                }
            }
            return (players == 1);
        }

        /// <summary>
        /// Determines if the number of blocks is equal to the number of targets.
        /// </summary>
        /// <returns><c>true</c> if ==; otherwise, <c>false</c></returns>
        public bool BlocksEqualTargets()
        {
            int blocks = 0;
            int targets = 0;

            for (int r = 0; r < LevelGrid.GetLength(0); r++)
            {
                for (int c = 0; c < LevelGrid.GetLength(1); c++)
                {
                    if (LevelGrid[r, c] == Parts.Block
                        || LevelGrid[r, c] == Parts.BlockOnGoal)
                    {
                        blocks++;
                    }
                    else if (LevelGrid[r, c] == Parts.Goal
                        || LevelGrid[r, c] == Parts.PlayerOnGoal
                        || LevelGrid[r, c] == Parts.BlockOnGoal)
                    {
                        targets++;
                    }
                }
            }
            return (blocks == targets);
        }
    }
}
