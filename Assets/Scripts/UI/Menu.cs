using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour
{
    public GameObject newGameButton;


    private void OnEnable()
    {
        //默认选择第一个按钮
        EventSystem.current.SetSelectedGameObject(newGameButton);
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}
