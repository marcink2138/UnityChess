using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Code;
using Code.ChessPieces;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour{
    [SerializeField] private GameObject[] chessPiecesPrefabs;
    [SerializeField] private Material[] teamColors;
    [SerializeField] private Material[] fieldsMaterials;

    [SerializeField] private float pieceShiftX = 1.5f;
    [SerializeField] private float pieceShiftY = 1.5f;
    [SerializeField] private float fieldSizeScale = 0.15f;
    [SerializeField] private float fieldXyShift = 0.75f;
    [SerializeField] private float fieldZShift = 0.1f;

    private Piece[,] board;
    private GameObject[,] fields;
    private List<GameObject> highlithedFields;
    private int BOARD_X_SIZE = 8;
    private int BOARD_Y_SIZE = 8;
    private Piece selectedPiece;

    private Camera _camera;
    private GameObject selectedField;

    // Start is called before the first frame update
    void Start(){ }

    // Update is called once per frame
    void Update(){
        HighLiteField();
    }

    private void Awake(){
        board = new Piece[8, 8];
        fields = new GameObject[8, 8];
        highlithedFields = new List<GameObject>();
        FillBoardWithPieces();
        CreateFields();
        SetPiecesPosition();
    }

    private void CreateFields(){
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

    private void HighLiteField(){
        if (_camera == null){
            _camera = Camera.current;
            return;
        }

        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, 200, LayerMask.GetMask("Field"))){
            /*if (selectedField != null)
            {
                selectedField.layer = LayerMask.NameToLayer("Field");
                selectedField.GetComponent<MeshRenderer>().material = fieldsMaterials[(int) FieldLayer.Field];
            }

            var field = raycastHit.transform.gameObject;
            selectedField = field;
            //highlitedField.layer = LayerMask.NameToLayer("HighlitedField");
            //highlitedField.GetComponent<MeshRenderer>().material = fieldsMaterials[(int) FieldLayer.HighlitedField];*/
            /*if (Input.GetMouseButton(0))
            {
                if (selectedField != raycastHit.transform.gameObject)
                {
                    foreach (var highlithedField in highlithedFields)
                    {
                        highlithedField.layer = LayerMask.NameToLayer("Field");
                        highlithedField.GetComponent<MeshRenderer>().material = fieldsMaterials[(int) FieldLayer.Field];
                    }

                    highlithedFields.Clear();
                    if (selectedPiece != null)
                    {
                        selectedPiece.transform.position = SetSinglePiecePosition(selectedPiece);
                    }
                }

                selectedField = raycastHit.transform.gameObject;
                Vector2 selectedFieldCords = findSelectedFieldCords();
                selectedPiece = board[(int) selectedFieldCords.x, (int) selectedFieldCords.y];
                selectedPiece.transform.position = new Vector3(selectedPiece.transform.position.x, 1,
                    selectedPiece.transform.position.z);
                selectedPiece.CalculatePossibleMoves(board);
                foreach (var possibleMove in selectedPiece.possibleMoves)
                {
                    fields[possibleMove.x, possibleMove.y].layer = LayerMask.NameToLayer("HighlitedField");
                    fields[possibleMove.x, possibleMove.y].GetComponent<MeshRenderer>().material =
                        fieldsMaterials[(int) FieldLayer.HighlitedField];
                    highlithedFields.Add(fields[possibleMove.x, possibleMove.y]);
                }
            }*/
        }

        if (Physics.Raycast(ray, out raycastHit, 200, LayerMask.GetMask("Field"))){
            selectedField = raycastHit.transform.gameObject;
            var vector2 = findSelectedFieldCords();
            var currentlySelectedPiece = board[(int) vector2.x, (int) vector2.y];
            if (Input.GetMouseButton(0) && currentlySelectedPiece != null){
                if (selectedPiece == null /*!= currentlySelectedPiece*/){
                    if (selectedPiece != null){
                        UnHighlightFields();
                        PieceDown(selectedPiece);
                    }

                    selectedPiece = currentlySelectedPiece;

                    selectedPiece.CalculatePossibleMoves(board);
                    foreach (var possibleMove in selectedPiece.possibleMoves){
                        fields[possibleMove.x, possibleMove.y].layer = LayerMask.NameToLayer("HighlitedField");
                        fields[possibleMove.x, possibleMove.y].GetComponent<MeshRenderer>().material =
                            possibleMove.MoveType.Equals(MoveType.Attack)
                                ? fieldsMaterials[(int) FieldLayer.HighliteAttackMaterial]
                                : fieldsMaterials[(int) FieldLayer.HighlitedField];
                        highlithedFields.Add(fields[possibleMove.x, possibleMove.y]);
                    }

                    PieceUp(selectedPiece);
                }
            }
        }

        if (Physics.Raycast(ray, out raycastHit, 200, LayerMask.GetMask("HighlitedField"))){
            selectedField = raycastHit.transform.gameObject;
            var vector = findSelectedFieldCords();
            if (Input.GetMouseButtonUp(0) && selectedPiece != null){
                if (board[(int) vector.x, (int) vector.y] != null){
                    board[(int) vector.x, (int) vector.y].transform.position = new Vector3(10, 10, 10);
                    board[(int) vector.x, (int) vector.y] = null;
                }

                if (!HandleCastling(vector)){
                    board[(int) vector.x, (int) vector.y] = selectedPiece;
                    board[selectedPiece.xCord, selectedPiece.yCord] = null;
                    selectedPiece.SetCords((int) vector.x, (int) vector.y);
                    selectedPiece.transform.position = SetSinglePiecePosition(selectedPiece);
                    selectedPiece.beforeFirstMove = false;
                }

                UnHighlightFields();
                selectedPiece = null;
            }
        }
        else{
            if (Input.GetMouseButtonUp(0) && selectedPiece != null){
                UnHighlightFields();
                PieceDown(selectedPiece);
                selectedPiece = null;
            }
        }
    }

    private bool HandleCastling(Vector2 selectedFieldCords){
        if (selectedPiece.chessPieceType.Equals(ChessPieceType.King) && selectedPiece.beforeFirstMove){
            var y = selectedPiece.teamType.Equals(TeamType.White) ? 0 : 7;
            var move = selectedPiece.FindMoveByCords((int) selectedFieldCords.x, (int) selectedFieldCords.y);
            if (move.MoveType.Equals(MoveType.ShortCastling)){
                board[5, y] = board[7, y];
                board[5, y].SetCords(5, y);
                board[7, y] = null;
                board[5, y].transform.position = SetSinglePiecePosition(board[5, y]);
                board[6, y] = board[4, y];
                board[6, y].SetCords(6, y);
                board[4, y] = null;
                board[6, y].transform.position = SetSinglePiecePosition(board[6, y]);
                board[5, y].beforeFirstMove = false;
                board[6, y].beforeFirstMove = false;
                return true;
            }

            if (move.MoveType.Equals(MoveType.LongCastling)){
                board[3, y] = board[0, y];
                board[3, y].SetCords(3, y);
                board[0, y] = null;
                board[3, y].transform.position = SetSinglePiecePosition(board[3, y]);
                board[2, y] = board[4, y];
                board[2, y].SetCords(2, y);
                board[4, y] = null;
                board[2, y].transform.position = SetSinglePiecePosition(board[2, y]);
                board[3, y].beforeFirstMove = false;
                board[2, y].beforeFirstMove = false;
                return true;
            }
        }

        return false;
    }

    private void UnHighlightFields(){
        foreach (var highlithedField in highlithedFields){
            highlithedField.layer = LayerMask.NameToLayer("Field");
            highlithedField.GetComponent<MeshRenderer>().material = fieldsMaterials[(int) FieldLayer.Field];
        }

        highlithedFields.Clear();
    }

    private Vector2 findSelectedFieldCords(){
        for (int x = 0; x < BOARD_X_SIZE; x++){
            for (int y = 0; y < BOARD_Y_SIZE; y++){
                if (fields[x, y] == selectedField){
                    return new Vector2(x, y);
                }
            }
        }

        return new Vector2(-1, -1);
    }

    private void FillBoardWithPieces(){
        board[0, 0] = CreateSinglePiece(ChessPieceType.Rook, TeamType.White, 0, 0);
        board[1, 0] = CreateSinglePiece(ChessPieceType.Knight, TeamType.White, 1, 0);
        board[2, 0] = CreateSinglePiece(ChessPieceType.Bishop, TeamType.White, 2, 0);
        board[3, 0] = CreateSinglePiece(ChessPieceType.Queen, TeamType.White, 3, 0);
        board[4, 0] = CreateSinglePiece(ChessPieceType.King, TeamType.White, 4, 0);
        board[5, 0] = CreateSinglePiece(ChessPieceType.Bishop, TeamType.White, 5, 0);
        board[6, 0] = CreateSinglePiece(ChessPieceType.Knight, TeamType.White, 6, 0);
        board[7, 0] = CreateSinglePiece(ChessPieceType.Rook, TeamType.White, 7, 0);

        board[0, 7] = CreateSinglePiece(ChessPieceType.Rook, TeamType.Black, 0, 7);
        board[1, 7] = CreateSinglePiece(ChessPieceType.Knight, TeamType.Black, 1, 7);
        board[2, 7] = CreateSinglePiece(ChessPieceType.Bishop, TeamType.Black, 2, 7);
        board[3, 7] = CreateSinglePiece(ChessPieceType.Queen, TeamType.Black, 3, 7);
        board[4, 7] = CreateSinglePiece(ChessPieceType.King, TeamType.Black, 4, 7);
        board[5, 7] = CreateSinglePiece(ChessPieceType.Bishop, TeamType.Black, 5, 7);
        board[6, 7] = CreateSinglePiece(ChessPieceType.Knight, TeamType.Black, 6, 7);
        board[7, 7] = CreateSinglePiece(ChessPieceType.Rook, TeamType.Black, 7, 7);

        for (var x = 0; x < BOARD_X_SIZE; x++){
            board[x, 1] = CreateSinglePiece(ChessPieceType.Pawn, TeamType.White, x, 1);
            board[x, 6] = CreateSinglePiece(ChessPieceType.Pawn, TeamType.Black, x, 6);
        }
    }

    private void SetPiecesPosition(){
        for (var x = 0; x < BOARD_X_SIZE; x++){
            for (var y = 0; y < BOARD_Y_SIZE; y++){
                if (board[x, y] != null){
                    board[x, y].transform.position = SetSinglePiecePosition(board[x, y]);
                }
            }
        }
    }

    private void PieceUp(Piece piece){
        piece.transform.position = new Vector3(piece.xCord * pieceShiftX, 1f, piece.yCord * pieceShiftY);
    }

    private void PieceDown(Piece piece){
        piece.transform.position = SetSinglePiecePosition(piece);
    }

    private Vector3 SetSinglePiecePosition(Piece piece){
        return piece.chessPieceType.Equals(ChessPieceType.Pawn)
            ? new Vector3(piece.xCord * pieceShiftX, 0.01f, piece.yCord * pieceShiftY)
            : new Vector3(piece.xCord * pieceShiftX, 0.2f, piece.yCord * pieceShiftY);
    }

    private Piece CreateSinglePiece(ChessPieceType chessPieceType, TeamType teamType, int x, int y){
        var piece = InstatiatePieceType(chessPieceType);
        piece.teamType = teamType;
        piece.chessPieceType = chessPieceType;
        piece.enemyTeamType = teamType.Equals(TeamType.White) ? TeamType.Black : TeamType.White;
        piece.beforeFirstMove = true;
        piece.GetComponent<MeshRenderer>().material = teamColors[(int) teamType];
        piece.transform.position = new Vector3(0, 0.1f, 0);
        piece.xCord = x;
        piece.yCord = y;
        if (chessPieceType.Equals(ChessPieceType.Knight) && teamType.Equals(TeamType.Black)){
            var rotationVector = piece.transform.rotation.eulerAngles;
            rotationVector.y = 180;
            piece.transform.rotation = Quaternion.Euler(rotationVector);
        }

        return piece;
    }

    private Piece InstatiatePieceType(ChessPieceType chessPieceType){
        Piece piece;
        switch (chessPieceType){
            case ChessPieceType.Pawn:
                chessPiecesPrefabs[(int) chessPieceType].layer = LayerMask.NameToLayer("Piece");
                piece = Instantiate(chessPiecesPrefabs[(int) chessPieceType], transform).GetComponent<Pawn>();
                break;
            case ChessPieceType.Knight:
                chessPiecesPrefabs[(int) chessPieceType].layer = LayerMask.NameToLayer("Piece");
                piece = Instantiate(chessPiecesPrefabs[(int) chessPieceType], transform).GetComponent<Knight>();
                break;
            case ChessPieceType.Bishop:
                chessPiecesPrefabs[(int) chessPieceType].layer = LayerMask.NameToLayer("Piece");
                piece = Instantiate(chessPiecesPrefabs[(int) chessPieceType], transform).GetComponent<Bishop>();
                break;
            case ChessPieceType.Rook:
                chessPiecesPrefabs[(int) chessPieceType].layer = LayerMask.NameToLayer("Piece");
                piece = Instantiate(chessPiecesPrefabs[(int) chessPieceType], transform).GetComponent<Rook>();
                break;
            case ChessPieceType.Queen:
                chessPiecesPrefabs[(int) chessPieceType].layer = LayerMask.NameToLayer("Piece");
                piece = Instantiate(chessPiecesPrefabs[(int) chessPieceType], transform).GetComponent<Queen>();
                break;
            case ChessPieceType.King:
                chessPiecesPrefabs[(int) chessPieceType].layer = LayerMask.NameToLayer("Piece");
                piece = Instantiate(chessPiecesPrefabs[(int) chessPieceType], transform).GetComponent<King>();
                break;
            default:
                throw new RuntimeWrappedException("Wrong piece type. Game terminated.");
        }

        return piece;
    }
}