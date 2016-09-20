using UnityEngine;
using System.Collections;

public class RandomShooterAi : MonoBehaviour {

    public NpcController _npcController;

    public float minWaitTime = 0.5f;
    public float maxWaitTime = 7f;
    
    float waitTime = 0;

    public WeaponController weaponController;

    public SpriteRenderer unitSprite;
    public SpriteRenderer weaponSprite;

    public LineRenderer line;

    RaycastHit2D targetHit;
    float tragetDistance = 0;

    void Start()
    {
        InvokeRepeating("ReadyToShoot", 1, 0.1f);
    }

    void ReadyToShoot()
    {
        if (GameManager.instance.playerInGame != null)
        {
            targetHit = Physics2D.Raycast(transform.position, GameManager.instance.playerInGame.transform.position - transform.position, tragetDistance, 1 << 9);
            tragetDistance = Vector2.Distance(transform.position, GameManager.instance.playerInGame.transform.position);
            if (tragetDistance < 17f)
            {
                line.SetPosition(0, transform.position);
                line.SetPosition(1, GameManager.instance.playerInGame.transform.position);

                if (targetHit)
                {
                    line.SetPosition(1, targetHit.point);
                    print(targetHit.point);
                }
                else
                {
                    Aiming();
                    waitTime -= Time.deltaTime;

                    if (waitTime <= 0)
                        Shoot();
                }
                // else
                //     print(hit.collider.gameObject.name); // ass
            }
        }
        unitSprite.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;
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
        }
        // FLIP SPRITES BASED ON ROTATION
        if (weaponController.transform.localRotation.z < -0.75f || weaponController.transform.localRotation.z > 0.75f)
        {
            weaponController.transform.localScale = new Vector3(1, -1, 1);
        }
        else if (weaponController.transform.localRotation.z >= -0.75f || weaponController.transform.localRotation.z <= 0.75f)
        {
            weaponController.transform.localScale = new Vector3(1, 1, 1);
        }
        // weapon sorting
        if (weaponController.transform.localRotation.z > 0) // weapon behind
        {
            weaponSprite.sortingOrder = unitSprite.sortingOrder - 1;
        }
        else if (weaponController.transform.localRotation.z < 0) // weapon in front
        {
            weaponSprite.sortingOrder = unitSprite.sortingOrder + 1;
        }
    }
}