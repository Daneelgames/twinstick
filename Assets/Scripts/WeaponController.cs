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
    public float damage = 1;
    public List<GameObject> bullets;

    public Type weaponAmmoType = Type.Bullet;
    public ParticleSystem bloodSplatter;
    public ParticleSystem.EmissionModule bloodSplatterEmission;
    public AudioSource au;
    public AudioClip noAmmoClip;
    public List<AudioClip> attackClips;
    public GameObject shotParticles;
    public GameObject shotSolidParticles;
    public GameObject shotMobParticles;
    public LayerMask attackMask;
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
                au.PlayOneShot(attackClips[Random.Range(0, attackClips.Count)]);
                h.Damage(damage + Random.Range(-damage / 4, damage / 4));
            }
        }
    }

    IEnumerator DisableBlood()
    {
        yield return new WaitForSeconds(0.2f);
        bloodSplatterEmission.rate = 0;
    }

    public void Attack()
    {
        if (curCooldown <= 0)
        {
            if (weaponAmmoType == Type.Bullet || weaponAmmoType == Type.Shell)
            {
                if (ammo > 0)
                {
                    ammo -= 1;
                    StateManager.instance.UseBullet(this);
                    curCooldown = cooldownTime;

                    Instantiate(shotParticles, shotHolder.transform.position, Quaternion.identity);
                    Vector3 fwd = shotHolder.transform.TransformDirection(Vector3.forward);
                    RaycastHit objHit;
                    if (Physics.Raycast(shotHolder.transform.position, fwd, out objHit, GameManager.instance.playerController.maxAimDistance, attackMask))
                    {
                        print(objHit.collider.gameObject.name);
                        if (objHit.collider.gameObject.tag == "Solid")
                        {
                            if (!objHit.collider.isTrigger)
                                Instantiate(shotSolidParticles, objHit.point, Quaternion.identity);
                        }
                        if (objHit.collider.gameObject.tag == "HealthCollider")
                        {
                            if (!objHit.collider.isTrigger)
                                Instantiate(shotMobParticles, objHit.point, Quaternion.identity);
                            objHit.collider.gameObject.GetComponent<HealthCollider>().masterHealth.Damage(damage + Random.Range(-damage / 4, damage / 4));
                        }
                    }

                    au.pitch = Random.Range(0.75f, 1.25f);
                    au.PlayOneShot(attackClips[Random.Range(0, attackClips.Count)]);
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