using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour ,ISaveble
{
    [Header("�¼�����")]
    public VoidEventSO newGameEvent;

    [Header("��������")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("�����޵�")]
    public float invulnerableDuration;          //�޵�ʱ��
    [HideInInspector] public float invulnerableCounter;          //��ʱ��
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
        //ǿ��ִ��Isaveable���������ݹ�ȥ
        ISaveble saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= newGame;
        //ǿ��ִ��Isaveable���������ݹ�ȥ
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
                //���� ����Ѫ��
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
            //ִ������
            onTakeDamage?.Invoke(attcker.transform);
            TriggerInvulnerable();
        }
        else
        {
            currentHealth = 0;
            //��������
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

    //��������
    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    //���ݱ���
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

    //���ݼ���
    public void LoadData(Data data)
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            this.currentHealth = data.floatSaveData[GetDataID().ID + "Health"];
            this.currentPower = data.floatSaveData[GetDataID().ID + "Power"];
            transform.position = data.characterPosDict[GetDataID().ID].ToVector3();

            //֪ͨUI����
            OnHealthChange?.Invoke(this);
        }
    }
}
