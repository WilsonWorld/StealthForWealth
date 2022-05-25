using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class SpottedAIState : AIState
{
    public SpottedAIState(Player owningCharacter, AIPlayerController aiController)
        : base(owningCharacter, aiController)
    {
    }

    public override void Activate()
    {
        // Move the main camera to face the player from the AI perspective
        Vector3 direction = AIController.PlayerRef.transform.position - AIController.transform.position;
        AIController.PlayerRef.GetComponent<Player>().Controller.AddLedgeDir(-direction.normalized);

        AIController.StartCoroutine(FailTimer());
    }

    public override void Deactivate()
    {
        Vector3 direction = AIController.PlayerRef.transform.position - AIController.transform.position;
        AIController.PlayerRef.GetComponent<Player>().Controller.AddLedgeDir(direction.normalized);
        AIController.NavAgent.destination = AIController.NextPatrolPoint.transform.position;
        AIController.PlayerRef.gameObject.GetComponent<Player>().OnGroundMaxSpeed = 15.0f;
    }

    public override void Update()
    {
        if (AIController.Target != null)
        {
                AIController.NavAgent.destination = AIController.transform.position;
                AIController.transform.LookAt(AIController.Target.transform);
                AIController.AimPosition = AIController.Target.transform.position;
                AIController.PlayerRef.gameObject.GetComponent<Player>().OnGroundMaxSpeed = 0.0f;
                AIController.PlayerRef.GetComponent<CapsuleCollider>().attachedRigidbody.velocity = new Vector3(0, 0, 0);
        }
    }

    public override string GetName()
    {
        return "Spotted Target State";
    }

    public override void GetDebugOutput(StringBuilder debugOutput)
    {
        debugOutput.AppendLine("Off mesh link info:");
        debugOutput.AppendFormat("    On off mesh link:  {0}\n", AIController.NavAgent.isOnOffMeshLink);

        debugOutput.AppendFormat("   link type: {0}", AIController.NavAgent.currentOffMeshLinkData.linkType.ToString());
    }

    void OpenFailScreen()
    {
        GameObject lmObj = GameObject.Find("LevelManager");
        lmObj.GetComponent<LevelManager>().OpenFailureScreen();
    }

    IEnumerator FailTimer()
    {
        yield return new WaitForSeconds(2.0f);

        OpenFailScreen();
    }

}
