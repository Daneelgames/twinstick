using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 6f;
    public float speed = 6f;

    float curSpeed = 6f;

    //public float turnSmooth = 50f;

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

    public Transform weaponHolder;

    public bool realoading = false;

    public IKLookControl ikController;

    public bool aim = false;

    int rotateY = 0;

    public Vector3 aimTarget;

    public LineRenderer line;

    void FixedUpdate()
    {
        if (playerHealth.health > 0)
        {
            inputH = Input.GetAxisRaw("Horizontal");
            inputV = Input.GetAxisRaw("Vertical");

            if(!realoading && !GameManager.instance.cutScene)
                Move();
        }
    }

    void Update()
    {
        curSpeed = Mathf.Lerp(curSpeed, speed, 0.75f);

        if (playerHealth.health > 0 && !GameManager.instance.cutScene)
        {
            if (!realoading)
            {
                Aiming();

                if (weaponController != null)
                    Shooting();
            }

            if (weaponController != null)
                Reloading();

            Animate();

        }
    }

    IEnumerator CamShakeShort(float amount)
    {
        GameManager.instance.camAnim.SetFloat("ShakeAmount", amount);
        yield return new WaitForSeconds(0.25f);
        GameManager.instance.camAnim.SetFloat("ShakeAmount", 0);
    }

    void Shooting()
    {
        if (weaponController.ammo > 0 && Input.GetButtonDown("Fire1") && aim && weaponController.curCooldown <= 0)
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
            }

            if (canShoot)
            {
                StartCoroutine("CamShakeShort", Random.Range(0.2f, 0.4f));
                anim.SetTrigger("Shoot");
                weaponController.Shot(aimTarget);
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
                int reloadAmount = weaponController.ammoCap - weaponController.ammo;

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

    void ReloadWeapon(int reloadAmount)
    {
        GameManager.instance.gui.reloadController.StartReload(Random.Range(-30f, 30f), reloadAmount);
    }

    void Move()
    {
        if (!Input.GetButton("Aim") && !GameManager.instance.gui.reloadController.reload)
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
            rb.velocity = movement;

            if (inputH != 0 || inputV != 0)
            {
                Vector3 playerToTarget = (transform.position + movement) - transform.position;

                playerToTarget.y = transform.position.y;

                Quaternion newRotation = Quaternion.LookRotation(playerToTarget);
                newRotation = Quaternion.Slerp(newRotation, transform.rotation, Time.deltaTime * 45F);
                rb.MoveRotation(newRotation);
                transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
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

    public void SetWeapon(int weaponIndex)
    {
        GameObject wpn = Instantiate(GameManager.instance.playerWeapons[weaponIndex], Vector3.zero, Quaternion.identity) as GameObject;
        
        weaponController = wpn.GetComponent<WeaponController>();
        weaponController.gameObject.SetActive(true);
        weapon = wpn.transform;

        weaponController.SetInHands();
        wpn.name = "Weapon";
        wpn.transform.SetParent(weaponHolder);
        wpn.transform.localPosition = Vector3.zero;
        wpn.transform.localEulerAngles = Vector3.zero;
    }

    void Animate()
    {
        if (inputH != 0 || inputV != 0)
        {
            if (!Input.GetButton("Aim"))
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

    void Aiming()
    {
        if (Input.GetButtonDown("Aim"))
        {
            if (weaponController != null)
                weaponController.curCooldown = 0.5f;
        }

        if (Input.GetButton("Aim") && !GameManager.instance.gui.reloadController.reload && Time.timeScale > 0)
        {
            if (weapon != null)
            {
                anim.SetBool("Aim", true);
            }
            /*
            Vector2 mousePosition = Input.mousePosition;
            Vector2 normalized = new Vector2(mousePosition.x / Screen.width, mousePosition.y / Screen.height);
            Ray camRay = GameManager.instance.mainCam.ScreenPointToRay(GameManager.instance.mainCam.ViewportToScreenPoint(normalized));
            */

            Ray camRay = GameManager.instance.mainCam.ScreenPointToRay(Input.mousePosition);

            RaycastHit floorHit;

            if (Physics.Raycast(camRay, out floorHit, 30f, aimLayers))
            {

                if (weaponController != null)
                {
                    line.SetPosition(0, weaponController.shotHolder.transform.position);
                    line.SetPosition(1, floorHit.point);
                }

                ikController.SetTarget(floorHit.point, true);
                Vector3 playerToMouse = floorHit.point - transform.position;

                aimTarget = floorHit.point;

                playerToMouse.y = 0f;

                aim = true;

                Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
                newRotation = Quaternion.Slerp(newRotation, transform.rotation, Time.deltaTime * 40f);
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
                ikController.SetTarget(ikController.lookPos, false);
                aim = false;
            }
        }
        else
        {
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

}