namespace Code
{
    public class Move
    {
        public int x;
        public int y;
        public MoveType MoveType;

        public Move(int x, int y, MoveType moveType)
        {
            this.x = x;
            this.y = y;
            MoveType = moveType;
        }
    }
}