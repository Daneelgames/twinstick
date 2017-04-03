using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthCollider : MonoBehaviour
{
    public List<WeaponController.Type> immuneTo;
    public int attackPower = 3;
    public bool dangerous = false;
    public bool dangerousGrab = false;
    public int grabAttackPower = 3;
    public float grabAttackTime = 1f;
    public Transform grabTransform;
    public string grabAnimBool = "";
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
        if (canHurt)
        {
            print(amount);
            masterHealth.Damage(amount);
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        if (dangerous && coll.tag == "HealthCollider")
        {
            print(coll.gameObject.layer);
            if (coll.transform.parent != transform.parent)
            {
                coll.GetComponent<HealthCollider>().Damage(attackPower + Random.Range(-attackPower * 1.0f / 4.0f, attackPower * 1.0f / 4.0f), WeaponController.Type.Melee);
            }
        }
        else if (dangerousGrab && coll.tag == "HealthCollider" && coll.gameObject.layer == 11)
        {
            print(coll.gameObject.layer);
            GameManager.instance.playerController.playerHealth.DamageGrab(grabAttackPower, grabAnimBool, grabAttackTime, grabTransform);
        }
    }
}
