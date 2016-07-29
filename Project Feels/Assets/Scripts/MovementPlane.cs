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

        routesInString = new string[route.Length];
        for(int i = 0; i < route.Length; i++)
        {
            routesInString[i] = route[i].xMovement + " " + route[i].yMovement;
        }
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
