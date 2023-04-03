using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("�����¼�")]
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

    [Header("��������")]
    public float speed;
    public float jumpForce;
    public float wallJumpForce;
    public float hurtForce;
    public float slideDistance;
    public float slideSpeed;
    private float runSpeed;
    public int slidePowerCost;

    private float walkSpeed => speed / 2.5f;

    [Header("�������")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("״̬")]
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;
    public bool wallJump;
    public bool isSlide;

    private Vector2 originalOffset;                 //������ײ��ԭʼλ����ߴ�
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

        //��Ծ
        inputControl.GamePlay.Jump.started += Jump; //+=��ע�ắ�������ݣ�������������ӵ��������µ�ʱ��ִ��

        #region ǿ����·
        runSpeed = speed;
        inputControl.GamePlay.WalkButton.performed += ctx => {
            if (physicCheck.isGround) speed = walkSpeed;
        };
        inputControl.GamePlay.WalkButton.canceled += ctx => {
            if (physicCheck.isGround) speed = runSpeed;
        };
        #endregion

        //����
        inputControl.GamePlay.Attack.started += PlayerAttack;

        //����
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

    //�������ع���ֹͣ����
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputControl.GamePlay.Disable();
    }

    //��ȡ��Ϸ����
    private void OnLoadDataEvent()
    {
        isDead = false;
    }

    //�������ع��ָ̻�����
    private void OnAfterSceneLoadEvent()
    {
        inputControl.GamePlay.Enable();
    }


    public void Move()
    {
        //�����¶�/����/��ǽ�ϲ�������
        if (!isCrouch && !isAttack && !wallJump)
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        int faceDir = (int)transform.localScale.x;

        //���﷭ת
        if (inputDirection.x > 0) faceDir = 1;
        if (inputDirection.x < 0) faceDir = -1;
        
        transform.localScale = new Vector3(faceDir, 1, 1);

        //�¶�
        isCrouch = inputDirection.y < -0.5f && physicCheck.isGround;

        if (isCrouch)
        {
            //�޸���ײ���С��λ��
            coll.offset = new Vector2(-0.06f, 0.85f);
            coll.size = new Vector2(0.6f,1.7f);

        }
        else
        {
            //��ԭ֮ǰ��ײ�����
            coll.size = originalSize;
            coll.offset = originalOffset;
        }
    }

    //��Ծ
    private void Jump(InputAction.CallbackContext obj)
    {
        //Debug.Log("JUMP");
        if (physicCheck.isGround)
        {
            //��ֱ�����ϸ�һ����
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            GetComponent<AudioDefination>()?.PlayAudioClip();

            //���Э��
            isSlide = false;
            StopAllCoroutines();
        }
        else if (physicCheck.onWall)
        {
            //ǽ����Ծ
            rb.AddForce(new Vector2(-inputDirection.x, 2.5f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    //����
    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        //ֻ���ڵ����ϲſ��Թ���
        if (!physicCheck.isGround)
            return;
        playerAnimation.PlayAttack();
        isAttack = true;
    }

    //����
    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;

            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);

            //����ͼ�� ����͵�����ײ����
            //gameObject.layer = LayerMask.NameToLayer("Enemy");

            //����Э��
            StartCoroutine(TriggerSlide(targetPos));

            character.OnSlide(slidePowerCost);
        }
    }

    //Э��
    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            //ͣ��һ֡���жϺ���
            yield return null;
            if (!physicCheck.isGround)
                break;

            //����������ײǽ
            if (physicCheck.touchLeftWall && transform.localScale.x < 0f || physicCheck.touchRightWall && transform.localScale.x > 0f)
            {
                isSlide = false;
                break;
            }

            //��Ŀ�귽��һֱ�ƶ�
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));
        } while (MathF.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
        //gameObject.layer = LayerMask.NameToLayer("Player");
    }

    #region Events�¼�
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

    //��������
    private void CheckState()
    {
        coll.sharedMaterial = physicCheck.isGround ? normal : wall;

        //��ǽ�ٶȸ���
        if (physicCheck.onWall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);

        if (wallJump && rb.velocity.y < 0f)
        {
            wallJump = false;
        }

        //����������ʱ����ͼ���Ҳ��ܸ�������
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
