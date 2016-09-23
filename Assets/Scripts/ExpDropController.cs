using UnityEngine;
using System.Collections;

public class ExpDropController : MonoBehaviour {

    public float explosionForce = 5f;
    public Rigidbody2D rb;

    public bool player = false;
    
	public void Explosion (Transform origin) {
        Vector2 explosionVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.AddForceAtPosition(origin.position, explosionVector * explosionForce, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            GameManager.instance.GetExp();
            Destroy(gameObject);
        }
    }

    public void SetPlayer()
    {
        player = true;
    }

    public void DestroyOnRespawn()
    {
        if (player)
        {
            player = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}