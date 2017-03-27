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
    public float turnSpeed = 120f;

    public LayerMask aimLayers;

    public MainAudioController au;
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

    public bool quickTurn = false;
    public Collider targetCollider;
    public bool aim = false;
    public bool autoAim = false;
    public float maxAimDistance = 5f;

    float flashlightCooldown = 0f;
    public GameObject flashlight;
    public AudioClip flashlightSound;
    public IKLookControl ikController;
    Vector3 slerpMovement;

    int rotateY = 0;

    public Vector3 aimTarget;

    public LineRenderer line;

    public GameObject cameraFocus;
    public bool run = false;

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
            if (weaponController != null)
            {
                Attacking();
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
            au.au.PlayOneShot(flashlightSound);
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
            if (weaponController.ammo > 0 || weaponController.weaponAmmoType == WeaponController.Type.Melee)
                canShoot = true;

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

                if (weaponController.name == "RevolverWeaponInventory" && StateManager.instance.revolverBullets > 0)
                {
                    canReload = true;
                    if (reloadAmount > StateManager.instance.revolverBullets)
                        reloadAmount = StateManager.instance.revolverBullets;
                }
                else
                    canReload = false;

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
        if (quickTurn)
        {
            transform.Rotate(Vector3.up * 360f * Time.deltaTime);
        }
        if (!GameManager.instance.gui.reloadController.reload && !playerHealth.invisible && !moveBack && !attacking && !quickTurn)
        {
            //Vector3 m = transform.forward * inputV * speed * Time.deltaTime;
            //rb.MovePosition(rb.position + m);
            float turn = 0;
            if (inputV == 0)
            {
                turn = inputH * turnSpeed * Time.deltaTime;
            }
            else
            {
                if (aim)
                    turn = inputH * turnSpeed * Time.deltaTime;
                else
                    turn = inputH * turnSpeed / 2 * Time.deltaTime;
            }
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);

            if (aim)
            {
                rb.velocity = Vector3.zero; // character don't move if aiming
                if (inputH != 0)
                    anim.SetBool("LegsTurn", true);
                else
                    anim.SetBool("LegsTurn", false);
            }
            else
            {
                if (inputH != 0 && inputV == 0)
                    anim.SetBool("LegsTurn", true);
                else if (inputH == 0 || inputV != 0)
                {
                    anim.SetBool("LegsTurn", false);
                }
            }

            if (!aim)
            {
                Vector3 newVel = transform.forward * inputV * speed * 50 * Time.deltaTime;
                newVel = new Vector3(newVel.x, rb.velocity.y, newVel.z);
                rb.velocity = newVel;

                if (Input.GetAxisRaw("Vertical") < 0 && Input.GetButtonDown("Run"))
                {
                    StartCoroutine("QuickTurn");
                }

                if (Input.GetButton("Run"))
                {
                    if (inputV > 0)
                    {
                        float animSpeed = Mathf.Lerp(anim.GetFloat("Speed"), 1, 0.1f);
                        anim.SetFloat("Speed", animSpeed);
                        speed = maxSpeed * 2.5f;
                        run = true;
                    }
                    else
                    {
                        anim.SetFloat("Speed", -1);
                        speed = maxSpeed * 0.75f;
                        run = false;
                    }
                }
                else
                {
                    float animSpeed = 0;
                    run = false;
                    if (inputV < 0)
                    {
                        animSpeed = Mathf.Lerp(anim.GetFloat("Speed"), -1, 0.1f);
                        speed = maxSpeed * 0.75f;
                    }
                    else
                    {
                        animSpeed = Mathf.Lerp(anim.GetFloat("Speed"), 0, 0.1f);
                        speed = maxSpeed;
                    }
                    anim.SetFloat("Speed", animSpeed);
                }
                /*
                    movement.Set(inputH, 0f, inputV);
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
                */
            }
        }
        else
        {
            rb.velocity = Vector3.zero; // character don't move if reloading
        }
    }

    IEnumerator QuickTurn()
    {
        quickTurn = true;
        yield return new WaitForSeconds(0.5f);
        quickTurn = false;
    }
    void Animate()
    {
        if (inputV != 0)
        {
            if (!aim && !attacking && !moveBack && !reloading && !healing)
                anim.SetBool("Move", true);
            else
                anim.SetBool("Move", false);
        }
        else
        {
            anim.SetBool("Move", false);
            rb.velocity = Vector3.zero;
        }
        if (quickTurn)
        {
            anim.SetBool("Move", false);
            anim.SetBool("LegsTurn", true);
        }
    }

    public void SetWeapon(string weaponName)
    {
        bool wcFound = false;
        foreach (WeaponController j in weapons)
        {
            if (j.name == weaponName)
            {
                print(weaponName);
                j.gameObject.SetActive(true);
                weaponController = j;
                wcFound = true;
                anim.SetFloat("WeaponIndex", weapons.IndexOf(j) * 1.0f);
            }
            else
            {
                j.gameObject.SetActive(false);
                if (!wcFound)
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

            //Auto aim to closest enemy
            targetCollider = null;
            float closestTarger = maxAimDistance;
            foreach (Stateful st in GameManager.instance.statefulObjectsOnscene)
            {
                if (st.gameObject.activeInHierarchy && st.tag == "Mob" && st.mobController.health.health > 0)
                {
                    float d = Vector3.Distance(transform.position, st.transform.position);
                    if (d < closestTarger) // max aim 
                    {
                        targetCollider = st.mobController.aimCollder;
                        closestTarger = d;
                    }
                }
            }
            if (targetCollider)
            {
                autoAim = true;
            }
        }

        if (Input.GetButton("Aim") && !GameManager.instance.gui.reloadController.reload && Time.timeScale > 0 && !moveBack)
        {
            if (GameManager.instance.activeWeapon != "")
            {
                anim.SetBool("Aim", true);
                aim = true;
                if (weaponController.shotHolder)
                    Debug.DrawLine(weaponController.shotHolder.transform.position, weaponController.shotHolder.transform.TransformDirection(Vector3.forward) * 50f, Color.green);
                /*
                    Vector2 mousePosition = Input.mousePosition;
                    Vector2 normalized = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
                    Ray camRay = GameManager.instance.mainCam.ScreenPointToRay(GameManager.instance.mainCam.ViewportToScreenPoint(normalized));
                */
                if (!attacking)
                {
                    if (!targetCollider && !autoAim)
                    {
                        Vector3 fwd;
                        Vector3 aimPos;
                        if (weaponController.weaponAmmoType == WeaponController.Type.Melee)
                        {
                            fwd = transform.TransformDirection(Vector3.forward);
                            aimPos = transform.position;
                        }
                        else
                        {
                            fwd = weaponController.shotHolder.transform.TransformDirection(Vector3.forward);
                            aimPos = weaponController.shotHolder.transform.position;
                        }
                        RaycastHit objHit;
                        if (Physics.Raycast(aimPos, fwd, out objHit, maxAimDistance, weaponController.attackMask))
                        {
                            aimTarget = objHit.point;
                        }
                    }
                    else
                    {
                        if (weaponController != null && weaponController.shotHolder)
                        {
                            line.SetPosition(0, weaponController.shotHolder.transform.position);
                            line.SetPosition(1, targetCollider.transform.position);
                        }

                        //aim
                        Vector3 _target = new Vector3(targetCollider.transform.position.x, targetCollider.transform.position.y, targetCollider.transform.position.z);
                        ikController.SetTarget(_target, true);
                        Vector3 playerToMouse = _target - transform.position;

                        aimTarget = _target;
                        playerToMouse.y = 0f;

                        Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
                        newRotation = Quaternion.Slerp(newRotation, transform.rotation, turnSpeed * Time.deltaTime);
                        float difference = Mathf.Abs(Mathf.RoundToInt(newRotation.eulerAngles.y) - rotateY);
                        //print(difference);
                        if (difference > 4 && difference < 6)
                        {
                            rb.MoveRotation(newRotation);
                            //anim.SetBool("LegsTurn", true);
                        }
                        /*
                        else if (difference <= 4)
                        {
                            anim.SetBool("LegsTurn", false);
                        }
                        */
                        else if (difference >= 6)
                        {
                            autoAim = false;
                            //anim.SetBool("LegsTurn", false);
                        }
                        rotateY = Mathf.RoundToInt(transform.eulerAngles.y);
                    }
                }
            }
        }
        else
        {
            //print("ik false");
            ikController.SetTarget(ikController.lookPos, false);
            anim.SetBool("Aim", false);
            if (inputH == 0)
                anim.SetBool("LegsTurn", false);
            aim = false;
            autoAim = false;
        }

        if (!Input.GetButton("Aim"))
        {
            //print("ik false");
            ikController.SetTarget(ikController.lookPos, false);
            anim.SetBool("Aim", false);
            if (inputH == 0)
                anim.SetBool("LegsTurn", false);
            aim = false;
            autoAim = false;
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