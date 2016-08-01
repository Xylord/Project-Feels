using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AITurnManager : MonoBehaviour {

    public LevelGrid grid;
    public List<ComputerUnit> spawnedAIs = new List<ComputerUnit>();
    private bool playerTurn;

    // Use this for initialization
    void Start () {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	
	}

    public void StartCPUTurn()
    {
        spawnedAIs[0].IsTurn = true;
    }

    public bool PlayerTurn
    {
        get
        {
            return playerTurn;
        }
        set
        {
            playerTurn = value;
        }
    }
}
