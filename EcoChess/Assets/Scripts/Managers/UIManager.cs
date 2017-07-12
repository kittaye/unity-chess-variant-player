using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;
    public Canvas mainCanvas;

    private Text teamTurnLbl;
    private Text gameModeLbl;

    // Use this for initialization
    void Awake () {
        if(Instance != null && Instance != this) {
            Destroy(Instance);
        } else {
            Instance = this;
        }

        foreach (Text aText in mainCanvas.GetComponentsInChildren<Text>()) {
            if (aText.name.Equals("TeamTurn_lbl")) {
                teamTurnLbl = aText;
            }
            if (aText.name.Equals("GameMode_lbl")) {
                gameModeLbl = aText;
            }
        }
    }

    void Start() {
        UpdateGameModeText();
    }

    public void UpdateGameModeText() {
        gameModeLbl.text = "<color=white><b>Playing: </b></color>" + GameManager.Instance.chessGame.ToString();
    }

    public void OnTurnComplete() {
        if(GameManager.Instance.chessGame.GetCurrentTeamTurn() == Team.WHITE) {
            teamTurnLbl.text = "White's move";
            teamTurnLbl.color = Color.white;
        } else {
            teamTurnLbl.text = "Black's move";
            teamTurnLbl.color = Color.black;
        }
    }
	
}
