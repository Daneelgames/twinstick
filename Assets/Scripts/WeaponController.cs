using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour {

    public enum Type {Bullet, Shell, Melee};

    public int ammoMax = 0;
    public int ammo = 0;

    public bool automatic = false;

    public float cooldownTime = 0.25f;
    public float curCooldown = 0f;

    public GameObject shotHolder;

    public bool dangerous = false;
    public int meleeDamage = 1;
    public List<GameObject> bullets;

    public Type weaponAmmoType = Type.Bullet;

    /* TEST AIMING
    public LineRenderer line;

    void FixedUpdate()
    {
        if (inHands)
        {
            line.SetPosition(0, shotHolder.transform.position);
            line.SetPosition(1, GameManager.instance.playerController.aimTarget);
        }
    }
    */

    void Update () {
        if (curCooldown > 0)
        {
            curCooldown -= Time.deltaTime;
        }
    }

    public void Reload(int reloadAmount)
    {
        ammo += reloadAmount;
    }

    void OnTriggerEnter(Collider col)
    {
      if (dangerous && col.gameObject.layer == 8 && col.tag == "HealthCollider")   // 8 layer is Unit
      {
          col.GetComponent<HealthCollider>().Damage(meleeDamage);
      }
    }

    public void Attack(Vector3 target)
    {
        if (curCooldown <= 0)
        {
            if (weaponAmmoType == Type.Bullet || weaponAmmoType == Type.Shell)
            {
                if (ammo > 0)
                {
                    ammo -= 1;
                    curCooldown = cooldownTime;
                    for (int i = 0; i < bullets.Count; i++)
                    {
                        GameObject newBullet = Instantiate(bullets[i], shotHolder.transform.position, Quaternion.identity) as GameObject;
                        BulletController newBulletController = newBullet.GetComponent<BulletController>();

                        newBulletController.SetDirection(target);
                    }
                }
                else
                {
                    // play clip sound
                }
            }
            else
            {
                    curCooldown = cooldownTime;
            }
        }
    }
}