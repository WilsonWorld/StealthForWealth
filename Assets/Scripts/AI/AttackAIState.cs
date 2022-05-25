
class AttackAIState : AIState
{
    public AttackAIState(Player owningCharacter, AIPlayerController aiController)
        : base(owningCharacter, aiController)
    {
    }

    public override void Activate()
    {
        AIController.NavAgent.updateRotation = false;
    }

    public override void Deactivate()
    {
        AIController.NavAgent.updateRotation = true;

        AIController.UseItem = false;
    }

    public override void Update()
    {

        //If you don't have a target, wander
        if (AIController.Target == null)
        {
            return;
        }

        //If you are close enough, attack.  Otherwise get closer.
        float distFromTargetSqrd = (AIController.Target.transform.position - Owner.transform.position).sqrMagnitude;

        if (distFromTargetSqrd <= AIController.MaxAttackRange * AIController.MaxAttackRange)
        {
            AIController.UseItem = true;
        }
        else
        {
            AIController.SetState(new MoveToAttackRangeAIState(Owner, AIController));
        }

        //Aim towards target
        UpdateAimDirection();
    }

    public override string GetName()
    {
        return "Attack State";
    }

    private void UpdateAimDirection()
    {
        if (AIController.Target == null)
        {
            return;
        }

        AIController.AimPosition = AIController.Target.transform.position;
    }
}
