using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcController : MonoBehaviour
{
    public GameObject activeTarget;

    void Update()
    {
        if (activeTarget == null)
            activeTarget = GameManager.instance.playerInGame;
    }
}