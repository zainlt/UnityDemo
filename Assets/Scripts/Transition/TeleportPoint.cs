using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    [Header("添加广播")]
    public SceneLoadEventSO loadEventSO;

    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;
    public void TriggerAction()
    {
        Debug.Log("传送");

        //广播 sceneLoader监听
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }
}
