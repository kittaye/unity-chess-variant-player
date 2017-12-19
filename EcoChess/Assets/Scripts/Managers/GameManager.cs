using UnityEngine;
using UnityEngine.SceneManagement;
using ChessGameModes;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    public Chess chessGame { get; private set; }
    public static readonly int NUM_OF_VARIANTS = System.Enum.GetNames(typeof(GameMode)).Length;

    public Camera mainCamera;
    public GameObject piecePrefab;
    public GameObject boardChunkPrefab;
    public bool flipBoardEveryTurn = true;

    private static int modeIndex = 0;
    private string lastTurnLabel;
    private UIManager ui;

    public static event System.Action _OnGameFinished;

    void Awake () {
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        } else {
            Instance = this;
        }

        chessGame = GameModeFactory.Create((GameMode)modeIndex);
        chessGame.PopulateBoard();
    }

    private void Start() {
        CenterCameraToBoard(chessGame.board);
        ui = UIManager.Instance;
        ui.CreatePawnPromotionOptions(((FIDERuleset)chessGame).pawnPromotionOptions);
        lastTurnLabel = chessGame.GetCurrentTurnLabel();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.RightArrow) && modeIndex != NUM_OF_VARIANTS - 1) {
            LoadNextVariant();
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) && modeIndex != 0) {
            LoadPreviousVariant();
        }
    }

    public void FlipBoard() {
        mainCamera.transform.Rotate(new Vector3(0, 0, 180));
        foreach (ChessPiece piece in chessGame.GetPieces(true)) {
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

    public void OnTurnComplete() {
        chessGame.OnTurnComplete();
        ui.OnTurnComplete();

        if (chessGame.CheckWinState()) {
            if (_OnGameFinished != null) _OnGameFinished.Invoke();
        }

        if(chessGame.allowBoardFlipping && flipBoardEveryTurn && chessGame.GetCurrentTurnLabel().ToString() != lastTurnLabel) {
            FlipBoard();
        }
        lastTurnLabel = chessGame.GetCurrentTurnLabel().ToString();
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
        piece.gameObject.transform.SetParent(chessGame.board.gameBoardObj.transform);
        piece.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(piece.ToString());
    }
}
