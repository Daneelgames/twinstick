using UnityEngine;
using System.Collections;

public class WalkerToPlayerAi : MonoBehaviour {

    public NpcController _npcController;

    public SpriteRenderer unitSprite;

    public Rigidbody2D _rb;
    public float speed = 1;

    public bool moveToPlayer = false;
    bool repositioning = false;

    float inputH = 0;
    float inputV = 0;
    Vector2 movement;

    void Start()
    {
        InvokeRepeating("CheckWall", 1, 1f);
        InvokeRepeating("CheckDistance", 1, 1f);
    }

    void Update()
    {
        if (_npcController.health.health > 0)
        {
            Move();
        }
    }

    IEnumerator SetDirection()
    {
        moveToPlayer = false;
        repositioning = true;

        inputH = Random.Range(-2f, 2f);
        inputV = Random.Range(-2f, 2f);

        yield return new WaitForSeconds(Random.Range(1f, 2f));
        repositioning = false;
    }

    void Move()
    {
        if (moveToPlayer)
        {
            movement = _npcController.activeTarget.transform.position - transform.position; // direction to player
        }
        else
            movement.Set(inputH, inputV);

        movement = movement.normalized * speed * Time.deltaTime;
        _rb.MovePosition(new Vector2(transform.position.x, transform.position.y) + movement);
    }

    void CheckDistance()
    {
        if (!repositioning)
        {
            float distance = Vector2.Distance(transform.position, _npcController.activeTarget.transform.position);
            if (distance < 10)
                moveToPlayer = true;
        }
    }

    void CheckWall()
    {
        if (_npcController.health.health > 0)
        {
            unitSprite.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, movement, 1.25f, 1 << 9);
            if (hit.collider != null)
            {
                // if (hit.collider.gameObject.tag == "Solid")
                {
                    StartCoroutine ("SetDirection");
                    //print(hit.collider.gameObject.name);
                }
            }
        }
    }
}