using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

    public Rigidbody _rb;
    public Collider _coll;

    public Vector3 direction;
    public float speed;
    public int damage = 1;

    public float lifeTime = 0;
    public float curLifeTime = 0;

    public float maxBulletOffset = 5f;

    public HealthController healthController;
    public GameObject explosion;

    void Start()
    {
        speed += Random.Range(speed / -10, speed / 10);
    }

    public void SetDirection(Vector3 target)
    {
        float random = Random.Range(-maxBulletOffset, maxBulletOffset);

        target = new Vector3(target.x + random, target.y + random, target.z + random);

        transform.LookAt(target);

        //transform.eulerAngles = new Vector3(angles.x + random, angles.y + random, angles.z + random);
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
    }

    void FixedUpdate()
    {
        _rb.AddRelativeForce(Vector3.forward * speed);
    }

    void OnCollisionEnter(Collision coll)
    {
        DamageColl(coll);
    }

    void DamageColl(Collision coll)
    {
        //print(gameObject.name + " COLLISION " + coll.gameObject.name);
        HealthController collHealth = coll.gameObject.GetComponent<HealthController>() as HealthController;
        if (collHealth)
            collHealth.Damage(damage);

        if (explosion != null)
            Instantiate(explosion, transform.position, transform.rotation);

        DestroyBullet();
    }

    IEnumerator EnableCollision(Collider coll)
    {
        yield return new WaitForSeconds(0.3f);
        if (coll != null && _coll != null)
            Physics.IgnoreCollision(coll, _coll, false);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}