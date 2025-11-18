using UnityEngine;

public class Player_BasicAttackState : EntityState
{
    private float attackVelocityTimer;

    const int FirstComboIndex = 1;
    private int comboIndex = 1;
    private int comboIndexLimit = 3;

    private float lastTimeAttacked;

    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if(comboIndexLimit != player.attackVelocity.Length) 
            comboIndexLimit = player.attackVelocity.Length;
    }
    public override void Enter()
    {
        base.Enter();
        ResetComboIndex();
        anim.SetInteger("basicAttackIndex", comboIndex);
        attackVelocityTimer = player.attackVelocityDuration;
        ApplyAttackVelocity();
    }
    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();
        //detect and damage enemies

        if (triggercalled)
            stateMachine.ChangeState(player.idleState); 
    }
    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastTimeAttacked = Time.time;

    }

    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;

        if(attackVelocityTimer < 0)
            player.SetVelocity(0, rb.linearVelocity.y);

    }

    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];
        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * player.facingDir, attackVelocity.y);
    }

    private void ResetComboIndex()
    {
        if(Time.time > lastTimeAttacked + player.comboResetTime)
            comboIndex = FirstComboIndex;

        if (comboIndex > comboIndexLimit)
            comboIndex = FirstComboIndex;
    }   
}
