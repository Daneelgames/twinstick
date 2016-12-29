﻿using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour
{

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

                if (health > 0)
                    StartCoroutine("InvisibleFrames");

                if (player)
                {
                    GameManager.instance.playerController.MoveBack(0.75f);
                    GameManager.instance.gui.SetHealth();
                }
            }
        }
    }

    IEnumerator InvisibleFrames()
    {
        invisible = true;

        yield return new WaitForSeconds(0.5f);

        if (invisible)
            invisible = false;
    }

    void Death()
    {
        if (player)
        {
            Time.timeScale = 0.3f;
            anim.SetTrigger("Hurt");
            GameManager.instance.PlayerDead();
        }
        else
        {
            if (mobController)
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
    public void Heal(int amount)
    {
        health += amount;

        if (health > maxHealth)
            health = maxHealth;

        if (player)
        {
            GameManager.instance.gui.SetHealth();
            StateManager.instance.UsePainkillers();
            StateManager.instance.SetPlayerHealth(health);
        }
    }
}