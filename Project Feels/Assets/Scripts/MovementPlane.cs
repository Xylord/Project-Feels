using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[ExecuteInEditMode]
public class MovementPlane : MonoBehaviour
{
   

    public struct Movement
    {
        public int xMovement, yMovement;
        public BasicTile.Orientation orientation;

        public Movement(int xMove, int yMove)
        {
            xMovement = xMove;
            yMovement = yMove;

            orientation = BasicTile.Orientation.Directionless;
            if (xMove == 1)
            {
                if (yMove == 1)
                    orientation = BasicTile.Orientation.ForwardRight;
                
                else if (yMove == 0)
                    orientation = BasicTile.Orientation.Forward;
                
                else if (yMove == -1)
                    orientation = BasicTile.Orientation.ForwardLeft;
            }
            else if (xMove == 0)
            {
                if (yMove == 1)
                    orientation = BasicTile.Orientation.Right;
                
                else if (yMove == 0)
                    orientation = BasicTile.Orientation.Directionless;
                
                else if (yMove == -1)
                    orientation = BasicTile.Orientation.Left;
            }
            else if (xMove == -1)
            {
                if (yMove == 1)
                    orientation = BasicTile.Orientation.BackwardRight;
                
                else if (yMove == 0)
                    orientation = BasicTile.Orientation.Backward;
                
                else if (yMove == -1)
                    orientation = BasicTile.Orientation.BackwardLeft;
            }
        }
    }

    private LevelGrid grid;
    public GameObject presentTile;
    public GameObject AOEIndicator;
    private List<TileObject> objectsInRange = new List<TileObject>();
    public float offsetFromTile;
    
    public AITurnManager turnManager;

    public int movementCost;
    public Movement[] route;
    public string[] routesInString;
    public LineRenderer routeLine;
    public GameObject[] areaOfEffectObjects;
    private bool target, showingRange;
    public int damage, aOERange;

    //private TextMesh coordinates;

    // Use this for initialization
    void Start()
    {
        InitializeMovementPlane();
    }

    public void ExecuteAttack()
    {
        for(int i = 0; i < objectsInRange.Count; i++)
        {
            print("Object in range : " + objectsInRange[i]);
            objectsInRange[i].GetComponent<TileObject>().Damage(damage);
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < areaOfEffectObjects.Length; i++)
        {
            if (areaOfEffectObjects[i] != null)
            {
                Destroy(areaOfEffectObjects[i]);
            }
        }
        areaOfEffectObjects = null;
    }

    public void InitializeMovementPlane()
    {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        transform.parent = grid.gameObject.transform;
        turnManager = GameObject.Find("AITurnManager").GetComponent<AITurnManager>();
        //routeLine = gameObject.AddComponent<LineRenderer>();
        /*GameObject text = new GameObject();
        coordinates = text.AddComponent<TextMesh>();
        text.transform.parent = transform;

        text.transform.localPosition += new Vector3(0f, 1f, 0f);
        text.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        coordinates.text = "(" + presentTile.GetComponent<BasicTile>().XPosition + ", " + presentTile.GetComponent<BasicTile>().YPosition + ")";
        coordinates.fontSize = 1;*/
    }

