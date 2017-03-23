using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherBossAttackRangeController : MonoBehaviour
{
    public FatherBossController fbc;
    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "Player")
        {
			fbc.PlayerInAttackRange(true);
        }
    }
    void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Player")
        {
			fbc.PlayerInAttackRange(false);
        }
    }
}