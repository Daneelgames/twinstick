using UnityEngine;
using System.Collections;

public class HealthController : MonoBehaviour {

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

        GameManager.instance.playerController.unitSprite.color = Color.white;

        if (invisible)
            invisible = false;
    }

    void Death()
    {
        if (dropController != null)
            dropController.DeathDrop();

        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
}