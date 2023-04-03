using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO, Vector3, bool> loadRequestEvent;

    //������������
    //Ҫ���صĳ�����PlayerĿ�����ꡢ�Ƿ��뽥��
    public void RaiseLoadRequestEvent(GameSceneSO locationToLoad,Vector3 posToGo,bool fadeScreen)
    {
        loadRequestEvent?.Invoke(locationToLoad, posToGo, fadeScreen);
    }
}
