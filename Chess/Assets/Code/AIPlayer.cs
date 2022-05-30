using System;
using System.Collections.Generic;
using System.Linq;

namespace Code{
    public class AIPlayer{
        public static ComputerMoveWrapper MakeMove(List<Piece> piecesList){
            var piecesWithPossibleMoves = piecesList.FindAll(piece => piece.possibleMoves.Count > 0);
            var selectPieceRange = piecesWithPossibleMoves.Count - 1;
            if (selectPieceRange < 0){
                return null;
            }

            Random random = new Random();
            var temp = random.Next(0, selectPieceRange);
            var selectedPiece = piecesWithPossibleMoves.ElementAt(temp);
            var selectMoveRange = selectedPiece.possibleMoves.Count - 1;
            temp = random.Next(0, selectMoveRange);
            var move = selectedPiece.possibleMoves.ElementAt(temp);

            return new ComputerMoveWrapper(selectedPiece, move);
        }
    }
}