namespace Code.ChessPieces{
    public class Rook : Piece{
        public override void CalculatePossibleMoves(Piece[,] board){
            possibleMoves.Clear();
            MoveFinderHelper.RookLeftCheck(board, this);
            MoveFinderHelper.RookBottomCheck(board, this);
            MoveFinderHelper.RookRightCheck(board, this);
            MoveFinderHelper.RookTopCheck(board, this);
            MoveFinderHelper.CheckIfMoveIsAttackingKing(board, this);
        }
    }
}