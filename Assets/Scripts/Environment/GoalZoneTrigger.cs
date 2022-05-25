using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using UnityEngine;

public class GoalZoneTrigger : MonoBehaviour, Saveable
{
    private void OnTriggerEnter(Collider other)
    {
        Player playerObj = other.gameObject.GetComponent<Player>();
        AIPlayerController aiObj = other.gameObject.GetComponent<AIPlayerController>();

        if (playerObj != null && aiObj == null)
        {
            LevelManager LMObj = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            LMObj.OpenVictoryScreen();
        }
    }

    public void OnSave(Stream stream, IFormatter formatter)
    {
        SaveUtils.SerializeVector3(stream, formatter, transform.position);
        SaveUtils.SerializeQuaternion(stream, formatter, transform.rotation);
    }

    public void OnLoad(Stream stream, IFormatter formatter)
    {
        transform.position = SaveUtils.DeserializeVector3(stream, formatter);
        transform.rotation = SaveUtils.DeserializeQuaternion(stream, formatter);
    }
}
