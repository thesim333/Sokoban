using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Designer;

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
        
        public int GetMoveCount()
        {
            return MoveCount;
        }

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

        /**
         * Stores the name and level string from the level file
         */
        public void Load(string name, string newLevel)
        {
            LevelName = name;
            LevelString = newLevel;
        }

        public int GetRows()
        {
            return LevelGrid.GetLength(1);
        }

        public int GetCols()
        {
            return LevelGrid.GetLength(0);
        }

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

        public void Restart()
        {
            MoveCount = 0;
            MovesMade = new Stack<Move>();
            MakeLevelGrid();
        }

        protected void FindMyMovable(Parts part, Position pos)
        {
            if (part == Parts.Player || part == Parts.PlayerOnGoal)
            {
                PlayerPos = pos;
            }
            else if (part == Parts.Block || part == Parts.BlockOnGoal)
            {
                Blocks.Add(pos);
            }
        }

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

        /**
         * Accepts the string from a previously saved state.
         * moves,player-pos,block-pos,block-pos...
         * Loads the environment only from level string
         * places the movables from state string
         */
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

        protected bool PositionsAreSame(Position a, Position b)
        {
            return (a.Row == b.Row && a.Column == b.Column);
        }

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

        protected bool PositionClear(Position pos)
        {
            return (LevelGrid[pos.Row, pos.Column] == Parts.Empty || LevelGrid[pos.Row, pos.Column] == Parts.Goal);
        }

        protected bool PositionContainsBlock(Position pos)
        {
            return ((LevelGrid[pos.Row, pos.Column] == Parts.Block) ||
                (LevelGrid[pos.Row, pos.Column] == Parts.BlockOnGoal));
        }

        protected void MoveToNewPosReplaceWithEmpty(Position start, Position end)
        {
            Parts movable = GetMovable(start);
            LevelGrid[start.Row, start.Column] = GetEnvironment(start);
            LevelGrid[end.Row, end.Column] = GetCombined(end, movable);
        }

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

        public Parts GetPartAt(int row, int col)
        {
            return LevelGrid[row, col];
        }

        public string GetName()
        {
            return LevelName;
        }

        public State MakeState()
        {
            return new State(MoveCount, PlayerPos, Blocks.ToList());
        }
    }
}
