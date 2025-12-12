using UnityEngine;

public class Player_BasicAttackState : EntityState
{
    private float attackVelocityTimer;
    private float lastTimeAttacked;

    private bool comboAttackQueued;
    const int FirstComboIndex = 1;
    private int comboIndex = 1;
    private int comboIndexLimit = 3;

    private int attackDir;

    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if(comboIndexLimit != player.attackVelocity.Length) 
            comboIndexLimit = player.attackVelocity.Length;
    }
    public override void Enter()
    {
        base.Enter();
        comboAttackQueued = false;
        ResetComboIndex();

        anim.SetInteger("basicAttackIndex", comboIndex);
        ApplyAttackVelocity();
    }
    public override void Update()
    {
        base.Update();
        HandleAttackVelocity();
        //detect and damage enemies

        attackDir = player.moveInput.x != 0 ? (int)player.moveInput.x : player.facingDir;

        if (input.Player.Attack.WasPressedThisFrame())
            QueueNextAttack();

        if (triggerCalled)
            HandleStateExit();
        
     }
    public override void Exit()
    {
        base.Exit();
        comboIndex++;
        lastTimeAttacked = Time.time;
         
    }

    private void QueueNextAttack()
    {
        if (comboIndex < comboIndexLimit)
            comboAttackQueued = true;
    }

    private void HandleStateExit()
    {
        if (comboAttackQueued)
        {
            anim.SetBool(animBoolName, false);
            player.EnterAttackStateWithDelay();

        }
        else
            stateMachine.ChangeState(player.idleState);
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
        player.SetVelocity(attackVelocity.x * attackDir, attackVelocity.y);
    }
    
    private void ResetComboIndex()
    {
        if(Time.time > lastTimeAttacked + player.comboResetTime)
            comboIndex = FirstComboIndex;

        if (comboIndex > comboIndexLimit)
            comboIndex = FirstComboIndex;
    }   
}
