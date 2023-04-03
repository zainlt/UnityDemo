using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("监听事件")]
    public SceneLoadEventSO sceneLoadEvent;
    public VoidEventSO afterSceneLoadEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;

    public PlayerInputControls inputControl;
    public Rigidbody2D rb;
    public Vector2 inputDirection;
    public PhysicCheck physicCheck;
    private CapsuleCollider2D coll;
    private PlayerAnimation playerAnimation;
    private Character character;

    [Header("基本参数")]
    public float speed;
    public float jumpForce;
    public float wallJumpForce;
    public float hurtForce;
    public float slideDistance;
    public float slideSpeed;
    private float runSpeed;
    public int slidePowerCost;

    private float walkSpeed => speed / 2.5f;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("状态")]
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;

    private Vector2 originalOffset;                 //胶囊碰撞体原始位移与尺寸
    private Vector2 originalSize;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        physicCheck = GetComponent<PhysicCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();

        originalOffset = coll.offset;
        originalSize = coll.size;

        inputControl = new PlayerInputControls();

        //跳跃
        inputControl.GamePlay.Jump.started += Jump; //+=是注册函数的内容，将函数方法添加到按键按下的时候执行

        #region 强制走路
        runSpeed = speed;
        inputControl.GamePlay.WalkButton.performed += ctx => {
            if (physicCheck.isGround) speed = walkSpeed;
        };
        inputControl.GamePlay.WalkButton.canceled += ctx => {
            if (physicCheck.isGround) speed = runSpeed;
        };
        #endregion

        //攻击
        inputControl.GamePlay.Attack.started += PlayerAttack;

        //滑铲
        inputControl.GamePlay.Slide.started += Slide;

        inputControl.Enable();
    }


    private void OnEnable()
    {
        sceneLoadEvent.loadRequestEvent += OnLoadEvent;
        afterSceneLoadEvent.OnEventRaised += OnAfterSceneLoadEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
    }
    private void OnDisable()
    {
        inputControl.Disable();
        sceneLoadEvent.loadRequestEvent -= OnLoadEvent;
        afterSceneLoadEvent.OnEventRaised -= OnAfterSceneLoadEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
    }

    private void Update()
    {
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();

        CheckState();
    }

    private void FixedUpdate()
    {
        if(!isHurt && !isAttack)
            Move();
    }

    //场景加载过程停止控制
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputControl.GamePlay.Disable();
    }

    //读取游戏进度
    private void OnLoadDataEvent()
    {
        isDead = false;
    }

    //场景加载过程恢复控制
    private void OnAfterSceneLoadEvent()
    {
        inputControl.GamePlay.Enable();
    }


    public void Move()
    {
        //限制下蹲/攻击/在墙上不能行走
        if (!isCrouch && !isAttack && !wallJump)
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        int faceDir = (int)transform.localScale.x;

        //人物翻转
        if (inputDirection.x > 0) faceDir = 1;
        if (inputDirection.x < 0) faceDir = -1;
        
        transform.localScale = new Vector3(faceDir, 1, 1);

        //下蹲
        isCrouch = inputDirection.y < -0.5f && physicCheck.isGround;

        if (isCrouch)
        {
            //修改碰撞体大小和位移
            coll.offset = new Vector2(-0.06f, 0.85f);
            coll.size = new Vector2(0.6f,1.7f);

        }
        else
        {
            //还原之前碰撞体参数
            coll.size = originalSize;
            coll.offset = originalOffset;
        }
    }

    //跳跃
    private void Jump(InputAction.CallbackContext obj)
    {
        //Debug.Log("JUMP");
        if (physicCheck.isGround)
        {
            //竖直方向上给一个力
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            GetComponent<AudioDefination>()?.PlayAudioClip();

            //打断协程
            isSlide = false;
            StopAllCoroutines();
        }
        else if (physicCheck.onWall)
        {
            //墙上跳跃
            rb.AddForce(new Vector2(-inputDirection.x, 2.5f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    //攻击
    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        //只有在地面上才可以攻击
        if (!physicCheck.isGround)
            return;
        playerAnimation.PlayAttack();
        isAttack = true;
    }

    //滑铲
    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;

            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);

            //更换图层 避免和敌人碰撞受伤
            //gameObject.layer = LayerMask.NameToLayer("Enemy");

            //进入协程
            StartCoroutine(TriggerSlide(targetPos));

            character.OnSlide(slidePowerCost);
        }
    }

    //协程
    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            //停下一帧后判断后续
            yield return null;
            if (!physicCheck.isGround)
                break;

            //滑铲过程中撞墙
            if (physicCheck.touchLeftWall && transform.localScale.x < 0f || physicCheck.touchRightWall && transform.localScale.x > 0f)
            {
                isSlide = false;
                break;
            }

            //朝目标方向一直移动
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));
        } while (MathF.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
        //gameObject.layer = LayerMask.NameToLayer("Player");
    }

    #region Events事件
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.GamePlay.Disable();
    }
    #endregion

    //更换材质
    private void CheckState()
    {
        coll.sharedMaterial = physicCheck.isGround ? normal : wall;

        //滑墙速度更改
        if (physicCheck.onWall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);

        if (wallJump && rb.velocity.y < 0f)
        {
            wallJump = false;
        }

        //滑铲或死亡时更换图层且不能更换方向
        if(isDead || isSlide)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            //inputControl.GamePlay.Move.Disable();
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Player");
            //inputControl.GamePlay.Move.Enable();
        }
    }
}
