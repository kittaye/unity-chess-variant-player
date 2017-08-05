using UnityEngine;
using System.Collections.Generic;

public class MouseController : MonoBehaviour {
    public static MouseController Instance;
    public Camera mainCamera;
    public LayerMask boardLayer;
    public ChessPiece lastSelectedOccupier { get; private set; }

    private bool hasSelection;
    private bool gameFinished = false;
    private Collider hoveredObj = null;
    private List<BoardCoord> lastSelectedOccupierAvailableMoves = new List<BoardCoord>();
    private Vector3 lastMousePos;
    private readonly Color defaultColor = new Color(0.001f, 0.001f, 0.001f, 1f);
    private readonly Color highlightColor = new Color(0.1f, 0.1f, 0.1f);
    private Chess chessGame;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(Instance);
        } else {
            Instance = this;
        }

        GameManager._OnGameFinished += OnGameFinished;
    }

    private void Start() {
        chessGame = GameManager.Instance.chessGame;
    }

    private void OnDestroy() {
        GameManager._OnGameFinished -= OnGameFinished;
    }

    private void OnGameFinished() {
        gameFinished = true;
    }

    void Update () {
        UpdateHoveredObj();
        if (gameFinished) return;

        if (Input.GetMouseButtonDown(0)) {
            if (hoveredObj != null) {
                CoordInfo selectedCoord = chessGame.board.GetCoordInfo(GetHoveredBoardCoord());

                if (hasSelection && lastSelectedOccupierAvailableMoves.Contains(GetHoveredBoardCoord())) {
                    if (chessGame.MovePiece(lastSelectedOccupier, GetHoveredBoardCoord())) {
                        GameManager.Instance.OnTurnComplete();
                    }
                    DeSelect();
                    return;
                }

                if (selectedCoord.occupier != null && chessGame.IsMoversTurn(selectedCoord.occupier)) {
                    UIManager.Instance.OnDisplayPromotionOptions(false);
                    hasSelection = true;
                    lastSelectedOccupier = chessGame.board.GetCoordInfo(GetHoveredBoardCoord()).occupier;
                    CalculateLastOccupierAvailableMoves();
                    return;
                }
            }
        } else if (Input.GetMouseButtonDown(1)) {
            if (hasSelection) {
                DeSelect();
            }
        }
    }

    public void CalculateLastOccupierAvailableMoves() {
        chessGame.board.RemoveHighlightedCoordinates();
        lastSelectedOccupierAvailableMoves = chessGame.CalculateAvailableMoves(lastSelectedOccupier);
        chessGame.board.HighlightCoordinates(lastSelectedOccupierAvailableMoves.ToArray());
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

    private void DeSelect() {
        hasSelection = false;
        lastSelectedOccupierAvailableMoves.Clear();
        UIManager.Instance.OnDisplayPromotionOptions(false);
        chessGame.board.RemoveHighlightedCoordinates();
    }

    private BoardCoord GetHoveredBoardCoord() {
        if (hoveredObj != null) {
            BoardCoord coord;
            if (chessGame.board.TryGetCoordWithKey(hoveredObj.name, out coord)) {
                return coord;
            }
        }
        Debug.LogWarning("No board value is currently selected! Returning (-1,-1)...");
        return BoardCoord.NULL;
    }
}
