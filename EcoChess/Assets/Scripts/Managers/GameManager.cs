using UnityEngine;
using UnityEngine.SceneManagement;
using ChessGameModes;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    public static int modeIndex = 0;
    public Chess chessGame { get; private set; }

    private const int NUM_OF_VARIANTS = 28;
    private bool gameFinished;
    private UIManager ui;

    // Use this for initialization
    void Awake () {
        ui = FindObjectOfType<UIManager>();

        if (Instance != null && Instance != this) {
            Destroy(Instance);
        } else {
            Instance = this;
        }

        chessGame = GameModeFactory.Create((GameMode)modeIndex);
        //chessGame = new ChessGameModes.FIDERuleset();
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
            gameFinished = true;
        }
    }

    public bool isGameFinished() {
        return gameFinished;
    }

    private void CenterCameraToBoard(Board board) {
        float tallestDim = (board.GetHeight() > board.GetWidth()) ? board.GetHeight() : board.GetWidth();
        GameObject camera = FindObjectOfType<Camera>().gameObject;
        camera.GetComponent<Camera>().orthographicSize = tallestDim / camera.GetComponent<Camera>().aspect;
        camera.transform.position = new Vector3(board.GetWidth() / 2f - 0.5f, board.GetHeight() / 2f - 0.5f, camera.gameObject.transform.position.z);
    }
}
