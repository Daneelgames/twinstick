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
    public bool canAttack = true;
    public float attackTime = 0.5f;
    public HealthController health;
    public List<int> rowList = new List<int>();
    public int row = 2; // middle row
    public int newRow = 2;
    public int place = 1; // farther place
    public int newPlace = 1;
    [System.Serializable]
    public class Row
    {
        public List<Transform> places;
    }
    public List<Row> rows = new List<Row>();
    public bool playerInRange = false;
    public int turnDegrees = 0;

    public void PlayerInRange(bool inRange)
    {
        if (inRange)
        {
            if (turnDegrees == 0)
            {
                playerInRange = true;
                if (stateBoss == State.Sleep || stateBoss == State.Reposition)
                {
                    stateBoss = State.Follow;
                    anim.SetBool("Move", true);
                }
            }
        }
        else if (!inRange && stateBoss == State.Follow)
        {
            playerInRange = false;
            stateBoss = State.Reposition;
        }
    }
    public void PlayerInAttackRange(bool inRange)
    {
        if (inRange && canAttack)
        {
            if (stateBoss == State.Sleep || stateBoss == State.Reposition || stateBoss == State.Follow)
            {
                anim.SetTrigger("Attack");
                StartCoroutine("AttackOver");
                stateBoss = State.Attack;
            }
        }
    }

    IEnumerator Turn(int td)
    {

        turnDegrees = td;
        canAttack = false;
        float t = 0.75f;
        switch (turnDegrees)
        {
            case 180:
                t = 1.5f;
                break;
            default:
                t = 0.75f;
                break;
        }
        anim.SetBool("Turn", true);
        yield return new WaitForSeconds(t);
        anim.SetBool("Turn", false);
        turnDegrees = 0;
        canAttack = true;
        if (stateBoss == State.Sleep)
        {
            anim.SetBool("Move", false);
        }
    }
    IEnumerator AttackOver()
    {
        yield return new WaitForSeconds(attackTime);
        stateBoss = State.RunAway;
        ChooseNewPosition();
        StartCoroutine("Turn", 180);
    }

    void ChooseNewPosition()
    {
        rowList = new List<int>();
        for (int i = 0; i < 5; i++)
        {
            if (i != row)
            {
                //print(i);
                rowList.Add(i);
            }
        }
        newRow = rowList[Random.Range(0, 4)];
        newPlace = Random.Range(0, 2);
    }
    void StabilizeDegrees()
    {
        if (turnDegrees == 0)
        {
            float y = transform.rotation.eulerAngles.y;
            float angle = 0;
            if (y >= 315 || y < 45) // up
            {
                angle = 0f;
            }
            else if (y >= 45 && y < 135)// right
            {
                angle = 90f;
            }
            else if (y >= 135 && y < 230) // down
            {
                angle = 180f;
            }
            else // left
            {
                angle = 270f;
            }
            Quaternion turnRotation = Quaternion.identity;
            y = Mathf.Lerp(transform.rotation.eulerAngles.y, angle, 0.99f);
            turnRotation.eulerAngles = new Vector3(0, y, 0);
            transform.rotation = turnRotation;
        }
        else
        {
            if (stateBoss == State.RunAway || stateBoss == State.Follow)
            {
                if (row != newRow)
                {
                    float distance = Mathf.Abs(transform.position.z - 3.5f);
                    if (distance <= 0.75f)
                    {
                        float newZ = Mathf.Lerp(transform.position.z, 3.5f, 0.5f);
                        transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
                    }
                    else
                    {
                        float distanceSouth = Mathf.Abs(transform.position.z - 5.5f);
                        if (distanceSouth <= 0.75f)
                        {
                            float newZ = Mathf.Lerp(transform.position.z, 5.5f, 0.5f);
                            transform.position = new Vector3(transform.position.x, transform.position.y, newZ);
                        }
                    }
                }
                else // if father turns on true row
                {
                    float distance = Mathf.Abs(transform.position.x - rows[row].places[0].transform.position.x);
                    if (distance <= 0.75f)
                    {
                        float newX = Mathf.Lerp(transform.position.x, rows[row].places[0].transform.position.x, 0.5f);
                        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                    }
                }
            }
        }
    }
    void Update()
    {
        StabilizeDegrees();
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
        else if (stateBoss == State.Sleep)
        {
            rb.velocity = Vector3.zero;
            if (turnDegrees != 0) // father turns
            {
                float turnTo = 0;
                switch (turnDegrees)
                {
                    case 90:
                        turnTo = 1f;
                        break;
                    default:
                        turnTo = -1f;
                        break;
                }
                float turn = turnTo * 120f * Time.deltaTime;
                Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
                rb.MoveRotation(rb.rotation * turnRotation);
            }
        }
        else if (stateBoss == State.RunAway)
        {
            if (turnDegrees != 0) // father turns
            {
                float turnTo = 0;
                switch (turnDegrees)
                {
                    case 90:
                        turnTo = 1f;
                        break;
                    default:
                        turnTo = -1f;
                        break;
                }
                rb.velocity = Vector3.zero;
                float turn = turnTo * 120f * Time.deltaTime;
                Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
                rb.MoveRotation(rb.rotation * turnRotation);
            }
            else // he run away
            {
                Vector3 newVel = transform.forward * speed * 75 * Time.deltaTime;
                rb.velocity = newVel;
                if (row != newRow) // if he didn't found the row yet
                {
                    if (transform.rotation.eulerAngles.y > 340 || transform.rotation.eulerAngles.y < 40) // he runs north
                    {
                        float distance = Mathf.Abs(transform.position.z - 3.5f);
                        //print(distance);
                        if (distance <= 0.75)
                        {
                            if (newRow > row)
                                StartCoroutine("Turn", 90);
                            else
                                StartCoroutine("Turn", -90);
                        }
                    }
                    else if (transform.rotation.eulerAngles.y < 200 && transform.rotation.eulerAngles.y > 100) // he runs south
                    {
                        float distance = Mathf.Abs(transform.position.z + 5.5f);
                        print(distance);
                        if (distance <= 0.75f)
                        {
                            if (newRow > row)
                                StartCoroutine("Turn", -90);
                            else
                                StartCoroutine("Turn", 90);
                        }
                    }
                    else if (transform.position.z > 3.4f)
                    {
                        float distance = Mathf.Abs(transform.position.x - rows[newRow].places[0].transform.position.x);
                        if (distance < 0.5f)
                        {
                            if (newRow > row)
                                StartCoroutine("Turn", 90);
                            else
                                StartCoroutine("Turn", -90);
                            row = newRow;
                        }
                    }
                    else if (transform.position.z < -5.4f)
                    {
                        float distance = Mathf.Abs(transform.position.x - rows[newRow].places[0].transform.position.x);
                        if (distance < 0.5f)
                        {
                            if (newRow > row)
                                StartCoroutine("Turn", -90);
                            else
                                StartCoroutine("Turn", 90);
                            row = newRow;
                        }
                    }
                }
                else // he found the row
                {
                    float distance = Mathf.Abs(transform.position.z - rows[row].places[newPlace].transform.position.z);
                    //print (distance);
                    if (distance < 0.5f)
                    {
                        rb.velocity = Vector3.zero;
                        place = newPlace;
                        int r = Random.Range(0, 2);
                        switch (r)
                        {
                            case 0:
                                anim.SetBool("Move", false);
                                break;
                            case 1: // turn
                                StartCoroutine("Turn", 180);
                                break;
                        }
                        stateBoss = State.Sleep;
                    }
                }
            }
        }
    }
}