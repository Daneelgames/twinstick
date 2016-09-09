using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

    public int health = 1;

    public void Damage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
            Death();
    }

    void Death()
    {
        Destroy(gameObject);
    }
}
