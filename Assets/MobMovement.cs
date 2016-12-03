using UnityEngine;
using System.Collections;

public class MobMovement : MonoBehaviour {

    public enum State {Idle, Chase, Dead};

    public State mobState = State.Idle;

    public float reactionDistance = 10f;
    public float attackDistance = 1f;

    public Rigidbody rb;
    public float speed = 5f;
    public float turnSmooth = 5f;

    public Animator anim;

	// Use this for initialization
	void Start () {
        InvokeRepeating("CheckDistanceToPlayer", 0.5f, 1f);
	}

    void CheckDistanceToPlayer()
    {
        if (mobState != State.Dead)
        {
            float distance = Vector3.Distance(transform.position, GameManager.instance.playerInGame.transform.position);

            if (distance > reactionDistance)
            {
                mobState = State.Idle;
            }
            else if (distance <= reactionDistance)
            {
                mobState = State.Chase;
            }
        }
    }

    void FixedUpdate()
    {
        if (mobState == State.Chase)
        {
            ChasePlayer();
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }
    }

    void ChasePlayer()
    {
        anim.SetFloat("Speed", 1);

        //move to player
        Vector3 movement = GameManager.instance.playerInGame.transform.position - transform.position;
        movement = movement.normalized * speed;
        movement.y = rb.velocity.y;
        
        //set rotate.y = 0
        Quaternion rot = rb.rotation;
        rot.eulerAngles = new Vector3(0, rb.rotation.eulerAngles.y, 0);
        rb.rotation = rot;
        rb.velocity = movement;

        // rotate
        Vector3 mobToTarget = (transform.position + movement) - transform.position;
        mobToTarget.y = transform.position.y;
        Quaternion newRotation = Quaternion.LookRotation(mobToTarget);
        newRotation = Quaternion.Slerp(newRotation, transform.rotation, turnSmooth);
        rb.MoveRotation(newRotation);
    }
}