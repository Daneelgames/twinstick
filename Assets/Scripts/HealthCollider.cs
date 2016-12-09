using UnityEngine;
using System.Collections;

public class HealthCollider : MonoBehaviour {

    public int attackPower = 3;

    public bool dangerous = false;
    public HealthController masterHealth;

    public void Damage(int amount)
    {
        masterHealth.Damage(amount);
    }

    void OnTriggerEnter(Collider coll)
    {
        if (dangerous && coll.tag == "HealthCollider")
        {
            if (coll.transform.parent != transform.parent)
            {
                coll.GetComponent<HealthCollider>().Damage(attackPower);
            }
        }
    }
}
