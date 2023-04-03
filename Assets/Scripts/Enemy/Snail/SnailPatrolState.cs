public class SnailPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }


    public override void LogicUpdate()
    {
        //����player�л���skill
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.switchSate(NPCState.Skill);
        }

        //����ǽ�ȴ�
        if (!currentEnemy.physicCheck.isGround || (currentEnemy.physicCheck.touchLeftWall && currentEnemy.faceDirecter.x < 0) || (currentEnemy.physicCheck.touchRightWall && currentEnemy.faceDirecter.x > 0))
        {
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("walk", false);
        }
        else
        {
            currentEnemy.anim.SetBool("walk", true);
        }
    }


    public override void PhysicsUpdate() 
    { 

    }
    public override void OnExit()
    {

    }
}
