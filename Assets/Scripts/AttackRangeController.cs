using UnityEngine;
using System.Collections;

public class AttackRangeController : MonoBehaviour {

    public int rangeIndex = 0;
    public MobMovement mobController;

    void OnTriggerEnter(Collider coll)
    {
        if (coll.tag == "HealthCollider")
        {
            if (coll.transform.parent.tag == "Player")
                mobController.PlayerInRange(rangeIndex, true);
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "HealthCollider")
        {
            if (coll.transform.parent.tag == "Player")
                mobController.PlayerInRange(rangeIndex, false);
        }
    }
}