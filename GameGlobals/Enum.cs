namespace GameGlobals
{
    public enum Parts
    {
        Wall = (int)'#',
        Empty = (int)'-',
        Player = (int)'@',
        Goal = (int)'.',
        Block = (int)'$',
        BlockOnGoal = (int)'*',
        PlayerOnGoal = (int)'+'
    };

    public enum Direction { Up, Down, Left, Right };
}
