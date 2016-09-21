using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcController : MonoBehaviour
{
    public GameObject activeTarget;
    public HealthController health;

    void Update()
    {
        if (activeTarget == null)
            activeTarget = GameManager.instance.playerInGame;
    }
}