using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicCheck))]
public class Enemy : MonoBehaviour
{

    protected Rigidbody2D rb;
    [HideInInspector]public Animator anim;
    [HideInInspector] public PhysicCheck physicCheck;
    //public Transform playerTrans;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currentSpeed;
    public Vector3 faceDirecter;
    public float hurtForce;

    public Transform attacker;

    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("计时器")]
    public float waitTime;
    [HideInInspector] public float waitTimeCounter;
    public bool wait;
    public float lostTime;
    public float lostTimeCounter;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;

    private BaseState currenState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState skillState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicCheck = GetComponent<PhysicCheck>();
        currentSpeed = normalSpeed;
    }

    //被激活的时候
    private void OnEnable()
    {
        currenState = patrolState;
        currenState.OnEnter(this);
    }


    private void Update()
    {
        faceDirecter = new Vector3(-transform.localScale.x, 0, 0);
        currenState.LogicUpdate();
        TimeCounter();
    }


    private void FixedUpdate()
    {
        currenState.PhysicsUpdate();
        if (!isHurt && !isDead && !wait)
            Move();
    }

    private void OnDisable()
    {
        currenState.OnExit();
    }

    public virtual void Move()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("snailPreMove") && !anim.GetCurrentAnimatorStateInfo(0).IsName("snailRecover"))
            rb.velocity = new Vector2(currentSpeed * faceDirecter.x * Time.deltaTime, rb.velocity.y);
    }

    //计时器
    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if(waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDirecter.x, 1, 1);
            }
        }
        if (!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        else if(FoundPlayer())
        {
            lostTimeCounter = lostTime;
        }
    }


    //检测player
    public bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDirecter, checkDistance, attackLayer);
    }

    //枚举
    public void switchSate(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Skill => skillState,
            _ => null
        };

        currenState.OnExit();
        currenState = newState;
        currenState.OnEnter(this);
    }

    #region 事件执行方法
    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        //受击转身
        if (attackTrans.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        if (attackTrans.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1, 1, 1);

        //受击击退
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);

        //协程
        StartCoroutine(OnHurt(dir));
    }

    //协程进行
    private IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        //受伤击退后等待一定时间 再退出受伤状态 避免人物继续受到伤害
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }


    public void OnDie()
    {
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        isDead = true;
    }

    //死亡后销毁
    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance*-transform.localScale.x,0), 0.2f);
    }

}
