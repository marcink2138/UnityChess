using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;

public class Board : MonoBehaviour{
    private Piece[,] board;
    private List<Piece> piecesList;
    private int BOARD_X_SIZE = 8;
    private int BOARD_Y_SIZE = 8;
    private Piece selectedPiece;

    private TeamType currentPlayer;

    private Camera _camera;
    private GameObject selectedField;
    private Coroutine corutine;
    private BoardGUIController GUIController;

    void Update(){
        HandleHumanPlayerMove();
    }

    private void Awake(){
        board = new Piece[8, 8];
        GUIController = GetComponent<BoardGUIController>();
        //fields = new GameObject[8, 8];
        //highlightedFields = new List<GameObject>();
        piecesList = new List<Piece>();
        currentPlayer = TeamType.White;
        GUIController.FillBoardWithPieces(board, piecesList);
        //GUIController.CreateFields(transform);
        //CreateFields();
        GUIController.SetPiecesPosition(board);
        CheckMateDetector.CalculateAndRemoveIllegalMoves(board, piecesList, currentPlayer);
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
            var vector2 = GUIController.FindSelectedFieldCords(selectedField);
            var currentlySelectedPiece = board[(int) vector2.x, (int) vector2.y];
            if (Input.GetMouseButton(0) && currentlySelectedPiece != null){
                if (currentlySelectedPiece.teamType == currentPlayer){
                    if (selectedPiece == null){
                        selectedPiece = currentlySelectedPiece;
                        GUIController.HandleFieldHighlighting(selectedPiece);
                        GUIController.PieceUp(selectedPiece);
                    }
                }
            }
        }

        if (Physics.Raycast(ray, out raycastHit, 200, LayerMask.GetMask("HighlightedField"))){
            selectedField = raycastHit.transform.gameObject;
            var vector = GUIController.FindSelectedFieldCords(selectedField);
            if (Input.GetMouseButtonUp(0) && selectedPiece != null){
                if (board[(int) vector.x, (int) vector.y] != null){
                    piecesList.Remove(board[(int) vector.x, (int) vector.y]);
                    Destroy(board[(int) vector.x, (int) vector.y].gameObject);
                }

                if (!HandleCastling(vector)){
                    board[(int) vector.x, (int) vector.y] = selectedPiece;
                    board[selectedPiece.xCord, selectedPiece.yCord] = null;
                    selectedPiece.SetCords((int) vector.x, (int) vector.y);
                    selectedPiece.transform.position = GUIController.SetSinglePiecePosition(selectedPiece);
                    selectedPiece.beforeFirstMove = false;
                }

                GUIController.UnHighlightFields();
                selectedPiece = null;
                currentPlayer = currentPlayer == TeamType.White ? TeamType.Black : TeamType.White;
                CheckMateDetector.CalculateAndRemoveIllegalMoves(board, piecesList, currentPlayer);
                CheckMateDetector.CheckMateDetection(piecesList, currentPlayer);
                ChangeCamera();
            }
        }
        else{
            if (Input.GetMouseButtonUp(0) && selectedPiece != null){
                GUIController.UnHighlightFields();
                GUIController.PieceDown(selectedPiece);
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


    private bool HandleCastling(Vector2 selectedFieldCords){
        if (selectedPiece.pieceType.Equals(PieceType.King) && selectedPiece.beforeFirstMove){
            var y = selectedPiece.teamType.Equals(TeamType.White) ? 0 : 7;
            var move = selectedPiece.FindMoveByCords((int) selectedFieldCords.x, (int) selectedFieldCords.y);
            if (move.MoveType.Equals(MoveType.ShortCastling)){
                board[5, y] = board[7, y];
                board[5, y].SetCords(5, y);
                board[7, y] = null;
                board[5, y].transform.position = GUIController.SetSinglePiecePosition(board[5, y]);
                board[6, y] = board[4, y];
                board[6, y].SetCords(6, y);
                board[4, y] = null;
                board[6, y].transform.position = GUIController.SetSinglePiecePosition(board[6, y]);
                board[5, y].beforeFirstMove = false;
                board[6, y].beforeFirstMove = false;
                return true;
            }

            if (move.MoveType.Equals(MoveType.LongCastling)){
                board[3, y] = board[0, y];
                board[3, y].SetCords(3, y);
                board[0, y] = null;
                board[3, y].transform.position = GUIController.SetSinglePiecePosition(board[3, y]);
                board[2, y] = board[4, y];
                board[2, y].SetCords(2, y);
                board[4, y] = null;
                board[2, y].transform.position = GUIController.SetSinglePiecePosition(board[2, y]);
                board[3, y].beforeFirstMove = false;
                board[2, y].beforeFirstMove = false;
                return true;
            }
        }

        return false;
    }
}