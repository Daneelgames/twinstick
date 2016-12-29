using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 6f;
    public float speed = 6f;

    float curSpeed = 6f;

    //public float turnSmooth = 50f;

    public List<WeaponController> weapons = new List<WeaponController>();
    public WeaponController weaponController;
    public HealthController playerHealth;
    public float turnSpeed = 45f;

    public LayerMask aimLayers;

    [SerializeField]
    Animator anim;
    [SerializeField]

    public Rigidbody rb;

    // The vector to store the direction of the player's movement.
    Vector3 movement;
    float inputH = 0;
    float inputV = 0;

    public bool moveBack = false;
    public bool attacking = false;
    public bool reloading = false;
    public bool healing = false;
    public bool aim = false;

    float flashlightCooldown = 0f;
    public GameObject flashlight;
    public IKLookControl ikController;


    int rotateY = 0;

    public Vector3 aimTarget;

    public LineRenderer line;

    void FixedUpdate()
    {
        if (playerHealth.health > 0)
        {
            inputH = Input.GetAxisRaw("Horizontal");
            inputV = Input.GetAxisRaw("Vertical");

            if (!reloading && !healing && !GameManager.instance.cutScene)
            {
                Move();
                Aiming();
            }
        }
    }

    void Update()
    {
        curSpeed = Mathf.Lerp(curSpeed, speed, 0.75f);

        if (playerHealth.health > 0 && !GameManager.instance.cutScene)
        {
            Flashlight();
            Healing();

            if (weaponController != null)
            {
                Reloading();
                Attacking();
            }
            Animate();
        }
    }

    void Flashlight()
    {
        if (flashlightCooldown > 0)
            flashlightCooldown -= Time.deltaTime;

        if (Input.GetButtonDown("Flashlight") && !moveBack && !aim && !reloading && !attacking && flashlightCooldown <= 0)
        {
            if (StateManager.instance.flashlight)
            {
                flashlight.SetActive(false);
                StateManager.instance.SetFlashlight(false);
            }
            else
            {
                flashlight.SetActive(true);
                StateManager.instance.SetFlashlight(true);
            }
            flashlightCooldown = 0.25f;
        }
    }


    public void SetFlashlight(bool active)
    {
        flashlight.SetActive(active);
    }
    void Attacking()
    {
        if (Input.GetButtonDown("Fire1") && aim && weaponController.curCooldown <= 0 && !moveBack && !reloading)
        {
            bool canShoot = false;

            switch (weaponController.weaponAmmoType)
            {
                case WeaponController.Type.Bullet:
                    if (GameManager.instance.bullets > 0 && weaponController.ammo > 0)
                        canShoot = true;
                    break;

                case WeaponController.Type.Shell:
                    if (GameManager.instance.shells > 0 && weaponController.ammo > 0)
                        canShoot = true;
                    break;

                case WeaponController.Type.Melee:
                    canShoot = true;
                    break;
            }

            if (canShoot)
            {
                anim.SetTrigger("Shoot");
                attacking = true;
                StartCoroutine("AttackingEnd", weaponController.cooldownTime);
                weaponController.Attack(aimTarget);
            }
        }
    }

    IEnumerator AttackingEnd(float t)
    {
        yield return new WaitForSeconds(t);
        attacking = false;
    }

    void Reloading()
    {
        if (Input.GetButtonDown("Reload"))
        {
            if (weaponController.curCooldown <= 0 && weaponController.ammo < weaponController.ammoMax && !GameManager.instance.gui.reloadController.reload && !moveBack)
            {

                bool canReload = false;
                int reloadAmount = weaponController.ammoMax - weaponController.ammo;

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
                }
                if (canReload)
                {
                    ReloadWeapon(reloadAmount);
                }
            }
        }
    }

    void Healing()
    {
        if (Input.GetButtonDown("Heal") && !attacking && !healing && !reloading && !moveBack)
        {
            if (StateManager.instance.painkillers > 0 && playerHealth.health != playerHealth.maxHealth)
            {
                healing = true;
                GameManager.instance.gui.reloadController.StartReload(Random.Range(-30f, 30f), 0);
            }
            
        }
    }

    void ReloadWeapon(int reloadAmount)
    {
        reloading = true;
        GameManager.instance.gui.reloadController.StartReload(Random.Range(-30f, 30f), reloadAmount);
    }

    public void ReloadOver()
    {
        reloading = false;
    }
    public void HealOver()
    {
        healing = false;
    }


    public void MoveBack(float moveTime)
    {
        StartCoroutine("MoveBackCoroutine", moveTime);
    }

    IEnumerator MoveBackCoroutine(float moveTime)
    {
        moveBack = true;
        yield return new WaitForSeconds(moveTime);
        moveBack = false;
    }

    public void SetMaxSpeed(float value)
    {
        maxSpeed = value;
    }

    void Move()
    {
        if (!Input.GetButton("Aim") && !GameManager.instance.gui.reloadController.reload && !playerHealth.invisible && !moveBack && !attacking)
        {
            /* OLD MOVEMENT
            movement.Set(inputH, 0f, inputV);
            movement = movement.normalized * curSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + movement);
            */

            movement.Set(inputH, 0f, inputV);
            //movement = transform.TransformDirection(movement.normalized) * curSpeed;
            movement = movement.normalized * curSpeed;
            movement.y = rb.velocity.y;

            Quaternion rot = rb.rotation;
            rot.eulerAngles = new Vector3(0, rb.rotation.eulerAngles.y, 0);
            rb.rotation = rot;

            rb.velocity = movement;

            if (inputH != 0 || inputV != 0)
            {
                Vector3 playerToTarget = (transform.position + movement) - transform.position;

                playerToTarget.y = transform.position.y;

                Quaternion newRotation = Quaternion.LookRotation(playerToTarget);
                newRotation = Quaternion.Slerp(newRotation, transform.rotation, turnSpeed * Time.deltaTime);
                newRotation.eulerAngles = new Vector3(0, newRotation.eulerAngles.y, 0);
                rb.MoveRotation(newRotation);
            }

            if (Input.GetButton("Run"))
            {
                float animSpeed = Mathf.Lerp(anim.GetFloat("Speed"), 1, 0.1f);
                anim.SetFloat("Speed", animSpeed);
                speed = maxSpeed * 2.5f;
            }
            else
            {
                float animSpeed = Mathf.Lerp(anim.GetFloat("Speed"), 0, 0.1f);
                anim.SetFloat("Speed", animSpeed);
                speed = maxSpeed;
            }
        }
        else
        {
            rb.velocity = Vector3.zero; // character don't move if aiming or reloading
        }
    }


    void Animate()
    {
        if (inputH != 0 || inputV != 0)
        {
            if (!Input.GetButton("Aim") && !attacking && !moveBack && !reloading && !healing)
                anim.SetBool("Move", true);
            else
                anim.SetBool("Move", false);
        }
        else
        {
            anim.SetBool("Move", false);
            rb.velocity = Vector3.zero;
        }
    }

    public void SetWeapon(string weaponName)
    {
        foreach (WeaponController j in weapons)
        {
            if (j.name == weaponName)
            {
                j.gameObject.SetActive(true);
                weaponController = j;
                anim.SetFloat("WeaponIndex", weapons.IndexOf(j) * 1.0f);
            }
            else
            {
                j.gameObject.SetActive(false);
                weaponController = null;
            }
        }
    }

    void Aiming()
    {
        if (Input.GetButtonDown("Aim"))
        {
            if (weaponController != null)
                weaponController.curCooldown = 0.5f;
        }

        if (Input.GetButton("Aim") && !GameManager.instance.gui.reloadController.reload && Time.timeScale > 0 && !moveBack)
        {
            if (GameManager.instance.activeWeapon != "")
            {
                anim.SetBool("Aim", true);
            }
            /*
            Vector2 mousePosition = Input.mousePosition;
            Vector2 normalized = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
            Ray camRay = GameManager.instance.mainCam.ScreenPointToRay(GameManager.instance.mainCam.ViewportToScreenPoint(normalized));
            */

            if (!attacking)
            {
                Ray camRay = GameManager.instance.mainCam.ScreenPointToRay(Input.mousePosition);

                RaycastHit floorHit;

                if (Physics.Raycast(camRay, out floorHit, 30f, aimLayers))
                {

                    if (weaponController != null && weaponController.shotHolder)
                    {
                        line.SetPosition(0, weaponController.shotHolder.transform.position);
                        line.SetPosition(1, floorHit.point);
                    }

                    // if (weaponController /* || weaponController.weaponAmmoType != WeaponController.Type.Melee*/)
                    ikController.SetTarget(floorHit.point, true);
                    Vector3 playerToMouse = floorHit.point - transform.position;

                    aimTarget = floorHit.point;

                    playerToMouse.y = 0f;

                    aim = true;

                    Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
                    newRotation = Quaternion.Slerp(newRotation, transform.rotation, turnSpeed * Time.deltaTime);
                    float difference = Mathf.Abs(Mathf.RoundToInt(newRotation.eulerAngles.y) - rotateY);
                    //print(difference);
                    if (difference > 4)
                    {
                        rb.MoveRotation(newRotation);
                        anim.SetBool("LegsTurn", true);
                    }
                    else
                    {
                        anim.SetBool("LegsTurn", false);
                    }
                    rotateY = Mathf.RoundToInt(transform.eulerAngles.y);

                }
                else
                {
                    print("ik false");
                    ikController.SetTarget(ikController.lookPos, false);
                    aim = false;
                }
            }
        }
        else
        {
//            print("ik false");
            ikController.SetTarget(ikController.lookPos, false);
            anim.SetBool("Aim", false);
            anim.SetBool("LegsTurn", false);
            aim = false;
        }
    }

    public void SetAnimBool(string boolName, bool boolValue)
    {
        anim.SetBool(boolName, boolValue);
    }

    public void SetAnimTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }
}