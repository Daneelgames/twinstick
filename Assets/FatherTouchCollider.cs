using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherTouchCollider : MonoBehaviour
{
    public FatherBossController fbc;
    bool inRange = false;

    void OnTriggerEnter(Collider coll)
    {
        if (!inRange)
        {
            if (coll.tag == "Player" && fbc.stateBoss == FatherBossController.State.Sleep)
            {
				inRange = true;
				fbc.PlayerBehind();
            }
        }
    }
	void OnTriggerExit(Collider coll)
	{
		if (inRange && coll.tag == "Player")
		{
				inRange = false;
		}
	}
}