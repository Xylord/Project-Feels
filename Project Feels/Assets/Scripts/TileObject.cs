using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TileObject : MonoBehaviour {

    public LevelGrid grid;
    public GameObject presentTile, nextTile, movementPlane;
    public float offsetFromTile, speed;
    public GameObject[] possibleMoves;

    

    private float moveProgress;

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
        moveProgress = 0f;
    }
	
	// Update is called once per frame
	void Update () {
        Selecting();

        if (nextTile == null)
        {
            transform.localPosition = presentTile.transform.position + new Vector3(0f, offsetFromTile, 0f);
        }

        else
        {
            transform.localPosition = Vector3.Lerp(presentTile.transform.position, 
                nextTile.transform.position, moveProgress) 
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
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            print("Showing moves");
            ShowMoves();
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
                print(move.name);
                nextTile = move.GetComponent<MovementPlane>().presentTile;

                ClearMoves();
                return;
            }
        }
    }

    void ShowMoves()
    {
        possibleMoves = new GameObject[grid.xSize * grid.ySize];
        for (int i = 0; i <= grid.xSize - 1; i++)
        {
            for (int j = 0; j <= grid.ySize - 1; j++)
            {
                if ( i != presentTile.GetComponent<BasicTile>().XPosition || j != presentTile.GetComponent<BasicTile>().YPosition)
                {
                    possibleMoves[j * grid.xSize + i] = Instantiate(movementPlane);
                    possibleMoves[j * grid.xSize + i].GetComponent<MovementPlane>().presentTile = grid.Grid(i, j);
                    possibleMoves[j * grid.xSize + i].name = "Move " + i + " " + j;
                }
            }
        }
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
