using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TileObject : MonoBehaviour {

    public LevelGrid grid;
    public GameObject presentTile, nextTile, movementPlane, attackPlane, targetPlane;
    public float offsetFromTile, speed;
    public GameObject[] possibleMoves;
    public List<TileObject> objectsWithinRange = new List<TileObject>();
    public int maxMovementPoints, maxActionPoints;
    public AITurnManager turnManager;
    public int team;
    public int movementPoints, actionPoints;
    public Image descriptionBox;

    [HideInInspector]
    public bool isMoving;

    [HideInInspector]
    public int parsedMoves, routesFound;

	// Use this for initialization
	void Start () {
        InitializeTileObject();
    }

    public void InitializeTileObject()
    {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        transform.parent = grid.gameObject.transform;

        List<GameObject> possibleSpawns = new List<GameObject>();
        for(int i = 0; i < grid.spawnTiles.Count; i++)
        {
            GameObject tile = grid.spawnTiles[i].gameObject;
            for (int j = 0; j < tile.GetComponent<BasicTile>().spawnTags.Length; j++)
            {

                if (gameObject.CompareTag(tile.GetComponent<BasicTile>().spawnTags[j]) && !tile.GetComponent<BasicTile>().IsOccupied)
                {
                    possibleSpawns.Add(tile);
                }
            }
        }

        if (possibleSpawns.Count <= 0)
        {
            presentTile = null;
            print("No spawns!");
        }
        else
        {
            presentTile = possibleSpawns[Random.Range(0, possibleSpawns.Count - 1)];
            presentTile.GetComponent<BasicTile>().IsOccupied = true;
            presentTile.GetComponent<BasicTile>().CharacterStepping = this;
            nextTile = null;
        }

        turnManager = GameObject.Find("AITurnManager").GetComponent<AITurnManager>();
        isMoving = false;
        movementPoints = maxMovementPoints;
        actionPoints = maxActionPoints;

        //UI
        descriptionBox = gameObject.transform.FindChild("PlayerCanvas").FindChild("MouseOverText").GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!isMoving)
        {
            NotMovingUpdate();
        }
		/*
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
    }

    public void NotMovingUpdate()
    {
        if (nextTile == null)
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
    }

    static public MovementPlane.Movement[] TruncateRoute(MovementPlane.Movement[] truncatedRoute, int newMovementCost, bool removeLastMove)
    {
        List<MovementPlane.Movement> newRoute = new List<MovementPlane.Movement>();
        MovementPlane.Movement[] finalNewRoute = null;
        int moveCost = 0;
        for (int i = 0; i < truncatedRoute.Length; i++)
        {
            if (truncatedRoute[i].xMovement != 0 && truncatedRoute[i].yMovement != 0)
                moveCost += 3;

            else
                moveCost += 2;

            if (moveCost <= newMovementCost)
                newRoute.Add(truncatedRoute[i]);

            else
            {
                break;
            }
        }

        if (removeLastMove)
        {
            finalNewRoute = new MovementPlane.Movement[newRoute.Count - 1];
            for (int j = 0; j < finalNewRoute.Length; j++)
            {
                finalNewRoute[j] = newRoute[j];
            }
        }
        else
        {
            
            finalNewRoute = new MovementPlane.Movement[newRoute.Count];
            finalNewRoute = newRoute.ToArray();
        }

            return finalNewRoute;
    }

    public IEnumerator FollowRoute(MovementPlane.Movement[] route)
    {
        for(int i = 0; i < route.Length; i++)
        {
            float moveProgress = 0f;

            int X = presentTile.GetComponent<BasicTile>().XPosition,
                Y = presentTile.GetComponent<BasicTile>().YPosition;

            nextTile = grid.Grid(X + route[i].xMovement, Y + route[i].yMovement);

            while(moveProgress <= 1f)
            {
                transform.localPosition = CalculateMovement(moveProgress);
                moveProgress += Time.deltaTime * speed;
                yield return null;
            }

            presentTile.GetComponent<BasicTile>().IsOccupied = false;
            presentTile.GetComponent<BasicTile>().CharacterStepping = null;
            presentTile = nextTile;
            presentTile.GetComponent<BasicTile>().IsOccupied = true;
            presentTile.GetComponent<BasicTile>().CharacterStepping = this;
            nextTile = null;
            moveProgress = 0f;
        }
        isMoving = false;
        movementPoints -= MovementPlane.RouteMoveCost(route);
        FinishedMoving();
        yield break;
    }

    public void NewTurn()
    {
        movementPoints = maxMovementPoints;
        actionPoints = maxActionPoints;
    }

    public virtual void FinishedMoving()
    {

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

    public void DisplayAttacks(int attackRange)
    {
        ClearMoves();

        int xPos = presentTile.GetComponent<BasicTile>().XPosition,
            yPos = presentTile.GetComponent<BasicTile>().YPosition,
            maxRadius = attackRange / 2,
            attacksFound = 0;
        
        possibleMoves = new GameObject[(int)Mathf.Pow(maxRadius * 2 + 1, 2)];

        for (int i = -maxRadius; i <= maxRadius; i++)
        {
            for (int j = -maxRadius; j <= maxRadius; j++)
            {
                if (xPos + i < 0 || xPos + i >= grid.xSize)
                    continue;
                
                if (yPos + j < 0 || yPos + j >= grid.ySize)
                    continue;
                
                
                if (WithinZMovesFromThis(attackRange, grid.Grid(xPos + i, yPos + j).GetComponent<BasicTile>(), false) && !(i == 0 && j == 0))
                {
                    if (grid.Grid(xPos + i, yPos + j).GetComponent<BasicTile>().IsOccupied)
                    {
                        objectsWithinRange.Add(grid.Grid(xPos + i, yPos + j).GetComponent<BasicTile>().CharacterStepping);
                        possibleMoves[attacksFound] = Instantiate(targetPlane);
                    }
                    else
                    {
                        possibleMoves[attacksFound] = Instantiate(attackPlane);
                    }

                    possibleMoves[attacksFound].GetComponent<MovementPlane>().presentTile = grid.Grid(xPos + i, yPos + j);
                    possibleMoves[attacksFound].name = "Move " + (xPos + i) + " " + (yPos + j);
                    possibleMoves[attacksFound].GetComponent<MovementPlane>().route = null;
                    attacksFound++;
                }
            }
        }
    }

    public void FindMoves(int xPos, int yPos, MovementPlane.Movement[] route, int movesFound, int movementPointsLeft, int maxMoves, bool unitsBlock)
    {
        bool[] knownTiles = { true, true, true, true, true, true, true, true };
        bool[] reachedEndOfLine = { false, false, false, false, false, false, false, false };
        int endedLines = 0;
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

                        if (SetupMovement(directions[j], newRoute, movesFound, newXPos, newYPos, movementPointsLeft, maxMoves, unitsBlock))
                        {
                            //yield return new WaitForSeconds(0.1f);
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
                movePointsLeft = maxMoves - possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().movementCost;
            MovementPlane.Movement[] moveRoute = possibleMoves[parsedMoves - 1].GetComponent<MovementPlane>().route;

            FindMoves(X, Y, moveRoute, movesFound, movePointsLeft, maxMoves, unitsBlock);
        }
        return;
    }

    /*IEnumerator NewFindMovesCorout(int xPos, int yPos, MovementPlane.Movement[] route, int movesFound, int movementPointsLeft, int maxMoves, bool unitsBlock)
    {
        bool[] knownTiles = { true, true, true, true, true, true, true, true };
        bool[] reachedEndOfLine = { false, false, false, false, false, false, false, false };
        int endedLines = 0;
        int movesFoundThisRound = 0;
        parsedMoves++;

        int[] straightMoves = new int[8];

        MovementPlane.Movement[] directions = new MovementPlane.Movement[8];
        directions[4].xMovement = 1;
        directions[4].yMovement = 0;

        directions[0].xMovement = 1;
        directions[0].yMovement = 1;

        directions[5].xMovement = 0;
        directions[5].yMovement = 1;

        directions[1].xMovement = -1;
        directions[1].yMovement = 1;

        directions[6].xMovement = -1;
        directions[6].yMovement = 0;

        directions[2].xMovement = -1;
        directions[2].yMovement = -1;

        directions[7].xMovement = 0;
        directions[7].yMovement = -1;

        directions[3].xMovement = 1;
        directions[3].yMovement = -1;

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

                        if (SetupMovement(directions[j], newRoute, movesFound, newXPos, newYPos, movementPointsLeft, maxMoves, unitsBlock))
                        {
                            yield return new WaitForSeconds(0.2f);
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

            StartCoroutine(NewFindMovesCorout(X, Y, moveRoute, movesFound, movePointsLeft, maxMoves, unitsBlock));
        }
        yield break;
    }
    */

    bool DiagonalsCrossing(MovementPlane.Movement move1, MovementPlane.Movement move2)
    {
        bool crossing = false;

		if ((move1.xMovement != 0 && move1.yMovement != 0) || (move2.xMovement != 0 && move2.yMovement != 0)) 
		{
			if ((move1.xMovement == move2.xMovement && move1.yMovement == -move2.yMovement) || 
				(move1.yMovement == move2.yMovement && move1.xMovement == -move2.xMovement)) 
			{
				crossing = true;
			}
		}
		return crossing;
    }

    bool SetupMovement(MovementPlane.Movement move, MovementPlane.Movement[] route, 
        int movesFound, int xPos, int yPos, int movePointsLeft, int maxMoves, bool unitsBlock)
    {
        int moveCost = 0;
        bool foundMove = false;
        bool crossingDiagonal = false;

        if(move.xMovement != 0 && move.yMovement != 0)
        {
            for (int i = 0; i < movesFound; i++)
            {
                int X = possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().XPosition,
                    Y = possibleMoves[i].GetComponent<MovementPlane>().presentTile.GetComponent<BasicTile>().YPosition;

                if (((xPos + move.xMovement == X && yPos == Y) ||
					(xPos == X && yPos + move.yMovement == Y)) && 
					DiagonalsCrossing(move,
						possibleMoves[i].GetComponent<MovementPlane>().route[possibleMoves[i].GetComponent<MovementPlane>().route.Length - 1]))
                {
					crossingDiagonal = true;
                }
            }
        }

        if (!(xPos + move.xMovement < grid.xSize && xPos + move.xMovement >= 0
            &&
            yPos + move.yMovement < grid.ySize && yPos + move.yMovement >= 0))
            return foundMove;

        else if (crossingDiagonal)
            return foundMove;

        else if (grid.Grid(xPos + move.xMovement, yPos + move.yMovement).GetComponent<BasicTile>().Accessible(grid.Grid(xPos, yPos).GetComponent<BasicTile>(), unitsBlock, team))
        {
            foundMove = true;

            moveCost = MovementPlane.RouteMoveCost(route);

            if (move.xMovement != 0 && move.yMovement != 0)
                moveCost += 3;
            else
                moveCost += 2;
            if (moveCost <= maxMoves)
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

        if(grid.Grid(xPos + move.xMovement, yPos + move.yMovement).GetComponent<BasicTile>().IsOccupied)
        {
            objectsWithinRange.Add(grid.Grid(xPos + move.xMovement, yPos + move.yMovement).GetComponent<BasicTile>().CharacterStepping);
        }

        return foundMove;
    }

    public bool WithinZMovesFromThis(int z, BasicTile target, bool lineOfSight)
    {
        int targetX = target.XPosition,
            targetY = target.YPosition,
            tileX = presentTile.GetComponent<BasicTile>().XPosition,
            tileY = presentTile.GetComponent<BasicTile>().YPosition,
            deltaX = Mathf.Abs(targetX - tileX),
            deltaY = Mathf.Abs(targetY - tileY);

        bool isWithinRange = false, isVisible = true;

        if (targetX == tileX)
            isWithinRange = 2 * deltaY <= z ? true : false;
        
        else if (targetY == tileY)
            isWithinRange = 2 * deltaX <= z ? true : false;
        
        else if (deltaY < deltaX)
            isWithinRange = deltaY * 2 + (deltaX - deltaY) * 3 <= z ? true : false;
        
        else if (deltaX < deltaY)
            isWithinRange = deltaX * 2 + (deltaY - deltaX) * 3 <= z ? true : false;
        
        else if (deltaX == deltaY)
            isWithinRange = deltaX * 3 <= z ? true : false;
        
        else
        {
            print("WEIRD SHIT MAYN");
            isWithinRange = false;
        }

        if (lineOfSight)
        {
            /*if (z <= 3)
            {
                isVisible = target.Accessible(presentTile.GetComponent<BasicTile>(), false, team);
            }
            else
            {*/
                RaycastHit hitInfo = new RaycastHit();
                Ray ray = new Ray(presentTile.transform.position + new Vector3(0f, 1f, 0f), target.gameObject.transform.position - presentTile.transform.position);
                Debug.DrawRay(presentTile.transform.position + new Vector3(0f, 1f, 0f), target.gameObject.transform.position - presentTile.transform.position, Color.green, 60f);
                
                int mask = 1 << 8;
                bool hit = Physics.Raycast(ray, out hitInfo, 1000f, mask);
                if (hit)
                {

                print(target + " " + hitInfo.collider.gameObject + " " + hit);
                isVisible = false;
                }
            //}
        }

        if (lineOfSight)
            return isWithinRange && isVisible;

        else
            return isWithinRange;
    }

    void TestPath(int varX, int varY)
    {
        int X = presentTile.GetComponent<BasicTile>().XPosition,
            Y = presentTile.GetComponent<BasicTile>().YPosition;
        BasicTile targetTile = grid.Grid(X + varX, Y + varY).GetComponent<BasicTile>();

        if (targetTile.GetComponent<BasicTile>().Accessible(presentTile.GetComponent<BasicTile>(), true, team))
        {
            print("CAAAAN DOO");
        }
        else
            print("Wont go there");


    }

    public void ClearMoves()
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

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
        set
        {
            isMoving = value;
        }
    }

    

    /*void MoveForward()
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
    }*/
}
