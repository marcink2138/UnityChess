using System;
using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;

public class Board : MonoBehaviour{
    private Piece[,] board;
    private List<Piece> piecesList;
    private Piece selectedPiece;

    private TeamType currentPlayer;

    private Camera _camera;
    private GameObject selectedField;
    private Coroutine corutine;
    private BoardGUIController GUIController;

    public GameObject promotionUI;
    public GameObject endGameUI;
    private bool isPromotionSelection;
    private bool afterPromotion;

    void Update(){
        if (isPromotionSelection){
            return;
        }

        if (afterPromotion){
            afterPromotion = false;
            ChangeCurrentPlayerAndCalculateMoves();
        }

        HandleHumanPlayerMove();
    }

    private void Awake(){
        board = new Piece[8, 8];
        GUIController = GetComponent<BoardGUIController>();
        piecesList = new List<Piece>();
        currentPlayer = TeamType.White;
        GUIController.FillBoardWithPieces(board, piecesList);
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
        PieceClicked(ray, out raycastHit);

        if (Physics.Raycast(ray, out raycastHit, 200, LayerMask.GetMask("HighlightedField"))){
            selectedField = raycastHit.transform.gameObject;
            var vector = GUIController.FindSelectedFieldCords(selectedField);
            if (!Input.GetMouseButtonUp(0) && selectedPiece == null){
                return;
            }

            if (board[(int) vector.x, (int) vector.y] != null){
                DestroyPiece((int) vector.x, (int) vector.y);
            }

            if (!HandleCastling(vector) && !HandleEnPassant(vector)){
                MovePieceTo(vector);
            }

            isPromotionSelection = HandlePromotion(vector);
            if (!isPromotionSelection){
                ChangeCurrentPlayerAndCalculateMoves();
            }

            GUIController.UnHighlightFields();
        }
        else{
            if (Input.GetMouseButtonUp(0) && selectedPiece != null){
                GUIController.UnHighlightFields();
                GUIController.PieceDown(selectedPiece);
                selectedPiece = null;
            }
        }
    }

    private void ChangeCurrentPlayerAndCalculateMoves(){
        selectedPiece = null;
        currentPlayer = currentPlayer == TeamType.White ? TeamType.Black : TeamType.White;
        CheckMateDetector.CalculateAndRemoveIllegalMoves(board, piecesList, currentPlayer);
        HandleCheck();
        HandleCheckMate();
    }

    private void PieceClicked(Ray ray, out RaycastHit raycastHit){
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
    }

    private void HandleCheck(){
        Piece piece = CheckMateDetector.CheckDetection(piecesList, currentPlayer);
        if (piece != null)
            GUIController.SetMaterialOnCheckedField(currentPlayer, piece.xCord, piece.yCord);
        else
            GUIController.UnSetMaterialOnCheckedField(currentPlayer);
    }

    private void HandleCheckMate(){
        if (CheckMateDetector.CheckMateDetection(piecesList, currentPlayer)){
            endGameUI.SetActive(true);
            return;
        }

        ChangeCamera();
    }

    private void MovePieceTo(Vector2 vector){
        board[(int) vector.x, (int) vector.y] = selectedPiece;
        board[selectedPiece.xCord, selectedPiece.yCord] = null;
        selectedPiece.SetCords((int) vector.x, (int) vector.y);
        selectedPiece.transform.position = GUIController.SetSinglePiecePosition(selectedPiece);
        selectedPiece.beforeFirstMove = false;
    }

    private void DestroyPiece(int x, int y){
        piecesList.Remove(board[x, y]);
        Destroy(board[x, y].gameObject);
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

    private bool HandlePromotion(Vector2 selectedFieldCords){
        if (selectedPiece.pieceType == PieceType.Pawn){
            var move = selectedPiece.FindMoveByCords((int) selectedFieldCords.x, (int) selectedFieldCords.y);
            if (move.MoveType == MoveType.Promotion){
                promotionUI.SetActive(true);
                return true;
            }
        }

        return false;
    }

    public void GetPromotedPieceTypeFromGUI(String pieceTypeString){
        PieceType pieceType = (PieceType) Enum.Parse(typeof(PieceType), pieceTypeString);
        var x = selectedPiece.xCord;
        var y = selectedPiece.yCord;
        TeamType teamType = selectedPiece.teamType;
        DestroyPiece(x, y);
        board[x, y] = GUIController.CreateSinglePiece(pieceType, teamType, x, y, piecesList);
        board[x, y].transform.position = GUIController.SetSinglePiecePosition(board[x, y]);
        selectedPiece = null;
        promotionUI.SetActive(false);
        isPromotionSelection = false;
        afterPromotion = true;
    }

    private bool HandleCastling(Vector2 selectedFieldCords){
        if (selectedPiece.pieceType.Equals(PieceType.King) && selectedPiece.beforeFirstMove){
            var y = selectedPiece.teamType.Equals(TeamType.White) ? 0 : 7;
            var move = selectedPiece.FindMoveByCords((int) selectedFieldCords.x, (int) selectedFieldCords.y);
            if (move.MoveType.Equals(MoveType.ShortCastling)){
                ChangePositionDuringCastling(5, 7, y);
                ChangePositionDuringCastling(6, 4, y);
                return true;
            }

            if (move.MoveType.Equals(MoveType.LongCastling)){
                ChangePositionDuringCastling(3, 0, y);
                ChangePositionDuringCastling(2, 4, y);
                return true;
            }
        }

        return false;
    }

    private bool HandleEnPassant(Vector2 selectedFieldCords){
        if (selectedPiece.pieceType == PieceType.Pawn){
            var move = selectedPiece.FindMoveByCords((int) selectedFieldCords.x, (int) selectedFieldCords.y);
            if (move.MoveType == MoveType.EnPassant){
                var y = selectedPiece.teamType == TeamType.White
                    ? (int) selectedFieldCords.y - 1
                    : (int) selectedFieldCords.y + 1;
                MovePieceTo(selectedFieldCords);
                DestroyPiece((int) selectedFieldCords.x, y);
                return true;
            }
        }

        return false;
    }

    private void ChangePositionDuringCastling(int toX, int fromX, int y){
        board[toX, y] = board[fromX, y];
        board[toX, y].SetCords(toX, y);
        board[fromX, y] = null;
        board[toX, y].transform.position = GUIController.SetSinglePiecePosition(board[toX, y]);
        board[toX, y].beforeFirstMove = false;
    }
}