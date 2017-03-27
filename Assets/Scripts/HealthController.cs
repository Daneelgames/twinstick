using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HealthController : MonoBehaviour
{

    public int maxHealth = 1;
    public float health = 1;
    public float hurtCooldown = 1f;

    public bool player = false;

    public bool invisible = false;

    public Animator anim;

    public MobMovement mobController;
    public AudioSource au;
    public List<AudioClip> hurtClips;
    public List<AudioClip> deathClips;
    public BreakableObject breakableObj;
    public FatherBossController fbc;

    public void SetInvisible(bool invs)
    {
        invisible = invs;
    }

    public void SetHealth(float amount)
    {
        health = amount;
    }

    void Start()
    {
        if (au && mobController && health > 0)
            au.Play();
    }

    public void Damage(float dmg)
    {
        if (dmg > 0 && health > 0)
        {
            if (!invisible)
            {
                health -= dmg;
                if (au)
                    au.pitch = Random.Range(0.75f, 1.25f);

                if (health <= 0)
                {
                    health = 0;
                    if (mobController)
                        mobController.Dead();
                    Death();
                }
                else
                {
                    anim.SetTrigger("Hurt");
                    if (fbc)
                    {
                        fbc.Hurt();
                    }
                    if (mobController)
                        mobController.Hurt();
                    if (hurtClips.Count > 0)
                        au.PlayOneShot(hurtClips[Random.Range(0, hurtClips.Count)]);
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

        yield return new WaitForSeconds(hurtCooldown);

        if (invisible)
            invisible = false;
    }

    public void Death()
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
                au.Stop();
            if (deathClips.Count > 0)
                au.PlayOneShot(deathClips[Random.Range(0, deathClips.Count)]);
            anim.SetTrigger("Dead");
            anim.SetBool("Dead.persist", true);
            if (breakableObj)
                breakableObj.Break();
        }
    }

    public void StopSound()
    {
        if (mobController)
            au.Stop();
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