using System.Collections.Generic;
using System.Linq;
using GameGlobals;

namespace GameNS
{
    public class Game : IGame
    {
        protected string LevelName; //for file manipulations (state and stats)
        protected string LevelString;
        protected Parts[,] LevelGrid;
        protected int MoveCount;
        protected Position PlayerPos;
        protected List<Position> Blocks;
        protected Stack<Move> MovesMade;

        /// <summary>
        /// Gets the move count.
        /// </summary>
        /// <returns>The move count</returns>
        public int GetMoveCount()
        {
            return MoveCount;
        }

        /// <summary>
        /// Determines whether this game is finished.
        /// If all the block positions line up with the positions of targets the game is finished.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the game is finished; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFinished()
        {
            foreach (Position p in Blocks)
            {
                if (LevelGrid[p.Row, p.Column] != Parts.BlockOnGoal)
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Loads a new level.
        /// </summary>
        /// <param name="name">The name of the level.</param>
        /// <param name="newLevel">The new level as a string.</param>
        public void Load(string name, string newLevel)
        {
            LevelName = name;
            LevelString = newLevel;
        }

        /// <summary>
        /// Gets the rows in the level grid.
        /// </summary>
        /// <returns>The rows</returns>
        public int GetRows()
        {
            return LevelGrid.GetLength(1);
        }

        /// <summary>
        /// Gets the columns in the level grid.
        /// </summary>
        /// <returns>The columns</returns>
        public int GetCols()
        {
            return LevelGrid.GetLength(0);
        }

        /// <summary>
        /// Moves the player in the specified move direction.
        /// Moves a player or a player and a block or does nothing.
        /// If the move is successful store the move in the move stack and increase move count.
        /// </summary>
        /// <param name="moveDirection">The move direction.</param>
        /// <returns>The positions involved in the move or an empty array</returns>
        public Position[] Move(Direction moveDirection)
        {
            Position positionTwo = GetPositionFromMove(moveDirection, PlayerPos);
            List<Position> toReturn = new List<Position>();
            toReturn.Add(PlayerPos);
            toReturn.Add(positionTwo);

            if (PositionClear(positionTwo))
            {
                MoveToNewPosReplaceWithEmpty(PlayerPos, positionTwo);
                MovesMade.Push(new Move(PlayerPos, positionTwo));
                PlayerPos = positionTwo;
                MoveCount++;
                return toReturn.ToArray();
            }
            if (PositionContainsBlock(positionTwo))
            {
                Position positionThree = GetPositionFromMove(moveDirection, positionTwo);
                if (PositionClear(positionThree))
                {
                    MoveToNewPosReplaceWithEmpty(positionTwo, positionThree);
                    MoveToNewPosReplaceWithEmpty(PlayerPos, positionTwo);
                    MovesMade.Push(new Move(PlayerPos, positionTwo, positionThree));
                    PlayerPos = positionTwo;
                    MoveBlockPos(positionTwo, positionThree);
                    MoveCount++;
                    toReturn.Add(positionThree);
                    return toReturn.ToArray();
                }
            }
            return new Position[0];
        }

        /// <summary>
        /// Moves the block position.
        /// </summary>
        /// <param name="start">The start position.</param>
        /// <param name="end">The end position.</param>
        protected void MoveBlockPos(Position start, Position end)
        {
            for (int i = 0; i < Blocks.Count; i++)
            {
                if (PositionsAreSame(Blocks[i], start))
                {
                    Blocks[i] = end;
                    break;
                }
            }
        }

        /// <summary>
        /// Restarts the game.
        /// </summary>
        public void Restart()
        {
            MoveCount = 0;
            MovesMade = new Stack<Move>();
            MakeLevelGrid();
        }

        /// <summary>
        /// Makes the level grid from the string.
        /// Assigns the player position and the block positions.
        /// </summary>
        protected void MakeLevelGrid()
        {
            string[] lg = LevelString.Split(',');
            LevelGrid = new Parts[lg.Length,lg[0].Length];
            Blocks = new List<Position>();

            for (int r = 0; r < lg.Length; r++)
            {
                for (int c = 0; c < lg[0].Length; c++)
                {
                    LevelGrid[r, c] = (Parts)lg[r][c];
                    Position p = new Position(r, c);
                    Parts thisMovable = GetMovable(p);
                    
                    if (thisMovable == Parts.Player)
                    {
                        PlayerPos = p;
                    }
                    else if (thisMovable == Parts.Block)
                    {
                        Blocks.Add(p);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the state.
        /// Makes a grid that is only the environmental parts from the string.
        /// Combines the movables into the positions saved for these.
        /// </summary>
        /// <param name="state">The state from the file.</param>
        public void LoadState(State state)
        {
            MoveCount = state.Moves;
            PlayerPos = state.Player;
            Blocks = state.Blocks.ToList();

            string[] lg = LevelString.Split(',');
            LevelGrid = new Parts[lg.Length, lg[0].Length];

            for (int r = 0; r < lg.Length; r++)
            {
                for (int c = 0; c < lg[0].Length; c++)
                {
                    LevelGrid[r, c] = GetEnvironment((Parts)lg[r][c]);
                }
            }

            LevelGrid[PlayerPos.Row, PlayerPos.Column] = GetCombined(PlayerPos, Parts.Player);

            foreach (Position p in Blocks)
            {
                LevelGrid[p.Row, p.Column] = GetCombined(p, Parts.Block);
            }
        }

        /// <summary>
        /// Undo a move
        /// </summary>
        /// <returns></returns>
        public Position[] Undo()
        {
            if (MovesMade.Count > 0)
            {
                MoveCount++;
                Move theLast = MovesMade.Pop();
                MoveToNewPosReplaceWithEmpty(theLast.GetPos2(), theLast.GetPlayerStart());
                PlayerPos = theLast.GetPlayerStart();

                if (theLast.BoxWasMoved())
                {
                    MoveToNewPosReplaceWithEmpty(theLast.GetBoxEnd(), theLast.GetPos2());
                    MoveBlockPos(theLast.GetBoxEnd(), theLast.GetPos2());
                }
                return theLast.GetArray();
            }
            return new Position[0];
        }

        /// <summary>
        /// Checks if too positions contain the same x and y integers.
        /// </summary>
        /// <param name="a">Position a</param>
        /// <param name="b">Position b</param>
        /// <returns></returns>
        protected bool PositionsAreSame(Position a, Position b)
        {
            return (a.Row == b.Row && a.Column == b.Column);
        }

        /// <summary>
        /// Gets the neighbour position in the direction of the move.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="p">The position.</param>
        /// <returns>The new position to be moved to</returns>
        protected Position GetPositionFromMove(Direction direction, Position p)
        {
            switch (direction)
            {
                case Direction.Up:
                    p.Row--;
                    break;
                case Direction.Down:
                    p.Row++;
                    break;
                case Direction.Left:
                    p.Column--;
                    break;
                case Direction.Right:
                    p.Column++;
                    break;
            }
            return p;
        }

        /// <summary>
        /// Checks if the position is clear.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns><c>true</c> if the position is empty or a goal; otherwise, <c>false</c></returns>
        protected bool PositionClear(Position pos)
        {
            return (LevelGrid[pos.Row, pos.Column] == Parts.Empty || LevelGrid[pos.Row, pos.Column] == Parts.Goal);
        }

        /// <summary>
        /// Checks if the position contains a block.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns><c>true</c> if the position contains a block or a block on goal; otherwise, <c>false</c></returns>
        protected bool PositionContainsBlock(Position pos)
        {
            return ((LevelGrid[pos.Row, pos.Column] == Parts.Block) ||
                (LevelGrid[pos.Row, pos.Column] == Parts.BlockOnGoal));
        }

        /// <summary>
        /// Position end becomes the part at position start.
        /// Position start becomes empty or target.
        /// </summary>
        /// <param name="start">Position start.</param>
        /// <param name="end">Position end.</param>
        protected void MoveToNewPosReplaceWithEmpty(Position start, Position end)
        {
            Parts movable = GetMovable(start);
            LevelGrid[start.Row, start.Column] = GetEnvironment(start);
            LevelGrid[end.Row, end.Column] = GetCombined(end, movable);
        }

        /// <summary>
        /// Gets the movable part from the position, without the enviroment part.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        protected Parts GetMovable(Position pos)
        {
            switch (LevelGrid[pos.Row, pos.Column])
            {
                case Parts.Block:
                    return Parts.Block;
                case Parts.BlockOnGoal:
                    return Parts.Block;
                case Parts.Player:
                    return Parts.Player;
                case Parts.PlayerOnGoal:
                    return Parts.Player;
                default:
                    return Parts.Empty;
            }
        }

        /// <summary>
        /// Gets the environment part from the position, without the movable part.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns></returns>
        protected Parts GetEnvironment(Position pos)
        {
            switch (LevelGrid[pos.Row, pos.Column])
            {
                case Parts.BlockOnGoal:
                    return Parts.Goal;
                case Parts.PlayerOnGoal:
                    return Parts.Goal;
                case Parts.Goal:
                    return Parts.Goal;
                default:
                    return Parts.Empty;
            }
        }

        /// <summary>
        /// Gets the environment from the part, without the movable part.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <returns></returns>
        protected Parts GetEnvironment(Parts part)
        {
            switch (part)
            {
                case Parts.BlockOnGoal:
                    return Parts.Goal;
                case Parts.PlayerOnGoal:
                    return Parts.Goal;
                case Parts.Block:
                    return Parts.Empty;
                case Parts.Player:
                    return Parts.Empty;
                default:
                    return part;
            }
        }

        /// <summary>
        /// Gets the combined enviroment from pos and movable from m.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="m">The movable part.</param>
        /// <returns>The combined part</returns>
        protected Parts GetCombined(Position pos, Parts m)
        {
            Parts e = LevelGrid[pos.Row, pos.Column];

            if (e == Parts.Empty)
            {
                return m;
            }
            else if (m == Parts.Block)
            {
                return Parts.BlockOnGoal;
            }
            else
            {
                return Parts.PlayerOnGoal;
            }
        }

        /// <summary>
        /// Gets the part at grid [row, column].
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The column.</param>
        /// <returns>The part</returns>
        public Parts GetPartAt(int row, int col)
        {
            return LevelGrid[row, col];
        }

        /// <summary>
        /// Gets the name of the level.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return LevelName;
        }

        /// <summary>
        /// Makes the state to be saved.
        /// </summary>
        /// <returns>State object</returns>
        public State MakeState()
        {
            return new State(MoveCount, PlayerPos, Blocks.ToList());
        }
    }
}
