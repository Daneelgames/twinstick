using UnityEngine;
using System.Collections;

public class CampfireController : MonoBehaviour
{
    public Transform spawnTransform;

    public void SpawnPlayer()
    {
        PlayerMovement pm = GameManager.instance.playerController;
        pm.transform.position = spawnTransform.position;
        pm.transform.rotation = spawnTransform.rotation;
        pm.playerHealth.SetHealth(pm.playerHealth.maxHealth);
    }
}