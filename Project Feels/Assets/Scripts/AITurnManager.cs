using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AITurnManager : MonoBehaviour {

    public LevelGrid grid;
    //public List<ComputerUnit> spawnedAIs = new List<ComputerUnit>();
    private ComputerUnit[] spawnedAIs;
    private PlayerCharacter[] spawnedPCs;
    public bool playerTurn;
    private PlayerCharacter selectedUnit;
    private ComputerUnit actingAI;
    public GameObject mouseOverObject;
    private int turnCount;

    // Use this for initialization
    void Start () {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        spawnedAIs = FindObjectsOfType(typeof(ComputerUnit)) as ComputerUnit[];
        spawnedPCs = FindObjectsOfType(typeof(PlayerCharacter)) as PlayerCharacter[];
        turnCount = 0;
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
        (FindObjectOfType(typeof(AnimateCamera)) as AnimateCamera).TargetObject = 
            (FindObjectOfType(typeof(PlayerCharacter)) as PlayerCharacter).gameObject;
        grid.turnManager.PlayerTurn = true;
        spawnedPCs = FindObjectsOfType(typeof(PlayerCharacter)) as PlayerCharacter[];

        actingAI = null;

        for(int i = 0; i < spawnedPCs.Length; i++)
        {
            spawnedPCs[i].NewTurn();
        }

        turnCount++;
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
            actingAI = spawnedAIs[i];
            (FindObjectOfType(typeof(AnimateCamera)) as AnimateCamera).TargetObject = spawnedAIs[i].gameObject;
            while (actingAI == spawnedAIs[i])
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

    public PlayerCharacter SelectedUnit
    {
        get
        {
            return selectedUnit;
        }
        set
        {
            selectedUnit = value;
        }
    }

    public ComputerUnit ActingAI
    {
        get
        {
            return actingAI;
        }
        set
        {
            actingAI = value;
        }
    }

    public int TurnCount
    {
        get
        {
            return turnCount;
        }
        set
        {
            turnCount = value;
        }
    }

}

