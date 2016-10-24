using UnityEngine;
using System.Collections;

public class CampfireController : MonoBehaviour
{
    public Transform spawnTransform;

    public void SpawnPlayer()
    {
        GameManager.instance.playerInGame.transform.position = spawnTransform.position;
        GameManager.instance.playerInGame.transform.rotation = spawnTransform.rotation;
        GameManager.instance.playerController.playerHealth.Heal(GameManager.instance.playerController.playerHealth.maxHealth);
    }
}