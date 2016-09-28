using UnityEngine;
using System.Collections;

public class WeaponMeleeController : MonoBehaviour {

    public GameObject owner;

    public int damage = 1;

    public Animator anim;
    public Collider2D _coll;

    public float reloadTime = 0.25f;
    public float curReload = 0f;
    public bool meleeBounce = false;

    void Update()
    {

        if (curReload > 0)
        {
            curReload -= Time.deltaTime;
        }
    }

    public void Attack(bool bounce)
    {
        meleeBounce = bounce;

        if (curReload <= 0)
        {
            curReload = reloadTime;

            anim.SetTrigger("Attack");
            _coll.enabled = true;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Physics2D.IgnoreCollision(coll.collider, _coll, true);
        if (coll.gameObject != owner)
        {
            HealthController collHealth = coll.gameObject.GetComponent<HealthController>() as HealthController;

            if (collHealth != null && coll.gameObject.tag != "Bullet")
            {
                StartCoroutine("IgnoreCollision", coll);
                _coll.enabled = false;
                collHealth.Damage(damage);
            }
        }
    }

    IEnumerator IgnoreCollision(Collision2D coll)
    {
        yield return new WaitForSeconds(reloadTime);
        Physics2D.IgnoreCollision(coll.collider, _coll, false);
    }
}