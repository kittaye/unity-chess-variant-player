using UnityEngine;
using System.Collections.Generic;

public class MouseController : MonoBehaviour {
    public static MouseController Instance;
    public Camera mainCamera;
    public LayerMask boardLayer;
    public GameObject spriteMoverPrefab;
    public ChessPiece lastSelectedOccupier { get; private set; }

    private bool hasSelection;
    private GameObject selectedObject;
    private bool gameFinished = false;
    private Collider hoveredObj = null;
    private List<BoardCoord> lastSelectedOccupierAvailableMoves = new List<BoardCoord>();
    private Vector3 lastMousePos;
    private readonly Color defaultColor = new Color(0.001f, 0.001f, 0.001f, 1f);
    private readonly Color highlightColor = new Color(0.1f, 0.1f, 0.1f);
    private ChessGameModes.Chess chessGame;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        } else {
            Instance = this;
        }

        GameManager._OnGameFinished += OnGameFinished;
    }

    private void Start() {
        chessGame = GameManager.Instance.ChessGame;
        selectedObject = Instantiate(spriteMoverPrefab);
    }

    private void OnDestroy() {
        GameManager._OnGameFinished -= OnGameFinished;
    }

    private void OnGameFinished() {
        gameFinished = true;
    }

    void Update () {
        UpdateHoveredObj();

        if (gameFinished) {
            return;
        }

        if (hasSelection) {
            UpdateSelectedObjPosition();
        }

        // On left-mouse click.
        if (Input.GetMouseButtonDown(0)) {

            // If clicked on nothing, do nothing to current state.
            if(hoveredObj == null) {
                return;
            }

            // Otherwise, get the square that we clicked on.
            CoordInfo selectedCoord = chessGame.Board.GetCoordInfo(GetHoveredBoardCoord());

            // If we have already selected a piece, and it is placed down on a valid square, move it.
            if (hasSelection && lastSelectedOccupierAvailableMoves.Contains(GetHoveredBoardCoord())) {
                if (chessGame.MovePiece(lastSelectedOccupier, GetHoveredBoardCoord())) {
                    GameManager.Instance.OnMoveComplete();
                }
                DeSelect();
                return;
            }

            // If we clicked on an empty square or on a piece that is inactive, deselect.
            if (selectedCoord.occupier == null || !selectedCoord.occupier.gameObject.activeInHierarchy) {
                DeSelect();
                return;
            }

            string temp = "";
            for (int i = 0; i < selectedCoord.occupier.MoveStateHistory.ToArray().Length; i++) {
                temp += chessGame.Board.GetCoordInfo(selectedCoord.occupier.MoveStateHistory.ToArray()[i].position).algebraicKey + ", ";
            }
            Debug.Log(temp);

            // Otherwise, check if the selected piece is on the current mover's team.
            if (chessGame.IsMoversTurn(selectedCoord.occupier)) {
                DeSelect();

                // If so, calculate it's moves and display visual graphics for moving it around.
                List<BoardCoord> selectedOccupierMoves = chessGame.CalculateAvailableMoves(selectedCoord.occupier);
                if (selectedOccupierMoves.Count > 0) {
                    hasSelection = true;
                    lastSelectedOccupier = selectedCoord.occupier;
                    lastSelectedOccupierAvailableMoves = selectedOccupierMoves;
                    lastSelectedOccupier.gameObject.SetActive(false);

                    selectedObject.GetComponent<SpriteRenderer>().sprite = lastSelectedOccupier.gameObject.GetComponent<SpriteRenderer>().sprite;
                    selectedObject.GetComponent<SpriteRenderer>().material.color = lastSelectedOccupier.gameObject.GetComponent<SpriteRenderer>().material.color;
                    UpdateSelectedObjPosition();
                    selectedObject.gameObject.transform.rotation = Quaternion.identity;
                    if (chessGame.Board.isFlipped) {
                        selectedObject.gameObject.transform.Rotate(new Vector3(0, 0, 180));
                    }
                    selectedObject.SetActive(true);

                    chessGame.Board.HighlightCoordinates(selectedOccupierMoves.ToArray());
                }
                return;
            }

            // Else if we right-clicked with a selection, deselect.
        } else if (Input.GetMouseButtonDown(1)) {
            if (hasSelection) {
                DeSelect();
            }
        }
    }

    public void CalculateLastOccupierAvailableMoves() {
        chessGame.Board.RemoveHighlightedCoordinates();
        lastSelectedOccupierAvailableMoves = chessGame.CalculateAvailableMoves(lastSelectedOccupier);
        chessGame.Board.HighlightCoordinates(lastSelectedOccupierAvailableMoves.ToArray());
    }

    private void UpdateHoveredObj() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, boardLayer)) {
            if (hoveredObj == null || (hoveredObj != null && hoveredObj != hit.collider)) {
                if (hoveredObj != null) hoveredObj.GetComponent<Renderer>().material.SetColor("_EmissionColor", defaultColor);
                hoveredObj = hit.collider;
                Material mat = hoveredObj.GetComponent<Renderer>().material;
                mat.SetColor("_EmissionColor", mat.GetColor("_EmissionColor") + highlightColor);
            }
        } else {
            if (hoveredObj != null) {
                hoveredObj.GetComponent<Renderer>().material.SetColor("_EmissionColor", defaultColor);
                hoveredObj = null;
            }
        }
    }

    private void UpdateSelectedObjPosition() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -2;
        selectedObject.transform.position = mousePos;
    }

    private void DeSelect() {
        hasSelection = false;

        selectedObject.SetActive(false);

        if (lastSelectedOccupier != null && lastSelectedOccupier.IsAlive) {
            lastSelectedOccupier.gameObject.SetActive(true);
            lastSelectedOccupierAvailableMoves.Clear();
        }

        UIManager.Instance.OnDisplayPromotionOptions(false);
        UIManager.Instance.OnDisplayDefectionOptions(false);

        chessGame.Board.RemoveHighlightedCoordinates();
    }

    private BoardCoord GetHoveredBoardCoord() {
        if (hoveredObj != null) {
            BoardCoord coord;
            if (chessGame.Board.TryGetCoordWithKey(hoveredObj.name, out coord)) {
                return coord;
            }
        }
        Debug.LogWarning("No board value is currently selected! Returning (-1,-1)...");
        return BoardCoord.NULL;
    }
}
