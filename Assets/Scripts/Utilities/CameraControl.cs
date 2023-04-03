using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraControl : MonoBehaviour
{
    [Header("�¼�����")]
    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO camerShakeEvent;

    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    #region ��������ͷ�����¼�
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
        //��ȡBounds
        GetNewCameraBounds();
    }

    private void OnCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }
    #endregion


    //private void Start()
    //{
    //    //��ȡBounds
    //    GetNewCameraBounds();
    //}


    private void GetNewCameraBounds()
    {
        //�������ֻ�ȡ�߽磬ע���ļ����ֲ��ɳ���
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (obj == null) return;

        //���Ʋ��������
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        confiner2D.InvalidateCache();
    }

}
