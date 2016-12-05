using UnityEngine;
using System.Collections;

public class DamageOnG : MonoBehaviour {

    public HealthController health;

    void Update()
    {
        if (Input.GetKeyDown("g"))
        {
            health.Damage(1);
        }
    }

}
