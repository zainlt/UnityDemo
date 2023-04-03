using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public PlayerStartBar playerStartBar;

    [Header("事件监听")]
    public CharacterEventSO healthEvent;
    public SceneLoadEventSO unloadedSceneEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO gameoverEvent;
    public VoidEventSO backToMenuEvent;

    [Header("组件")]
    public GameObject gameoverPanel;
    public GameObject restartButton;
    public GameObject mobileTouch;

    private void Awake()
    {
        #if UNITY_STANDALONE
        mobileTouch.SetActive(false);
        #endif
    }

    private void OnEnable()
    {
        // onHealthEvent 订阅事件 onEventRaised
        healthEvent.OnEventRaised += OnHealthEvent;
        unloadedSceneEvent.loadRequestEvent += OnUnLoadedSceneEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        gameoverEvent.OnEventRaised += OnGameoverEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
    }


    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        unloadedSceneEvent.loadRequestEvent -= OnUnLoadedSceneEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        gameoverEvent.OnEventRaised -= OnGameoverEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
    }


    private void OnLoadDataEvent()
    {
        gameoverPanel.SetActive(false);
    }
    private void OnGameoverEvent()
    {
        gameoverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(restartButton);
    }

    private void OnUnLoadedSceneEvent(GameSceneSO sceneToLoad, Vector3 arg1, bool arg2)
    {
        var isMenu = sceneToLoad.sceneType == SceneType.Menu;
        playerStartBar.gameObject.SetActive(!isMenu);
    }

    private void OnHealthEvent(Character character)
    {
        var persentage = character.currentHealth / character.maxHealth;
        playerStartBar.OnhealthChange(persentage);
        playerStartBar.OnPowerChange(character);
    }
}
