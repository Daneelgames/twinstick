using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherBossVisionController : MonoBehaviour
{
    public FatherBossController fbc;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
            fbc.PlayerInRange(true);
    }

	void OnTriggerExit (Collider coll)
	{
        if (coll.tag == "Player")
            fbc.PlayerInRange(false);
	}
}