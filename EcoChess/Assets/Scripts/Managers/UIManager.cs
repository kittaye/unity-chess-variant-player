using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameResults { Checkmate, Stalemate };
public class UIManager : MonoBehaviour {
    public static UIManager Instance;
    public Canvas mainCanvas;
    public GameObject promotionWindow;
    public GameObject defectionWindow;
    public GameObject settingsWindow;
    public GameObject spritePrefab;
    public Button nextVariantButton;
    public Button prevVariantButton;

    private Text teamTurnLbl;
    private Text gameModeLbl;
    private Text promoteToLbl;
    private Text defectToLbl;
    private Text gameLogLbl;
    private List<GameObject> promotionOptions;
    private bool settingsVisible;

    // Use this for initialization
    void Awake () {
        if(Instance != null && Instance != this) {
            Destroy(Instance);
        } else {
            Instance = this;
        }

        promotionOptions = new List<GameObject>();
        GameManager._OnGameFinished += OnGameFinished;
        ChessGameModes.Chess._DisplayPromotionUI += OnDisplayPromotionOptions;
        ChessGameModes.Chess._OnPawnPromotionsChanged += OnPawnPromotionOptionsChanged;

        foreach (Text aText in mainCanvas.GetComponentsInChildren<Text>()) {
            if (aText.name.Equals("TeamTurn_lbl")) {
                teamTurnLbl = aText;
            }
            if (aText.name.Equals("GameMode_lbl")) {
                gameModeLbl = aText;
            }
            if (aText.name.Equals("PromoteTo_lbl")) {
                promoteToLbl = aText;
            }
            if (aText.name.Equals("DefectTo_lbl")) {
                defectToLbl = aText;
            }
            if (aText.name.Equals("GameLog_lbl")) {
                gameLogLbl = aText;
            }
        }
    }

    void Start() {
        UpdateGameModeText();

        if (GameManager.Instance.ChessGame is ChessGameModes.SovereignChess) {
            ChessGameModes.SovereignChess._DisplayDefectionUI += OnDisplayDefectionOptions;
            ChessGameModes.SovereignChess._SetDefectionOptions += OnSetDefectionOptions;
        }

        prevVariantButton.gameObject.SetActive(false);
        nextVariantButton.gameObject.SetActive(false);
        settingsWindow.SetActive(false);

        if (GameManager.GetCurrentVariantIndex() != 0) {
            prevVariantButton.gameObject.SetActive(true);
        }
        if (GameManager.GetCurrentVariantIndex() != GameManager.NUM_OF_VARIANTS - 1) {
            nextVariantButton.gameObject.SetActive(true);
        }

        OnDisplayPromotionOptions(false);
        OnDisplayDefectionOptions(false);
    }

    void OnDestroy() {
        GameManager._OnGameFinished -= OnGameFinished;
        ChessGameModes.Chess._DisplayPromotionUI -= OnDisplayPromotionOptions;
        ChessGameModes.Chess._OnPawnPromotionsChanged -= OnPawnPromotionOptionsChanged;
        if (GameManager.Instance.ChessGame is ChessGameModes.SovereignChess) {
            ChessGameModes.SovereignChess._DisplayDefectionUI -= OnDisplayDefectionOptions;
            ChessGameModes.SovereignChess._SetDefectionOptions -= OnSetDefectionOptions;
        }
    }

    private void OnGameFinished() {
        teamTurnLbl.text = "Finished";
        teamTurnLbl.color = Color.yellow;
    }

    public void LogCheckmate(string winningTeamName, string losingTeamName) {
        gameLogLbl.text = string.Format("Team {0} has been checkmated -- Team {1} wins!", winningTeamName, losingTeamName);
    }

    public void LogStalemate(string stalematerTeamName) {
        gameLogLbl.text = string.Format("Stalemate on {0}'s move!", stalematerTeamName);
    }

    public void LogCapturelessLimit(string stalematerTeamName) {
        gameLogLbl.text = string.Format("No captures or pawn moves in 50 turns. Stalemate on {0}'s move!", stalematerTeamName);
    }

    public void LogCustom(string message) {
        gameLogLbl.text = message;
    }

