using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlayerCharacter : TileObject {

	// Use this for initialization
	void Start () {
        InitializeTileObject();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isMoving)
        {
            Selecting();

            NotMovingUpdate();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            print("Showing moves");
            ShowMoves();
        }

    }

    void Selecting()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (hit)
            {
                isMoving = true;
                GameObject move = hitInfo.transform.gameObject;
                StartCoroutine(FollowRoute(TileObject.TruncateRoute(move.GetComponent<MovementPlane>().route, 32, false)));
                ClearMoves();
                return;
            }
        }
    }

    public override void FinishedMoving()
    {
        grid.turnManager.spawnedAIs[0].IsTurn = true;
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
