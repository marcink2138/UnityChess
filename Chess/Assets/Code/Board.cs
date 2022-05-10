using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Code;
using Code.ChessPieces;
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
    private List<Piece> piecesList;
    private GameObject[,] fields;
    private List<GameObject> highlightedFields;
    private int BOARD_X_SIZE = 8;
    private int BOARD_Y_SIZE = 8;
    private Piece selectedPiece;

    private TeamType currentPlayer;

    private Camera _camera;
    private GameObject selectedField;
    private Coroutine corutine;

    void Update(){
        HandleHumanPlayerMove();
    }

    private void Awake(){
        board = new Piece[8, 8];
        fields = new GameObject[8, 8];
        highlightedFields = new List<GameObject>();
        piecesList = new List<Piece>();
        currentPlayer = TeamType.White;
        FillBoardWithPieces();
        CreateFields();
        SetPiecesPosition();
        CheckMateDetector.CalculateAndRemoveIllegalMoves(board, piecesList, currentPlayer);
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

    private void HandleHumanPlayerMove(){
        if (_camera == null){
            _camera = Camera.current;
            return;
        }

        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, 200, LayerMask.GetMask("Field"))){
            selectedField = raycastHit.transform.gameObject;
            var vector2 = FindSelectedFieldCords();
            var currentlySelectedPiece = board[(int) vector2.x, (int) vector2.y];
            if (Input.GetMouseButton(0) && currentlySelectedPiece != null){
                if (currentlySelectedPiece.teamType == currentPlayer){
                    if (selectedPiece == null){
                        selectedPiece = currentlySelectedPiece;
                        HandleFieldHighlighting();
                        PieceUp(selectedPiece);
                    }
                }
            }
        }

        if (Physics.Raycast(ray, out raycastHit, 200, LayerMask.GetMask("HighlightedField"))){
            selectedField = raycastHit.transform.gameObject;
            var vector = FindSelectedFieldCords();
            if (Input.GetMouseButtonUp(0) && selectedPiece != null){
                if (board[(int) vector.x, (int) vector.y] != null){
                    piecesList.Remove(board[(int) vector.x, (int) vector.y]);
                    Destroy(board[(int) vector.x, (int) vector.y].gameObject);
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
                currentPlayer = currentPlayer == TeamType.White ? TeamType.Black : TeamType.White;
                CheckMateDetector.CalculateAndRemoveIllegalMoves(board, piecesList, currentPlayer);
                CheckMateDetector.CheckMateDetection(piecesList, currentPlayer);
                ChangeCamera();
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

    private void ChangeCamera(){
        Transform cameraTransform = _camera.transform;
        Vector3 position, rotation;
        if (corutine != null){
            StopCoroutine(corutine);
        }

        if (currentPlayer == TeamType.White){
            position = new Vector3(5, 15, -5);
            rotation = new Vector3(50, 0, 0);
            corutine = StartCoroutine(LerpFromTo(rotation, position, 5f, cameraTransform));
        }
        else{
            position = new Vector3(5, 15, 15.5f);
            rotation = new Vector3(50, 180, 0);
            corutine = StartCoroutine(LerpFromTo(rotation, position, 5f, cameraTransform));
        }
    }

    private IEnumerator LerpFromTo(Vector3 rotation, Vector3 destination, float duration, Transform objectTransform){
        for (float t = 0f; t < duration; t += Time.deltaTime){
            objectTransform.position = Vector3.Lerp(objectTransform.position, destination, t / duration);
            objectTransform.rotation = Quaternion.Lerp(objectTransform.rotation, Quaternion.Euler(rotation),
                t / duration);
            yield return 0;
        }

        objectTransform.position = destination;
        objectTransform.rotation = Quaternion.Euler(rotation);
    }

    private void HandleFieldHighlighting(){
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

    private bool HandleCastling(Vector2 selectedFieldCords){
        if (selectedPiece.pieceType.Equals(PieceType.King) && selectedPiece.beforeFirstMove){
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
        foreach (var highlightedField in highlightedFields){
            highlightedField.layer = LayerMask.NameToLayer("Field");
            highlightedField.GetComponent<MeshRenderer>().material = fieldsMaterials[(int) FieldLayer.Field];
        }

        highlightedFields.Clear();
    }

    private Vector2 FindSelectedFieldCords(){
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
        board[0, 0] = CreateSinglePiece(PieceType.Rook, TeamType.White, 0, 0);
        board[1, 0] = CreateSinglePiece(PieceType.Knight, TeamType.White, 1, 0);
        board[2, 0] = CreateSinglePiece(PieceType.Bishop, TeamType.White, 2, 0);
        board[3, 0] = CreateSinglePiece(PieceType.Queen, TeamType.White, 3, 0);
        board[4, 0] = CreateSinglePiece(PieceType.King, TeamType.White, 4, 0);
        board[5, 0] = CreateSinglePiece(PieceType.Bishop, TeamType.White, 5, 0);
        board[6, 0] = CreateSinglePiece(PieceType.Knight, TeamType.White, 6, 0);
        board[7, 0] = CreateSinglePiece(PieceType.Rook, TeamType.White, 7, 0);

        board[0, 7] = CreateSinglePiece(PieceType.Rook, TeamType.Black, 0, 7);
        board[1, 7] = CreateSinglePiece(PieceType.Knight, TeamType.Black, 1, 7);
        board[2, 7] = CreateSinglePiece(PieceType.Bishop, TeamType.Black, 2, 7);
        board[3, 7] = CreateSinglePiece(PieceType.Queen, TeamType.Black, 3, 7);
        board[4, 7] = CreateSinglePiece(PieceType.King, TeamType.Black, 4, 7);
        board[5, 7] = CreateSinglePiece(PieceType.Bishop, TeamType.Black, 5, 7);
        board[6, 7] = CreateSinglePiece(PieceType.Knight, TeamType.Black, 6, 7);
        board[7, 7] = CreateSinglePiece(PieceType.Rook, TeamType.Black, 7, 7);

        for (var x = 0; x < BOARD_X_SIZE; x++){
            board[x, 1] = CreateSinglePiece(PieceType.Pawn, TeamType.White, x, 1);
            board[x, 6] = CreateSinglePiece(PieceType.Pawn, TeamType.Black, x, 6);
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
        return piece.pieceType.Equals(PieceType.Pawn)
            ? new Vector3(piece.xCord * pieceShiftX, 0.01f, piece.yCord * pieceShiftY)
            : new Vector3(piece.xCord * pieceShiftX, 0.2f, piece.yCord * pieceShiftY);
    }

    private Piece CreateSinglePiece(PieceType pieceType, TeamType teamType, int x, int y){
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
}