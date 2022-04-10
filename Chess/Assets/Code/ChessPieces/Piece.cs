using Code;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public int xCord;
    public int yCord;
    public TeamType teamType;
    public ChessPieceType chessPieceType;

    public void SetCords(int x, int y)
    {
        xCord = x;
        yCord = y;
    }
    
}