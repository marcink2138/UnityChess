using UnityEngine;

namespace Code{
    public class ComputerMoveWrapper{
        public Piece selectedPiece;
        public Vector2 selectedField;

        public ComputerMoveWrapper(Piece selectedPiece, Move selectedMove){
            this.selectedPiece = selectedPiece;
            selectedField = new Vector2(selectedMove.x, selectedMove.y);
        }
    }
}