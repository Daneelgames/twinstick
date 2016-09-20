using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    public Rigidbody2D _rb;
    public bool ricochet = false;
    public float bulletRicochetCooldown = 0.3f;
    public bool shotThrough = false;

    public Vector2 direction;
    public float speed;
    public int damage = 1;

    public float lifeTime = 0;
    public float curLifeTime = 0;

    public float maxBulletOffset = 5f;

    public HealthController healthController;
    
    void Start()
    {
        speed += Random.Range(speed / -10, speed / 10);
    }

    public void SetDirection(float rot)
    {
        float random = Random.Range(-maxBulletOffset, maxBulletOffset);

        transform.eulerAngles = new Vector3(0, 0, rot + random);
    }

    void Update()
    {
        if (lifeTime > 0)
        {
            //transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);

            curLifeTime += Time.deltaTime;

            if (curLifeTime >= lifeTime)
                DestroyBullet();
        }
        if (bulletRicochetCooldown > 0)
            bulletRicochetCooldown -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        _rb.velocity = ((Vector2)transform.TransformDirection(Vector3.right)).normalized * speed;
    }
    
    void OnCollisionEnter2D (Collision2D coll)
    {
        if (!ricochet && !shotThrough)
        {
            DamageColl(coll);
            DestroyBullet();
        }

        if (ricochet)
        {

            if (coll.gameObject.tag == "Solid")
            {
                DamageColl(coll);
                Ricochet(coll);
            }
            else if (coll.gameObject.tag == "Bullet")
            {
                if (bulletRicochetCooldown <= 0)
                {
                    DamageColl(coll);
                    Ricochet(coll);
                }
            }
            else
            {
                DamageColl(coll);
                DestroyBullet();
            }
        }
    }

    void DamageColl(Collision2D coll)
    {
        //print("COLLISION");
        HealthController collHealth = coll.gameObject.GetComponent<HealthController>() as HealthController;
        if (collHealth != null)
        {
            collHealth.Damage(damage);
        }
    }

    void Ricochet(Collision2D coll)
    {
        healthController.Damage(1);
        ContactPoint2D contact = coll.contacts[0];
        Vector2 mVect = Vector2.Reflect((Vector2)transform.TransformDirection(Vector3.right), contact.normal);
        var rot = Mathf.Rad2Deg * Mathf.Atan2(mVect.y, mVect.x) + Random.Range(-5f, 5f);
        SetDirection(rot);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}