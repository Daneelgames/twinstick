using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatherBossController : MonoBehaviour
{
    public float speed = 2f;
    public Rigidbody rb;
    public Animator anim;
    public enum State { Sleep, Attack, Follow, RunAway, Hide, HideAttack, HideOver };
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
    public bool playerInAttackRange = false;
    public int turnDegrees = 0;

    public List<FatherBossDeadBodyController> deadBodies;
    public FatherBossDeadBodyController bodyToHideOn;

    public void PlayerInRange(bool inRange)
    {
        playerInRange = inRange;
        if (inRange)
        {
            if (turnDegrees == 0)
            {
                if (stateBoss == State.Sleep)
                {
                    stateBoss = State.Follow;
                    anim.SetBool("Move", true);
                }
            }
        }
        else if (!inRange && stateBoss == State.Follow)
        {
            Reposition();
        }
    }
    public void PlayerInAttackRange(bool inRange)
    {
        print("player in range is " + inRange);
        if (inRange)
        {
            playerInAttackRange = true;
            if (canAttack)
            {
                if (stateBoss == State.Sleep || stateBoss == State.Follow)
                {
                    Attack();
                    playerInAttackRange = false;
                }
                else if (stateBoss == State.Hide)
                {
                    StartCoroutine("HideAttack");
                    playerInAttackRange = false;
                }
            }
        }
        else
        {
            playerInAttackRange = false;
        }
    }

    IEnumerator HideAttack()
    {
        stateBoss = State.HideAttack;
        yield return new WaitForSeconds(1); // wait
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(2.625f); // hide attack time
        StartCoroutine("HideOver");
    }

    IEnumerator HideOver()
    {
        //set fathers new row
        if (transform.position.x < GameManager.instance.playerController.transform.position.x)
            row = bodyToHideOn.leftRow;
        else
            row = bodyToHideOn.rightRow;
        stateBoss = State.HideOver;
        yield return new WaitForSeconds(1.958f); // hide over time
        Reposition();
        bodyToHideOn = null;
        int random = Random.Range(0, 2);
        float dg = 0;
        if (random == 0)
            dg = 90;
        else
            dg = -90;
        Reposition();
        StartCoroutine("Turn", dg);
    }
    void Attack()
    {
        anim.SetTrigger("Attack");
        StartCoroutine("AttackOver");
        stateBoss = State.Attack;
    }
    IEnumerator AttackOver()
    {
        yield return new WaitForSeconds(attackTime);
        Reposition();
        StartCoroutine("Turn", 180);
    }
    public void PlayerBehind()
    {
        stateBoss = State.Follow;
        StartCoroutine("Turn", 180);
    }
    IEnumerator HideOnBody(int td)
    {
        canAttack = false;
        bodyToHideOn.DisableBody();
        anim.SetTrigger("Hide");
        yield return new WaitForSeconds(2.1f);
        anim.SetBool("Move", false);
        canAttack = true;
        // play hide anim, wait for seconds
        deadBodies.Remove(bodyToHideOn);
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
        else
        {
            anim.SetBool("Move", true);
        }
    }

    public void Hurt()
    {
        if (stateBoss == State.Sleep)
        {
            if (GameManager.instance.playerController.weaponController.weaponAmmoType != WeaponController.Type.Melee)
            {
                int random = Random.Range(0, 2);
                if (random > 0)
                    StartCoroutine("Turn", 180);
            }
            Reposition();
        }
        else if (stateBoss == State.Hide)
        {
            StartCoroutine("HideOver");
        }
    }

    public void Reposition()
    {
        stateBoss = State.RunAway;
        ChooseNewPosition();
        if (turnDegrees == 0)
            anim.SetBool("Move", true);
    }

    void ChooseNewPosition()
    {
        int random = 1;
        if (health.health < health.maxHealth / 2 && deadBodies.Count > 0)
        {
            random = Random.Range(0, 2);
            if (random == 1) // try to find a dead body
            {
                print("random is " + random);
                List<FatherBossDeadBodyController> newBodiesList = new List<FatherBossDeadBodyController>();
                foreach (FatherBossDeadBodyController i in deadBodies)
                {
                    Vector3 iPos = new Vector3(i.transform.position.x, GameManager.instance.playerController.transform.position.y, i.transform.position.z);
                    if (Vector3.Distance(iPos, GameManager.instance.playerController.transform.position) > 6) // add body to list if it's far enough
                    {
                        newBodiesList.Add(i);
                    }
                }
                print(newBodiesList.Count);
                if (newBodiesList.Count > 1 && deadBodies.Count > 13)
                {
                    //choose one body
                    bodyToHideOn = newBodiesList[Random.Range(0, newBodiesList.Count)];
                    // choose row
                    int r = Random.Range(0, 2);
                    if (r == 0)
                        newRow = bodyToHideOn.leftRow;
                    else
                        newRow = bodyToHideOn.rightRow;
                }
                else
                {
                    random = 0;
                    bodyToHideOn = null;
                }
            }
        }
        else
        {
            random = 0;
        }

        if (random == 0) // choose new row to Sleep
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
    void FixedUpdate()
    {
        StabilizeDegrees();
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
        if (stateBoss == State.Follow)
        {
            if (turnDegrees == 0) // father not turns
            {
                Vector3 newVel = transform.forward * speed * 75 * Time.deltaTime;
                rb.velocity = newVel;

                if (playerInAttackRange)
                {
                    Attack();
                }
            }
        }
        else if (stateBoss == State.Attack)
        {
            Vector3 newVel = transform.forward * speed * 20 * Time.deltaTime;
            rb.velocity = newVel;
        }
        else if (stateBoss == State.Sleep)
        {
            rb.velocity = Vector3.zero;
        }
        else if (stateBoss == State.RunAway)
        {
            if (turnDegrees == 0)  // he run away
            {
                Vector3 newVel = transform.forward * speed * 120 * Time.deltaTime; // he runs away with greater speed
                rb.velocity = newVel;

                if (playerInAttackRange)
                {
                    Attack();
                }

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
                        //print(distance);
                        if (distance <= 0.75f)
                        {
                            if (newRow > row)
                                StartCoroutine("Turn", -90);
                            else
                                StartCoroutine("Turn", 90);
                        }
                    }
                    else if (transform.position.z > 3.4f) // he at north, runs to side
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
                        else if (transform.rotation.eulerAngles.y < 100 && transform.rotation.eulerAngles.y > 80)// check if he runs right
                        {
                            // towards the wall ?
                            float _distance = Mathf.Abs(transform.position.x - rows[4].places[0].transform.position.x);
                            if (_distance < 0.5f)
                            {
                                StartCoroutine("Turn", 90);
                            }
                        }
                        else if (transform.rotation.eulerAngles.y < 280 && transform.rotation.eulerAngles.y > 260)// check if he runs left
                        {
                            // towards the wall ?
                            float _distance = Mathf.Abs(transform.position.x - rows[0].places[0].transform.position.x);
                            if (_distance < 0.5f)
                            {
                                StartCoroutine("Turn", -90);
                            }
                        }
                    }
                    else if (transform.position.z < -5.4f)// he at south, runs to side
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
                        else if (transform.rotation.eulerAngles.y < 100 && transform.rotation.eulerAngles.y > 80)// check if he runs right
                        {
                            // towards the wall ?
                            float _distance = Mathf.Abs(transform.position.x - rows[4].places[0].transform.position.x);
                            if (_distance < 0.5f)
                            {
                                StartCoroutine("Turn", -90);
                            }
                        }
                        else if (transform.rotation.eulerAngles.y < 280 && transform.rotation.eulerAngles.y > 260)// check if he runs left
                        {
                            // towards the wall ?
                            float _distance = Mathf.Abs(transform.position.x - rows[0].places[0].transform.position.x);
                            if (_distance < 0.5f)
                            {
                                StartCoroutine("Turn", 90);
                            }
                        }
                    }
                }
                else // he found the row
                {
                    if (bodyToHideOn != null)
                    {
                        float distance = Mathf.Abs(transform.position.z - bodyToHideOn.transform.position.z);
                        if (distance < 0.25f)
                        {
                            if (transform.position.x <= bodyToHideOn.transform.position.x) // father at west
                            {
                                if (transform.rotation.eulerAngles.y < 190 && transform.rotation.eulerAngles.y > 170) // father at north
                                {
                                    StartCoroutine("HideOnBody", -90);
                                }
                                else if (transform.rotation.eulerAngles.y < 10 || transform.rotation.eulerAngles.y > 350) // father at south
                                {
                                    StartCoroutine("HideOnBody", 90);
                                }
                            }
                            else // father at east
                            {
                                if (transform.rotation.eulerAngles.y < 190 && transform.rotation.eulerAngles.y > 170) // father at north
                                {
                                    StartCoroutine("HideOnBody", 90);
                                }
                                else if (transform.rotation.eulerAngles.y < 10 || transform.rotation.eulerAngles.y > 350) // father at south
                                {
                                    StartCoroutine("HideOnBody", -90);
                                }
                            }
                            stateBoss = State.Hide;
                            anim.ResetTrigger("Hurt");
                            InvokeRepeating("TurnToPlayer", 0.1f, 0.1f);
                        }
                    }
                    else // go to sleep place
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
        else if (stateBoss == State.Hide)
        {
            rb.velocity = Vector3.zero;

            if (bodyToHideOn != null)
            {
                Vector3 newPos = Vector3.Lerp(transform.position, new Vector3(bodyToHideOn.transform.position.x, transform.position.y, bodyToHideOn.transform.position.z), 0.5f);
                transform.position = newPos;
            }
        }
        else if (stateBoss == State.HideOver)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, new Vector3(rows[row].places[0].transform.position.x, transform.position.y, transform.position.z), 0.1f);
            transform.position = newPos;
        }
    }

    void TurnToPlayer()
    {
        if (stateBoss == State.Hide)
        {
            if (transform.position.x > GameManager.instance.playerController.transform.position.x)
            {
                Quaternion turnRotation = Quaternion.identity;
                turnRotation.eulerAngles = new Vector3(0, 270, 0);
                transform.rotation = turnRotation;
            }
            else
            {
                Quaternion turnRotation = Quaternion.identity;
                turnRotation.eulerAngles = new Vector3(0, 90, 0);
                transform.rotation = turnRotation;
            }
        }
    }
}