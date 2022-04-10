using System;
using Code;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject[] chessPiecesPrefabs;
    [SerializeField] private Material[] teamColors;

    [SerializeField] private float pieceShiftX = 1.5f;
    [SerializeField] private float pieceShiftY = 1.5f;

    private Piece[,] board;
    private int BOARD_X_SIZE = 8;
    private int BOARD_Y_SIZE = 8;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void Awake()
    {
        board = new Piece[8, 8];
        FillBoardWithPieces();
        SetPiecesPosition();
    }

    private void FillBoardWithPieces()
    {
        board[0, 0] = CreateSinglePiece(ChessPieceType.Rook, TeamType.White);
        board[1, 0] = CreateSinglePiece(ChessPieceType.Knight, TeamType.White);
        board[2, 0] = CreateSinglePiece(ChessPieceType.Bishop, TeamType.White);
        board[3, 0] = CreateSinglePiece(ChessPieceType.Queen, TeamType.White);
        board[4, 0] = CreateSinglePiece(ChessPieceType.King, TeamType.White);
        board[5, 0] = CreateSinglePiece(ChessPieceType.Bishop, TeamType.White);
        board[6, 0] = CreateSinglePiece(ChessPieceType.Knight, TeamType.White);
        board[7, 0] = CreateSinglePiece(ChessPieceType.Rook, TeamType.White);

        board[0, 7] = CreateSinglePiece(ChessPieceType.Rook, TeamType.Black);
        board[1, 7] = CreateSinglePiece(ChessPieceType.Knight, TeamType.Black);
        board[2, 7] = CreateSinglePiece(ChessPieceType.Bishop, TeamType.Black);
        board[3, 7] = CreateSinglePiece(ChessPieceType.Queen, TeamType.Black);
        board[4, 7] = CreateSinglePiece(ChessPieceType.King, TeamType.Black);
        board[5, 7] = CreateSinglePiece(ChessPieceType.Bishop, TeamType.Black);
        board[6, 7] = CreateSinglePiece(ChessPieceType.Knight, TeamType.Black);
        board[7, 7] = CreateSinglePiece(ChessPieceType.Rook, TeamType.Black);

        for (int x = 0; x < BOARD_X_SIZE; x++)
        {
            board[x, 1] = CreateSinglePiece(ChessPieceType.Pawn, TeamType.White);
            board[x, 6] = CreateSinglePiece(ChessPieceType.Pawn, TeamType.Black);
        }
    }

    private void SetPiecesPosition()
    {
        for (int x = 0; x < BOARD_X_SIZE; x++)
        {
            for (int y = 0; y < BOARD_Y_SIZE; y++)
            {
                if (board[x, y] != null)
                {
                    board[x, y].transform.position = board[x, y].chessPieceType.Equals(ChessPieceType.Pawn)
                        ? new Vector3(x * pieceShiftX, 0.01f, y * pieceShiftY)
                        : new Vector3(x * pieceShiftX, 0.2f, y * pieceShiftY);
                }
            }
        }
    }

    private Piece CreateSinglePiece(ChessPieceType chessPieceType, TeamType teamType)
    {
        var piece = Instantiate(chessPiecesPrefabs[(int) chessPieceType], transform).GetComponent<Piece>();
        piece.teamType = teamType;
        piece.chessPieceType = chessPieceType;
        piece.GetComponent<MeshRenderer>().material = teamColors[(int) teamType];
        piece.transform.position = new Vector3(0, 0.1f, 0);
        if (chessPieceType.Equals(ChessPieceType.Knight) && teamType.Equals(TeamType.Black))
        {
            var rotationVector = piece.transform.rotation.eulerAngles;
            rotationVector.y = 180;
            piece.transform.rotation = Quaternion.Euler(rotationVector);
        }

        return piece;
    }
}