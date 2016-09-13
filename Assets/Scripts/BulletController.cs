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

    public HealthController healthController;


    void Start()
    {
        speed += Random.Range(speed / -10, speed / 10);
    }

    public void SetDirection(float rot)
    {
        transform.eulerAngles = new Vector3(0, 0, rot);
    }

    void Update()
    {
        if (lifeTime > 0)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);

            curLifeTime += Time.deltaTime;

            if (curLifeTime >= lifeTime)
                DestroyBullet();
        }
    }
    
    void OnCollisionEnter2D (Collision2D coll)
    {
        //print("COLLISION");
        HealthController collHealth = coll.gameObject.GetComponent<HealthController>() as HealthController;
        if (collHealth != null)
        {
            collHealth.Damage(damage);
        }

        if (!ricochet && !shotThrough)
        {
            DestroyBullet();
        }

        if (ricochet)
        {
            if (coll.gameObject.tag == "Solid" || coll.gameObject.tag == "Bullet")
            {
                healthController.Damage(1);

                // CAST RAY
                ContactPoint2D contact = coll.contacts[0];
                Vector2 mVect = Vector2.Reflect(transform.position, contact.normal);
                float rot = Mathf.Atan2(mVect.x, mVect.y) * Mathf.Rad2Deg;
                rot = rot + Random.Range(-5f, 5f);
                SetDirection(rot);
            }
            else
                DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}