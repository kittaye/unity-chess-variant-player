using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;
    public Canvas mainCanvas;
    public GameObject promotionWindow;
    public GameObject promoterPrefab;

    private Text teamTurnLbl;
    private Text gameModeLbl;
    private Text promoteToLbl;
    private List<GameObject> promotionOptions;

    // Use this for initialization
    void Awake () {
        if(Instance != null && Instance != this) {
            Destroy(Instance);
        } else {
            Instance = this;
        }

        promotionOptions = new List<GameObject>();
        ChessGameModes.FIDERuleset._DisplayPromotionUI += OnDisplayPromotionOptions;
        ChessGameModes.FIDERuleset._OnPawnPromotionsChanged += OnPawnPromotionOptionsChanged;

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
        }

        OnDisplayPromotionOptions(false);
    }

    void OnDestroy() {
        ChessGameModes.FIDERuleset._DisplayPromotionUI -= OnDisplayPromotionOptions;
        ChessGameModes.FIDERuleset._OnPawnPromotionsChanged -= OnPawnPromotionOptionsChanged;
    }

    public void CreatePawnPromotionOptions(Piece[] pieces) {
        promotionOptions.Clear();
        Button[] buttons = promotionWindow.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++) {
            buttons[i].onClick.RemoveListener(() => SelectPawnPromotion((Piece)System.Enum.Parse(typeof(Piece), buttons[i].name)));
            Destroy(buttons[i].gameObject);
        }

        for (int i = 0; i < pieces.Length; i++) {
            GameObject go = Instantiate(promoterPrefab, promotionWindow.transform);
            go.name = pieces[i].ToString();
            go.GetComponent<Image>().sprite = Resources.Load<Sprite>(GameManager.Instance.chessGame.GetCurrentTeamTurn().ToString() + "_" + go.name);
            int j = i;
            go.GetComponent<Button>().onClick.AddListener(() => SelectPawnPromotion(pieces[j]));
            promotionOptions.Add(go);
        }
    }

    public void OnDisplayPromotionOptions(bool value) {
        promotionWindow.SetActive(value);
    }

    public void OnPawnPromotionOptionsChanged(Piece[] pieces) {
        promoteToLbl.text = "<color=white>Promote to:\n</color><b>" + ((ChessGameModes.FIDERuleset)GameManager.Instance.chessGame).selectedPawnPromotion.ToString() + "</b>";
        CreatePawnPromotionOptions(pieces);
    }

    public void SelectPawnPromotion(Piece value) {
        promoteToLbl.text = "<color=white>Promote to:\n</color><b>" + value.ToString() + "</b>";
        ((ChessGameModes.FIDERuleset)GameManager.Instance.chessGame).SetPawnPromotionTo(value);
    }

    void Start() {
        UpdateGameModeText();
    }

    public void UpdateGameModeText() {
        gameModeLbl.text = "<color=white><b>Playing: </b></color>" + GameManager.Instance.chessGame.ToString();
    }

    public void OnTurnComplete() {
        Team newCurrentTeam = GameManager.Instance.chessGame.GetCurrentTeamTurn();
        if (newCurrentTeam == Team.WHITE) {
            teamTurnLbl.text = "White's move";
            teamTurnLbl.color = Color.white;
        } else {
            teamTurnLbl.text = "Black's move";
            teamTurnLbl.color = Color.black;
        }

        foreach (GameObject item in promotionOptions) {
            item.GetComponent<Image>().sprite = Resources.Load<Sprite>(newCurrentTeam.ToString() + "_" + item.name);
        }
    }
}
