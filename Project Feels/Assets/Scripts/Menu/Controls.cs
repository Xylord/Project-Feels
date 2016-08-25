using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour
{

    // Use this for initialization
    private GameObject mainMenu;
    public Menu CurrentMenu;
   
    void Start()
    {
        mainMenu = GameObject.Find("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mainMenu.SetActive(true);
            CurrentMenu.IsOpen = true;
           
        }

    }

}