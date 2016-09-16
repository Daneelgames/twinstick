using UnityEngine;
using System.Collections;

public class RandomWalkerAi : MonoBehaviour {

    public Rigidbody2D _rb;
    public float speed = 1;

    public float minWalkTime = 0.5f;
    public float maxWalkTime = 3f;

    float walkTime = 0;

    float waitTime = 0;

    float inputH = 0;
    float inputV = 0;

    void SetWalkTime()
    {
        SetDirection();

        walkTime = Random.Range(minWalkTime, maxWalkTime);
        waitTime = Random.Range(minWalkTime, maxWalkTime);
    }

    void Update()
    {
        if (walkTime > 0)
        {
            walkTime -= Time.deltaTime;
            Move();
        }
        else
        {
            if (waitTime > 0)
            {
                waitTime -= Time.deltaTime;
            }
            else
            {
                SetWalkTime();
            }
        }
    }

    void SetDirection()
    {
        inputH = Random.Range(-2f, 2f);
        inputV = Random.Range(-2f, 2f);
    }

    void Move()
    {
        Vector3 movement = new Vector3();
        movement.Set(inputH, inputV, 0f);
        movement = movement.normalized * speed * Time.deltaTime;
        _rb.MovePosition(transform.position + movement);
    }

    void FixedUpdate()
    {
        if (walkTime > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(inputH, inputV), 1.25f, 1 << 9);
            if (hit.collider != null)
            {
              // if (hit.collider.gameObject.tag == "Solid")
                {
                    SetDirection();
                    //print(hit.collider.gameObject.name);
                }
            }
        }
    }
}