using UnityEngine;
using UnityEngine.SceneManagement;
using ChessGameModes;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    public Chess ChessGame { get; private set; }
    public static readonly int NUM_OF_VARIANTS = System.Enum.GetNames(typeof(GameMode)).Length;

    public Camera mainCamera;
    public GameObject piecePrefab;
    public GameObject boardChunkPrefab;

    private static int modeIndex = (int)0;
    private UIManager ui;
    private GameObject gameBoardObj;

    public static event System.Action _OnGameFinished;

    void Awake () {
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        } else {
            Instance = this;
        }

        // Initalise the chess game.
        {
            ChessGame = GameModeFactory.Create((GameMode)modeIndex);

            ChessDebugLogger.InitLogListeners(ChessGame);

            ChessGame.OnChessPieceCreated += ChessGame_OnChessPieceCreated;
            ChessGame.OnChessPieceDestroyed += ChessGame_OnChessPieceDestroyed;
            ChessGame.OnChessPieceMoved += ChessGame_OnChessPieceMoved;
            ChessGame.OnChessPieceCaptured += ChessGame_OnChessPieceCaptured;
            ChessGame.OnUndoChessPieceCapture += ChessGame_OnUndoChessPieceCapture;

            ChessGame.Board.OnShowBoardChunkObject += Board_OnShowBoardChunkObject;
            ChessGame.Board.OnHideBoardChunkObject += Board_OnHideBoardChunkObject;
            ChessGame.Board.OnDestroyBoardChunkObject += Board_OnDestroyBoardChunkObject;

            gameBoardObj = InstantiateGameBoard(ChessGame.Board, boardChunkPrefab);

            ChessGame.PopulateBoard();

            ChessGame.IncrementGameAndPieceStateHistory();
        }
    }

    private void Start() {
        CenterCameraToBoard(ChessGame.Board);
        ui = UIManager.Instance;
        ui.CreatePawnPromotionOptions(ChessGame.PawnPromotionOptions);
    }

    private void ChessGame_OnChessPieceCreated(ChessPiece piece) {
        InstantiateChessPieceObject(piece);
    }

    private void ChessGame_OnChessPieceDestroyed(ChessPiece piece) {
        Destroy((GameObject)piece.graphicalObject);
    }

    private void Board_OnHideBoardChunkObject(object boardChunkObject) {
        ((GameObject)boardChunkObject).SetActive(false);
    }

    private void Board_OnShowBoardChunkObject(object boardChunkObject) {
        ((GameObject)boardChunkObject).SetActive(true);
    }

    private void Board_OnDestroyBoardChunkObject(object boardChunkObject) {
        Destroy((GameObject)boardChunkObject);
    }

    private void ChessGame_OnChessPieceMoved(ChessPiece piece, BoardCoord newPosition) {
        ((GameObject)piece.graphicalObject).transform.position = new Vector3(newPosition.x, newPosition.y, -1);
    }

    private void ChessGame_OnChessPieceCaptured(ChessPiece piece) {
        ((GameObject)piece.graphicalObject).SetActive(false);
    }

    private void ChessGame_OnUndoChessPieceCapture(ChessPiece piece) {
        ((GameObject)piece.graphicalObject).SetActive(true);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.RightArrow) && modeIndex != NUM_OF_VARIANTS - 1) {
            LoadNextVariant();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) && modeIndex != 0) {
            LoadPreviousVariant();
        }
    }

    public void FlipBoard() {
        // Makes sure that the board is never flipped such that the current team to move is at the top.
        if (!ChessGame.Board.isFlipped && ChessGame.GetCurrentTeamTurn() == Team.WHITE) {
            return;
        } else if (ChessGame.Board.isFlipped && ChessGame.GetCurrentTeamTurn() == Team.BLACK) {
            return;
        }

        ChessGame.Board.isFlipped = !ChessGame.Board.isFlipped;

        mainCamera.transform.Rotate(new Vector3(0, 0, 180));
        foreach (ChessPiece piece in ChessGame.GetPiecesOfType<ChessPiece>()) {
            ((GameObject)piece.graphicalObject).transform.Rotate(new Vector3(0, 0, 180));
        }
    }

    public void LoadNextVariant() {
        modeIndex = Mathf.Clamp(++modeIndex, 0, NUM_OF_VARIANTS - 1);
        SceneManager.LoadScene(0);
    }

    public void LoadPreviousVariant() {
        modeIndex = Mathf.Clamp(--modeIndex, 0, NUM_OF_VARIANTS - 1);
        SceneManager.LoadScene(0);
    }

    public static int GetCurrentVariantIndex() {
        return modeIndex;
    }

    public void OnMoveComplete() {
        ChessGame.OnMoveComplete();
        ChessGame.IncrementGameAndPieceStateHistory();

        if (ChessGame.CheckWinState()) {
            if (_OnGameFinished != null) _OnGameFinished.Invoke();
        }

        ui.OnMoveComplete();

        if (ChessGame.Board.allowFlipping) {
            FlipBoard();
        }
    }

    public void UndoLastGameMove() {
        if (ChessGame.UndoLastMove()) {
            ui.OnUndoLastGameMove();
        }
    }

    private void CenterCameraToBoard(Board board) {
        float tallestDim = Mathf.Max(board.GetHeight(), board.GetWidth());
        mainCamera.orthographicSize = tallestDim / mainCamera.aspect;
        mainCamera.transform.position = new Vector3(board.GetWidth() / 2f - 0.5f, board.GetHeight() / 2f - 0.5f, mainCamera.gameObject.transform.position.z);
    }

    private void InstantiateChessPieceObject(ChessPiece piece) {
        BoardCoord piecePosition = piece.GetBoardPosition();
        piece.graphicalObject = Instantiate(piecePrefab, new Vector3(piecePosition.x, piecePosition.y, -1), piecePrefab.transform.rotation);

        GameObject pieceGameObject = (GameObject)piece.graphicalObject;

        pieceGameObject.SetActive(true);
        pieceGameObject.name = piece.ToString();
        pieceGameObject.transform.SetParent(gameBoardObj.transform);

        // For neutral pieces in sovereign chess.
        if (piece.GetTeam() == Team.NONE) {
            pieceGameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("WHITE_" + piece.GetCanonicalName());
        } else {
            pieceGameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(piece.ToString());
        }

        if (piece.GetTeam() == Team.BLACK && ChessGame.Board.isFlipped) {
            pieceGameObject.transform.Rotate(new Vector3(0, 0, 180));
        }
    }

    private GameObject InstantiateGameBoard(Board board, GameObject boardChunkObj) {
        GameObject gameBoardObj = new GameObject("Board");

        for (int x = 0; x < board.GetWidth(); x++) {
            for (int y = 0; y < board.GetHeight(); y++) {
                BoardCoord coord = new BoardCoord(x, y);

                // Ensure the coord still exists as some variants remove squares.
                if (board.ContainsCoord(coord)) {

                    //Instantiate board piece at (x,y) and parent it to the board.
                    GameObject go = Instantiate(boardChunkObj, new Vector3(coord.x, coord.y), boardChunkObj.transform.rotation, gameBoardObj.transform);
                    board.GetCoordInfo(coord).graphicalObject = go;

                    // Set name of the object to be the algebraic key.
                    go.name = board.GetCoordInfo(coord).algebraicKey;

                    //Alternate piece colour with each instantiation.
                    Material mat = go.GetComponent<Renderer>().material;
                    if ((coord.y + coord.x) % 2 != 0) {
                        mat.color = new Color(board.primaryBoardColour.r, board.primaryBoardColour.g, board.primaryBoardColour.b);
                    } else {
                        mat.color = new Color(board.secondaryBoardColour.r, board.secondaryBoardColour.g, board.secondaryBoardColour.b);
                    }
                }
            }
        }

        return gameBoardObj;
    }
}
