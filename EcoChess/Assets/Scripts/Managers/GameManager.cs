using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    public Chess chessGame { get; private set; }

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

        chessGame = new ChessGameModes.ChecklessChess();
        chessGame.PopulateBoard();
        CenterCameraToBoard(chessGame.board);
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
