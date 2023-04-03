using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.InputSystem;


[DefaultExecutionOrder(-100)]
public class DataManager : MonoBehaviour 
{
    public static DataManager instance;

    [Header("�¼�����")]
    public VoidEventSO saveDataEvent;
    public VoidEventSO loadDataEvent;

    private List<ISaveble> saveableList = new List<ISaveble>();     //����һ������
    private Data saveData;

    private string jsonFolder;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this.gameObject);

        saveData = new Data();

        jsonFolder = Application.persistentDataPath+"/SAVE DATA/";

        ReadSavedData();
    }

    private void OnEnable()
    {
        saveDataEvent.OnEventRaised += Save;
        loadDataEvent.OnEventRaised += Load;
    }


    private void OnDisable()
    {
        saveDataEvent.OnEventRaised -= Save;
        loadDataEvent.OnEventRaised -= Load;
    }

    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Load();
        }
    }


    //ע��
    public void RegisterSaveDdata(ISaveble saveable)
    {
        if (!saveableList.Contains(saveable))
        {
            saveableList.Add(saveable);
        }
    }

    //ע��
    public void UnRegisterSaveDdata(ISaveble saveable)
    {
        saveableList.Remove(saveable);
    }

    public void Save()
    {
        foreach(var saveable in saveableList)
        {
            saveable.GetSaveData(saveData);
        }
        var resultPath = jsonFolder + "data.sav";

        //���л�
        var jsonData = JsonConvert.SerializeObject(saveData);

        if (!File.Exists(resultPath))
        {
            Directory.CreateDirectory(jsonFolder);
        }

        File.WriteAllText(resultPath, jsonData);

        //foreach (var item in saveData.characterPosDict)
        //{
        //    Debug.Log(item.Key + "  " + item.Value);
        //}
    }

    public void Load()
    {
        foreach (var saveable in saveableList)
        {
            saveable.LoadData(saveData);
        }
    }

    private void ReadSavedData()
    {
        var resultPath = jsonFolder + "data.sav";
        if (File.Exists(resultPath))
        {
            var stringData = File.ReadAllText(resultPath);

            //�����л� ����stringdata���͵�data�ļ�
            var jsonData = JsonConvert.DeserializeObject<Data>(stringData);

            saveData = jsonData;
        }
    }
}
