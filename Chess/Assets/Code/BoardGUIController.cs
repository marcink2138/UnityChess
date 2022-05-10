using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Code.ChessPieces;
using UnityEngine;

namespace Code{
    public class BoardGUIController : MonoBehaviour{
        private GameObject[,] fields;
        private List<GameObject> highlightedFields;
        [SerializeField] private GameObject[] chessPiecesPrefabs;
        [SerializeField] private Material[] teamColors;
        [SerializeField] private Material[] fieldsMaterials;
        [SerializeField] private float fieldXyShift = 0.75f;
        [SerializeField] private float fieldZShift = 0.1f;
        [SerializeField] private float pieceShiftX = 1.5f;
        [SerializeField] private float pieceShiftY = 1.5f;
        [SerializeField] private float fieldSizeScale = 0.15f;
        private int BOARD_X_SIZE = 8;
        private int BOARD_Y_SIZE = 8;

        private void Awake(){
            fields = new GameObject[8, 8];
            highlightedFields = new List<GameObject>();
            CreateFields();
        }

        public void CreateFields(){
            var bounds = new Vector3(fieldXyShift, 0, fieldXyShift);
            for (int x = 0; x < BOARD_X_SIZE; x++)
            for (int y = 0; y < BOARD_Y_SIZE; y++)
                fields[x, y] = CreateField(x, y, bounds);
        }

        private GameObject CreateField(float x, float y, Vector3 bounds){
            GameObject field = new GameObject(string.Format("X{0}, Y{1}", x, y));
            field.transform.parent = transform;
            Mesh mesh = new Mesh();
            field.AddComponent<MeshFilter>().mesh = mesh;
            field.AddComponent<MeshRenderer>();
            Vector3[] vector3s = new Vector3[4];
            vector3s[0] = new Vector3(x * fieldSizeScale, fieldZShift, y * fieldSizeScale) - bounds;
            vector3s[1] = new Vector3(x * fieldSizeScale, fieldZShift, (y + 1) * fieldSizeScale) - bounds;
            vector3s[2] = new Vector3((x + 1) * fieldSizeScale, fieldZShift, y * fieldSizeScale) - bounds;
            vector3s[3] = new Vector3((x + 1) * fieldSizeScale, fieldZShift, (y + 1) * fieldSizeScale) - bounds;
            int[] triangels ={0, 1, 2, 1, 3, 2};
            mesh.vertices = vector3s;
            mesh.triangles = triangels;
            field.AddComponent<BoxCollider>();
            field.layer = LayerMask.NameToLayer("Field");
            field.GetComponent<MeshRenderer>().material = fieldsMaterials[(int) FieldLayer.Field];
            return field;
        }

        public void HandleFieldHighlighting(Piece selectedPiece){
            foreach (var possibleMove in selectedPiece.possibleMoves){
                if (possibleMove.MoveType != MoveType.KingAttack){
                    fields[possibleMove.x, possibleMove.y].layer = LayerMask.NameToLayer("HighlightedField");
                    fields[possibleMove.x, possibleMove.y].GetComponent<MeshRenderer>().material =
                        possibleMove.MoveType.Equals(MoveType.Attack)
                            ? fieldsMaterials[(int) FieldLayer.HighlightedAttackMaterial]
                            : fieldsMaterials[(int) FieldLayer.HighlightedField];
                    highlightedFields.Add(fields[possibleMove.x, possibleMove.y]);
                }
            }
        }
        
        public void UnHighlightFields(){
            foreach (var highlightedField in highlightedFields){
                highlightedField.layer = LayerMask.NameToLayer("Field");
                highlightedField.GetComponent<MeshRenderer>().material = fieldsMaterials[(int) FieldLayer.Field];
            }

            highlightedFields.Clear();
        }
        
        public Vector2 FindSelectedFieldCords(GameObject selectedField){
            for (int x = 0; x < BOARD_X_SIZE; x++){
                for (int y = 0; y < BOARD_Y_SIZE; y++){
                    if (fields[x, y] == selectedField){
                        return new Vector2(x, y);
                    }
                }
            }

            return new Vector2(-1, -1);
        }

