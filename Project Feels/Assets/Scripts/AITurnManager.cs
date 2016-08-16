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
    public GameObject mouseOverObject;

    // Use this for initialization
    void Start () {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        spawnedAIs = FindObjectsOfType(typeof(ComputerUnit)) as ComputerUnit[];
        spawnedPCs = FindObjectsOfType(typeof(PlayerCharacter)) as PlayerCharacter[];
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

        for(int i = 0; i < spawnedPCs.Length; i++)
        {
            spawnedPCs[i].NewTurn();
        }
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
            (FindObjectOfType(typeof(AnimateCamera)) as AnimateCamera).TargetObject = spawnedAIs[i].gameObject;
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
}
