using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.anim.SetBool("run", true);
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
            currentEnemy.switchSate(NPCState.Patrol);
        //×²Ç½Á¢¼´·­×ª
        if (!currentEnemy.physicCheck.isGround || (currentEnemy.physicCheck.touchLeftWall && currentEnemy.faceDirecter.x < 0) || (currentEnemy.physicCheck.touchRightWall && currentEnemy.faceDirecter.x > 0))
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDirecter.x, 1, 1);
        }
    }


    public override void PhysicsUpdate()
    {
    }
    public override void OnExit()
    {
        //currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        currentEnemy.anim.SetBool("run", false);
    }
}
