using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Designer;

namespace Sokoban
{
    public class ImageHandler
    {
        public readonly Image Wall = Image.FromFile("wall38.jpg");
        public readonly Image Player = Image.FromFile("player38.png");
        public readonly Image Goal = Image.FromFile("target38.jpg");
        public readonly Image Block = Image.FromFile("block38.png");
        public readonly Image PlayerGoal = Image.FromFile("playertarget38.jpg");
        public readonly Image BlockGoal = Image.FromFile("blocktarget38.jpg");
        public readonly Image Empty = Image.FromFile("empty38.jpg");

        public Image GetMyPart(Parts part)
        {
            switch (part)
            {
                case Parts.Wall:
                    return Wall;
                case Parts.Block:
                    return Block;
                case Parts.Goal:
                    return Goal;
                case Parts.BlockOnGoal:
                    return BlockGoal;
                case Parts.PlayerOnGoal:
                    return PlayerGoal;
                case Parts.Player:
                    return Player;
                default:
                    return Empty;
            }
        }
    }
}
