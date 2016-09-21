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
                if (health > 0)
                    StartCoroutine("PlayerInvisibleFrames");

                GameManager.instance.gui.SetHealth();
            }
        }
    }

    IEnumerator PlayerInvisibleFrames()
    {
        invisible = true;

        GameManager.instance.playerController.unitSprite.color = Color.clear;
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerController.unitSprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerController.unitSprite.color = Color.clear;
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerController.unitSprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.playerController.unitSprite.color = Color.clear;
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