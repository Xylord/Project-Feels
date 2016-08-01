using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class LevelGrid : MonoBehaviour {
    [SerializeField]
    private GameObject[] grid;

    public AITurnManager turnManager;
    public GameObject emptyTile;
    public float gridScale, verticalScale;
    public int minHeight, maxHeight, xSize, ySize;

    public Mesh foundation, stair;
    public Material[] tileMaterials;
	public List<BasicTile> spawnTiles = new List<BasicTile>();

	// Use this for initialization
	void Start () {
        //GenerateEmptyGrid();

        
	}

    public void GenerateEmptyGrid()
    {
        ClearGrid();

        grid = new GameObject[xSize * ySize];
        for (int i = 0; i <= xSize - 1; i++)
        {
            for (int j = 0; j <= ySize - 1; j++)
            {
                grid[j * xSize + i] = Instantiate(emptyTile);
                grid[j * xSize + i].GetComponent<BasicTile>().XPosition = i;
                grid[j * xSize + i].GetComponent<BasicTile>().YPosition = j;
                grid[j * xSize + i].transform.parent = transform;
                grid[j * xSize + i].name = "Tile " + i + " " + j;

                grid[j * xSize + i].GetComponent<BasicTile>().InitializeTile();
            }
        }
        
    }

    public void GenerateEmptyGridEditor()
    {
        ClearGridEditor();

        grid = new GameObject[xSize * ySize];
        for (int i = 0; i <= xSize - 1; i++)
        {
            for (int j = 0; j <= ySize - 1; j++)
            {
                grid[j * xSize + i] = Instantiate(emptyTile);
                grid[j * xSize + i].GetComponent<BasicTile>().XPosition = i;
                grid[j * xSize + i].GetComponent<BasicTile>().YPosition = j;
                grid[j * xSize + i].transform.parent = transform;
                grid[j * xSize + i].name = "Tile " + i + " " + j;

                grid[j * xSize + i].GetComponent<BasicTile>().InitializeTile();
            }
        }

    }

    public void ClearGrid()
    {
        if (grid != null)
        {
            for (int i = 0; i <= grid.Length - 1; i++)
            {
                Destroy(grid[i]);
            }
            grid = null;
        }
    }

    public void ClearGridEditor()
    {
        if (grid != null)
        {
            for (int i = 0; i <= grid.Length - 1; i++)
            {
                DestroyImmediate(grid[i]);
            }
            grid = null;
        }
    }

    public void UpdateGrid()
    {
        turnManager = GameObject.Find("AITurnManager").GetComponent<AITurnManager>();
        spawnTiles.Clear ();

        if (grid != null)
        {
            for (int i = 0; i <= grid.Length - 1; i++)
            {
                grid[i].GetComponent<BasicTile>().InitializeTile();
            }
        }
    }

    public GameObject FindSpawn()
    {
        int i = Random.Range(0, xSize - 1), j = Random.Range(0, ySize - 1);
        grid[j * xSize + i].GetComponent<BasicTile>().IsSpawn = true;
        grid[j * xSize + i].name += " Spawn";

        return grid[j * xSize + i];
    }
    
	
	// Update is called once per frame
	private void Update () {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ClearGrid();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            GenerateEmptyGrid();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            FindSpawn();
        }
    }

    public float GridScale
    {
        get
        {
            return gridScale;

        }
    }

    public float VerticalScale
    {
        get
        {
            return verticalScale;

        }
    }

    public GameObject Grid (int X, int Y)
    {
        return grid[Y * xSize + X];
    }
}
