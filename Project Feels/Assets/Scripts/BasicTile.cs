using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BasicTile : MonoBehaviour {

    public enum Orientation
    {
        Directionless,
        Forward,
        Backward,
        Right,
        Left,  
        ForwardRight,
        ForwardLeft,
        BackwardRight,
        BackwardLeft
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
	public string[] spawnTags;
    public List<AuraManager.Effect> tileEffects = new List<AuraManager.Effect>();

    //private int nextHeight;
    private float continuousHeight, heightVariationProgress, randomHeightVariation;
    public bool isOccupied;
    private bool isUnderwater;
    public TileObject characterStepping;
    public GameObject foundation, top;

    public int EFFECTSSTFET;
    

	// Use this for initialization
	void Awake () {
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

        EFFECTSSTFET = tileEffects.Count;
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
        characterStepping = null;
        isOccupied = false;

        randomHeightVariation = Random.Range(-randomHeightVariationRange, randomHeightVariationRange);
        
		if (isSpawn) 
		{
			grid.spawnTiles.Add (this);
		}

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

        //foundation = Instantiate(grid.emptyTile);
        

        if (type == TileKind.Empty)
        {
            foundation = Instantiate(grid.emptyTile);
            foundation.name = "foundation";
            foundation.transform.parent = transform;
            foundation.transform.localPosition = Vector3.zero;
        }

        else if (type == TileKind.Flat)
        {
            foundation = Instantiate(grid.foundation);
            foundation.name = "foundation";
            foundation.transform.parent = transform;
            foundation.transform.localPosition = Vector3.zero;
            //foundation.GetComponent<MeshRenderer>().material = grid.tileMaterials[1];
        }

        else if (type == TileKind.Stair)
        {
            top = Instantiate(grid.stair);
            top.name = "top";
            top.transform.parent = transform;
            top.transform.localPosition = Vector3.zero;

            foundation = Instantiate(grid.foundation);
            foundation.name = "foundation";
            foundation.transform.parent = transform;
            foundation.transform.localPosition = Vector3.zero;
            foundation.transform.localPosition = new Vector3(0f, -0.5f, 0f);
            top.transform.localPosition = new Vector3(0f, 0.25f, 0f);


            switch (orientation)
            {
                case Orientation.Forward:
                    top.transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
                    break;

                case Orientation.Backward:
                    top.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    break;

                case Orientation.Right:
                    top.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    break;

                case Orientation.Left:
                    top.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                    break;

                case Orientation.Directionless:
                    top.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
            }
        }
    }

    public bool Accessible(BasicTile originTile, bool unitsBlock, int team)
    {
        bool diagonal = false, aroundCorner = false, 
            sameHeight = originTile.PresentHeight == presentHeight;

        Orientation moveDirection = new MovementPlane.Movement(xPosition - originTile.XPosition, yPosition - originTile.YPosition).orientation;

        diagonal = IsDiagonal(originTile, this);

        /*if (!diagonal)
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
        }*/

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

        else if (isOccupied && unitsBlock && team != CharacterStepping.team)
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
            else if (unitsBlock)
            {
                if (moveDirection == Orientation.ForwardRight && grid.Grid(xPosition, yPosition + 1).GetComponent<BasicTile>().IsOccupied && grid.Grid(xPosition - 1, yPosition).GetComponent<BasicTile>().IsOccupied)
                {
                    return false;
                }

                else if (moveDirection == Orientation.BackwardRight && grid.Grid(xPosition, yPosition + 1).GetComponent<BasicTile>().IsOccupied && grid.Grid(xPosition + 1, yPosition).GetComponent<BasicTile>().IsOccupied)
                {
                    return false;
                }

                else if (moveDirection == Orientation.BackwardLeft && grid.Grid(xPosition, yPosition - 1).GetComponent<BasicTile>().IsOccupied && grid.Grid(xPosition + 1, yPosition).GetComponent<BasicTile>().IsOccupied)
                {
                    return false;
                }

                else if (moveDirection == Orientation.ForwardLeft && grid.Grid(xPosition, yPosition - 1).GetComponent<BasicTile>().IsOccupied && grid.Grid(xPosition - 1, yPosition).GetComponent<BasicTile>().IsOccupied)
                {
                    return false;
                }

                else
                {
                    return true;
                }
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

            if (moveDirection != InvertOrientation(originTile.orientation) && moveDirection != originTile.orientation && originTile.orientation == orientation)
            {
                if (type == TileKind.Stair
                    && sameHeight)
                {
                    return true;
                }
            }
            /*if (!(originTile.orientation == moveDirection || InvertOrientation(originTile.orientation) == moveDirection))
            {
                return false;
            }*/

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

            else if (moveDirection != InvertOrientation(originTile.orientation) && moveDirection != originTile.orientation)
            {
                print("Move thing");
                if (type == TileKind.Stair
                    && sameHeight)
                {
                    return true;
                }
            }

            else
            {
                return false;
            }
        }

        else if (originTile.type == TileKind.Flat && type == TileKind.Stair)
        {
            //print(orientation + " " + moveDirection + " " + this);
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

    public TileObject CharacterStepping
    {
        get
        {
            return characterStepping;
        }
        set
        {
            characterStepping = value;
        }
    }
    
}
