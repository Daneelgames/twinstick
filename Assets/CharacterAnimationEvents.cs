using UnityEngine;
using System.Collections;

public class CharacterAnimationEvents : MonoBehaviour
{

    public PlayerMovement pm;

    public void Step()
    {
        if (pm)
        {
            float t = 0f;
            if (Input.GetButton("Run"))
                t = 0.075f;
            else
                t = 0.2f;

            StartCoroutine("ReduceSpeedWhileStepping", t);
        }
    }

    IEnumerator ReduceSpeedWhileStepping(float t)
    {
        pm.SetMaxSpeed(0.6f);
        yield return new WaitForSeconds(t);
        pm.SetMaxSpeed(1f);
    }
}
