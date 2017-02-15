using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthCollider : MonoBehaviour
{
    public List<WeaponController.Type> immuneTo;
    public int attackPower = 3;
    public bool dangerous = false;
    public HealthController masterHealth;

    public void Damage(float amount, WeaponController.Type attackType)
    {
        bool canHurt = true;
        foreach (WeaponController.Type j in immuneTo)
        {
            if (j == attackType)
            {
                canHurt = false;
                break;
            }
        }
        print(canHurt);
        if (canHurt)
        {
            masterHealth.Damage(amount);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (dangerous && coll.tag == "HealthCollider")
        {
            if (coll.transform.parent != transform.parent)
            {
                coll.GetComponent<HealthCollider>().Damage(attackPower + Random.Range(-attackPower * 1.0f / 4.0f, attackPower * 1.0f / 4.0f), WeaponController.Type.Melee);
            }
        }
    }
}
