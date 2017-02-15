using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobMovement : MonoBehaviour
{

    public enum State { Idle, Chase, Dead };

    public State mobState = State.Idle;

    public float reactionDistance = 10f;
    public float stopDistance = 1f;
    public int nextAttackIndex = 0;
    public List<bool> attackRanges = new List<bool>();
    public List<string> attackTriggerName = new List<string>();
    public List<float> attackCooldownMax = new List<float>();
    public float attackCooldown = 2f;

    public Collider mainCollider;

    public float hurtCooldownMax = 1f;
    float hurtCooldown = 1f;

    public Rigidbody rb;
    public float speed = 5f;
    public float turnSmooth = 5f;

    float distanceToPlayer = 0;

    public Animator anim;
    float animSpeed = 0;
    float animNewSpeed = 0;

    public Stateful stateful;
    public HealthController health;
    public Collider aimCollder;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("CheckDistanceToPlayer", 0f, 0.5f);
        InvokeRepeating("SetNextAttack", 1f, 1f);
    }

    void Update()
    {
        if (mobState != State.Dead)
        {
            if (attackCooldown > 0)
            {
                attackCooldown -= Time.deltaTime;
            }

            if (hurtCooldown > 0)
            {
                hurtCooldown -= Time.deltaTime;
            }

            SetAnimSpeedSmooth();
        }
    }

    public void Hurt()
    {
        hurtCooldown = hurtCooldownMax;
    }

    void SetNextAttack()
    {
        List<int> indexes = new List<int>();
        for (int i = 0; i < attackRanges.Count; i++)
        {
            if (attackRanges[i] != false)
            {
                indexes.Add(i);
            }
        }
        if (indexes.Count > 0)
        {
            int maxIndex = indexes.Count;

            nextAttackIndex = indexes[Random.Range(0, maxIndex)];
        }
    }

    void CheckDistanceToPlayer()
    {
        if (mobState != State.Dead)
        {
            distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.playerInGame.transform.position);

            if (distanceToPlayer > reactionDistance)
            {
                mobState = State.Idle;
            }
            else if (distanceToPlayer <= reactionDistance)
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
        //Attack
        if (attackRanges[nextAttackIndex] && attackCooldown <= 0)
        {
            animNewSpeed = 0;
            anim.SetTrigger(attackTriggerName[nextAttackIndex]);
            attackCooldown = attackCooldownMax[nextAttackIndex];
        }
        else if (attackCooldown <= 0 && hurtCooldown <= 0)
        {
            animNewSpeed = 1;

            // choose movement direction
            Vector3 movement = GameManager.instance.playerInGame.transform.position - transform.position;
            movement = movement.normalized * speed;
            movement.y = rb.velocity.y;

            // rotate
            Vector3 mobToTarget = (transform.position + movement) - transform.position;
            mobToTarget.y = transform.position.y;
            Quaternion newRotation = Quaternion.LookRotation(mobToTarget);
            newRotation = Quaternion.Slerp(newRotation, transform.rotation, turnSmooth);
            rb.MoveRotation(newRotation);


            //set rotate.y = 0
            Quaternion rot = rb.rotation;
            rot.eulerAngles = new Vector3(0, rb.rotation.eulerAngles.y, 0);
            rb.rotation = rot;

            //move mob
            float distance = Vector3.Distance(transform.position, GameManager.instance.playerInGame.transform.position);
            float rotationOffset = Mathf.Abs(newRotation.eulerAngles.y - transform.rotation.eulerAngles.y);
            // print (rotationOffset);
            if (distance > stopDistance && rotationOffset < 0.3f)
                rb.velocity = movement;
            else
                rb.velocity = Vector3.zero;

        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void PlayerInRange(int index, bool inside)
    {
        attackRanges[index] = inside;
    }

    void SetAnimSpeedSmooth()
    {
        animSpeed = Mathf.Lerp(animSpeed, animNewSpeed, 0.1f);
        anim.SetFloat("Speed", animSpeed);
    }

    public void Dead()
    {
        mobState = State.Dead;
        stateful.MobDead();
        if (rb && mainCollider)
            StartCoroutine("KinematicAfterTime");
    }

    public void DeadOnStart()
    {
        health.SetHealth(0);
        mobState = State.Dead;
        rb.isKinematic = true;
        mainCollider.enabled = false;
    }

    IEnumerator KinematicAfterTime()
    {
        yield return new WaitForSeconds(1f);
        rb.isKinematic = true;
        mainCollider.enabled = false;
    }

    public void StepSetSpeed(float sp)
    {
        speed = sp;
    }
}