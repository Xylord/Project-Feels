using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TileObject : MonoBehaviour {

    public LevelGrid grid;
    public GameObject presentTile, nextTile, movementPlane;
    public float offsetFromTile, speed;
    public GameObject[] possibleMoves;
    public int maxMovementPoints;


    private int parsedMoves, routesFound;
    //private float moveProgress;

	// Use this for initialization
	void Start () {
        InitializeTileObject();
    }

    public void InitializeTileObject()
    {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        transform.parent = grid.gameObject.transform;
        presentTile = grid.Grid(0,0);
        grid.Grid(0, 0).GetComponent<BasicTile>().IsOccupied = true;
        nextTile = null;
        //moveProgress = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        Selecting();

        /*if (nextTile == null)
        {
            switch (presentTile.GetComponent<BasicTile>().type)
            {
                case BasicTile.TileKind.Flat:
                    transform.localPosition = presentTile.transform.position + new Vector3(0f, offsetFromTile, 0f);
                    break;

                case BasicTile.TileKind.Stair:
                    transform.localPosition = presentTile.transform.position + new Vector3(0f, offsetFromTile - 0.25f, 0f);
                    break;
            }
        }

        else
        {
            Vector3 oldPosition = Vector3.zero, newPosition = Vector3.zero;

            switch (presentTile.GetComponent<BasicTile>().type)
            {
                case BasicTile.TileKind.Flat:
                    oldPosition = presentTile.transform.position;
                    break;

                case BasicTile.TileKind.Stair:
                    oldPosition = presentTile.transform.position - new Vector3(0f, 0.25f, 0f);
                    break;
            }

            switch (nextTile.GetComponent<BasicTile>().type)
            {
                case BasicTile.TileKind.Flat:
                    newPosition = nextTile.transform.position;
                    break;

                case BasicTile.TileKind.Stair:
                    newPosition = nextTile.transform.position - new Vector3(0f, 0.25f, 0f);
                    break;
            }

            transform.localPosition = Vector3.Lerp(oldPosition, 
                newPosition, moveProgress) 
                + new Vector3(0f, offsetFromTile, 0f);
            moveProgress += Time.deltaTime * speed;

            if(moveProgress >= 1f)
            {
                presentTile.GetComponent<BasicTile>().IsOccupied = false;
                presentTile = nextTile;
                presentTile.GetComponent<BasicTile>().IsOccupied = true;
                nextTile = null;
                moveProgress = 0f;
            }
            return;
        }*/

        if (Input.GetKeyDown(KeyCode.M))
        {
            print("Showing moves");
            ShowMoves();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            print("Trying");
            TestPath(1, 0);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            print("Trying");
            TestPath(-1, 0);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            print("Trying");
            TestPath(0, 1);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            print("Trying");
            TestPath(0, -1);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            print("Trying");
            TestPath(1, 1);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            MoveForward();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveBackward();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveRight();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
        }

    }

    void Selecting()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if(hit)
            {
                GameObject move = hitInfo.transform.gameObject;
                StartCoroutine(FollowRoute(move.GetComponent<MovementPlane>().route));
                ClearMoves();
                return;
            }
        }
    }

    void FollowPath(MovementPlane.Movement[] route)
    {

    }

    IEnumerator FollowRoute(MovementPlane.Movement[] route)
    {
        for(int i = 0; i < route.Length; i++)
        {
            float moveProgress = 0f;

            int X = presentTile.GetComponent<BasicTile>().XPosition,
                Y = presentTile.GetComponent<BasicTile>().YPosition;

            nextTile = grid.Grid(X + route[i].xMovement, Y + route[i].yMovement);

            print(route[i].xMovement + ", " + route[i].yMovement);

            while(moveProgress <= 1f)
            {
                transform.localPosition = CalculateMovement(moveProgress);
                moveProgress += Time.deltaTime * speed;
                yield return null;
            }

            presentTile.GetComponent<BasicTile>().IsOccupied = false;
            presentTile = nextTile;
            presentTile.GetComponent<BasicTile>().IsOccupied = true;
            nextTile = null;
            moveProgress = 0f;
        }

        yield break;
    }

    Vector3 CalculateMovement(float moveProgress)
    {
        Vector3 oldPosition = Vector3.zero, newPosition = Vector3.zero;

        switch (presentTile.GetComponent<BasicTile>().type)
        {
            case BasicTile.TileKind.Flat:
                oldPosition = presentTile.transform.position;
                break;

            case BasicTile.TileKind.Stair:
                oldPosition = presentTile.transform.position - new Vector3(0f, 0.25f, 0f);
                break;
        }

        switch (nextTile.GetComponent<BasicTile>().type)
        {
            case BasicTile.TileKind.Flat:
                newPosition = nextTile.transform.position;
                break;

            case BasicTile.TileKind.Stair:
                newPosition = nextTile.transform.position - new Vector3(0f, 0.25f, 0f);
                break;
        }

        return Vector3.Lerp(oldPosition, newPosition, moveProgress) + new Vector3(0f, offsetFromTile, 0f);
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

        //FindMoves(xPos, yPos, startRoute, routesFound, maxMovementPoints);
        StartCoroutine(NewFindMovesCorout(xPos, yPos, startRoute, routesFound, maxMovementPoints));
        
        
    }

    void FindMoves(int xPos, int yPos, MovementPlane.Movement[] route, int movesFound, int movementPointsLeft)
    {
        //bool N = true, NE = true, E = true, SE = true, S = true, SW = true, W = true, NW = true;
        bool[] knownTiles = { true, true, true, true, true, true, true, true };
        bool[] reachedEndOfLine = { false, false, false, false, false, false, false, false };
        int endedLines = 0;
        //int repeatedTiles = 0, 
        int movesFoundThisRound = 0;
        parsedMoves++;

        int[] straightMoves = new int[8];

        

        MovementPlane.Movement[] directions = new MovementPlane.Movement[8];
        directions[0].xMovement = 1;
        directions[0].yMovement = 0;

        directions[4].xMovement = 1;
        directions[4].yMovement = 1;

        directions[1].xMovement = 0;
        directions[1].yMovement = 1;

        directions[5].xMovement = -1;
        directions[5].yMovement = 1;

        directions[2].xMovement = -1;
        directions[2].yMovement = 0;

        directions[6].xMovement = -1;
        directions[6].yMovement = -1;

        directions[3].xMovement = 0;
        directions[3].yMovement = -1;

        directions[7].xMovement = 1;
        directions[7].yMovement = -1;

        for (int i = 0; i < straightMoves.Length; i++)
        {
            straightMoves[i] = 1;
        }

        int initXPos = presentTile.GetComponent<BasicTile>().XPosition,
            initYPos = presentTile.GetComponent<BasicTile>().YPosition;

        while(endedLines < 8)
        {
            for (int j = 0; j <= 7; j++)
            {
                if (!reachedEndOfLine[j])
                {

                    for (int i = 0; i < movesFound; i++)
                    {
                        int X = possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().XPosition,
                            Y = possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().YPosition,
                            newXPos = xPos, newYPos = yPos;

                        newXPos += straightMoves[j] * directions[j].xMovement;
                        newYPos += straightMoves[j] * directions[j].yMovement;
                        

                        if ((newXPos == initXPos && newYPos == initYPos) ||
                            (newXPos == X && newYPos == Y))
                        {
                            knownTiles[j] = false;
                        }
                    }

                    if (knownTiles[j])
                    {
                        MovementPlane.Movement[] newRoute = new MovementPlane.Movement[route.Length + straightMoves[j]];
                        int newXPos = xPos, newYPos = yPos;
                        for (int k = 0; k < route.Length; k++)
                        {
                            newRoute[k] = route[k];
                        }

                        for (int k = 0; k < straightMoves[j]; k++)
                        {
                            newRoute[route.Length + k] = directions[j];
                        }

                        newXPos += straightMoves[j] * directions[j].xMovement;
                        newYPos += straightMoves[j] * directions[j].yMovement;

                        if (SetupMovement(directions[j], newRoute, movesFound, newXPos, newYPos, movementPointsLeft))
                        {
                            straightMoves[j]++;
                            movesFound++;
                            movesFoundThisRound++;
                        }

                        else
                        {
                            reachedEndOfLine[j] = true;
                            endedLines++;
                        }
                    }

                    else
                    {
                        reachedEndOfLine[j] = true;
                        endedLines++;
                    }
                }
            }
        }

        /*for (int i = 0; i < movesFound; i++)
        {
            int X = possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().XPosition,
                Y = possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().YPosition;

            for (int j = 0; j <= 7; j++)
            {
                if ((xPos + directions[j].xMovement == initXPos && yPos + directions[j].yMovement == initYPos) ||
                    (xPos + directions[j].xMovement == X && yPos + directions[j].yMovement == Y))
                {
                    knownTiles[j] = false;
                }
            }
        }

        //Going all the ways


        for (int i = 0; i <= 7; i++)
        {
            if (knownTiles[i])
            {
                if (SetupMovement(directions[i], route, movesFound, xPos, yPos, movementPointsLeft))
                {
                    movesFound++;
                    movesFoundThisRound++;
                }
            }
        }*/

                /*if (movesFoundThisRound == 0)
                {
                    yield break;
                }*/

        if (possibleMoves[parsedMoves - 1] != null)
        { 
            int X = possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().XPosition,
                Y = possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().YPosition,
                movePointsLeft = maxMovementPoints - possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().movementCost;
            MovementPlane.Movement[] moveRoute = possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().route;

            FindMoves(X, Y, moveRoute, movesFound, movePointsLeft);
        }
        return;
    }

    IEnumerator NewFindMovesCorout(int xPos, int yPos, MovementPlane.Movement[] route, int movesFound, int movementPointsLeft)
    {
        //bool N = true, NE = true, E = true, SE = true, S = true, SW = true, W = true, NW = true;
        bool[] knownTiles = { true, true, true, true, true, true, true, true };
        bool[] reachedEndOfLine = { false, false, false, false, false, false, false, false };
        int endedLines = 0;
        //int repeatedTiles = 0, 
        int movesFoundThisRound = 0;
        parsedMoves++;

        int[] straightMoves = new int[8];



        MovementPlane.Movement[] directions = new MovementPlane.Movement[8];
        directions[0].xMovement = 1;
        directions[0].yMovement = 0;

        directions[4].xMovement = 1;
        directions[4].yMovement = 1;

        directions[1].xMovement = 0;
        directions[1].yMovement = 1;

        directions[5].xMovement = -1;
        directions[5].yMovement = 1;

        directions[2].xMovement = -1;
        directions[2].yMovement = 0;

        directions[6].xMovement = -1;
        directions[6].yMovement = -1;

        directions[3].xMovement = 0;
        directions[3].yMovement = -1;

        directions[7].xMovement = 1;
        directions[7].yMovement = -1;

        for (int i = 0; i < straightMoves.Length; i++)
        {
            straightMoves[i] = 1;
        }

        int initXPos = presentTile.GetComponent<BasicTile>().XPosition,
            initYPos = presentTile.GetComponent<BasicTile>().YPosition;

        while (endedLines < 8)
        {
            for (int j = 0; j <= 7; j++)
            {
                if (!reachedEndOfLine[j])
                {

                    for (int i = 0; i < movesFound; i++)
                    {
                        int X = possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().XPosition,
                            Y = possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().YPosition,
                            newXPos = xPos, newYPos = yPos;

                        newXPos += straightMoves[j] * directions[j].xMovement;
                        newYPos += straightMoves[j] * directions[j].yMovement;


                        if ((newXPos == initXPos && newYPos == initYPos) ||
                            (newXPos == X && newYPos == Y))
                        {
                            knownTiles[j] = false;
                        }
                    }

                    if (knownTiles[j])
                    {
                        MovementPlane.Movement[] newRoute = new MovementPlane.Movement[route.Length + straightMoves[j] - 1];
                        int newXPos = xPos, newYPos = yPos;

                        for (int k = 0; k < route.Length; k++)
                        {
                            newRoute[k] = route[k];
                        }

                        for (int k = 0; k < straightMoves[j] - 1; k++)
                        {
                            newRoute[route.Length + k] = directions[j];
                        }

                        newXPos += (straightMoves[j] - 1) * directions[j].xMovement;
                        newYPos += (straightMoves[j] - 1) * directions[j].yMovement;

                        if (SetupMovement(directions[j], newRoute, movesFound, newXPos, newYPos, movementPointsLeft))
                        {
                            yield return new WaitForSeconds(0.1f);
                            straightMoves[j]++;
                            movesFound++;
                            movesFoundThisRound++;
                        }

                        else
                        {
                            reachedEndOfLine[j] = true;
                            endedLines++;
                        }
                    }

                    else
                    {
                        reachedEndOfLine[j] = true;
                        endedLines++;
                    }
                }
            }
        }

        

        if (possibleMoves[parsedMoves - 1] != null)
        {
            int X = possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().XPosition,
                Y = possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().YPosition,
                movePointsLeft = maxMovementPoints - possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().movementCost;
            MovementPlane.Movement[] moveRoute = possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().route;

            StartCoroutine(NewFindMovesCorout(X, Y, moveRoute, movesFound, movePointsLeft));
        }
        yield break;
    }

    IEnumerator FindMovesCorout(int xPos, int yPos, MovementPlane.Movement[] route, int movesFound, int movementPointsLeft)
    {
        //bool N = true, NE = true, E = true, SE = true, S = true, SW = true, W = true, NW = true;
        bool[] knownTiles = { true, true, true, true, true, true, true, true };

        //int repeatedTiles = 0, 
        int movesFoundThisRound = 0;
        parsedMoves++;

        MovementPlane.Movement[] directions = new MovementPlane.Movement[8];
        directions[0].xMovement = 1;
        directions[0].yMovement = 0;

        directions[1].xMovement = 1;
        directions[1].yMovement = 1;

        directions[2].xMovement = 0;
        directions[2].yMovement = 1;

        directions[3].xMovement = -1;
        directions[3].yMovement = 1;

        directions[4].xMovement = -1;
        directions[4].yMovement = 0;

        directions[5].xMovement = -1;
        directions[5].yMovement = -1;

        directions[6].xMovement = 0;
        directions[6].yMovement = -1;

        directions[7].xMovement = 1;
        directions[7].yMovement = -1;

        int initXPos = presentTile.GetComponent<BasicTile>().XPosition,
            initYPos = presentTile.GetComponent<BasicTile>().YPosition;

        for (int i = 0; i < movesFound; i++)
        {
            int X = possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().XPosition,
                Y = possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().YPosition;

            for (int j = 0; j <= 7; j++)
            {
                if ((xPos + directions[j].xMovement == initXPos && yPos + directions[j].yMovement == initYPos) ||
                    (xPos + directions[j].xMovement == X && yPos + directions[j].yMovement == Y))
                {
                    knownTiles[j] = false;
                }
            }
        }

        //Going all the ways


        for (int i = 0; i <= 7; i++)
        {
            if (knownTiles[i])
            {
                if (SetupMovement(directions[i], route, movesFound, xPos, yPos, movementPointsLeft))
                {
                    yield return new WaitForSeconds(0.2f);
                    movesFound++;
                    movesFoundThisRound++;
                }
            }
        }

        /*if (movesFoundThisRound == 0)
        {
            yield break;
        }*/

        if (possibleMoves[parsedMoves - 1] != null)
        {
            int X = possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().XPosition,
                Y = possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().YPosition,
                movePointsLeft = maxMovementPoints - possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().movementCost;
            MovementPlane.Movement[] moveRoute = possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().route;

            StartCoroutine(FindMovesCorout(X, Y, moveRoute, movesFound, movePointsLeft));
        }
        yield break;
    }

    bool SetupMovement(MovementPlane.Movement move, MovementPlane.Movement[] route, 
        int movesFound, int xPos, int yPos, int movePointsLeft)
    {
        int moveCost = 0;
        bool foundMove = false;
        if (!(xPos + move.xMovement < grid.xSize && xPos + move.xMovement >= 0
            &&
            yPos + move.yMovement < grid.ySize && yPos + move.yMovement >= 0))
            return foundMove;

        else if (grid.Grid(xPos + move.xMovement, yPos + move.yMovement).GetComponent<BasicTile>().Accessible(grid.Grid(xPos, yPos).GetComponent<BasicTile>()))
        {
            foundMove = true;

            for (int i = 0; i < route.Length; i++)
            {
                if (route[i].xMovement != 0 && route[i].yMovement != 0)
                    moveCost += 3;

                else
                    moveCost += 2;
            }

            if (move.xMovement != 0 && move.yMovement != 0)
                moveCost += 3;
            else
                moveCost += 2;
            if (moveCost <= maxMovementPoints)
            {
                possibleMoves[movesFound] = Instantiate(movementPlane);
                possibleMoves[movesFound].GetComponent<MovementPlane>().presentTile = grid.Grid(xPos + move.xMovement, yPos + move.yMovement);
                possibleMoves[movesFound].name = "Move " + (xPos + move.xMovement) + " " + (yPos + move.yMovement);
                possibleMoves[movesFound].GetComponent<MovementPlane>().route = new MovementPlane.Movement[route.Length + 1];

                for (int i = 0; i < route.Length; i++)
                {
                    possibleMoves[movesFound].GetComponent<MovementPlane>().route[i] = route[i];
                }

                possibleMoves[movesFound].GetComponent<MovementPlane>().route[route.Length] = move;

                possibleMoves[movesFound].GetComponent<MovementPlane>().movementCost = moveCost;
                possibleMoves[movesFound].name += " costs " + possibleMoves[movesFound].GetComponent<MovementPlane>().movementCost;

                foundMove = true;
            }
            else
            {
                foundMove = false;
                //MovementPlane.PrintRoute(route);
            }
        }
        return foundMove;
    }

    void TestPath(int varX, int varY)
    {
        int X = presentTile.GetComponent<BasicTile>().XPosition,
            Y = presentTile.GetComponent<BasicTile>().YPosition;
        BasicTile targetTile = grid.Grid(X + varX, Y + varY).GetComponent<BasicTile>();

        if (targetTile.GetComponent<BasicTile>().Accessible(presentTile.GetComponent<BasicTile>()))
        {
            print("CAAAAN DOO");
        }
        else
            print("Wont go there");


    }

    void ClearMoves()
    {
        if (possibleMoves != null)
        {
            for (int i = 0; i <= possibleMoves.Length - 1; i++)
            {
                Destroy(possibleMoves[i]);
            }
            possibleMoves = null;
        }
    }

    void MoveForward()
    {
        int X = presentTile.GetComponent<BasicTile>().XPosition, 
            Y = presentTile.GetComponent<BasicTile>().YPosition;
        
        if(grid.Grid(X + 1, Y).GetComponent<BasicTile>().YPosition != Y)
        {
            nextTile = null;
            print("Out of bounds!");
        }
        else
        {
            nextTile = grid.Grid(X + 1, Y);
        }
    }

    void MoveBackward()
    {
        int X = presentTile.GetComponent<BasicTile>().XPosition,
            Y = presentTile.GetComponent<BasicTile>().YPosition;
        
        if (grid.Grid(X - 1, Y).GetComponent<BasicTile>().YPosition != Y)
        {
            nextTile = null;
            print("Out of bounds!");
        }
        else
        {
            nextTile = grid.Grid(X - 1, Y);
        }
    }

    void MoveRight()
    {
        int X = presentTile.GetComponent<BasicTile>().XPosition,
            Y = presentTile.GetComponent<BasicTile>().YPosition;

        if (grid.ySize <= Y - 1 || 0 > Y - 1)
        {
            nextTile = null;
            print("Out of bounds!");
            return;
        }
        nextTile = grid.Grid(X, Y - 1);
    }

    void MoveLeft()
    {
        int X = presentTile.GetComponent<BasicTile>().XPosition,
            Y = presentTile.GetComponent<BasicTile>().YPosition;

        if (grid.ySize <= Y + 1 || 0 > Y + 1)
        {
            nextTile = null;
            print("Out of bounds!");
            return;
        }
        nextTile = grid.Grid(X, Y + 1);
    }
}
