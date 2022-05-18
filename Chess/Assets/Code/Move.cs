namespace Code
{
    public class Move
    {
        public int x;
        public int y;
        public MoveType MoveType;
        public PieceType PieceType;
        public PieceType PromotedPieceType = PieceType.None;

        public Move(int x, int y, MoveType moveType, PieceType pieceType)
        {
            this.x = x;
            this.y = y;
            MoveType = moveType;
            PieceType = pieceType;
        }
    }
}