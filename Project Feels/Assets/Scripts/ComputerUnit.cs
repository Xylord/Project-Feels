using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ComputerUnit : TileObject {
    public enum AIState
    {
        Pursuing,
        Defending,
        Fleeing,
        Berserk
    }

    private AIState aiState;

    
    public bool isTurn;

    public int detectionRange;

	// Use this for initialization
	void Start () {
        InitializeTileObject();
        turnManager.spawnedAIs.Add(this);
    }
	
	// Update is called once per frame
	void Update () {
        if (!isMoving)
        {
            NotMovingUpdate();
            
        }

        if (isTurn && !isMoving)
        {
            StateManager();
        }

    }

    void StateManager()
    {
        ClearMoves();

        int xPos = presentTile.GetComponent<BasicTile>().XPosition,
            yPos = presentTile.GetComponent<BasicTile>().YPosition;

        MovementPlane.Movement[] startRoute = new MovementPlane.Movement[0];

        possibleMoves = new GameObject[grid.xSize * grid.ySize];

        routesFound = 0;
        parsedMoves = 0;

        FindMoves(xPos, yPos, startRoute, routesFound, detectionRange, detectionRange, false);
        

        for(int i = 0; i < possibleMoves.Length; i++)
        {
            //print(possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().CharacterStepping.name);
            if(possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().CharacterStepping != null)
            {
                isMoving = true;
                if (possibleMoves[i].GetComponent<MovementPlane>().movementCost <= maxMovementPoints)
                {
                    print("in eange");
                    StartCoroutine(FollowRoute(TileObject.TruncateRoute(possibleMoves[i].GetComponent<MovementPlane>().route, maxMovementPoints, true)));
                }
                else
                    StartCoroutine(FollowRoute(TileObject.TruncateRoute(possibleMoves[i].GetComponent<MovementPlane>().route, maxMovementPoints, false)));
                ClearMoves();
                break;
            }
        }
        //StartCoroutine(NewFindMovesCorout(xPos, yPos, startRoute, routesFound, maxMovementPoints));


    }

    public override void FinishedMoving()
    {
        isTurn = false;
    }

    public bool IsTurn
    {
        get
        {
            return isTurn;
        }
        set
        {
            isTurn = value;
        }
    }
}