    // Update is called once per frame
    void Update()
    {
        if(presentTile.GetComponent<BasicTile>().type == BasicTile.TileKind.Stair)
        {
            transform.localPosition = presentTile.transform.position + new Vector3(0f, offsetFromTile - 0.25f, 0f);
            Quaternion rot = Quaternion.identity;
            switch (presentTile.GetComponent<BasicTile>().orientation)
            {
                case BasicTile.Orientation.Forward:
                    rot = Quaternion.Euler(90f - Mathf.Rad2Deg * Mathf.Atan(0.5f), 90f, 0f);
                    break;

                case BasicTile.Orientation.Backward:
                    rot = Quaternion.Euler(90f + Mathf.Rad2Deg * Mathf.Atan(0.5f), 90f, 0f);
                    break;

                case BasicTile.Orientation.Right:
                    rot = Quaternion.Euler(90f + Mathf.Rad2Deg * Mathf.Atan(0.5f), 0f, 0f);
                    break;

                case BasicTile.Orientation.Left:
                    rot = Quaternion.Euler(90f - Mathf.Rad2Deg * Mathf.Atan(0.5f), 0f, 0f);
                    break;
            }

            transform.localRotation = rot;
        }
        else
            transform.localPosition = presentTile.transform.position + new Vector3(0f, offsetFromTile, 0f);

        if (route != null)
        {
            //DrawRoute(route);

            routesInString = new string[route.Length];
            for (int i = 0; i < route.Length; i++)
            {
                routesInString[i] = route[i].xMovement + " " + route[i].yMovement;
            }
        }

        if (turnManager.mouseOverObject == gameObject)
            ShowAOE();

        else
            HideAOE();
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

    public void ShowAOE()
    {
        for (int i = 0; i < areaOfEffectObjects.Length; i++)
        {
            if (areaOfEffectObjects[i] != null)
            {
                areaOfEffectObjects[i].SetActive(true);
            }
        }
    }

    public void HideAOE()
    {
        for(int i = 0; i < areaOfEffectObjects.Length; i++)
        {
            if(areaOfEffectObjects[i] != null)
            {
                areaOfEffectObjects[i].SetActive(false);
            }
        }
    }

    public void FindAOE()
    {
        InitializeMovementPlane();

        int xPos = presentTile.GetComponent<BasicTile>().XPosition,
            yPos = presentTile.GetComponent<BasicTile>().YPosition,
            maxRadius = aOERange / 2,
            affectedTiles = 0;

        areaOfEffectObjects = new GameObject[(int)Mathf.Pow(maxRadius * 2 + 1, 2)];

        for (int i = -maxRadius; i <= maxRadius; i++)
        {
            for (int j = -maxRadius; j <= maxRadius; j++)
            {
                if (xPos + i < 0 || xPos + i >= grid.xSize)
                    continue;

                if (yPos + j < 0 || yPos + j >= grid.ySize)
                    continue;

                if (WithinZMovesFromThis(aOERange, grid.Grid(xPos + i, yPos + j).GetComponent<BasicTile>(), false))
                {
                    if (grid.Grid(xPos + i, yPos + j).GetComponent<BasicTile>().IsOccupied)
                    {
                        objectsInRange.Add(grid.Grid(xPos + i, yPos + j).GetComponent<BasicTile>().CharacterStepping);
                    }

                    areaOfEffectObjects[affectedTiles] = Instantiate(AOEIndicator);
                    areaOfEffectObjects[affectedTiles].GetComponent<BasicTileObject>().presentTile = grid.Grid(xPos + i, yPos + j);
                    

                    affectedTiles++;
                }
            }
        }

        HideAOE();
    }

    public static int RouteMoveCost(Movement[] route)
    {
        int moveCost = 0;

        for (int i = 0; i < route.Length; i++)
        {
            if (route[i].xMovement != 0 && route[i].yMovement != 0)
            {
                moveCost += 3;
            }

            else
                moveCost += 2;
        }

        return moveCost;
    }

    void DrawRoute(Movement[] drawnRoute)
    {
        int X = presentTile.GetComponent<BasicTile>().XPosition,
                Y = presentTile.GetComponent<BasicTile>().YPosition,
                positionCount = drawnRoute.Length;

        Vector3 offset = new Vector3(0f, 1f, 0f);
        Vector3[] positions = new Vector3[positionCount];

        for (int i = drawnRoute.Length - 1; i >= 0; i--)
        {
            GameObject nowTile = grid.Grid(X, Y),
                        nextTile = grid.Grid(X - route[i].xMovement, Y - route[i].yMovement);

            X -= route[i].xMovement;
            Y -= route[i].yMovement;

            positions[i] = nowTile.transform.localPosition + offset;
        }

        routeLine.SetVertexCount(positionCount);
        routeLine.SetPositions(positions);
        routeLine.SetWidth(0.1f, 0.1f);
    }

    static public void PrintRoute(Movement[] route)
    {
        string routeString = "";
        for (int i = 0; i < route.Length; i++)
        {
            routeString += "(" + route[i].xMovement + " " + route[i].yMovement + ") ";
        }
        print(routeString);
    }

    public bool Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }

    public bool ShowingRange
    {
        get
        {
            return showingRange;
        }
        set
        {
            showingRange = value;
        }
    }

    public int Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;
        }
    }
}
