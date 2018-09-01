using UnityEngine;
using System.Collections.Generic;

public class MouseController : MonoBehaviour {
    public static MouseController Instance;
    public Camera mainCamera;
    public LayerMask boardLayer;
    public GameObject spriteMoverPrefab;
    public ChessPiece lastSelectedOccupier { get; private set; }

    private bool hasSelection;
    private GameObject spriteMover;
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
        spriteMover = Instantiate(spriteMoverPrefab);
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

        if (hasSelection) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = -2;
            spriteMover.transform.position = mousePos;
        }

        if (Input.GetMouseButtonDown(0)) {
            if (hoveredObj != null) {
                CoordInfo selectedCoord = chessGame.Board.GetCoordInfo(GetHoveredBoardCoord());

                if (hasSelection && lastSelectedOccupierAvailableMoves.Contains(GetHoveredBoardCoord())) {
                    if (chessGame.MovePiece(lastSelectedOccupier, GetHoveredBoardCoord())) {
                        GameManager.Instance.OnTurnComplete();
                    }
                    DeSelect();
                    return;

                } else if (selectedCoord.occupier == null || !selectedCoord.occupier.gameObject.activeInHierarchy) {
                    DeSelect();
                    return;

                } else if (chessGame.IsMoversTurn(selectedCoord.occupier)) {
                    List<BoardCoord> selectedOccupierMoves = chessGame.CalculateAvailableMoves(selectedCoord.occupier);
                    if (selectedOccupierMoves.Count > 0) {
                        DeSelect();

                        hasSelection = true;

                        lastSelectedOccupier = selectedCoord.occupier;
                        lastSelectedOccupierAvailableMoves = selectedOccupierMoves;
                        lastSelectedOccupier.gameObject.SetActive(false);

                        spriteMover.GetComponent<SpriteRenderer>().sprite = lastSelectedOccupier.gameObject.GetComponent<SpriteRenderer>().sprite;
                        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        mousePos.z = -2;
                        spriteMover.transform.position = mousePos;
                        spriteMover.SetActive(true);

                        UIManager.Instance.OnDisplayPromotionOptions(false);
                        UIManager.Instance.OnDisplayDefectionOptions(false);

                        chessGame.Board.HighlightCoordinates(selectedOccupierMoves.ToArray());
                    }
                    return;
                }
            }
            DeSelect();

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

    private void DeSelect() {
        hasSelection = false;

        spriteMover.SetActive(false);

        if (lastSelectedOccupier != null) {
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
