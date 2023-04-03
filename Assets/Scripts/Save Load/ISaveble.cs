using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveble
{
    DataDefination GetDataID();
    void RegisterSaveData() => DataManager.instance.RegisterSaveDdata(this);
    void UnRegisterSaveData() => DataManager.instance.UnRegisterSaveDdata(this);

    void GetSaveData(Data data);
    void LoadData(Data data);
}
