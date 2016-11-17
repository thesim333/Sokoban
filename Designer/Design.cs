namespace Designer
{
    public class Design : IDesign
    {
        protected Parts[,] LevelGrid;
        protected string Name;
        protected bool UnSavedChanges = false;

        public string GetName()
        {
            return Name;
        }

        public void SetSaved()
        {
            UnSavedChanges = false;
        }

        public bool HasUnsavedChanges()
        {
            return UnSavedChanges;
        }

        public int GetColumnCount()
        {
            return LevelGrid.GetLength(1);
        }
        
        public int GetRowCount()
        {
            return LevelGrid.GetLength(0);
        }

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

        public void NewLevel(int rows, int cols)
        {
            LevelGrid = GetNew2DArray(rows, cols, Parts.Empty);
            Name = string.Empty;
            UnSavedChanges = false;
        }

        public void SetPart(int row, int col, Parts part)
        {
            LevelGrid[row, col] = part;
            UnSavedChanges = true;
        }

        public Parts WhatsAt(int row, int column)
        {
            return LevelGrid[row, column];
        }

        protected Parts[,] GetNew2DArray(int x, int y, Parts initialValue)
        {
            Parts[,] parts = new Parts[x, y];
            for (int i = 0; i < x * y; i++) parts[i % x, i / x] = initialValue;
            return parts;
        }

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
