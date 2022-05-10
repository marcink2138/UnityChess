namespace Code.ChessPieces{
    public class Queen : Piece{
        public override void CalculatePossibleMoves(Piece[,] board){
            possibleMoves.Clear();
            MoveFinderHelper.RookBottomCheck(board, this);
            MoveFinderHelper.RookLeftCheck(board, this);
            MoveFinderHelper.RookRightCheck(board, this);
            MoveFinderHelper.RookTopCheck(board, this);
            MoveFinderHelper.BishopBottomLeftCheck(board, this);
            MoveFinderHelper.BishopBottomRightCheck(board, this);
            MoveFinderHelper.BishopTopLeftCheck(board, this);
            MoveFinderHelper.BishopTopRightCheck(board, this);
            MoveFinderHelper.CheckIfMoveIsAttackingKing(board, this);
        }
    }
}