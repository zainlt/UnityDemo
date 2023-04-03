using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    //字典保存
    public string sceneToSave;
    public Dictionary<string, SerializeVector3> characterPosDict = new Dictionary<string, SerializeVector3>();
    public Dictionary<string, float> floatSaveData = new Dictionary<string, float>();

    public void SaveGameScene(GameSceneSO savedScene)
    {
        //JsonUtility可以将一个Object转换成string类型
        sceneToSave = JsonUtility.ToJson(savedScene);
    }

    public GameSceneSO GetSavedScene()
    {
        //创建空实例 so文件
        var newScene = ScriptableObject.CreateInstance<GameSceneSO>();

        //反序列化后覆盖newScene
        JsonUtility.FromJsonOverwrite(sceneToSave, newScene);

        return newScene;
    }
}


public class SerializeVector3
{
    public float x, y, z;

    public SerializeVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}