using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BasicTile : MonoBehaviour {

    public enum Orientation
    {
        Directionless,
        Forward,
        Backward,
        Right,
        Left
    }

    public enum TileKind
    {
        Empty,
        Flat,
        Stair,
        Impassable
    }

    [SerializeField]
    private int xPosition, yPosition;

    public int presentHeight;
    public LevelGrid grid;
    public TileKind type;
    public Orientation orientation;
    public float randomHeightVariationRange;
    public bool isSpawn;

    //private int nextHeight;
    private float continuousHeight, heightVariationProgress, randomHeightVariation;
    private bool isOccupied;
    private bool isUnderwater;
    public GameObject foundation, top;
    
    

	// Use this for initialization
	void Start () {
        InitializeTile();
	}

	// Update is called once per frame
	void Update ()
    {
        //height = transform.localPosition.magnitude;
        /*if(isOccupied == true)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
        }*/
        
        transform.localPosition = new Vector3(xPosition * grid.GridScale, continuousHeight * grid.VerticalScale + randomHeightVariation, yPosition * grid.GridScale);
    }

    public void ChangeHeight(int heightChange)
    {
        StartCoroutine(HeightChanging(heightChange));
    }

    IEnumerator HeightChanging(int heightChange)
    {
        continuousHeight = presentHeight;
        heightVariationProgress = 0f;

        while(heightVariationProgress <= 1f)
            { continuousHeight = Mathf.Lerp(presentHeight, presentHeight + heightChange, heightVariationProgress);
            heightVariationProgress += Time.deltaTime;

            if (heightVariationProgress >= 1f)
            {
                heightVariationProgress = 0f;
                presentHeight += heightChange;
                break;
            }

            yield return null;
        }
    }

    public void InitializeTile()
    {
        grid = GameObject.Find("Grid").GetComponent<LevelGrid>();
        heightVariationProgress = 0f;
        continuousHeight = presentHeight;

        randomHeightVariation = Random.Range(-randomHeightVariationRange, randomHeightVariationRange);
        

        if (Application.isEditor)
        {
            int i = gameObject.transform.childCount;
            while (i > 0){
                DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
                i--;
            }
        }
        else
        {
            if (gameObject.transform.childCount > 0)
                Destroy(gameObject.transform.GetChild(0).gameObject);
            if (gameObject.transform.childCount > 0)
                Destroy(gameObject.transform.GetChild(1).gameObject);
        }

        GameObject myFoundation = Instantiate(foundation);
        myFoundation.name = "foundation";
        myFoundation.transform.parent = transform;

        myFoundation.transform.localPosition = Vector3.zero;

        if (type == TileKind.Empty)
        {
            myFoundation.GetComponent<MeshFilter>().mesh = grid.foundation;
            myFoundation.GetComponent<MeshRenderer>().material = grid.tileMaterials[0];
            return;
        }

        else if (type == TileKind.Flat)
        {
            myFoundation.GetComponent<MeshFilter>().mesh = grid.foundation;
            myFoundation.GetComponent<MeshRenderer>().material = grid.tileMaterials[1];
        }

        else if (type == TileKind.Stair)
        {
            GameObject myTop = Instantiate(top);
            myTop.name = "top";
            myTop.transform.parent = transform;
            myTop.transform.localPosition = Vector3.zero;

            myFoundation.GetComponent<MeshFilter>().mesh = grid.foundation;
            myFoundation.GetComponent<MeshRenderer>().material = grid.tileMaterials[1];
            myTop.GetComponent<MeshFilter>().mesh = grid.stair;
            myTop.GetComponent<MeshRenderer>().material = grid.tileMaterials[1];

            myFoundation.transform.localPosition -= new Vector3(0f, 0.5f, 0f);
            myTop.transform.localPosition += new Vector3(0f, 0.25f, 0f);


            switch (orientation)
            {
                case Orientation.Forward:
                    myTop.transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
                    break;

                case Orientation.Backward:
                    myTop.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    break;

                case Orientation.Right:
                    myTop.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    break;

                case Orientation.Left:
                    myTop.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                    break;

                case Orientation.Directionless:
                    myTop.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
            }
        }
    }

    public bool Accessible(BasicTile originTile)
    {
        bool availability = true, diagonal = false, aroundCorner = false, 
            sameHeight = originTile.PresentHeight == presentHeight;

        Orientation moveDirection = Orientation.Directionless;

        diagonal = IsDiagonal(originTile, this);

        if (!diagonal)
        {
            if (xPosition - originTile.XPosition == 1)
                moveDirection = Orientation.Forward;
            else if (xPosition - originTile.XPosition == -1)
                moveDirection = Orientation.Backward;
            else if (yPosition - originTile.YPosition == 1)
                moveDirection = Orientation.Left;
            else if (yPosition - originTile.YPosition == -1)
                moveDirection = Orientation.Right;
            else
            {
                moveDirection = Orientation.Directionless;
                print("Caution, weird moves");
            }
        }

        if (type == TileKind.Empty || type == TileKind.Impassable)
        {
            return false;
        }

        else if (!(originTile.PresentHeight == presentHeight + 1 
            || originTile.PresentHeight == presentHeight 
            || originTile.PresentHeight == presentHeight - 1))
        {
            return false;
        }

        else if (diagonal)
        {
            aroundCorner = IsAroundCorner(originTile, this);
            if (aroundCorner)
            {
                return false;
            }
            else if (!sameHeight || type != TileKind.Flat || originTile.type != TileKind.Flat)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        else if (originTile.type == TileKind.Flat && type == TileKind.Flat && sameHeight)
        {
            return true;
        }

        else if (originTile.type == TileKind.Stair)
        {
            if (!(originTile.orientation == moveDirection || InvertOrientation(originTile.orientation) == moveDirection))
            {
                return false;
            }
            else if (originTile.orientation == moveDirection)
            {
                if (type == TileKind.Flat && sameHeight)
                {
                    return true;
                }
                else if (type == TileKind.Stair 
                    && orientation == moveDirection && originTile.PresentHeight == presentHeight - 1)
                {
                    return true;
                }
                else if (type == TileKind.Stair 
                    && InvertOrientation(orientation) == moveDirection && sameHeight)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else if (InvertOrientation(originTile.orientation) == moveDirection)
            {
                if (type == TileKind.Flat 
                    && originTile.PresentHeight == presentHeight + 1)
                {
                    return true;
                }
                else if (type == TileKind.Stair && orientation == InvertOrientation(moveDirection) 
                    && originTile.PresentHeight == presentHeight + 1)
                {
                    return true;
                }
                else if (type == TileKind.Stair 
                    && orientation == moveDirection && sameHeight)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else
            {
                return false;
            }
        }

        else if (originTile.type == TileKind.Flat && type == TileKind.Stair)
        {
            if (orientation == moveDirection && originTile.PresentHeight == presentHeight - 1)
            {
                return true;
            }

            else if (InvertOrientation(orientation) == moveDirection && sameHeight)
            {
                return true;
            }

            else
            {
                return false;
            }
        }
        return false;
    }

    Orientation InvertOrientation(Orientation orient)
    {
        if (orient == Orientation.Forward)
            return Orientation.Backward;
        else if (orient == Orientation.Backward)
            return Orientation.Forward;
        else if (orient == Orientation.Right)
            return Orientation.Left;
        else if (orient == Orientation.Left)
            return Orientation.Right;
        else
            return Orientation.Directionless;
    }

    bool IsAroundCorner(BasicTile initial, BasicTile final)
    {
        if (grid.Grid(initial.XPosition, final.YPosition).GetComponent<BasicTile>().type != TileKind.Flat
            ||
            grid.Grid(final.XPosition, initial.YPosition).GetComponent<BasicTile>().type != TileKind.Flat
            ||
            grid.Grid(initial.XPosition, final.YPosition).GetComponent<BasicTile>().PresentHeight != initial.PresentHeight
            ||
            grid.Grid(final.XPosition, initial.YPosition).GetComponent<BasicTile>().PresentHeight != initial.PresentHeight)
        {
            return true;
        }

        else
            return false;
    }

    bool IsDiagonal(BasicTile initial, BasicTile final)
    {
        int yChange = final.YPosition - initial.YPosition, xChange = final.XPosition - initial.XPosition;

        if (yChange != 0 && xChange != 0)
            return true;

        else
            return false;
    }

    public int XPosition
    {
        get
        {
            return xPosition;
        }
        set
        {
            xPosition = value;
        }
    }

    public int YPosition
    {
        get
        {
            return yPosition;
        }
        set
        {
            yPosition = value;
        }
    }

    public int PresentHeight
    {
        get
        {
            return presentHeight;
        }
        set
        {
            presentHeight = value;
        }
    }

    public bool IsSpawn
    {
        get
        {
            return isSpawn;
        }
        set
        {
            isSpawn = value;
        }
    }

    public bool IsOccupied
    {
        get
        {
            return isOccupied;
        }
        set
        {
            isOccupied = value;
        }
    }
}
