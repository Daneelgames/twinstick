﻿using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;

    float curSpeed = 6f;

    public float turnSmooth = 50f;

    public WeaponController weaponController;
    public HealthController playerHealth;

    public LayerMask aimLayers;

    [SerializeField]
    Animator anim;
    [SerializeField]
    Transform weapon;

    public Rigidbody rb;

    // The vector to store the direction of the player's movement.
    Vector3 movement;
    float inputH = 0;
    float inputV = 0;

    float weaponCooldownPercentageBonus = 0f;
    public bool meleeBounce;

    public Transform weaponHolder;

    public bool realoading = false;

    public IKLookControl ikController;
    
    int rotateY = 0;

    void FixedUpdate()
    {
        if (playerHealth.health > 0)
        {
            inputH = Input.GetAxisRaw("Horizontal");
            inputV = Input.GetAxisRaw("Vertical");

            if(!realoading)
                Move();
        }
    }

    void Update()
    {
        curSpeed = speed;

        if (playerHealth.health > 0)
        {
            if (!realoading)
            {
                Aiming();
                Shooting();
            }

            Reloading();

            Animate();

        }
    }

    IEnumerator CamShakeShort(float amount)
    {
        GameManager.instance.camAnim.SetFloat("ShakeAmount", amount);
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.camAnim.SetFloat("ShakeAmount", 0);
    }

    void Shooting()
    {
        // !auto
        if (weaponController.ammo > 0 && Input.GetButtonDown("Fire1") && Input.GetButton("Aim") && weaponController.curCooldown - weaponController.cooldownTime / 100 * weaponCooldownPercentageBonus <= 0 && !weaponController.automatic)
        {
            bool canShoot = false;

            switch (weaponController.weaponAmmoType)
            {
                case WeaponController.Type.Bullet:
                    if (GameManager.instance.bullets > 0)
                        canShoot = true;
                    else
                        canShoot = false;
                    break;

                case WeaponController.Type.Shell:
                    if (GameManager.instance.shells > 0)
                        canShoot = true;
                    else
                        canShoot = false;
                    break;

                case WeaponController.Type.Explosive:
                    if (GameManager.instance.explosive > 0)
                        canShoot = true;
                    else
                        canShoot = false;
                    break;
            }

            if (canShoot)
            {
                StartCoroutine("CamShakeShort", Random.Range(0.2f, 0.4f));
                anim.SetTrigger("Shoot");
                weaponController.Shot();
                //GameManager.instance.SetAmmo(weaponController.weaponAmmoType, -1); 
                GameManager.instance.gui.SetAmmo(weaponController.weaponAmmoType);
                GameManager.instance.gui.SetWeapon();
            }
        }

        //auto
        if (weaponController.ammo > 0 && Input.GetButton("Fire1") && Input.GetButton("Aim") && weaponController.curCooldown - weaponController.cooldownTime / 100 * weaponCooldownPercentageBonus <= 0 && weaponController.automatic)
        {
            bool canShoot = false;

            switch (weaponController.weaponAmmoType)
            {
                case WeaponController.Type.Bullet:
                    if (GameManager.instance.bullets > 0)
                        canShoot = true;
                    else
                        canShoot = false;
                    break;

                case WeaponController.Type.Shell:
                    if (GameManager.instance.shells > 0)
                        canShoot = true;
                    else
                        canShoot = false;
                    break;

                case WeaponController.Type.Explosive:
                    if (GameManager.instance.explosive > 0)
                        canShoot = true;
                    else
                        canShoot = false;
                    break;
            }

            if (canShoot)
            {
                StartCoroutine("CamShakeShort", Random.Range(0.1f, 0.3f));
                weaponController.Shot();
                //GameManager.instance.SetAmmo(weaponController.weaponAmmoType, -1);
                GameManager.instance.gui.SetAmmo(weaponController.weaponAmmoType);
                GameManager.instance.gui.SetWeapon();
            }
        }
    }

    void Reloading()
    {
        if (weaponController.curCooldown <= 0 && weaponController.ammo < weaponController.ammoCap && !GameManager.instance.gui.reloadController.reload)
        {
            if (Input.GetButtonDown("Reload"))
            {

                bool canReload = false;
                int reloadAmount = reloadAmount = weaponController.ammoCap - weaponController.ammo;

                switch (weaponController.weaponAmmoType)
                {
                    case WeaponController.Type.Bullet:
                        if (GameManager.instance.bullets > 0)
                        {
                            canReload = true;
                            if (reloadAmount > GameManager.instance.bullets)
                                reloadAmount = GameManager.instance.bullets;
                        }
                        else
                            canReload = false;
                        break;

                    case WeaponController.Type.Shell:
                        if (GameManager.instance.shells > 0)
                        {
                            canReload = true;
                            if (reloadAmount > GameManager.instance.shells)
                                reloadAmount = GameManager.instance.shells;
                        }
                        else
                            canReload = false;
                        break;

                    case WeaponController.Type.Explosive:
                        if (GameManager.instance.explosive > 0)
                        {
                            canReload = true;
                            if (reloadAmount > GameManager.instance.explosive)
                                reloadAmount = GameManager.instance.explosive;
                        }
                        else
                            canReload = false;
                        break;
                }
                if (canReload)
                {
                    ReloadWeapon(reloadAmount);
                }
            }
        }
    }

    void ReloadWeapon(int reloadAmount)
    {
        GameManager.instance.gui.reloadController.StartReload(Random.Range(-30f, 30f), reloadAmount);
    }

    void Move()
    {
        if (!Input.GetButton("Aim") && !GameManager.instance.gui.reloadController.reload)
        {
            movement.Set(inputH, 0f, inputV);
            movement = movement.normalized * curSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + movement);

            if (inputH != 0 || inputV != 0)
            {

                Vector3 playerToTarget = (transform.position + movement) - transform.position;

                playerToTarget.y = 0f;

                Quaternion newRotation = Quaternion.LookRotation(playerToTarget);
                newRotation = Quaternion.Slerp(newRotation, transform.rotation, Time.deltaTime * turnSmooth);
                rb.MoveRotation(newRotation);
            }
        }
    }

    public void SetWeapon(GameObject wpn, bool newWeapon)
    {
        if (newWeapon)
        {
            if (GameManager.instance.playerWeapons.Count == 1)
            {
                if (weaponController != null) // if have weapon in hands
                {
                    weaponController.gameObject.SetActive(false);
                }
            }
            else if (GameManager.instance.playerWeapons.Count == 2)
            {
                GameManager.instance.RemovePlayerWeapon(weaponController.gameObject);
                weaponController.SwitchInhands(false);
                weaponController = null;
            }

            GameManager.instance.AddPlayerWeapon(wpn);
            GameManager.instance.gui.SetAmmo(WeaponController.Type.Bullet);
            GameManager.instance.gui.SetAmmo(WeaponController.Type.Shell);
            GameManager.instance.gui.SetAmmo(WeaponController.Type.Explosive);
        }
        else
        {
            weaponController.gameObject.SetActive(false);
        }

        weaponController = wpn.GetComponent<WeaponController>();
        weaponController.gameObject.SetActive(true);
        weapon = wpn.transform;

        wpn.GetComponent<WeaponController>().SwitchInhands(true);
        wpn.name = "Weapon";
        wpn.transform.SetParent(transform);
        wpn.transform.localPosition = Vector3.zero;

        GameManager.instance.gui.SetWeapon();
    }

    void Animate()
    {
        if (inputH != 0 || inputV != 0)
            anim.SetBool("Move", true);
        else
            anim.SetBool("Move", false);
    }

    void Aiming()
    {
        if (Input.GetButton("Aim") && !GameManager.instance.gui.reloadController.reload)
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit floorHit;

            if (Physics.Raycast(camRay, out floorHit, 30f, aimLayers))
            {
                ikController.SetTarget(floorHit.point, true);
                Vector3 playerToMouse = floorHit.point - transform.position;
                

                playerToMouse.y = 0f;


                Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
                newRotation = Quaternion.Slerp(newRotation, transform.rotation, Time.deltaTime * turnSmooth * 1.2f);

                if (Mathf.Abs(Mathf.RoundToInt(newRotation.eulerAngles.y) - rotateY) > 5)
                {
                    rb.MoveRotation(newRotation);
                    anim.SetBool("LegsTurn", true);
                    rotateY = Mathf.RoundToInt(transform.localEulerAngles.y);
                }
                else
                {
                    anim.SetBool("LegsTurn", false);
                }

                if (weapon != null)
                {
                    anim.SetBool("Aim", true);
                }
            }
            else
            {
                ikController.SetTarget(ikController.lookPos, false);
            }
        }
        else
        {
            ikController.SetTarget(ikController.lookPos, false);
            anim.SetBool("Aim", false);
            anim.SetBool("LegsTurn", false);
        }
    }
    
}