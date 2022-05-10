namespace Code.ChessPieces{
    public class Knight : Piece{
        public override void CalculatePossibleMoves(Piece[,] board){
            possibleMoves.Clear();
            CheckTopRight(board);
            CheckBottomRight(board);
            CheckBottomLeft(board);
            CheckTopLeft(board);
            MoveFinderHelper.CheckIfMoveIsAttackingKing(board, this);
        }

        private void CheckTopRight(Piece[,] board){
            if (yCord <= 5){
                if (xCord <= 5){
                    MoveFinderHelper.AddToMovesList(board, this, xCord + 1, yCord + 2);
                    MoveFinderHelper.AddToMovesList(board, this, xCord + 2, yCord + 1);
                }

                if (xCord == 6)
                    MoveFinderHelper.AddToMovesList(board, this, xCord + 1, yCord + 2);
            }

            if (yCord == 6 && xCord <= 5)
                MoveFinderHelper.AddToMovesList(board, this, xCord + 2, yCord + 1);
        }

        private void CheckBottomRight(Piece[,] board){
            if (yCord >= 2){
                if (xCord <= 5){
                    MoveFinderHelper.AddToMovesList(board, this, xCord + 1, yCord - 2);
                    MoveFinderHelper.AddToMovesList(board, this, xCord + 2, yCord - 1);
                }

                if (xCord == 6)
                    MoveFinderHelper.AddToMovesList(board, this, xCord + 1, yCord - 2);
            }

            if (yCord == 1 && xCord <= 5)
                MoveFinderHelper.AddToMovesList(board, this, xCord + 2, yCord - 1);
        }

        private void CheckBottomLeft(Piece[,] board){
            if (yCord >= 2){
                if (xCord >= 2){
                    MoveFinderHelper.AddToMovesList(board, this, xCord - 1, yCord - 2);
                    MoveFinderHelper.AddToMovesList(board, this, xCord - 2, yCord - 1);
                }

                if (xCord == 1)
                    MoveFinderHelper.AddToMovesList(board, this, xCord - 1, yCord - 2);
            }

            if (yCord == 1 && xCord >= 2)
                MoveFinderHelper.AddToMovesList(board, this, xCord - 2, yCord - 1);
        }

        private void CheckTopLeft(Piece[,] board){
            if (yCord <= 5){
                if (xCord >= 2){
                    MoveFinderHelper.AddToMovesList(board, this, xCord - 1, yCord + 2);
                    MoveFinderHelper.AddToMovesList(board, this, xCord - 2, yCord + 1);
                }

                if (xCord == 1)
                    MoveFinderHelper.AddToMovesList(board, this, xCord - 1, yCord + 2);
            }

            if (yCord == 6 && xCord >= 2)
                MoveFinderHelper.AddToMovesList(board, this, xCord - 2, yCord + 1);
        }
    }
}