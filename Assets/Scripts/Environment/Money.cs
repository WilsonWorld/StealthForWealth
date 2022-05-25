using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public int Value = 1;

    private void OnTriggerEnter(Collider other)
    {
        Player playerObj = other.gameObject.GetComponent<Player>();
        AIPlayerController aiObj = other.gameObject.GetComponent<AIPlayerController>();

        if (playerObj != null && aiObj == null)
        {
            playerObj.MoneyCount += Value;

            LevelManager LM = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            LM.UpdateMoneyCounter(playerObj.MoneyCount);

            Destroy(this.gameObject);
        }
    }
}
