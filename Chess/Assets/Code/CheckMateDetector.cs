using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Code{
    public class CheckMateDetector{
        private CheckMateDetector(){ }

        public static void CheckMateDetection(List<Piece> piecesList, TeamType currentPlayer){
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
                var attackedPiece = board[possibleMove.x, possibleMove.y];
                board[possibleMove.x, possibleMove.y] = piece;
                board[(int) initPosition.x, (int) initPosition.y] = null;
                if (attackedPiece != null){
                    enemyPiecesList.Remove(attackedPiece);
                }
                if (!CheckMoveLegality(board, enemyPiecesList)){
                    movesToDelete.Add(possibleMove);
                }

                board[(int) initPosition.x, (int) initPosition.y] = piece;
                board[possibleMove.x, possibleMove.y] = attackedPiece;
                if (attackedPiece != null){
                    enemyPiecesList.Add(attackedPiece);
                }
            }

            Debug.Log(movesToDelete.Count);
            movesToDelete.ForEach(moveToDelete => { Debug.Log(piece.possibleMoves.Remove(moveToDelete)); });
        }

        private static bool CheckMoveLegality(Piece[,] board, List<Piece> enemyPieces){
            enemyPieces.ForEach(piece => piece.CalculatePossibleMoves(board));
            foreach (var piece in enemyPieces){
                if (FindKingCheck(piece, board)){
                    Debug.Log("Wchodzi do if");
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