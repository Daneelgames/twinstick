using UnityEngine;
using System.Collections;

public class CampfireController : MonoBehaviour
{
    public Transform spawnTransform;


    public void SpawnPlayer()
    {
        GameManager.instance.playerInGame.transform.position = spawnTransform.position;
        GameManager.instance.playerController.playerHealth.Heal(GameManager.instance.playerController.playerHealth.maxHealth);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && GameManager.instance.playerController.playerHealth.health > 0)
        {
            GameManager.instance.CampfireToInteract(this);
        }
    }
    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player" && GameManager.instance.playerController.playerHealth.health > 0)
        {
            GameManager.instance.CampfireToInteract(null);
        }
    }
}