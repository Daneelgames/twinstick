using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {
    
    public int damage = 10;
    public float lifeTime = 1f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    
    void OnTriggerEnter2D (Collider2D coll)
    {
        HealthController collHealth = coll.gameObject.GetComponent<HealthController>();
        if (collHealth != null)
        {
            float distanceDamage = damage - Vector2.Distance(transform.position, coll.transform.position) * 3;
            collHealth.Damage(damage);
        }
    }
}
