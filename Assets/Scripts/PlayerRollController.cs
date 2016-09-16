using UnityEngine;
using System.Collections;

public class PlayerRollController : MonoBehaviour {

    public Animator _anim;

    public float rollSpeed = 9f;
    public float rollTime = 0.75f;
    public bool roll = false;

    public PlayerMovement pm;

    public float rollCooldown = 0.1f;
    float cooldown = 0;

    public ParticleSystem rollParticles;

    void Update()
    {
        if (Input.GetButtonDown("Fire2") && !roll && cooldown <= 0)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                StartCoroutine("Roll");
            }
        }

        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }
    

    IEnumerator Roll ()
    {
        // roll
        SetRoll(true);
        yield return new WaitForSeconds(rollTime);
        if (roll)
        {
            // roll stop 
            SetRoll(false);
        }
    }

    void OnCollisionEnter2D (Collision2D coll)
    {
        if (coll.gameObject.tag == "Solid" && roll)
        {
            SetRoll(false);
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Solid" && roll)
        {
            SetRoll(false);
        }
    }

    void SetRoll(bool rollOn)
    {
        //print("roll " + rollOn);
        pm.Roll(rollOn, rollSpeed);
        roll = rollOn;
        _anim.SetBool("Roll", rollOn);

        if (rollOn == false)
        {
            cooldown = rollCooldown;
            rollParticles.Stop();
        }
        else
            rollParticles.Play();
    }
}