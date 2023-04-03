using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    [Header("广播")]
    public VoidEventSO saveDataEvent;

    [Header("变量参数")]
    public SpriteRenderer spriteRenderer;
    public GameObject lightObj;
    public Sprite darkSign;
    public Sprite lightSign;
    public bool isDone;


    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? lightSign : darkSign;
        lightObj.SetActive(isDone);
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            isDone = true;
            spriteRenderer.sprite = lightSign;
            lightObj.SetActive(true);

            //todo保存数据
            saveDataEvent.RaiseEvent();

            this.gameObject.tag = "Untagged";
        }
    }
}
