using UnityEngine;
using UnityEngine.SceneManagement;
using ChessGameModes;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    public Chess chessGame { get; private set; }
    public GameObject piecePrefab;
    public GameObject boardChunkPrefab;

    private const int NUM_OF_VARIANTS = 30;
    private static int modeIndex = 0;
    private UIManager ui;

    public static event System.Action _OnGameFinished;

    // Use this for initialization
    void Awake () {
        ui = FindObjectOfType<UIManager>();

        if (Instance != null && Instance != this) {
            Destroy(Instance);
        } else {
            Instance = this;
        }

        chessGame = GameModeFactory.Create((GameMode)modeIndex);
        //chessGame = new ChessGameModes.Weak();
        chessGame.PopulateBoard();
        CenterCameraToBoard(chessGame.board);
        ui.CreatePawnPromotionOptions(((FIDERuleset)chessGame).pawnPromotionOptions);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.RightArrow) && modeIndex != NUM_OF_VARIANTS - 1) {
            modeIndex = Mathf.Clamp(++modeIndex, 0, NUM_OF_VARIANTS - 1);
            SceneManager.LoadScene(0);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) && modeIndex != 0) {
            modeIndex = Mathf.Clamp(--modeIndex, 0, NUM_OF_VARIANTS - 1);
            SceneManager.LoadScene(0);
        }
    }

    public void OnTurnComplete() {
        chessGame.OnTurnComplete();
        ui.OnTurnComplete();
        if (chessGame.CheckWinState()) {
            if (_OnGameFinished != null) _OnGameFinished.Invoke();
        }
    }

    private void CenterCameraToBoard(Board board) {
        float tallestDim = (board.GetHeight() > board.GetWidth()) ? board.GetHeight() : board.GetWidth();
        GameObject camera = FindObjectOfType<Camera>().gameObject;
        camera.GetComponent<Camera>().orthographicSize = tallestDim / camera.GetComponent<Camera>().aspect;
        camera.transform.position = new Vector3(board.GetWidth() / 2f - 0.5f, board.GetHeight() / 2f - 0.5f, camera.gameObject.transform.position.z);
    }

    public void InstantiateChessPiece(ChessPiece piece) {
        piece.gameObject = Instantiate(piecePrefab, piece.GetBoardPosition(), piecePrefab.transform.rotation);
        piece.gameObject.SetActive(true);
        piece.gameObject.name = piece.ToString();
        piece.gameObject.transform.SetParent(chessGame.board.gameBoardObj.transform);
        piece.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(piece.ToString());
    }
}
