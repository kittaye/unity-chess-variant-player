using UnityEngine;
using System.Collections.Generic;

public class MouseController : MonoBehaviour {
    public Camera mainCamera;
    public LayerMask boardLayer;

    private bool hasSelection;
    private bool gameFinished = false;
    private Collider hoveredObj = null;
    private List<BoardCoord> lastOccupierAvailableMoves = new List<BoardCoord>();
    private BoardCoord lastSelectedCoord;
    private Vector3 lastMousePos;
    private readonly Color defaultColor = new Color(0.001f, 0.001f, 0.001f, 1f);
    private readonly Color highlightColor = new Color(0.1f, 0.1f, 0.1f);
    private Chess chessGame;

    private void Awake() {
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

                if (hasSelection && lastOccupierAvailableMoves.Contains(GetHoveredBoardCoord())) {
                    if (chessGame.MovePiece(chessGame.board.GetCoordInfo(lastSelectedCoord).occupier, GetHoveredBoardCoord())) {
                        GameManager.Instance.OnTurnComplete();
                    }
                    DeSelect();
                    return;
                }

                if (selectedCoord.occupier != null && selectedCoord.occupier.GetTeam() == chessGame.GetCurrentTeamTurn()) {
                    UIManager.Instance.OnDisplayPromotionOptions(false);
                    hasSelection = true;
                    lastSelectedCoord = GetHoveredBoardCoord();
                    lastOccupierAvailableMoves = chessGame.CalculateAvailableMoves(selectedCoord.occupier);
                    chessGame.board.HighlightCoordinates(lastOccupierAvailableMoves.ToArray());
                    return;
                }
            }
        } else if (Input.GetMouseButtonDown(1)) {
            if (hasSelection) {
                DeSelect();
            }
        }
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
        lastSelectedCoord = BoardCoord.NULL;
        lastOccupierAvailableMoves.Clear();
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
