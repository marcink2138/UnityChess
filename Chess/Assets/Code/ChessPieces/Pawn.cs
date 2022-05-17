namespace Code.ChessPieces{
    public class Pawn : Piece{
        public override void CalculatePossibleMoves(Piece[,] board){
            possibleMoves.Clear();
            CheckWhitePawnMoves(board);
            CheckBlackPawnMoves(board);
            MoveFinderHelper.CheckIfMoveIsAttackingKing(board, this);
        }

        private void CheckWhitePawnMoves(Piece[,] board){
            if (teamType.Equals(TeamType.White)){
                //If first move
                if (yCord == 1 && board[xCord, yCord + 1] == null && board[xCord, yCord + 2] == null){
                    possibleMoves.Add(new Move(xCord, yCord + 2, MoveType.Normal));
                }

                //Standard move
                if (board[xCord, yCord + 1] == null){
                    possibleMoves.Add(new Move(xCord, yCord + 1, CheckWhitePromotion(MoveType.Normal)));
                }

                //Left
                if (xCord == 0){
                    if (board[xCord + 1, yCord + 1] != null){
                        if (board[xCord + 1, yCord + 1].teamType.Equals(enemyTeamType))
                            possibleMoves.Add(new Move(xCord + 1, yCord + 1,
                                CheckWhitePromotion(MoveType.Attack)));
                    }

                    CheckRightEnPassant(board, TeamType.White);
                    return;
                }

                //Right
                if (xCord == 7){
                    if (board[xCord - 1, yCord + 1] != null){
                        if (board[xCord - 1, yCord + 1].teamType.Equals(enemyTeamType))
                            possibleMoves.Add(new Move(xCord - 1, yCord + 1,
                                CheckWhitePromotion(MoveType.Attack)));
                    }

                    CheckLeftEnPassant(board, TeamType.White);
                    return;
                }

                //Midlle
                if (board[xCord - 1, yCord + 1] != null){
                    if (board[xCord - 1, yCord + 1].teamType.Equals(enemyTeamType))
                        possibleMoves.Add(new Move(xCord - 1, yCord + 1,
                            CheckWhitePromotion(MoveType.Attack)));
                }

                if (board[xCord + 1, yCord + 1] != null){
                    if (board[xCord + 1, yCord + 1].teamType.Equals(enemyTeamType))
                        possibleMoves.Add(new Move(xCord + 1, yCord + 1,
                            CheckWhitePromotion(MoveType.Attack)));
                }

                CheckRightEnPassant(board, TeamType.White);
                CheckLeftEnPassant(board, TeamType.White);
            }
        }

        private void CheckBlackPawnMoves(Piece[,] board){
            if (teamType.Equals(TeamType.Black)){
                //If first move
                if (yCord == 6 && board[xCord, yCord - 1] == null && board[xCord, yCord - 2] == null){
                    possibleMoves.Add(new Move(xCord, yCord - 2, MoveType.Normal));
                }

                //Standard move
                if (board[xCord, yCord - 1] == null){
                    possibleMoves.Add(new Move(xCord, yCord - 1, CheckBlackPromotion(MoveType.Normal)));
                }

                //Left
                if (xCord == 0){
                    if (board[xCord + 1, yCord - 1] != null){
                        if (board[xCord + 1, yCord - 1].teamType.Equals(enemyTeamType))
                            possibleMoves.Add(new Move(xCord + 1, yCord - 1, CheckBlackPromotion(MoveType.Attack)));
                    }

                    CheckRightEnPassant(board, TeamType.Black);
                    return;
                }

                //Right
                if (xCord == 7){
                    if (board[xCord - 1, yCord - 1] != null){
                        if (board[xCord - 1, yCord - 1].teamType.Equals(enemyTeamType))
                            possibleMoves.Add(new Move(xCord - 1, yCord - 1, CheckBlackPromotion(MoveType.Attack)));
                    }

                    CheckLeftEnPassant(board, TeamType.Black);
                    return;
                }

                //Midlle
                if (board[xCord - 1, yCord - 1] != null){
                    if (board[xCord - 1, yCord - 1].teamType.Equals(enemyTeamType))
                        possibleMoves.Add(new Move(xCord - 1, yCord - 1, CheckBlackPromotion(MoveType.Attack)));
                }

                if (board[xCord + 1, yCord - 1] != null){
                    if (board[xCord + 1, yCord - 1].teamType.Equals(enemyTeamType))
                        possibleMoves.Add(new Move(xCord + 1, yCord - 1, CheckBlackPromotion(MoveType.Attack)));
                }

                CheckRightEnPassant(board, TeamType.Black);
                CheckLeftEnPassant(board, TeamType.Black);
            }
        }

        private void CheckRightEnPassant(Piece[,] board, TeamType teamType){
            //Right en passant
            if (teamType.Equals(TeamType.White)){
                if (yCord == 4 && board[xCord + 1, yCord] != null){
                    if (board[xCord + 1, yCord].pieceType.Equals(PieceType.Pawn) &&
                        board[xCord + 1, yCord].teamType.Equals(enemyTeamType)){
                        if (board[xCord + 1, yCord + 1] == null)
                            possibleMoves.Add(new Move(xCord + 1, yCord + 1, MoveType.EnPassant));
                    }
                }
            }
            else{
                if (yCord == 3 && board[xCord + 1, yCord] != null){
                    if (board[xCord + 1, yCord].pieceType.Equals(PieceType.Pawn) &&
                        board[xCord + 1, yCord].teamType.Equals(enemyTeamType)){
                        if (board[xCord + 1, yCord - 1] == null)
                            possibleMoves.Add(new Move(xCord + 1, yCord - 1, MoveType.EnPassant));
                    }
                }
            }
        }

        private void CheckLeftEnPassant(Piece[,] board, TeamType teamType){
            //Left en passant
            if (teamType.Equals(TeamType.White)){
                if (yCord == 4 && board[xCord - 1, yCord] != null){
                    if (board[xCord - 1, yCord].pieceType.Equals(PieceType.Pawn) &&
                        board[xCord - 1, yCord].teamType.Equals(enemyTeamType)){
                        if (board[xCord - 1, yCord + 1] == null)
                            possibleMoves.Add(new Move(xCord - 1, yCord + 1, MoveType.EnPassant));
                    }
                }
            }
            else{
                if (yCord == 3 && board[xCord - 1, yCord] != null){
                    if (board[xCord - 1, yCord].pieceType.Equals(PieceType.Pawn) &&
                        board[xCord - 1, yCord].teamType.Equals(enemyTeamType)){
                        if (board[xCord - 1, yCord + 1] == null)
                            possibleMoves.Add(new Move(xCord - 1, yCord - 1, MoveType.EnPassant));
                    }
                }
            }
        }

        private MoveType CheckBlackPromotion(MoveType moveType){
            return yCord - 1 == 0 ? MoveType.Promotion : moveType;
        }

        private MoveType CheckWhitePromotion(MoveType moveType){
            return yCord + 1 == 7 ? MoveType.Promotion : moveType;
        }
    }
}