using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SceneLoader : MonoBehaviour , ISaveble
{
    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;
    [Header("�¼�����")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    [Header("��ӹ㲥")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent;
    public SceneLoadEventSO unLoadedSceneEvent;

    [Header("����")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;
    private GameSceneSO currentLoadScene;
    private GameSceneSO sceneToLoad;
    private Vector3 positionToGo;
    private bool fadeScreen;

    private bool isLoading;
    public float fadeDuration;

    private void Awake()
    {
        //���س���
        //Addressables.LoadSceneAsync(firstLoadScene.sceneReference, LoadSceneMode.Additive);
        //currentLoadScene = firstLoadScene;
        //currentLoadScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
    }


    private void Start()
    {
        //NewGame();
        loadEventSO.RaiseLoadRequestEvent(menuScene, menuPosition, true);
    }

    //ע���¼�
    private void OnEnable()
    {
        loadEventSO.loadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += NewGame;

        ISaveble saveble = this;
        saveble.RegisterSaveData();

        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;
    }

    //ע���¼�
    private void OnDisable()
    {
        loadEventSO.loadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= NewGame;

        ISaveble saveble = this;
        saveble.UnRegisterSaveData();

        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;
    }

    //������ѡ���˳�ʱ�ص��˵�
    private void OnBackToMenuEvent()
    {
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
    }

    //�µ���Ϸ
    private void NewGame()
    {
        sceneToLoad = firstLoadScene;
        //OnLoadRequestEvent(sceneToLoad, firstPosition, true);
        //��������
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, firstPosition, true);
    }


    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        this.fadeScreen = fadeScreen;

        if (isLoading)
            return;

        isLoading = true;

        if (currentLoadScene != null)
        {
            StartCoroutine(UnLoadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
    }


    //Э�� ж�ؾɳ���
    private IEnumerator UnLoadPreviousScene()
    {
        if (fadeScreen)
        {
            //ʵ�ֽ���
            fadeEvent.FadeIn(fadeDuration);
        }

        //�ȴ� fadeDuration��
        yield return new WaitForSeconds(fadeDuration);

        //�㲥�¼�����Ѫ����ʾ
        unLoadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);

        //�ȴ�������ȫж��
        yield return currentLoadScene.sceneReference.UnLoadScene();

        //�ر�����
        playerTrans.gameObject.SetActive(false);
        //�����³���
        LoadNewScene();
    }


    //�����³���
    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
    }

    //�������سɹ�֮��
    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        //��ȡ�����л���ĵ�ǰ����
        currentLoadScene = sceneToLoad;


        playerTrans.position = positionToGo;
        playerTrans.gameObject.SetActive(true);
        if (fadeScreen)
        {
            //ʵ�ֽ���
            fadeEvent.FadeOut(fadeDuration);
        }

        isLoading = false;

        if (currentLoadScene.sceneType != SceneType.Menu)
            //�㲥  �����Ѿ��л����
            afterSceneLoadedEvent.RaiseEvent();
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadScene);
    }

    public void LoadData(Data data)
    {
        var playerID = playerTrans.GetComponent<DataDefination>().ID;
        if (data.characterPosDict.ContainsKey(playerID))
        {
            positionToGo = data.characterPosDict[playerID].ToVector3();
            sceneToLoad = data.GetSavedScene();

            OnLoadRequestEvent(sceneToLoad, positionToGo, true);
        }
    }
}
