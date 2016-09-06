using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class ComputerUnit : TileObject {

    //public Image healthBar;

    public enum AIState
    {
        Pursuing,
        Defending,
        Fleeing,
        Berserk
    }

    private AIState aiState;

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

            if (turnManager.ActingAI == this)
            {
                StateManager();
            }
        }

        RotationUpdate();

        UpdateEffects();
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
                if (targetObject.WithinZMovesFromThis(3, possibleMoves[i].GetComponent<MovementPlane>().PresentTile.GetComponent<BasicTile>(), false))// && targetObject.presentTile.GetComponent<BasicTile>().Accessible(possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>(), true))
                {
                    isMoving = true;
                    /*if (possibleMoves[i].GetComponent<MovementPlane>().movementCost <= maxMovementPoints)
                    {
                        StartCoroutine(FollowRoute(TileObject.TruncateRoute(possibleMoves[i].GetComponent<MovementPlane>().route, maxMovementPoints, true)));
                    }
                    else*/
                    StartCoroutine(FollowRoute(TileObject.TruncateRoute(possibleMoves[i].GetComponent<MovementPlane>().Route, maxMovementPoints, false)));
                    ClearMoves();
                    break;
                }
            }
        }
    }

    public void AttackTargets()
    {
        DisplayAttacks(3, 2, 1, 2);

        float weaknessLevel = 2;
        int weakestTarget = -1;

        for(int i = 0; i < objectsWithinRange.Count; i++)
        {
            if ((float)objectsWithinRange[i].hP / (float)objectsWithinRange[i].maxHP < weaknessLevel)
            {
                weaknessLevel = (float)objectsWithinRange[i].hP / (float)objectsWithinRange[i].maxHP;
                weakestTarget = i;
            }
        }

        if (weakestTarget != -1)
            possibleMoves[objectsInRangeIndex[weakestTarget]].GetComponent<MovementPlane>().ExecuteAttack();
    }

    public void ShowMoveAttackRange()
    {

    }

    public override void FinishedMoving()
    {
        AttackTargets();
        ClearMoves();
        turnManager.ActingAI = null;
    }

}
