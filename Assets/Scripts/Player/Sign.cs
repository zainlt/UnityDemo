using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sign : MonoBehaviour
{
    private PlayerInputControls playerInput;
    public Transform playerTrans;
    private Animator anim;
    public GameObject signSprite;
    private IInteractable targetItem;
    public bool canPress;

    private void Awake()
    {
        //anim.GetComponentInChildren<Animator>();
        anim = signSprite.GetComponent<Animator>();

        playerInput = new PlayerInputControls();
        playerInput.Enable();
    }


    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;

        playerInput.GamePlay.Confirm.started += OnConfirm;
    }

    private void OnDisable()
    {
        canPress = false;
    }

    private void Update()
    {
        signSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        signSprite.transform.localScale = playerTrans.localScale;
    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {
        if (canPress)
        {
            targetItem.TriggerAction();
            
            //也可以获取targetItem身上的AudioDefination来播放音效
            //根据物品的音效选择
            GetComponent<AudioDefination>()?.PlayAudioClip();
        }
    }

    //获取当前使用什么设备进行游戏
    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        //固定写法
        if(actionChange == InputActionChange.ActionStarted)
        {
            var d = ((InputAction)obj).activeControl.device;

            switch (d.device)
            {
                case Keyboard:
                    anim.Play("keyBoard");
                    break;
            }
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
        {
            canPress = true;
            //获取接口 到OnConfirm调用
            targetItem = other.GetComponent<IInteractable>();
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        canPress = false;
    }
}
