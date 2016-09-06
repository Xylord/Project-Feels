using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AuraManager : MonoBehaviour
{
    public TileObject ownerObject;
    //private List<TileObject> objectsWithinRange = new List<TileObject>();
    //public List<int> objectsInRangeIndex = new List<int>();
    private GameObject[] tilesInRange;
    public Effect auraEffect;
    public int auraRange;
    private bool auraEnabled;

    public struct Effect
    {
        public string effectName;
        public int attackBonus, defenseBonus, speedBonus, hPHealing, sanityHealing, duration;

        public Effect(string name, int attBonus, int defBonus, int spdBonus, int hPHeal, int sanityHeal, int durationTime)
        {
            effectName = name;
            attackBonus = attBonus;
            defenseBonus = defBonus;
            speedBonus = spdBonus;
            hPHealing = hPHeal;
            sanityHealing = sanityHeal;
            duration = durationTime;
        }

        public void ReduceTimeLeft(int turnsPassed)
        {
            duration -= turnsPassed;
        }
    }

	// Use this for initialization
	void Start () {
        ownerObject = gameObject.GetComponent<TileObject>();
        auraEffect = new Effect("test effect", 2, 0, 0, 0, 0, 1);
        auraEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {
        UpdateAura();

    }

    void UpdateAura()
    {
        FindAura(auraRange);
        AffectTiles();
    }

    void AffectTiles()
    {
        if (!auraEnabled)
            return;

        string thingamajing = "";

        for (int i = 0; i < tilesInRange.Length; i++)
        {
            if (tilesInRange[i] != null)
            {
                thingamajing += tilesInRange[i].name + " ";
                if (tilesInRange[i].GetComponent<BasicTile>().isOccupied)
                {
                    if(!tilesInRange[i].GetComponent<BasicTile>().CharacterStepping.activeEffects.Contains(auraEffect))
                        tilesInRange[i].GetComponent<BasicTile>().CharacterStepping.activeEffects.Add(auraEffect);
                }
            }
        }
    }

    void FindAura(int radius)
    {
        if (!auraEnabled)
            return;

        //objectsWithinRange.Clear();
        //objectsInRangeIndex.Clear();

        int xPos = ownerObject.presentTile.GetComponent<BasicTile>().XPosition,
            yPos = ownerObject.presentTile.GetComponent<BasicTile>().YPosition,
            maxRadius = radius,
            tilesFound = 0;

        tilesInRange = new GameObject[(int)Mathf.Pow(maxRadius * 2 + 1, 2)];

        for (int i = -maxRadius; i <= maxRadius; i++)
        {
            for (int j = -maxRadius; j <= maxRadius; j++)
            {
                if (xPos + i < 0 || xPos + i >= ownerObject.grid.xSize)
                    continue;

                if (yPos + j < 0 || yPos + j >= ownerObject.grid.ySize)
                    continue;


                if (ownerObject.WithinZMovesFromThis(radius * 2, ownerObject.grid.Grid(xPos + i, yPos + j).GetComponent<BasicTile>(), false) && !(i == 0 && j == 0))
                {

                    /*if (ownerObject.grid.Grid(xPos + i, yPos + j).GetComponent<BasicTile>().IsOccupied)
                    {
                        objectsWithinRange.Add(ownerObject.grid.Grid(xPos + i, yPos + j).GetComponent<BasicTile>().CharacterStepping);
                        objectsInRangeIndex.Add(tilesFound);
                    }*/

                    tilesInRange[tilesFound] = ownerObject.grid.Grid(xPos + i, yPos + j);
                    tilesFound++;
                }
            }
        }
    }
}
