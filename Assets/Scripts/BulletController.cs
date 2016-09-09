using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    public Rigidbody2D _rb;
    public bool ricochet = false;
    public bool shotThrough = false;

    public Vector2 direction;
    public float speed;
    public int damage = 1;

    public float lifeTime = 0;
    public float curLifeTime = 0;

    void Start()
    {
        speed += Random.Range(speed / -10, speed / 10);
    }

    public void SetDirection(Vector2 newDir)
    {
        direction = newDir;
    }

    void Update()
    {
        if (lifeTime > 0) //
        {
            curLifeTime += Time.deltaTime;

            if (curLifeTime >= lifeTime)
                DestroyBullet();
        }
    }

    void FixedUpdate()
    {
        direction = direction.normalized * speed * 2 * Time.deltaTime;
        _rb.MovePosition(transform.position + new Vector3(direction.x, direction.y, 0));
    }

    void OnTriggerEnter2D (Collider2D coll)
    {
        print("COLLISION");
        HealthController collHealth = coll.gameObject.GetComponent<HealthController>() as HealthController;
        if (collHealth != null)
        {
            collHealth.Damage(damage);
        }
        if (!shotThrough && !ricochet)
            DestroyBullet();
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}