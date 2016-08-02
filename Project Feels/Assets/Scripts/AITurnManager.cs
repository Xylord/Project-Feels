using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AITurnManager : MonoBehaviour {

    public LevelGrid grid;
    //public List<ComputerUnit> spawnedAIs = new List<ComputerUnit>();
    ComputerUnit[] spawnedAIs;
    public bool playerTurn;

    // Use this for initialization
    void Start () {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        spawnedAIs = (FindObjectsOfType(typeof(ComputerUnit)) as ComputerUnit[]);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
	
	}

    public void PlayerTurnEnd()
    {
        PlayerTurn = false;
        StartCoroutine(StartCPUTurn());
    }

    public void AITurnEnd()
    {
        grid.turnManager.PlayerTurn = true;
    }

    /*public void StartCPUTurn()
    {
        spawnedAIs[0].IsTurn = true;
    }*/

    IEnumerator StartCPUTurn()
    {
        spawnedAIs = (FindObjectsOfType(typeof(ComputerUnit)) as ComputerUnit[]);
        for (int i = 0; i < spawnedAIs.Length; i++)
        {
            spawnedAIs[i].IsTurn = true;
            while (spawnedAIs[i].IsTurn)
            {
                yield return null;
            }
        }

        AITurnEnd();
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
