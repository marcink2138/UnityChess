using System.Collections.Generic;
using Code;
using UnityEngine;

public abstract class Piece : MonoBehaviour{
    public int xCord;
    public int yCord;
    public TeamType teamType;
    public TeamType enemyTeamType;
    public ChessPieceType chessPieceType;
    public bool beforeFirstMove;
    public List<Move> possibleMoves = new List<Move>();

    public void SetCords(int x, int y){
        xCord = x;
        yCord = y;
    }

    public abstract void CalculatePossibleMoves(Piece[,] board);

    public Move FindMoveByCords(int x, int y){
        foreach (var possibleMove in possibleMoves){
            if (possibleMove.x == x && possibleMove.y == y){
                return possibleMove;
            }
        }

        return new Move(-1, -1, MoveType.Normal);
    }
}