    public void CreatePawnPromotionOptions(Piece[] pieces) {
        promotionOptions.Clear();
        Button[] buttons = promotionWindow.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++) {
            buttons[i].onClick.RemoveListener(() => SelectPawnPromotion((Piece)System.Enum.Parse(typeof(Piece), buttons[i].name)));
            Destroy(buttons[i].gameObject);
        }

        for (int i = 0; i < pieces.Length; i++) {
            GameObject go = Instantiate(spritePrefab, promotionWindow.transform);
            go.name = pieces[i].ToString();
            go.GetComponent<Image>().sprite = Resources.Load<Sprite>(GameManager.Instance.ChessGame.GetCurrentTeamTurn().ToString() + "_" + go.name);
            int j = i;
            go.GetComponent<Button>().onClick.AddListener(() => SelectPawnPromotion(pieces[j]));
            promotionOptions.Add(go);
        }
    }

    public void OnDisplayPromotionOptions(bool value) {
        promoteToLbl.text = "<color=white>Promote to:\n</color><b>" + (GameManager.Instance.ChessGame.SelectedPawnPromotion.ToString()) + "</b>";
        promotionWindow.SetActive(value);
    }

    public void OnDisplayDefectionOptions(bool value) {
        if (GameManager.Instance.ChessGame is ChessGameModes.SovereignChess) {
            defectToLbl.text = "Click the king again to defect to team\n<b>" + SovereignExtensions.GetColourName(((ChessGameModes.SovereignChess)GameManager.Instance.ChessGame).selectedDefection).ToString() + "</b>";
        } else {
            if (value == true) {
                Debug.LogError("Wrong game mode! Do not use this method in any game mode other than Sovereign Chess.");
                return;
            }
        }
        defectionWindow.SetActive(value);
    }

    public void OnSetDefectionOptions(Color[] clrs) {
        Button[] buttons = defectionWindow.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++) {
            buttons[i].onClick.RemoveListener(() => SelectDefectOption(clrs[i]));
            Destroy(buttons[i].gameObject);
        }

        for (int i = 0; i < clrs.Length; i++) {
            GameObject go = Instantiate(spritePrefab, defectionWindow.transform);
            go.name = clrs[i].ToString();
            go.GetComponent<Image>().color = clrs[i];
            int j = i;
            go.GetComponent<Button>().onClick.AddListener(() => SelectDefectOption(clrs[j]));
        }
    }

    public void OnPawnPromotionOptionsChanged(Piece[] pieces) {
        CreatePawnPromotionOptions(pieces);
    }

    public void SelectPawnPromotion(Piece value) {
        promoteToLbl.text = "<color=white>Promote to:\n</color><b>" + value.ToString() + "</b>";
        GameManager.Instance.ChessGame.SetPawnPromotionTo(value);
    }

    public void SelectDefectOption(Color value) {
        if (GameManager.Instance.ChessGame is ChessGameModes.SovereignChess) {
            defectToLbl.text = "Click the king again to defect to team\n<b>" + SovereignExtensions.GetColourName(value) + "</b>";
            ((ChessGameModes.SovereignChess)GameManager.Instance.ChessGame).SetDefectOptionTo(value);
        }
    }

    public void UpdateGameModeText() {
        gameModeLbl.text = "<color=white><b>Playing: </b></color>" + GameManager.Instance.ChessGame.ToString();
    }

    public void OnTurnComplete() {
        teamTurnLbl.text = GameManager.Instance.ChessGame.GetCurrentTurnLabel();
        Team newCurrentTeam = GameManager.Instance.ChessGame.GetCurrentTeamTurn();

        foreach (GameObject item in promotionOptions) {
            item.GetComponent<Image>().sprite = Resources.Load<Sprite>(newCurrentTeam.ToString() + "_" + item.name);
        }
    }

    public void OnClickSettingsButton() {
        settingsVisible = !settingsVisible;
        settingsWindow.SetActive(settingsVisible);
    }

    public void OnToggleFlipBoard() {
        GameManager.Instance.ChessGame.allowBoardFlipping = !GameManager.Instance.ChessGame.allowBoardFlipping;
    }
}
