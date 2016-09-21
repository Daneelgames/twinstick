using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

    public int maxHealth = 1;
    public int health = 1;

    public bool player = false;

    public bool invisible = false;

    public DropOnDeathController dropController;

    public void SetInvisible(bool invs)
    {
        invisible = invs;
    }

    public void Damage(int dmg)
    {
        if(!invisible)
            health -= dmg;

        if (player)
            GameManager.instance.gui.SetHealth();

        if (health <= 0)
            Death();
    }

    void Death()
    {
        if (dropController != null)
            dropController.DeathDrop();

        Destroy(gameObject);
    }
}