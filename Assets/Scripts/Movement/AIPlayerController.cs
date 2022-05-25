using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AIPlayerController : MonoBehaviour, PlayerController, Saveable
{
    //AI Settings.  These will be shared with all of the AI States
    public float MaxAttackRange = 15.0f;

    public float MaxSightRange = 25.0f;

    public float MinTimeToChangeDirection = 1.0f;
    public float MaxTimeToChangeDirection = 5.0f;

    public float ArriveAtDestinationDist = 2.0f;

    public int PreferedWeaponIndex = 0;

    public bool UseNavMeshAgentMovement = false;

    public GameObject PlayerRef;

    public List<GameObject> PatrolPoints;

    public int EnemyID = -1;

    public Renderer Renderer;

    bool HasSpottedPlayer = false;

    public void Init(Player owner)
    {
        Owner = owner;

        m_ItemToSwitchTo = PreferedWeaponIndex;

        //Set up nav mesh   
        NavAgent = GetComponent<NavMeshAgent>();

        //We want to use the actual player's movement instead of the nav mesh movement.  This will turn
        //off the nav mesh agent automatic movement.
        if (!UseNavMeshAgentMovement)
        {
            NavAgent.updatePosition = false;
        }

        HasSpottedPlayer = false;
    }

    public void UpdateControls()
    {
        //Update the state if you have one
        if (m_CurrentAIState != null)
        {
            m_CurrentAIState.Update();
        }

        //Since NavMeshAgent.updatePosition is false, the AI's position will not be automatically be
        //synchronized with the internal NavMeshAgent position.  This call will update the position
        //within the NavMeshAgent
        if (!UseNavMeshAgentMovement)
        {
            NavAgent.nextPosition = transform.position;
        }

        //Update debug info
        UpdateDebugDisplay();
    }

    public void SetState(AIState state)
    {
        if (state != m_CurrentAIState && m_CurrentAIState != null && m_CurrentAIState.GetName() == state.GetName())
            return;

        //Deactivate your old state
        if (m_CurrentAIState != null)
        {
            m_CurrentAIState.Deactivate();
        }

        //switch to the new state
        m_CurrentAIState = state;

        //Activate the new state
        if (m_CurrentAIState != null)
        {
            m_CurrentAIState.Activate();
        }
    }

    void Update()
    {
        if (Renderer)
        {
            if (EnemyID == 0)
            {
                Renderer.material.SetVector("_Position1", transform.position);
                Renderer.material.SetVector("_Direction1", transform.forward);
            }
            else if (EnemyID == 1)
            {
                Renderer.material.SetVector("_Position2", transform.position);
                Renderer.material.SetVector("_Direction2", transform.forward);
            }
            else if (EnemyID == 2)
            {
                Renderer.material.SetVector("_Position3", transform.position);
                Renderer.material.SetVector("_Direction3", transform.forward);
            }
            else
            {
                DebugUtils.LogError("Invalid Enemy ID #");
                return;
            }
        }

        bool PlayerIsSeen = IsPointInsideCone(PlayerRef.transform.position, Owner.transform.position, Owner.transform.forward, 45.0f, MaxSightRange);

        if (PlayerIsSeen)
        {
            Vector3 direction = PlayerRef.transform.position - Owner.transform.position;

            RaycastHit hit;
            Physics.Raycast(Owner.transform.position, direction.normalized, out hit, MaxSightRange);

            if (hit.collider.gameObject.GetComponent<Player>() != null && hit.collider.gameObject.GetComponent<AIPlayerController>() == null)
            {
                PlayerIsSeen = true;
            }
            else
            {
                PlayerIsSeen = false;
            }
        }

        if (PlayerIsSeen)
        {
                SetState(new SpottedAIState(Owner, this));
                Target = PlayerRef;
        }
        else
        {
            SetState(new PatrolAIState(Owner, this));
            Target = null;
        }
    }

    bool IsPointInsideCone(Vector3 point, Vector3 originPos, Vector3 originDir, float maxAngle, float maxDist)
    {
        var DistFromOrigin = (point - originPos).magnitude;

        if (DistFromOrigin < maxDist)
        {
            var pointDir = point - originPos;
            var angle = Vector3.Angle(originDir, pointDir);

            if (angle < maxAngle)
            {
                return true;
            }
        }

        return false;
    }

    public void SetFacingDirection(Vector3 direction)
    {
        Owner.transform.rotation = Quaternion.LookRotation(direction);
    }

    public void AddLedgeDir(Vector3 ledgeDir)
    {
    }

    public void OnSave(Stream stream, IFormatter formatter)
    {
        SaveUtils.SerializeVector3(stream, formatter, transform.position);
        SaveUtils.SerializeQuaternion(stream, formatter, transform.rotation);
        //SaveUtils.SerializeObjectRef(stream, formatter, NextPatrolPoint);

        for (int i = 0; i < PatrolPoints.Count; i++)
        {
            SaveUtils.SerializeObjectRef(stream, formatter, PatrolPoints[i]);
        }
    }

    public void OnLoad(Stream stream, IFormatter formatter)
    {
        transform.position = SaveUtils.DeserializeVector3(stream, formatter);
        transform.rotation = SaveUtils.DeserializeQuaternion(stream, formatter);
        //NextPatrolPoint = SaveUtils.DeserializeObjectRef(stream, formatter);

        for (int i = 0; i < PatrolPoints.Count; i++)
        {
            PatrolPoints[i] = SaveUtils.DeserializeObjectRef(stream, formatter);
        }

        Renderer = GameObject.Find("TestSurface").GetComponent<Renderer>();
        Renderer.material.shader = Shader.Find("FieldOfView");

        NextPatrolPoint = PatrolPoints[0];
    }

    public Player Owner { get; private set; }

    public GameObject Target { get; set; }

    public GameObject NextPatrolPoint { get; set; }

    public Vector3 AimPosition { get; set; }

    public bool UseItem { get; set; }

    public UnityEngine.AI.NavMeshAgent NavAgent { get; private set; }

    #region Input Getting Functions

    public Vector3 GetControlRotation()
    {
        Vector3 lookDirection = AimPosition - transform.position;
        lookDirection.y = 0.0f;

        if (lookDirection.sqrMagnitude > MathUtils.CompareEpsilon)
        {
            return Quaternion.LookRotation(lookDirection).eulerAngles;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 GetMoveInput()
    {
        if (m_CurrentAIState != null)
        {
            return m_CurrentAIState.GetMoveInput();
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 GetLookInput()
    {
        return Vector3.zero;
    }

    public Vector3 GetAimTarget()
    {
        return AimPosition;
    }

    public bool IsJumping()
    {
        return false;
    }

    public bool IsFiring()
    {
        return UseItem;
    }

    public bool IsAiming()
    {
        return false;
    }

    public bool ToggleCrouch()
    {
        return false;
    }

    public bool SwitchToItem1()
    {
        return HandleItemSwitch(0);
    }

    public bool SwitchToItem2()
    {
        return HandleItemSwitch(1);
    }

    public bool SwitchToItem3()
    {
        return HandleItemSwitch(2);
    }

    public bool SwitchToItem4()
    {
        return HandleItemSwitch(3);
    }

    public bool SwitchToItem5()
    {
        return HandleItemSwitch(4);
    }

    public bool SwitchToItem6()
    {
        return HandleItemSwitch(5);
    }

    public bool SwitchToItem7()
    {
        return HandleItemSwitch(6);
    }

    public bool SwitchToItem8()
    {
        return HandleItemSwitch(7);
    }

    public bool SwitchToItem9()
    {
        return HandleItemSwitch(8);
    }

    public Vector3 GetLedgeDir()
    {
        return Vector3.zero;
    }

    #endregion


    #region Private Members

    [Conditional("UNITY_EDITOR")]
    void UpdateDebugDisplay()
    {
        //Display useful vectors
        UnityEngine.Debug.DrawLine(transform.position, NavAgent.destination);
        UnityEngine.Debug.DrawLine(transform.position, transform.position + NavAgent.desiredVelocity, Color.red);

        //Display to the AI debug GUI
        AIDebugGUI debugGUI = Camera.main.GetComponent<AIDebugGUI>();
        if (debugGUI != null)
        {
            //Ignore this if the object isn't selected.  Note that this won't work properly if there is more
            //than one ai entity selected.
            if (Selection.Contains(gameObject))
            {
                StringBuilder debugOutput = new StringBuilder();

                //Output state
                debugOutput.Append("CurrentState = ");

                if (m_CurrentAIState != null)
                {
                    debugOutput.Append(m_CurrentAIState.GetName());
                }
                else
                {
                    debugOutput.Append("null");
                }

                //Ouput other debug info
                if (m_CurrentAIState != null)
                {
                    debugOutput.AppendLine();

                    m_CurrentAIState.GetDebugOutput(debugOutput);
                }

                //Set Debug String
                debugGUI.AIDebugDisplayMsg = debugOutput.ToString();
            }
        }
    }

    bool HandleItemSwitch(int indexToCheck)
    {
        if (indexToCheck == m_ItemToSwitchTo)
        {
            m_ItemToSwitchTo = InvalidWeaponIndex;

            return true;
        }
        else
        {
            return false;
        }
    }

    const int InvalidWeaponIndex = -1;

    AIState m_CurrentAIState;

    int m_ItemToSwitchTo;

    #endregion
}
