using System.Collections.Generic;
using UnityEngine;

namespace Code{
    public class CheckMateDetector{
        private CheckMateDetector(){ }

        public static bool CheckMateDetection(List<Piece> piecesList, TeamType currentPlayer){
            var currentPlayerPieces = piecesList.FindAll(piece => piece.teamType == currentPlayer);
            var flag = true;
            foreach (var piece in currentPlayerPieces){
                if (piece.possibleMoves.Count != 0){
                    flag = false;
                    break;
                }
            }

            if (flag){
                Debug.Log("Przegrał gracz : " + currentPlayer);
            }

            return flag;
        }

        public static Piece CheckDetection(List<Piece> piecesList, TeamType currentPlayer){
            var enemyPlayerPieces = piecesList.FindAll(piece => piece.teamType != currentPlayer);
            Piece currentPlayerKing =
                piecesList.Find(piece => piece.pieceType == PieceType.King && piece.teamType == currentPlayer);
            foreach (var piece in enemyPlayerPieces){
                foreach (var move in piece.possibleMoves){
                    if (move.x == currentPlayerKing.xCord && move.y == currentPlayerKing.yCord){
                        return currentPlayerKing;
                    }
                }
            }

            return null;
        }

        public static void CalculateAndRemoveIllegalMoves(Piece[,] board, List<Piece> piecesList,
            TeamType currentPlayer){
            var currentPlayerPieces = piecesList.FindAll(piece => piece.teamType == currentPlayer);
            var enemyPlayerPieces = piecesList.FindAll(piece => piece.teamType != currentPlayer);
            currentPlayerPieces.ForEach(currentPlayerPiece => {
                currentPlayerPiece.CalculatePossibleMoves(board);
                SimulatePieceMoves(currentPlayerPiece, board, enemyPlayerPieces);
            });
            enemyPlayerPieces.ForEach(enemyPlayerPiece => enemyPlayerPiece.CalculatePossibleMoves(board));
        }

        private static void SimulatePieceMoves(Piece piece, Piece[,] board, List<Piece> enemyPiecesList){
            var initPosition = new Vector2(piece.xCord, piece.yCord);
            var movesToDelete = new List<Move>();
            foreach (var possibleMove in piece.possibleMoves){
                if (possibleMove.MoveType == MoveType.ShortCastling || possibleMove.MoveType == MoveType.LongCastling){
                    if (SimulateCastling(piece, board, enemyPiecesList))
                        movesToDelete.Add(possibleMove);
                    continue;
                }

                Piece attackedPiece = null; /*= board[possibleMove.x, possibleMove.y];
                board[possibleMove.x, possibleMove.y] = piece;*/
                SimulateNormalMove(piece, board, possibleMove, ref attackedPiece);
                SimulateEnPassant(piece, board, possibleMove, ref attackedPiece);
                board[(int) initPosition.x, (int) initPosition.y] = null;
                if (attackedPiece != null){
                    enemyPiecesList.Remove(attackedPiece);
                }

                if (!CheckMoveLegality(board, enemyPiecesList)){
                    movesToDelete.Add(possibleMove);
                }

                board[(int) initPosition.x, (int) initPosition.y] = piece;
                board[possibleMove.x, possibleMove.y] = null;
                if (attackedPiece != null){
                    enemyPiecesList.Add(attackedPiece);
                    board[attackedPiece.xCord, attackedPiece.yCord] = attackedPiece;
                }
            }

            movesToDelete.ForEach(moveToDelete => piece.possibleMoves.Remove(moveToDelete));
        }

        private static void SimulateEnPassant(Piece piece, Piece[,] board, Move move, ref Piece attackedPiece){
            if (piece.pieceType == PieceType.Pawn && move.MoveType == MoveType.EnPassant){
                var attackedPieceY = piece.teamType == TeamType.White ? move.y - 1 : move.y + 1;
                var attackedPieceX = move.x;
                attackedPiece = board[attackedPieceX, attackedPieceY];
                board[attackedPieceX, attackedPieceY] = null;
                board[move.x, move.y] = piece;
            }
        }

        private static void SimulateNormalMove(Piece piece, Piece[,] board, Move move, ref Piece attackedPiece){
            if (move.MoveType == MoveType.Attack || move.MoveType == MoveType.Normal ||
                move.MoveType == MoveType.Promotion){
                attackedPiece = board[move.x, move.y];
                board[move.x, move.y] = piece;
            }
        }

        private static bool SimulateCastling(Piece piece, Piece[,] board, List<Piece> enemyPiecesList){
            var y = piece.teamType == TeamType.White ? 0 : 7;
            enemyPiecesList.ForEach(enemyPiece => enemyPiece.CalculatePossibleMoves(board));
            return enemyPiecesList.FindAll(enemyPiece =>
                    enemyPiece.possibleMoves
                        .FindAll(move1 =>
                            (move1.x == 5 && move1.y == y) || (move1.x == 6 && move1.y == y) ||
                            (move1.x == 4 && move1.y == y) || (move1.x == 3 && move1.y == y))
                        .Count > 0)
                .Count > 0;
        }

        private static bool CheckMoveLegality(Piece[,] board, List<Piece> enemyPieces){
            enemyPieces.ForEach(piece => piece.CalculatePossibleMoves(board));
            foreach (var piece in enemyPieces){
                if (FindKingCheck(piece, board)){
                    return false;
                }
            }

            return true;
        }

        private static bool FindKingCheck(Piece piece, Piece[,] board){
            return piece.possibleMoves.FindAll(move => board[move.x, move.y] != null)
                .FindAll(move => board[move.x, move.y].pieceType == PieceType.King).Count > 0;
        }
    }
}