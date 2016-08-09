using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteInEditMode]
public class PlayerCharacter : TileObject {

    public bool mouseOver;
    

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

        else if (mouseOver)
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
                    DisplayAttacks(8);
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
                    turnManager.SelectedUnit = null;
                    isMoving = true;
                    GameObject move = hitInfo.transform.gameObject;
                    StartCoroutine(FollowRoute(TileObject.TruncateRoute(move.GetComponent<MovementPlane>().route, 32, false)));
                    ClearMoves();
                }
            }
            else
            {
                if (hitInfo.transform.gameObject.GetComponent<PlayerCharacter>() == this)
                    mouseOver = true;
            }
        }
        else
            mouseOver = false;
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
