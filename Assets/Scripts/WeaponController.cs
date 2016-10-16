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


    public void SwitchInhands(bool hands)
    {
        canPick = false;

        if (!hands)
        {
            transform.SetParent(null);
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        }
        inHands = hands;

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

    public void Shot()
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

                    newBulletController.SetDirection(transform.eulerAngles.z);
                }
            }
            else
            {
                // play clip sound
            }
        }
    }
}