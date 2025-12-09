using UnityEngine;

public class Player_JumpAttack : EntityState
{
    private bool touchedGround;
    public Player_JumpAttack(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        touchedGround = false;
        player.SetVelocity(player.jumpAttackVelocity.x * player.facingDir, player.jumpAttackVelocity.y);
    }
    public override void Update()
    {
        base.Update();

        if (player.groundDetected && !touchedGround)
        {
            touchedGround = true;
            anim.SetTrigger("jumpAttackTrigger");
            player.SetVelocity(0, rb.linearVelocity.y);
        }

        if(player.groundDetected && triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
