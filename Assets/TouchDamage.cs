using UnityEngine;
using System.Collections;

public class TouchDamage : MonoBehaviour {

    public Collider2D _collider;

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == gameObject.layer)
        {
            Physics2D.IgnoreCollision(coll.collider, _collider, true);

            if (coll.gameObject.tag == "Player")
            {
                GameManager.instance.playerController.playerHealth.Damage(1);
            }
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject.layer == gameObject.layer)
        {
            Physics2D.IgnoreCollision(coll.collider, _collider, false);
        }
    }

}
