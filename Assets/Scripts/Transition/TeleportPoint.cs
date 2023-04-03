using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    [Header("��ӹ㲥")]
    public SceneLoadEventSO loadEventSO;

    public GameSceneSO sceneToGo;
    public Vector3 positionToGo;
    public void TriggerAction()
    {
        Debug.Log("����");

        //�㲥 sceneLoader����
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }
}
