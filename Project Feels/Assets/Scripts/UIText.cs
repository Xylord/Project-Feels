using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIText : MonoBehaviour {


    private AITurnManager turnManager;
    private Text turn, turnCount;

	// Use this for initialization
	void Start () {
        turnManager = GameObject.Find("AITurnManager").GetComponent<AITurnManager>();
        turn = transform.GetChild(0).GetComponent<Text>();
        turnCount = transform.GetChild(1).GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        turn.text = turnManager.ActingAI == null ? "Player turn" : turnManager.ActingAI.name;
        turnCount.text = "Turn " + turnManager.TurnCount;
	}
}
