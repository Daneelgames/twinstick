using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

    public int maxHealth = 1;
    public int health = 1;

    public bool player = false;

    public bool invisible = false;

    public Animator anim;

    public MobMovement mobController;

    public void SetInvisible(bool invs)
    {
        invisible = invs;
    }

    public void SetHealth(int amount)
    {
        health = amount;
    }

    public void Damage(int dmg)
    {
        if (dmg > 0 && health > 0)
        {
            if (!invisible)
            {
                health -= dmg;

                if (health <= 0)
                {
                    health = 0;
                    Death();
                }
                else
                {
                    anim.SetTrigger("Hurt");

                    if (mobController)
                        mobController.Hurt();
                }

                if (player)
                {
                    if (health > 0)
                        StartCoroutine("PlayerInvisibleFrames");

                    GameManager.instance.playerController.MoveBack(0.75f);
                    GameManager.instance.gui.SetHealth();
                }
            }
        }
    }

    public void AddToMaxHealth(int amount)
    {
        maxHealth += amount;
        Heal(maxHealth);

        if (player)
            GameManager.instance.gui.SetHealth();
    }

    IEnumerator PlayerInvisibleFrames()
    {
        invisible = true;

        yield return new WaitForSeconds(0.75f);

        if (invisible)
            invisible = false;
    }

    void Death()
    {
        if (player)
        {
            GameManager.instance.PlayerDead();
        }
        else
        {
            mobController.Dead();
            anim.SetTrigger("Dead");
            anim.SetBool("Dead.persist", true);
        }
    }

    void DestroyObject()
    {
        // use for destroying bullets
        Destroy(gameObject);
    }

    public void Heal (int amount)
    {
        health += amount;

        if (health > maxHealth)
            health = maxHealth;

        if (player)
            GameManager.instance.gui.SetHealth();
    }
}