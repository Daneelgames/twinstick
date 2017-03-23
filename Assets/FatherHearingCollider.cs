using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherHearingCollider : MonoBehaviour
{
    public FatherBossController fbc;
    public bool playerInRange = false;
    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player" && !playerInRange)
        {
            playerInRange = true;
            if (fbc.stateBoss == FatherBossController.State.Sleep && GameManager.instance.playerController.run) // if player already runs
            {
                StartCoroutine("FatherReposition");
            }
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Player" && playerInRange)
        {
            playerInRange = false;
        }
    }

    IEnumerator FatherReposition()
    {
        yield return new WaitForSeconds(0.5f);
        fbc.Reposition();
    }

    void Update()
    {
        if (playerInRange)
        {
            if (fbc.stateBoss == FatherBossController.State.Sleep && GameManager.instance.playerController.run) // if player already runs
            {
                StartCoroutine("FatherReposition");
            }
        }
    }
}