using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    [SerializeField]
    Animator anim;
    [SerializeField]
    Transform weapon;
    [SerializeField]
    Rigidbody2D rb;

    // The vector to store the direction of the player's movement.
    Vector3 movement;
    float inputH = 0;
    float inputV = 0;

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
    }

    void Move()
    {
        movement.Set(inputH, inputV, 0f);
        movement = movement.normalized * speed * Time.deltaTime
            ;
        rb.MovePosition(transform.position + movement);
    }

    public void SetWeapon(GameObject wpn)
    {
        weapon = wpn.transform;
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
            if (weapon.localRotation.z < -0.75f || weapon.localRotation.z > 0.75f)
            {
                weapon.localScale = new Vector3(1, -1, 1);
                anim.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (weapon.localRotation.z >= -0.75f || weapon.localRotation.z <= 0.75f)
            {
                weapon.localScale = new Vector3(1, 1, 1);
                anim.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}