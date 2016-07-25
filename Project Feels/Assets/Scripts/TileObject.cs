using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TileObject : MonoBehaviour {

    public LevelGrid grid;
    public GameObject presentTile;
    public float offsetFromTile;

	// Use this for initialization
	void Start () {
        transform.parent = grid.gameObject.transform;
        presentTile = grid.FindSpawn();

	}
	
	// Update is called once per frame
	void Update () {
        if (grid == null)
            return;

        else if(presentTile == null)
            presentTile = grid.FindSpawn();
        
        transform.localPosition = presentTile.transform.position + new Vector3(0f, offsetFromTile, 0f);

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

    void MoveForward()
    {
        int X = presentTile.GetComponent<BasicTile>().XPosition, 
            Y = presentTile.GetComponent<BasicTile>().YPosition;

        presentTile = grid.Grid(X + 1, Y);
    }

    void MoveBackward()
    {
        int X = presentTile.GetComponent<BasicTile>().XPosition,
            Y = presentTile.GetComponent<BasicTile>().YPosition;

        presentTile = grid.Grid(X - 1, Y);
    }

    void MoveRight()
    {
        int X = presentTile.GetComponent<BasicTile>().XPosition,
            Y = presentTile.GetComponent<BasicTile>().YPosition;

        presentTile = grid.Grid(X, Y + 1);
    }

    void MoveLeft()
    {
        int X = presentTile.GetComponent<BasicTile>().XPosition,
            Y = presentTile.GetComponent<BasicTile>().YPosition;

        presentTile = grid.Grid(X, Y - 1);
    }
}
