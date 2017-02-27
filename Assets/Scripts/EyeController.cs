using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeController : MonoBehaviour
{
    public bool lookAtPlayer = false;
    public Animator anim;
    float cooldown = 1f;
    void Update()
    {
        if (cooldown <= 0)
        {
			cooldown = Random.Range(0.1f,4f);
			anim.SetTrigger("Blink");
        }
		else
		{
			cooldown -= Time.deltaTime;
		}
        if (lookAtPlayer)
            transform.LookAt(GameManager.instance.playerInGame.transform.position + Vector3.up);
        else
            transform.LookAt(Camera.main.transform.position);
    }
}