        public void FillBoardWithPieces(Piece[,] board, List<Piece> piecesList){
            board[0, 0] = CreateSinglePiece(PieceType.Rook, TeamType.White, 0, 0, piecesList);
            board[1, 0] = CreateSinglePiece(PieceType.Knight, TeamType.White, 1, 0, piecesList);
            board[2, 0] = CreateSinglePiece(PieceType.Bishop, TeamType.White, 2, 0, piecesList);
            board[3, 0] = CreateSinglePiece(PieceType.Queen, TeamType.White, 3, 0, piecesList);
            board[4, 0] = CreateSinglePiece(PieceType.King, TeamType.White, 4, 0, piecesList);
            board[5, 0] = CreateSinglePiece(PieceType.Bishop, TeamType.White, 5, 0, piecesList);
            board[6, 0] = CreateSinglePiece(PieceType.Knight, TeamType.White, 6, 0, piecesList);
            board[7, 0] = CreateSinglePiece(PieceType.Rook, TeamType.White, 7, 0, piecesList);

            board[0, 7] = CreateSinglePiece(PieceType.Rook, TeamType.Black, 0, 7, piecesList);
            board[1, 7] = CreateSinglePiece(PieceType.Knight, TeamType.Black, 1, 7, piecesList);
            board[2, 7] = CreateSinglePiece(PieceType.Bishop, TeamType.Black, 2, 7, piecesList);
            board[3, 7] = CreateSinglePiece(PieceType.Queen, TeamType.Black, 3, 7, piecesList);
            board[4, 7] = CreateSinglePiece(PieceType.King, TeamType.Black, 4, 7, piecesList);
            board[5, 7] = CreateSinglePiece(PieceType.Bishop, TeamType.Black, 5, 7, piecesList);
            board[6, 7] = CreateSinglePiece(PieceType.Knight, TeamType.Black, 6, 7, piecesList);
            board[7, 7] = CreateSinglePiece(PieceType.Rook, TeamType.Black, 7, 7, piecesList);

            for (var x = 0; x < BOARD_X_SIZE; x++){
                board[x, 1] = CreateSinglePiece(PieceType.Pawn, TeamType.White, x, 1, piecesList);
                board[x, 6] = CreateSinglePiece(PieceType.Pawn, TeamType.Black, x, 6, piecesList);
            }
        }

        private Piece CreateSinglePiece(PieceType pieceType, TeamType teamType, int x, int y, List<Piece> piecesList){
            var piece = InstantiatePieceType(pieceType);
            piece.teamType = teamType;
            piece.pieceType = pieceType;
            piece.enemyTeamType = teamType.Equals(TeamType.White) ? TeamType.Black : TeamType.White;
            piece.beforeFirstMove = true;
            piece.GetComponent<MeshRenderer>().material = teamColors[(int) teamType];
            piece.transform.position = new Vector3(0, 0.1f, 0);
            piece.xCord = x;
            piece.yCord = y;
            if (pieceType.Equals(PieceType.Knight) && teamType.Equals(TeamType.Black)){
                var rotationVector = piece.transform.rotation.eulerAngles;
                rotationVector.y = 180;
                piece.transform.rotation = Quaternion.Euler(rotationVector);
            }

            piecesList.Add(piece);
            return piece;
        }

        private Piece InstantiatePieceType(PieceType pieceType){
            Piece piece;
            switch (pieceType){
                case PieceType.Pawn:
                    chessPiecesPrefabs[(int) pieceType].layer = LayerMask.NameToLayer("Piece");
                    piece = Instantiate(chessPiecesPrefabs[(int) pieceType], transform).GetComponent<Pawn>();
                    break;
                case PieceType.Knight:
                    chessPiecesPrefabs[(int) pieceType].layer = LayerMask.NameToLayer("Piece");
                    piece = Instantiate(chessPiecesPrefabs[(int) pieceType], transform).GetComponent<Knight>();
                    break;
                case PieceType.Bishop:
                    chessPiecesPrefabs[(int) pieceType].layer = LayerMask.NameToLayer("Piece");
                    piece = Instantiate(chessPiecesPrefabs[(int) pieceType], transform).GetComponent<Bishop>();
                    break;
                case PieceType.Rook:
                    chessPiecesPrefabs[(int) pieceType].layer = LayerMask.NameToLayer("Piece");
                    piece = Instantiate(chessPiecesPrefabs[(int) pieceType], transform).GetComponent<Rook>();
                    break;
                case PieceType.Queen:
                    chessPiecesPrefabs[(int) pieceType].layer = LayerMask.NameToLayer("Piece");
                    piece = Instantiate(chessPiecesPrefabs[(int) pieceType], transform).GetComponent<Queen>();
                    break;
                case PieceType.King:
                    chessPiecesPrefabs[(int) pieceType].layer = LayerMask.NameToLayer("Piece");
                    piece = Instantiate(chessPiecesPrefabs[(int) pieceType], transform).GetComponent<King>();
                    break;
                default:
                    throw new RuntimeWrappedException("Wrong piece type. Game terminated.");
            }

            return piece;
        }
        public void SetPiecesPosition(Piece[,] board){
            for (var x = 0; x < BOARD_X_SIZE; x++){
                for (var y = 0; y < BOARD_Y_SIZE; y++){
                    if (board[x, y] != null){
                        board[x, y].transform.position = SetSinglePiecePosition(board[x, y]);
                    }
                }
            }
        }

        public void PieceUp(Piece piece){
            piece.transform.position = new Vector3(piece.xCord * pieceShiftX, 1f, piece.yCord * pieceShiftY);
        }

        public void PieceDown(Piece piece){
            piece.transform.position = SetSinglePiecePosition(piece);
        }

        public Vector3 SetSinglePiecePosition(Piece piece){
            return piece.pieceType.Equals(PieceType.Pawn)
                ? new Vector3(piece.xCord * pieceShiftX, 0.01f, piece.yCord * pieceShiftY)
                : new Vector3(piece.xCord * pieceShiftX, 0.2f, piece.yCord * pieceShiftY);
        }
    }
}