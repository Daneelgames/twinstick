using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

    public int health = 1;

    public bool invisible = false;

    public void SetInvisible(bool invs)
    {
        invisible = invs;
    }

    public void Damage(int dmg)
    {
        if(!invisible)
            health -= dmg;

        if (health <= 0)
            Death();
    }

    void Death()
    {
        Destroy(gameObject);
    }
}