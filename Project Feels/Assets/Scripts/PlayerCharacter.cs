using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlayerCharacter : TileObject {

    public bool isSelected, mouseOver;


	// Use this for initialization
	void Start () {
        InitializeTileObject();
    }
	
	// Update is called once per frame
	void Update () {
        if (isSelected)
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);

        else if (mouseOver)
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        
        else
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);


        if (!isMoving)
        {
            NotMovingUpdate();

            if (turnManager.PlayerTurn)
            {
                Selecting();

                if (Input.GetKeyDown(KeyCode.M) && isSelected)
                {
                    print("Showing moves");
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
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        if (hit)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (hitInfo.transform.gameObject.GetComponent<PlayerCharacter>() == this)
                {
                    hitInfo.transform.gameObject.GetComponent<PlayerCharacter>().isSelected = true;
                }
                else if (hitInfo.transform.gameObject.GetComponent<MovementPlane>() != null && isSelected)
                {
                    isSelected = false;
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
        turnManager.PlayerTurnEnd();
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

        FindMoves(xPos, yPos, startRoute, routesFound, maxMovementPoints, maxMovementPoints, true);
        //StartCoroutine(NewFindMovesCorout(xPos, yPos, startRoute, routesFound, maxMovementPoints));


    }


}
