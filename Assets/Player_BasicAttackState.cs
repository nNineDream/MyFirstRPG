using UnityEngine;

public class Player_BasicAttackState : EntityState
{
    private float attackVelocityTimer;
    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        attackVelocityTimer = player.attackVelocityDuration;
        GenerateAttackVelocity();
    }
    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();

        //detect and damage enemies


        if (triggercalled)
            stateMachine.ChangeState(player.idleState); 
    }

    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if(attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);

    }

    private void GenerateAttackVelocity()
    {
        player.SetVelocity(player.attackVelocity.x * player.facingDir, player.attackVelocity.y);
    }
}
