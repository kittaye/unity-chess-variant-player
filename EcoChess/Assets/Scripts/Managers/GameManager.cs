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

    public static event System.Action _OnGameFinished;

    void Awake () {
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        } else {
            Instance = this;
        }

        ChessGame = GameModeFactory.Create((GameMode)modeIndex);
        //ChessGame = new DummyVariant();
        ChessGame.PopulateBoard();
        ChessGame.UpdateGameStateHistory();
    }

    private void Start() {
        CenterCameraToBoard(ChessGame.Board);
        ui = UIManager.Instance;
        ui.CreatePawnPromotionOptions(ChessGame.PawnPromotionOptions);
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
        if(!ChessGame.Board.isFlipped && ChessGame.GetCurrentTeamTurn() == Team.WHITE) {
            return;
        } else if(ChessGame.Board.isFlipped && ChessGame.GetCurrentTeamTurn() == Team.BLACK) {
            return;
        }

        ChessGame.Board.isFlipped = !ChessGame.Board.isFlipped;

        mainCamera.transform.Rotate(new Vector3(0, 0, 180));
        foreach (ChessPiece piece in ChessGame.GetAllPieces(true)) {
            piece.gameObject.transform.Rotate(new Vector3(0, 0, 180));
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
        ChessGame.UpdateGameStateHistory();

        if (ChessGame.CheckWinState()) {
            if (_OnGameFinished != null) _OnGameFinished.Invoke();
        }

        ui.OnMoveComplete();

        if (ChessGame.Board.allowFlipping) {
            FlipBoard();
        }
    }

    public void UndoLastGameMove() {
        if (ChessGame.GameMoveNotations.Count > 0) {
            ChessGame.UndoLastMove();
            ui.OnUndoLastGameMove();
        }
    }

    private void CenterCameraToBoard(Board board) {
        float tallestDim = Mathf.Max(board.GetHeight(), board.GetWidth());
        mainCamera.orthographicSize = tallestDim / mainCamera.aspect;
        mainCamera.transform.position = new Vector3(board.GetWidth() / 2f - 0.5f, board.GetHeight() / 2f - 0.5f, mainCamera.gameObject.transform.position.z);
    }

    public void InstantiateChessPiece(ChessPiece piece) {
        piece.gameObject = Instantiate(piecePrefab, piece.GetBoardPosition(), piecePrefab.transform.rotation);
        piece.gameObject.SetActive(true);
        piece.gameObject.name = piece.ToString();
        piece.gameObject.transform.SetParent(ChessGame.Board.gameBoardObj.transform);
        piece.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(piece.ToString());
    }

    public void DestroyChessPiece(ChessPiece piece) {
        Destroy(piece.gameObject);
    }
}
