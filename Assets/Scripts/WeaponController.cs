using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{

    public enum Type { Bullet, Shell, Melee };

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
    public ParticleSystem bloodSplatter;
    public ParticleSystem.EmissionModule bloodSplatterEmission;
    public AudioSource au;
    public AudioClip noAmmoClip;
    public List<AudioClip> meleeAttackClips;
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

    void Start()
    {
        bloodSplatterEmission = bloodSplatter.emission;
    }

    void Update()
    {
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
            HealthCollider h = col.GetComponent<HealthCollider>();
            if (h.masterHealth.health > 0)
            {
                bloodSplatterEmission.rate = 500;
                StartCoroutine("DisableBlood");
            }
            if (!h.masterHealth.invisible && h.masterHealth.health > 0)
            {
                au.pitch = Random.Range(0.75f, 1.25f);
                au.PlayOneShot(meleeAttackClips[Random.Range(0, meleeAttackClips.Count)]);
                h.Damage(meleeDamage);
            }
        }
    }

    IEnumerator DisableBlood()
    {
        yield return new WaitForSeconds(0.2f);
        bloodSplatterEmission.rate = 0;
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
                    au.PlayOneShot(noAmmoClip);
                    // play clip sound
                }
            }
            else // if  melee
            {
                curCooldown = cooldownTime;
            }
        }
    }
}