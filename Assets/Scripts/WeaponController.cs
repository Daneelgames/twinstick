using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour {

    public enum Type {Bullet, Shell, Explosive};

    public int ammoCap = 0;
    public int ammo = 0;

    public bool automatic = false;

    public float cooldownTime = 0.25f;
    public float curCooldown = 0f;

    public GameObject shotHolder;

    public List<GameObject> bullets;

    bool canPick = false;
    public bool inHands = false;

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

    public void PickUp() // use fore pick up
    {
        canPick = false;
        inHands = true;
        GameManager.instance.AddPlayerWeapon(gameObject);
        gameObject.SetActive(false);
        
    }

    public void SetInHands()
    {
        inHands = true;
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.tag == "Player" && !canPick && !inHands)
        {
            GameManager.instance.WeaponToPick(gameObject);
            canPick = true;
        }
    }

    void OnTriggerExit(Collider coll)
    {
        if (coll.tag == "Player")
        {
            GameManager.instance.WeaponToPick(null);
            canPick = false;
        }
    }

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

    public void Shot(Vector3 target)
    {
        if (curCooldown <= 0)
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
    }
}