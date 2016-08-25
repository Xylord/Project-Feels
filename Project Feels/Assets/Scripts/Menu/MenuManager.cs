using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour {

    public Menu CurrentMenu;
    private GameObject mainMenu;    
    //private bool IsActivated = true;

    public void Start()
    {
        ShowMenu(CurrentMenu);
        mainMenu = GameObject.Find("MainMenu");
       
    }

    public void ShowMenu(Menu menu)
    {
        if (CurrentMenu != null)
            CurrentMenu.IsOpen = false;

        CurrentMenu = menu;
        CurrentMenu.IsOpen = true;


    }
    public void MenuSetDisable(bool IsDisabled)
    {
        if (IsDisabled)
             mainMenu.SetActive(false);
       
    }

    public void ChangeLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame(bool onClicked)
    {
        if(onClicked)
         Application.Quit();
    }


}
