using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherBossController : MonoBehaviour
{
    public float speed = 2f;
    public Rigidbody rb;
    public Animator anim;
    public enum State { Sleep, Attack, Follow, Reposition, RunAway, Hide };
    public State stateBoss = State.Sleep;
    public float attackTime = 0.5f;
    public HealthController health;

    public void PlayerInRange(bool inRange)
    {
        if (inRange)
        {
            if (stateBoss == State.Sleep || stateBoss == State.Reposition)
            {
                stateBoss = State.Follow;
                anim.SetBool("Move", true);
            }
        }
        else if (!inRange && stateBoss == State.Follow)
        {
            stateBoss = State.Reposition;
        }
    }
    public void PlayerInAttackRange(bool inRange)
    {
        if (inRange)
        {
            if (stateBoss == State.Sleep || stateBoss == State.Reposition || stateBoss == State.Follow)
            {
                anim.SetTrigger("Attack");
                StartCoroutine("AttackOver");
                stateBoss = State.Attack;
            }
        }
    }
    IEnumerator AttackOver()
    {
        yield return new WaitForSeconds(attackTime);
        stateBoss = State.RunAway;
    }
    void Update()
    {
        if (stateBoss == State.Follow)
        {
            Vector3 newVel = transform.forward * speed * 50 * Time.deltaTime;
            rb.velocity = newVel;
        }
        else if (stateBoss == State.Attack)
        {
            Vector3 newVel = transform.forward * speed * 10 * Time.deltaTime;
            rb.velocity = newVel;
        }
    }
}