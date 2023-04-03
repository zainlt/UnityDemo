using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour ,ISaveble
{
    [Header("事件监听")]
    public VoidEventSO newGameEvent;

    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("受伤无敌")]
    public float invulnerableDuration;          //无敌时间
    [HideInInspector] public float invulnerableCounter;          //计时器
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> onTakeDamage;
    public UnityEvent onDie;

    private void newGame()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }



    private void OnEnable()
    {
        newGameEvent.OnEventRaised += newGame;
        //强制执行Isaveable，将自身传递过去
        ISaveble saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= newGame;
        //强制执行Isaveable，将自身传递过去
        ISaveble saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }

        if (currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            if (currentHealth > 0)
            {
                currentHealth = 0;
                //死亡 更新血量
                OnHealthChange?.Invoke(this);
                onDie?.Invoke();
            }
        }
    }

    public void TakeDamage(Attack attcker)
    {
        if (invulnerable) return;

        if(currentHealth - attcker.damage > 0)
        {
            currentHealth -= attcker.damage;
            //执行受伤
            onTakeDamage?.Invoke(attcker.transform);
            TriggerInvulnerable();
        }
        else
        {
            currentHealth = 0;
            //触发死亡
            onDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
    }

    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    //滑铲消耗
    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    //数据保存
    public void GetSaveData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            data.characterPosDict[GetDataID().ID] = new SerializeVector3(transform.position);
            data.floatSaveData[GetDataID().ID + "Health"] = this.currentHealth;
            data.floatSaveData[GetDataID().ID + "Power"] = this.currentPower;
        }
        else
        {
            data.characterPosDict.Add(GetDataID().ID, new SerializeVector3(transform.position));
            data.floatSaveData.Add(GetDataID().ID + "Health", this.currentHealth);
            data.floatSaveData.Add(GetDataID().ID + "Power", this.currentPower);
        }
    }

    //数据加载
    public void LoadData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            this.currentHealth = data.floatSaveData[GetDataID().ID + "Health"];
            this.currentPower = data.floatSaveData[GetDataID().ID + "Power"];
            transform.position = data.characterPosDict[GetDataID().ID].ToVector3();

            //通知UI更新
            OnHealthChange?.Invoke(this);
        }
    }
}
