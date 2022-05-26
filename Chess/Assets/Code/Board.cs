using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code;
using TMPro;
using UnityEngine;
using Random = System.Random;

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
    public TextMeshProUGUI endGameText;
    public TextMeshProUGUI whitePlayerTimerText;
    public TextMeshProUGUI blackPlayerTimerText;
    public AudioSource moveAudio;
    private float whiteTimer;
    private float blackTimer;
    private float computerThinkingTime;
    private bool isPromotionSelection;
    private bool afterPromotion;
    private bool isPickingForbidden;
    private bool stopGame;
    private bool isComputerThinking;
    private bool isGameAgainstComputer;
    private List<Move> listOfMoves;

    void Update(){
        if (!stopGame)
            HandleTimer();
        else
            return;

        if (isPromotionSelection)
            return;

        if (afterPromotion){
            afterPromotion = false;
            ChangeCurrentPlayerAndCalculateMoves();
        }

        if (!isComputerThinking)
            HandleHumanPlayerMove();

        if (isGameAgainstComputer)
            HandleComputerPlayerMove();
    }

    private void Awake(){
        board = new Piece[8, 8];
        GUIController = GetComponent<BoardGUIController>();
        piecesList = new List<Piece>();
        currentPlayer = TeamType.White;
        GUIController.FillBoardWithPieces(board, piecesList);
        GUIController.SetPiecesPosition(board);
        CheckMateDetector.CalculateAndRemoveIllegalMoves(board, piecesList, currentPlayer);
        listOfMoves = new List<Move>();
        whiteTimer = MainMenuController.selectedGameTimer;
        blackTimer = MainMenuController.selectedGameTimer;
        isGameAgainstComputer = MainMenuController.isGameAgainstComputer;
        DisplayTime(whiteTimer, whitePlayerTimerText);
        DisplayTime(blackTimer, blackPlayerTimerText);
    }

    private void HandleHumanPlayerMove(){
        if (_camera == null){
            _camera = Camera.current;
            return;
        }

        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (isPickingForbidden){
            return;
        }

        PieceClicked(ray, out raycastHit);
        PieceDropped(ray, out raycastHit);
    }

    private void HandleComputerPlayerMove(){
        if (currentPlayer == TeamType.White){
            return;
        }

        if (!isComputerThinking){
            var random = new Random();
            var selectedGameTimer = MainMenuController.selectedGameTimer;
            var maxRange = Mathf.FloorToInt(0.1f * selectedGameTimer);
            var minRange = 1;
            computerThinkingTime = random.Next(minRange, maxRange);
            isComputerThinking = true;
            return;
        }

        computerThinkingTime -= Time.deltaTime;
        if (computerThinkingTime > 0){
            return;
        }

        isComputerThinking = false;


        var blackPieces = piecesList.FindAll(piece => piece.teamType == TeamType.Black);
        var computerMoveWrapper = AIPlayer.MakeMove(blackPieces);
        var pieceSelectedByAI = computerMoveWrapper.selectedPiece;
        selectedPiece = pieceSelectedByAI;
        var selectedFieldCords = computerMoveWrapper.selectedField;
        if (board[(int) selectedFieldCords.x, (int) selectedFieldCords.y] != null){
            DestroyPiece((int) selectedFieldCords.x, (int) selectedFieldCords.y);
        }

        if (!HandleCastling(selectedFieldCords) && !HandleEnPassant(selectedFieldCords)){
            MovePieceTo(selectedFieldCords);
        }

        HandleComputerPromotion(selectedFieldCords);
        ChangeCurrentPlayerAndCalculateMoves();
    }

    private void HandleTimer(){
        if (currentPlayer == TeamType.White){
            whiteTimer = whiteTimer > 0 ? whiteTimer - Time.deltaTime : 0;
            DisplayTime(whiteTimer, whitePlayerTimerText);

            if (whiteTimer == 0){
                ShowEndGameGUI("Black");
            }
        }

        if (currentPlayer == TeamType.Black){
            blackTimer = blackTimer > 0 ? blackTimer - Time.deltaTime : 0;
            DisplayTime(blackTimer, blackPlayerTimerText);
            if (blackTimer == 0){
                ShowEndGameGUI("White");
            }
        }
    }

    private void DisplayTime(float timer, TextMeshProUGUI playerTimerText){
        var minutes = Mathf.FloorToInt(timer / 60);
        var seconds = Mathf.FloorToInt(timer % 60);
        var milliSeconds = (timer % 1) * 1000;
        playerTimerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliSeconds);
    }

    private void PieceDropped(Ray ray, out RaycastHit raycastHit){
        if (Physics.Raycast(ray, out raycastHit, 200, LayerMask.GetMask("HighlightedField"))){
            selectedField = raycastHit.transform.gameObject;
            var vector = GUIController.FindSelectedFieldCords(selectedField);
            if (!Input.GetMouseButtonUp(0) || selectedPiece == null){
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
        if (!Physics.Raycast(ray, out raycastHit, 200, LayerMask.GetMask("Field"))){
            return;
        }

        selectedField = raycastHit.transform.gameObject;
        var vector2 = GUIController.FindSelectedFieldCords(selectedField);
        var currentlySelectedPiece = board[(int) vector2.x, (int) vector2.y];
        if (!Input.GetMouseButton(0) || currentlySelectedPiece == null){
            return;
        }

        if (currentlySelectedPiece.teamType != currentPlayer){
            return;
        }

        if (selectedPiece == null){
            selectedPiece = currentlySelectedPiece;
            GUIController.HandleFieldHighlighting(selectedPiece);
            GUIController.PieceUp(selectedPiece);
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
            var winner = currentPlayer == TeamType.White ? TeamType.Black.ToString() : TeamType.White.ToString();
            ShowEndGameGUI(winner);
            return;
        }

        if (isGameAgainstComputer){
            return;
        }

        ChangeCamera();
    }

    private void ShowEndGameGUI(string winner){
        endGameUI.SetActive(true);
        isPickingForbidden = true;
        endGameText.SetText(winner + " wins!");
        stopGame = true;
        listOfMoves.ForEach(move => Debug.Log(move.ToString()));
    }

    private void MovePieceTo(Vector2 vector){
        board[(int) vector.x, (int) vector.y] = selectedPiece;
        board[selectedPiece.xCord, selectedPiece.yCord] = null;
        selectedPiece.SetCords((int) vector.x, (int) vector.y);
        selectedPiece.transform.position = GUIController.SetSinglePiecePosition(selectedPiece);
        selectedPiece.beforeFirstMove = false;
        selectedPiece.incrementNumberOfMoves();
        var move = selectedPiece.FindMoveByCords((int) vector.x, (int) vector.y);
        listOfMoves.Add(move);
        moveAudio.Play();
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
        if (selectedPiece.pieceType != PieceType.Pawn){
            return false;
        }

        var move = selectedPiece.FindMoveByCords((int) selectedFieldCords.x, (int) selectedFieldCords.y);
        if (move.MoveType == MoveType.Promotion){
            promotionUI.SetActive(true);
            isPickingForbidden = true;
            listOfMoves.Add(move);
            return true;
        }

        return false;
    }

    private void HandleComputerPromotion(Vector2 selectedFieldCords){
        if (selectedPiece.pieceType != PieceType.Pawn){
            return;
        }

        var move = selectedPiece.FindMoveByCords((int) selectedFieldCords.x, (int) selectedFieldCords.y);
        if (move.MoveType == MoveType.Promotion){
            var x = (int) selectedFieldCords.x;
            var y = (int) selectedFieldCords.y;
            TeamType teamType = selectedPiece.teamType;
            DestroyPiece(x, y);
            board[x, y] = GUIController.CreateSinglePiece(PieceType.Queen, teamType, x, y, piecesList);
            board[x, y].transform.position = GUIController.SetSinglePiecePosition(board[x, y]);
            selectedPiece = null;
        }
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
        isPickingForbidden = false;
        Move promotion = listOfMoves.Last();
        promotion.PromotedPieceType = pieceType;
    }

    private bool HandleCastling(Vector2 selectedFieldCords){
        if (!selectedPiece.pieceType.Equals(PieceType.King) && !selectedPiece.beforeFirstMove){
            return false;
        }

        var y = selectedPiece.teamType.Equals(TeamType.White) ? 0 : 7;
        var move = selectedPiece.FindMoveByCords((int) selectedFieldCords.x, (int) selectedFieldCords.y);
        if (move.MoveType.Equals(MoveType.ShortCastling)){
            ChangePositionDuringCastling(5, 7, y);
            ChangePositionDuringCastling(6, 4, y);
            listOfMoves.Add(move);
            moveAudio.Play();
            return true;
        }

        if (move.MoveType.Equals(MoveType.LongCastling)){
            ChangePositionDuringCastling(3, 0, y);
            ChangePositionDuringCastling(2, 4, y);
            listOfMoves.Add(move);
            moveAudio.Play();
            return true;
        }

        return false;
    }

    private bool HandleEnPassant(Vector2 selectedFieldCords){
        if (selectedPiece.pieceType != PieceType.Pawn){
            return false;
        }

        var move = selectedPiece.FindMoveByCords((int) selectedFieldCords.x, (int) selectedFieldCords.y);
        if (move.MoveType == MoveType.EnPassant){
            var y = selectedPiece.teamType == TeamType.White
                ? (int) selectedFieldCords.y - 1
                : (int) selectedFieldCords.y + 1;
            MovePieceTo(selectedFieldCords);
            DestroyPiece((int) selectedFieldCords.x, y);
            moveAudio.Play();
            listOfMoves.Add(move);
            return true;
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