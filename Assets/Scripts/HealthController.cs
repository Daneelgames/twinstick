﻿using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

    public enum Type { Unit, Projectile}

    public Type objectType = Type.Unit;

    public int maxHealth = 1;
    public int health = 1;

    public bool player = false;

    public bool invisible = false;

    public DropOnDeathController dropController;

    public SpriteRenderer _sprite;

    public void SetInvisible(bool invs)
    {
        invisible = invs;
    }

    public void Damage(int dmg)
    {
        if (dmg > 0)
        {
            if (!invisible)
                health -= dmg;

            if (health <= 0)
            {
                health = 0;
                Death();
            }

            if (player)
            {
                if (health > 0 && gameObject.activeInHierarchy)
                    StartCoroutine("PlayerInvisibleFrames");

                GameManager.instance.gui.SetHealth();
            }
            else
            {
                if (gameObject.activeInHierarchy)
                    StartCoroutine("MobDamaged");
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

    IEnumerator MobDamaged()
    {
        _sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = Color.white;
    }

    IEnumerator PlayerInvisibleFrames()
    {
        invisible = true;

        _sprite.color = Color.clear;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = Color.clear;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = Color.clear;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = Color.clear;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = Color.clear;
        yield return new WaitForSeconds(0.1f);
        _sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);

        GameManager.instance.playerController.unitSprite.color = Color.white;

        if (invisible)
            invisible = false;
    }

    void Death()
    {
        if (dropController != null && !player)
            dropController.DeathDrop(false);

        switch (objectType)
        {
            case Type.Unit:
                gameObject.SetActive(false);
                break;

            case Type.Projectile:
                DestroyObject();
                break;
        }

        if (player)
        {
            GameManager.instance.PlayerDead();
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