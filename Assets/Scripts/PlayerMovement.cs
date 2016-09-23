using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;

    float curSpeed = 6f;

    public WeaponController weaponController;
    public WeaponMeleeController weaponMeleeController;
    public HealthController playerHealth;

    [SerializeField]
    Animator anim;
    [SerializeField]
    Transform weapon;

    public Rigidbody2D rb;

    // The vector to store the direction of the player's movement.
    Vector3 movement;
    float inputH = 0;
    float inputV = 0;

    public SpriteRenderer unitSprite;
    public SpriteRenderer weaponSprite;

    bool rolling = false;
    
    void Start()
    {
        InvokeRepeating("Sort", 1, 0.1f);
    }

    void Sort()
    {
        // SORTING
        unitSprite.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

        // weapon sorting
        if (weapon.localRotation.z > 0) // weapon behind
        {
            weaponSprite.sortingOrder = unitSprite.sortingOrder - 2;
        }
        else if (weapon.localRotation.z < 0) // weapon in front
        {
            weaponSprite.sortingOrder = unitSprite.sortingOrder + 2;
        }
    }

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
        if (playerHealth.health > 0)
        {
            Aiming();
            Animate();
            Shooting();
            Melee();

            //reduce roll speed
            if (rolling)
            {
                curSpeed = Mathf.Lerp(curSpeed, speed, 0.075f);
            }
        }
    }

    public void Roll(bool roll, float rollSpeed)
    {
        rolling = roll;

        if (roll == true)
        {
            curSpeed = rollSpeed;
        }
        else
        {
            curSpeed = speed;
        }
    }

    void Melee()
    {
        if (Input.GetButtonDown("Fire2") && weaponMeleeController.curReload <= 0)
        {
            weaponMeleeController.Attack();
            StartCoroutine("CamShakeShort", Random.Range(0.15f, 0.3f));
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
        if (Input.GetButtonDown("Fire1") && weaponController.curReload <= 0 && !weaponController.automatic)
        {
            bool canShoot = false;
            StartCoroutine("CamShakeShort", Random.Range(0.2f, 0.4f));

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
                weaponController.Shot();
                GameManager.instance.SetAmmo(weaponController.weaponAmmoType, -1); 
                GameManager.instance.gui.SetAmmo(weaponController.weaponAmmoType);
                GameManager.instance.gui.SetWeapon();
            }
        }

        if (Input.GetButton("Fire1") && weaponController.curReload <= 0 && weaponController.automatic)
        {
            StartCoroutine("CamShakeShort", Random.Range(0.1f, 0.3f));

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
                weaponController.Shot();
                GameManager.instance.SetAmmo(weaponController.weaponAmmoType, -1);
                GameManager.instance.gui.SetAmmo(weaponController.weaponAmmoType);
                GameManager.instance.gui.SetWeapon();
            }
        }
    }

    void Move()
    {
        if (!rolling)
            movement.Set(inputH, inputV, 0f);

        movement = movement.normalized * curSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
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

        weaponSprite = wpn.GetComponentInChildren<SpriteRenderer>();

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
        if (weapon != null)
        {
            Vector3 mouse_pos = Input.mousePosition;
            mouse_pos.z = 5.23f; //The distance between the camera and object
            Vector3 object_pos = Camera.main.WorldToScreenPoint(weapon.position);
            mouse_pos.x = mouse_pos.x - object_pos.x;
            mouse_pos.y = mouse_pos.y - object_pos.y;
            float angle = Mathf.Atan2(mouse_pos.y, mouse_pos.x) * Mathf.Rad2Deg;
            weapon.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            weaponMeleeController.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // FLIP SPRITES BASED ON ROTATION
            if (weapon.localRotation.z < -0.75f || weapon.localRotation.z > 0.75f) // left
            {
                weapon.localScale = new Vector3(1, -1, 1);
                weaponMeleeController.gameObject.transform.localScale = new Vector3(1, -1, 1);
                anim.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (weapon.localRotation.z >= -0.75f || weapon.localRotation.z <= 0.75f) // right
            {
                weapon.localScale = new Vector3(1, 1, 1);
                weaponMeleeController.gameObject.transform.localScale = new Vector3(1, 1, 1);
                anim.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}