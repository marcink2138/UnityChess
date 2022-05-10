namespace Code{
    public class MoveFinderHelper{
        private MoveFinderHelper(){ }

        public static void BishopTopRightCheck(Piece[,] board, Piece piece){
            for (int x = piece.xCord + 1, y = piece.yCord + 1; x < 8 && y < 8; x++, y++){
                if (board[x, y] == null){
                    piece.possibleMoves.Add(new Move(x, y, MoveType.Normal));
                }
                else{
                    if (board[x, y].teamType.Equals(piece.enemyTeamType))
                        piece.possibleMoves.Add(new Move(x, y, MoveType.Attack));
                    break;
                }
            }
        }

        public static void BishopBottomLeftCheck(Piece[,] board, Piece piece){
            for (int x = piece.xCord - 1, y = piece.yCord - 1; x >= 0 && y >= 0; x--, y--){
                if (board[x, y] == null){
                    piece.possibleMoves.Add(new Move(x, y, MoveType.Normal));
                }
                else{
                    if (board[x, y].teamType.Equals(piece.enemyTeamType))
                        piece.possibleMoves.Add(new Move(x, y, MoveType.Attack));
                    break;
                }
            }
        }

        public static void BishopBottomRightCheck(Piece[,] board, Piece piece){
            for (int x = piece.xCord + 1, y = piece.yCord - 1; x < 8 && y >= 0; x++, y--){
                if (board[x, y] == null){
                    piece.possibleMoves.Add(new Move(x, y, MoveType.Normal));
                }
                else{
                    if (board[x, y].teamType.Equals(piece.enemyTeamType))
                        piece.possibleMoves.Add(new Move(x, y, MoveType.Attack));
                    break;
                }
            }
        }

        public static void BishopTopLeftCheck(Piece[,] board, Piece piece){
            for (int x = piece.xCord - 1, y = piece.yCord + 1; x >= 0 && y < 8; x--, y++){
                if (board[x, y] == null){
                    piece.possibleMoves.Add(new Move(x, y, MoveType.Normal));
                }
                else{
                    if (board[x, y].teamType.Equals(piece.enemyTeamType))
                        piece.possibleMoves.Add(new Move(x, y, MoveType.Attack));
                    break;
                }
            }
        }

        public static void RookBottomCheck(Piece[,] board, Piece piece){
            for (int y = piece.yCord - 1; y >= 0; y--){
                if (board[piece.xCord, y] == null){
                    piece.possibleMoves.Add(new Move(piece.xCord, y, MoveType.Normal));
                }
                else{
                    if (board[piece.xCord, y].teamType.Equals(piece.enemyTeamType))
                        piece.possibleMoves.Add(new Move(piece.xCord, y, MoveType.Attack));
                    break;
                }
            }
        }

        public static void RookTopCheck(Piece[,] board, Piece piece){
            for (int y = piece.yCord + 1; y < 8; y++){
                if (board[piece.xCord, y] == null){
                    piece.possibleMoves.Add(new Move(piece.xCord, y, MoveType.Normal));
                }
                else{
                    if (board[piece.xCord, y].teamType.Equals(piece.enemyTeamType))
                        piece.possibleMoves.Add(new Move(piece.xCord, y, MoveType.Attack));
                    break;
                }
            }
        }

        public static void RookLeftCheck(Piece[,] board, Piece piece){
            for (int x = piece.xCord - 1; x >= 0; x--){
                if (board[x, piece.yCord] == null){
                    piece.possibleMoves.Add(new Move(x, piece.yCord, MoveType.Normal));
                }
                else{
                    if (board[x, piece.yCord].teamType.Equals(piece.enemyTeamType))
                        piece.possibleMoves.Add(new Move(x, piece.yCord, MoveType.Attack));
                    break;
                }
            }
        }

        public static void RookRightCheck(Piece[,] board, Piece piece){
            for (int x = piece.xCord + 1; x < 8; x++){
                if (board[x, piece.yCord] == null){
                    piece.possibleMoves.Add(new Move(x, piece.yCord, MoveType.Normal));
                }
                else{
                    if (board[x, piece.yCord].teamType.Equals(piece.enemyTeamType))
                        piece.possibleMoves.Add(new Move(x, piece.yCord, MoveType.Attack));
                    break;
                }
            }
        }

        public static void AddToMovesList(Piece[,] board, Piece piece, int x, int y){
            if (board[x, y] == null){
                piece.possibleMoves.Add(new Move(x, y, MoveType.Normal));
            }
            else if (board[x, y].teamType.Equals(piece.enemyTeamType)){
                piece.possibleMoves.Add(new Move(x, y, MoveType.Attack));
            }
        }

        public static void CheckIfMoveIsAttackingKing(Piece[,] board, Piece piece){
            piece.possibleMoves.FindAll(move => board[move.x, move.y] != null)
                .FindAll(move => board[move.x, move.y].pieceType == PieceType.King)
                .ForEach(move => move.MoveType = MoveType.KingAttack);
        }
    }
}