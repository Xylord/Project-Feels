using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//ATTACH THIS TO THE ENEMY
//NOTE : The Enemy gameobject must contain the following : Canvas ("EnemyCanvas") -> Image("HealthBg") -> Image ("Health")
public class Enemy : MonoBehaviour {

    public int health, maxHealth;
    private Image healthBar;

	// Use this for initialization
	void Start ()
    {
        health = 100;
        maxHealth = 100;
        healthBar = transform.FindChild("EnemyCanvas").FindChild("HealthBg").FindChild("Health").GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //To debug
        if (Input.GetKeyDown(KeyCode.J))
            Hit(10);

        if (health == 0)
            Destroy(gameObject);          
	
	}

    public void Hit(int damage)
    {
        health -= damage;
        healthBar.fillAmount = (float)health / (float)maxHealth;
    }
}
