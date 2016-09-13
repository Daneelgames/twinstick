using UnityEngine;
using System.Collections;

public class RandomShooterAi : MonoBehaviour {

    public NpcController _npcController;

    public float minWaitTime = 0.5f;
    public float maxWaitTime = 7f;
    
    float waitTime = 0;

    public WeaponController weaponController;

    void Update()
    {
        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
        }

        Aiming();
    }

    void FixedUpdate()
    {
        if (waitTime <= 0)
        {
            if (GameManager.instance.playerInGame != null)
            {
                float distance = Vector2.Distance(transform.position, GameManager.instance.playerInGame.transform.position);
                if (distance < 17f)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, GameManager.instance.playerInGame.transform.position, distance, 1 << 9);
                    if (!hit)
                    {
                        Shoot();
                    }
                    else
                        print(hit.collider.gameObject.name); // ЖОПА
                }
            }
        }
    }

    void SetWaitTime()
    {
        waitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    void Shoot()
    {
        SetWaitTime();
        //print("MOB SHOOTS");
        weaponController.Shot();
    }

    void Aiming()
    {
        if (weaponController != null && _npcController.activeTarget != null)
        {
            Vector3 target_pos = _npcController.activeTarget.transform.position - transform.position;

            float angle = Mathf.Atan2(target_pos.y, target_pos.x) * Mathf.Rad2Deg;
            weaponController.transform.localEulerAngles = new Vector3(0, 0, angle);

            // FLIP SPRITES BASED ON ROTATION
            if (weaponController.transform.localRotation.z < -0.75f || weaponController.transform.localRotation.z > 0.75f)
            {
                weaponController.transform.localScale = new Vector3(1, -1, 1);
            }
            else if (weaponController.transform.localRotation.z >= -0.75f || weaponController.transform.localRotation.z <= 0.75f)
            {
                weaponController.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}