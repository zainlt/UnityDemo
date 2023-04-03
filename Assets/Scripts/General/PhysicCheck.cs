using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicCheck : MonoBehaviour
{
    private PlayerController playerController;
    private Rigidbody2D rb;
    [Header("������")]
    public bool isPlayer;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public float checkRadius;
    public LayerMask groundLayer;

    [Header("״̬")]
    public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;
    public bool onWall;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (isPlayer)
        {
            playerController = GetComponent<PlayerController>();
        }
    }
    private void Update()
    {
        Check();
    }
    public void Check()
    {
        //������
        if (onWall)
            isGround = Physics2D.OverlapCircle((Vector2)transform.position +
                new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius, groundLayer);
        else
            isGround = Physics2D.OverlapCircle((Vector2)transform.position +
                new Vector2(bottomOffset.x * transform.localScale.x, 0), checkRadius, groundLayer);

        //���ǽ��
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + 
            new Vector2(leftOffset.x,leftOffset.y), checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position +
             new Vector2(rightOffset.x, rightOffset.y), checkRadius, groundLayer);

        //��ǽ��
        if (isPlayer)
            onWall = (touchLeftWall && playerController.inputDirection.x < 0f || touchRightWall && playerController.inputDirection.x > 0f) && rb.velocity.y < 0;
    }

    //��һ�����Ȧ
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + 
            new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position +
            new Vector2(leftOffset.x, leftOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position +
            new Vector2(rightOffset.x, rightOffset.y), checkRadius);
    }
}
