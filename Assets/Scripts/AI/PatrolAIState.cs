using UnityEngine;
using System.Text;

public class PatrolAIState : AIState
{
    public PatrolAIState(Player owningCharacter, AIPlayerController aiController)
        : base(owningCharacter, aiController)
    {
    }

    public override void Activate()
    {
        PatrolPointsIndex = 0;
        AIController.NextPatrolPoint = AIController.PatrolPoints[PatrolPointsIndex];
    }

    public override void Deactivate()
    {
    }

    public override void Update()
    {
        if (AIController.PatrolPoints != null)
        {
            Vector3 distance = Owner.transform.position - AIController.NextPatrolPoint.transform.position;
            if (Vector3.Distance(Owner.transform.position, AIController.NextPatrolPoint.transform.position) < 1.0f)
            {
                UpdateNextPatrolPoint();
            }
            else
            {
                AIController.NavAgent.SetDestination(AIController.NextPatrolPoint.transform.position);
                AIController.AimPosition = AIController.NextPatrolPoint.transform.position;
            }
        }
    }

    public override string GetName()
    {
        return "Patrol State";
    }

    public override void GetDebugOutput(StringBuilder debugOutput)
    {
        debugOutput.AppendLine("Off mesh link info:");
        debugOutput.AppendFormat("   On off mesh link:  {0}\n", AIController.NavAgent.isOnOffMeshLink);

        debugOutput.AppendFormat("   current link type: {0}\n", AIController.NavAgent.currentOffMeshLinkData.linkType.ToString());

        debugOutput.AppendFormat("   next link type: {0}\n", AIController.NavAgent.nextOffMeshLinkData.linkType.ToString());
    }

    void UpdateNextPatrolPoint()
    {
        if (PatrolPointsIndex < AIController.PatrolPoints.Count)
        {
            AIController.NextPatrolPoint = AIController.PatrolPoints[PatrolPointsIndex];
            ++PatrolPointsIndex;
        }
        else if (PatrolPointsIndex == AIController.PatrolPoints.Count)
        {
            PatrolPointsIndex = 0;
            AIController.NextPatrolPoint = AIController.PatrolPoints[PatrolPointsIndex];
        }
    }

    int PatrolPointsIndex = -1;
}
