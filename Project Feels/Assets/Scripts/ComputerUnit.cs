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
    public TileObject targetObject;

	// Use this for initialization
	void Start () {
        InitializeTileObject();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isMoving)
        {
            NotMovingUpdate();

            if (isTurn)
            {
                StateManager();
            }
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

        FindMoves(xPos, yPos, startRoute, routesFound, detectionRange, detectionRange, true);
        
        for(int i = 0; i < objectsWithinRange.Count; i++)
        {
            if (objectsWithinRange[i].team != team)
            {
                targetObject = objectsWithinRange[i];
                break;
            }
        }
        if (targetObject != null)
        {
            for (int i = 0; i < possibleMoves.Length; i++)
            {
                if (targetObject.WithinZMovesFromThis(3, possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>(), true))// && targetObject.presentTile.GetComponent<BasicTile>().Accessible(possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>(), true))
                {
                    isMoving = true;
                    /*if (possibleMoves[i].GetComponent<MovementPlane>().movementCost <= maxMovementPoints)
                    {
                        StartCoroutine(FollowRoute(TileObject.TruncateRoute(possibleMoves[i].GetComponent<MovementPlane>().route, maxMovementPoints, true)));
                    }
                    else*/
                    StartCoroutine(FollowRoute(TileObject.TruncateRoute(possibleMoves[i].GetComponent<MovementPlane>().route, maxMovementPoints, false)));
                    ClearMoves();
                    break;
                }


                //print(possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().CharacterStepping.name);
                /*if(possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().CharacterStepping != null && possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().CharacterStepping.team != team)
                {

                }*/
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
