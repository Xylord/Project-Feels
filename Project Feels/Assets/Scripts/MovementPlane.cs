using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class MovementPlane : MonoBehaviour
{

    public struct Movement
    {
        public int xMovement, yMovement;

        public Movement(int xMove, int yMove)
        {
            xMovement = xMove;
            yMovement = yMove;
        }
    }

    public LevelGrid grid;
    public GameObject presentTile;
    public float offsetFromTile;

    public int movementCost;
    public Movement[] route;
    public string[] routesInString;
    public LineRenderer routeLine;

    //private TextMesh coordinates;

    // Use this for initialization
    void Start()
    {
        InitializeMovementPlane();
    }

    public void InitializeMovementPlane()
    {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        transform.parent = grid.gameObject.transform;
        routeLine = gameObject.AddComponent<LineRenderer>();
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
        transform.localPosition = presentTile.transform.position + new Vector3(0f, offsetFromTile, 0f);
        DrawRoute(route);

        routesInString = new string[route.Length];
        for(int i = 0; i < route.Length; i++)
        {
            routesInString[i] = route[i].xMovement + " " + route[i].yMovement;
        }
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

    
}
