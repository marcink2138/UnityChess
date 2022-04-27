namespace Code.ChessPieces{
    public class King : Piece{
        public override void CalculatePossibleMoves(Piece[,] board){
            possibleMoves.Clear();
            CheckTop(board);
            CheckBottom(board);
            CheckLeft(board);
            CheckBottomLeft(board);
            CheckBottomRight(board);
            CheckTopLeft(board);
            CheckTopRight(board);
            CheckRight(board);
            CheckCastling(board);
        }

        private void CheckTop(Piece[,] board){
            if (yCord != 7){
                MoveFinderHelper.AddToMovesList(board, this, xCord, yCord + 1);
            }
        }

        private void CheckTopRight(Piece[,] board){
            if (yCord != 7 && xCord != 7){
                MoveFinderHelper.AddToMovesList(board, this, xCord + 1, yCord + 1);
            }
        }

        private void CheckBottomRight(Piece[,] board){
            if (yCord != 0 && xCord != 7){
                MoveFinderHelper.AddToMovesList(board, this, xCord + 1, yCord - 1);
            }
        }

        private void CheckBottom(Piece[,] board){
            if (yCord != 0){
                MoveFinderHelper.AddToMovesList(board, this, xCord, yCord - 1);
            }
        }

        private void CheckBottomLeft(Piece[,] board){
            if (yCord != 0 && xCord != 0){
                MoveFinderHelper.AddToMovesList(board, this, xCord - 1, yCord - 1);
            }
        }

        private void CheckLeft(Piece[,] board){
            if (xCord != 0){
                MoveFinderHelper.AddToMovesList(board, this, xCord - 1, yCord);
            }
        }

        private void CheckTopLeft(Piece[,] board){
            if (xCord != 0 && yCord != 7){
                MoveFinderHelper.AddToMovesList(board, this, xCord - 1, yCord + 1);
            }
        }

        private void CheckRight(Piece[,] board){
            if (xCord != 7){
                MoveFinderHelper.AddToMovesList(board, this, xCord + 1, yCord);
            }
        }

        private void CheckCastling(Piece[,] board){
            var y = teamType == TeamType.Black ? 7 : 0;
            if (xCord == 4 && yCord == y && beforeFirstMove){
                if (board[xCord - 1, yCord] == null && board[xCord - 2, yCord] == null &&
                    board[xCord - 3, yCord] == null &&
                    board[xCord - 4, yCord].chessPieceType.Equals(ChessPieceType.Rook)){
                    possibleMoves.Add(new Move(xCord - 2, yCord, MoveType.LongCastling));
                }

                if (board[xCord + 1, yCord] == null && board[xCord + 2, yCord] == null &&
                    board[xCord + 3, yCord].chessPieceType.Equals(ChessPieceType.Rook)){
                    possibleMoves.Add(new Move(xCord + 2, yCord, MoveType.ShortCastling));
                }
            }
        }
    }
}