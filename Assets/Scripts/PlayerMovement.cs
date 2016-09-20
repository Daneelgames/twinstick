using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;

    float curSpeed = 6f;

    WeaponController weaponController;

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
    }

    void FixedUpdate()
    {
        inputH = Input.GetAxisRaw("Horizontal");
        inputV = Input.GetAxisRaw("Vertical");

        Move();
    }

    void Update()
    {
        Aiming();
        Animate();
        Shooting();

        //reduce roll speed
        if (rolling)
        {
            curSpeed = Mathf.Lerp(curSpeed, speed, 0.075f);
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

    void Shooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            weaponController.Shot();
        }
    }

    void Move()
    {
        if (!rolling)
            movement.Set(inputH, inputV, 0f);

        movement = movement.normalized * curSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }

    public void SetWeapon(GameObject wpn)
    {
        weaponController = wpn.GetComponent<WeaponController>();
        weapon = wpn.transform;

        weaponSprite = wpn.GetComponentInChildren<SpriteRenderer>();
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

            // FLIP SPRITES BASED ON ROTATION
            if (weapon.localRotation.z < -0.75f || weapon.localRotation.z > 0.75f) // left
            {
                weapon.localScale = new Vector3(1, -1, 1);
                anim.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (weapon.localRotation.z >= -0.75f || weapon.localRotation.z <= 0.75f) // right
            {
                weapon.localScale = new Vector3(1, 1, 1);
                anim.transform.localScale = new Vector3(1, 1, 1);
            }
            // weapon sorting
            if (weapon.localRotation.z > 0) // weapon behind
            {
                weaponSprite.sortingOrder = unitSprite.sortingOrder - 1;
            }
            else if (weapon.localRotation.z < 0) // weapon in front
            {
                weaponSprite.sortingOrder = unitSprite.sortingOrder + 1;
            }
        }
    }
}