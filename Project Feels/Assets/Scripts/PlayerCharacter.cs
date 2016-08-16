using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class PlayerCharacter : TileObject {

    //public bool mouseOver;
    

	// Use this for initialization
	void Start () {
        InitializeTileObject();
    }
	
	// Update is called once per frame
	void Update () {
        if (turnManager.SelectedUnit == this)
        {
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
            descriptionBox.enabled = true;
        }

        else if (turnManager.mouseOverObject == gameObject)
        {
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            descriptionBox.enabled = true;
        }

        else
        {
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            descriptionBox.enabled = false;
        }


        if (!isMoving)
        {
            NotMovingUpdate();

            if (turnManager.PlayerTurn)
            {
                Selecting();

                if (Input.GetKeyDown(KeyCode.B))
                {
                    turnManager.PlayerTurnEnd();
                }

                if (Input.GetKeyDown(KeyCode.A) && turnManager.SelectedUnit == this)
                {
                    DisplayAttacks(8, 1, 3);
                }

                if (Input.GetKeyDown(KeyCode.M) && turnManager.SelectedUnit == this)
                {
                    ShowMoves();
                }
                
                else if (Input.GetKeyDown(KeyCode.N))
                {
                    FinishedMoving();
                }
            }
        }

    }

    void Selecting()
    {
        RaycastHit hitInfo = new RaycastHit();
        int mask = ~(1 << 8) ;
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, mask);

        if (hit)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (hitInfo.transform.gameObject.GetComponent<PlayerCharacter>() == this)
                {
                    turnManager.SelectedUnit = hitInfo.transform.gameObject.GetComponent<PlayerCharacter>();
                }
                else if (hitInfo.transform.gameObject.GetComponent<MovementPlane>() != null && turnManager.SelectedUnit == this)
                {
                    MovementPlane movePlane = hitInfo.transform.gameObject.GetComponent<MovementPlane>();
                    if (movePlane.Target)
                    {
                        print("Doing attack");
                        movePlane.ExecuteAttack();
                    }
                    else
                    {
                        isMoving = true;
                        GameObject move = hitInfo.transform.gameObject;
                        StartCoroutine(FollowRoute(TileObject.TruncateRoute(move.GetComponent<MovementPlane>().route, 32, false)));
                    }
                    turnManager.SelectedUnit = null;
                    ClearMoves();
                }
            }
            else
            {
                if (hitInfo.transform.gameObject.GetComponent<PlayerCharacter>() == this)
                    turnManager.mouseOverObject = gameObject;
                else if (hitInfo.transform.gameObject.GetComponent<MovementPlane>() != null)
                {
                    if (hitInfo.transform.gameObject.GetComponent<MovementPlane>().Target)
                    {
                        
                        turnManager.mouseOverObject = hitInfo.transform.gameObject;
                    }
                }
            }
        }
        else
            if(turnManager.mouseOverObject == gameObject)
                turnManager.mouseOverObject = null;
    }

    public override void FinishedMoving()
    {

    }

    void ShowMoves()
    {
        ClearMoves();

        int xPos = presentTile.GetComponent<BasicTile>().XPosition,
            yPos = presentTile.GetComponent<BasicTile>().YPosition;

        MovementPlane.Movement[] startRoute = new MovementPlane.Movement[0];

        possibleMoves = new GameObject[grid.xSize * grid.ySize];

        routesFound = 0;
        parsedMoves = 0;

        FindMoves(xPos, yPos, startRoute, routesFound, movementPoints, movementPoints, true);
        //StartCoroutine(NewFindMovesCorout(xPos, yPos, startRoute, routesFound, maxMovementPoints));

    }

    

}
