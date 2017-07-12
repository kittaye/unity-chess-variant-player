using UnityEngine;

public class ChessObjectSpawner : MonoBehaviour {
    public static ChessObjectSpawner Instance;
    public GameObject piecePrefab;

    public GameObject boardChunkPrefab;
    public Color primaryBoardColour;
    public Color secondaryBoardColour;

    private const int MAX_DIM = 26;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    public GameObject InstantiateGameBoard(Board board) {
        if (board.GetWidth() > MAX_DIM || board.GetHeight() > MAX_DIM) {
            Debug.LogWarning(string.Format("Board dimensions greater than {0} will not have naming support.", MAX_DIM));
        }

        GameObject gameBoardObj = new GameObject("Board");

        //Create the board from the bottom up, row by row.
        for (int y = 0; y < board.GetHeight(); y++) {
            for (int x = 0; x < board.GetWidth(); x++) {
                //Instantiate board piece at (x,y) and parent it to the board.
                GameObject go = Instantiate(boardChunkPrefab, new Vector3(x, y), boardChunkPrefab.transform.rotation);
                go.transform.SetParent(gameBoardObj.transform);

                //Rename piece to match the board coordinate its on.
                if (board.ContainsCoord(new BoardCoord(x, y))) {
                    go.name = board.GetCoordInfo(new BoardCoord(x, y)).algebraicKey;
                    board.GetCoordInfo(new BoardCoord(x, y)).boardChunk = go;
                }

                //Alternate piece colour with each instantiation.
                Material mat = go.GetComponent<Renderer>().material;
                mat.color = ((y + x) % 2 != 0) ? primaryBoardColour : secondaryBoardColour;
            }
        }
        return gameBoardObj;
    }

    public static void InstantiateChessPiece(Chess chessGame, GameObject piecePrefab, ChessPiece piece) {
        piece.gameObject = Instantiate(piecePrefab, piece.GetBoardPosition(), piecePrefab.transform.rotation);
        piece.gameObject.SetActive(true);
        piece.gameObject.name = piece.ToString();
        piece.gameObject.transform.SetParent(chessGame.board.gameBoardObj.transform);
        piece.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(piece.ToString());
    }
}