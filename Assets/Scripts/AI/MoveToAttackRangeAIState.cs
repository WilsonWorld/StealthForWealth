﻿using System.Text;


class MoveToAttackRangeAIState : AIState
{
    public MoveToAttackRangeAIState(Player owningCharacter, AIPlayerController aiController)
        : base(owningCharacter, aiController)
    {
    }

    public override void Activate()
    {
        AIController.NavAgent.stoppingDistance = AIController.MaxAttackRange;
    }

    public override void Deactivate()
    {
    }

    public override void Update()
    {
        //If you are close enough to the target, attack.  Otherwise, move towards the target.
        float distFromTargetSqrd = (AIController.Target.transform.position - Owner.transform.position).sqrMagnitude;

        if (distFromTargetSqrd <= AIController.MaxAttackRange * AIController.MaxAttackRange)
        {
            AIController.SetState(new AttackAIState(Owner, AIController));
        }
        else
        {
            AIController.NavAgent.SetDestination(AIController.Target.transform.position);
            AIController.AimPosition = AIController.Target.transform.position;
        }
    }

    public override string GetName()
    {
        return "Move to attack range State";
    }

    public override void GetDebugOutput(StringBuilder debugOutput)
    {
        debugOutput.AppendLine("Off mesh link info:");
        debugOutput.AppendFormat("   On off mesh link:  {0}\n", AIController.NavAgent.isOnOffMeshLink);

        debugOutput.AppendFormat("   current link type: {0}\n", AIController.NavAgent.currentOffMeshLinkData.linkType.ToString());

        debugOutput.AppendFormat("   next link type: {0}\n", AIController.NavAgent.nextOffMeshLinkData.linkType.ToString());
    }
}
