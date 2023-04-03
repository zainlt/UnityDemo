using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraControl : MonoBehaviour
{
    [Header("事件监听")]
    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO camerShakeEvent;

    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    #region 监听摄像头抖动事件
    private void OnEnable()
    {
        camerShakeEvent.OnEventRaised += OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoaded;
    }

    private void OnDisable()
    {
        camerShakeEvent.OnEventRaised -= OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoaded;
    }

    private void OnAfterSceneLoaded()
    {
        //获取Bounds
        GetNewCameraBounds();
    }

    private void OnCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }
    #endregion


    //private void Start()
    //{
    //    //获取Bounds
    //    GetNewCameraBounds();
    //}


    private void GetNewCameraBounds()
    {
        //根据名字获取边界，注意文件名字不可出错
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null) return;

        //复制并清楚缓存
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        confiner2D.InvalidateCache();
    }

}
