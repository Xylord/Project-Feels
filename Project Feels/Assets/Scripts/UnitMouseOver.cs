using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitMouseOver : MonoBehaviour
{
    private AITurnManager turnManager;
    private TileObject unit;
    private Text movePoints;
    public Image healthBar, sanityBar;


    // Use this for initialization
    void Start()
    {
        turnManager = GameObject.Find("AITurnManager").GetComponent<AITurnManager>();
        unit = transform.parent.GetComponent<TileObject>();
        movePoints = transform.FindChild("MovePoints").GetComponent<Text>();
        healthBar = transform.FindChild("Health").FindChild("HealthGreen").GetComponent<Image>();
        sanityBar = transform.FindChild("Sanity").FindChild("SanityBlue").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        movePoints.text = unit.movementPoints + " MovePoints";
        BarManager();
    }

    private void BarManager()
    {
        healthBar.fillAmount = (float)unit.hP / (float)unit.maxHP;
        sanityBar.fillAmount = (float)unit.sanity / (float)unit.maxSanity;
    }
}
