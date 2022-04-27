namespace Code.ChessPieces{
    public class Bishop : Piece{
        public override void CalculatePossibleMoves(Piece[,] board){
            possibleMoves.Clear();
            MoveFinderHelper.BishopBottomLeftCheck(board, this);
            MoveFinderHelper.BishopBottomRightCheck(board, this);
            MoveFinderHelper.BishopTopLeftCheck(board, this);
            MoveFinderHelper.BishopTopRightCheck(board, this);
        }
    }
}