using UnityEngine;
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
    
    void FixedUpdate()
    {
        if (playerHealth.health > 0)
        {
            inputH = Input.GetAxisRaw("Horizontal");
            inputV = Input.GetAxisRaw("Vertical");

            Move();
        }
    }

    void Update()
    {
        curSpeed = speed;

        if (playerHealth.health > 0)
        {
            Aiming();
            Animate();

            if (!GameManager.instance.pointerOverMenu)
            {
                Shooting();
            }

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
        if (Input.GetButtonDown("Fire1") && weaponController.curReload - weaponController.reloadTime / 100 * weaponCooldownPercentageBonus <= 0 && !weaponController.automatic)
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
                weaponController.Shot();
                GameManager.instance.SetAmmo(weaponController.weaponAmmoType, -1); 
                GameManager.instance.gui.SetAmmo(weaponController.weaponAmmoType);
                GameManager.instance.gui.SetWeapon();
            }
        }

        if (Input.GetButton("Fire1") && weaponController.curReload - weaponController.reloadTime / 100 * weaponCooldownPercentageBonus <= 0 && weaponController.automatic)
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
                GameManager.instance.SetAmmo(weaponController.weaponAmmoType, -1);
                GameManager.instance.gui.SetAmmo(weaponController.weaponAmmoType);
                GameManager.instance.gui.SetWeapon();
            }
        }
    }

    void Move()
    {
        if (!Input.GetButton("Aim"))
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
        if (weapon != null && Input.GetButton("Aim"))
        {
            /*
            Vector3 mouse_pos = Input.mousePosition;
            mouse_pos.z = 5.23f; //The distance between the camera and object
            Vector3 object_pos = Camera.main.WorldToScreenPoint(weapon.position);
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y;
            float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
            weapon.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            */

            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit floorHit;

            if (Physics.Raycast(camRay, out floorHit, 30f, aimLayers))
            {
                Vector3 playerToMouse = floorHit.point - transform.position;

                playerToMouse.y = 0f;

                Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
                newRotation = Quaternion.Slerp(newRotation, transform.rotation, Time.deltaTime * turnSmooth);
                rb.MoveRotation(newRotation);
            }
        }
    }

    public void SetSpeed(float amount)
    {
        speed = amount;
    }

    public void SetWeaponCooldownPercentageBonus(float amount)
    {
        weaponCooldownPercentageBonus = amount;
    }

    public void SetMeleeBounce(bool bounce)
    {
        meleeBounce = bounce;
    }

    public void SetRegen(bool regen)
    {
        if (regen)
            InvokeRepeating("Regen", 30, 30);
        else
            CancelInvoke("Regen");
    }

    void Regen()
    {
        playerHealth.Heal(1);
    }
}