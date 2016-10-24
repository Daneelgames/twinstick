using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKLookControl : MonoBehaviour
{
    protected Animator animator;

    public bool ikActive = false;
    public Vector3 lookPos;

    public float lookSpeed = 0.25f;

    float lookWeight = 0f;
    float bodyWeight = 0f;
    float headWeight = 0f;
    float clampWeight = 1f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetTarget(Vector3 target, bool active)
    {
        ikActive = active;
        lookPos = target;
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {
            float newLookWeight = 0f;
            float newHeadWeight = 0f;
            float newBodyWeight = 0f;
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                newLookWeight = 1f;
                newHeadWeight = 1f;
                newBodyWeight = 1f;
                animator.SetLookAtPosition(lookPos);
            }
            // make smooth
            lookWeight = Mathf.Lerp(lookWeight, newLookWeight, Time.deltaTime * lookSpeed);
            headWeight = Mathf.Lerp(headWeight, newHeadWeight, Time.deltaTime * lookSpeed);
            bodyWeight = Mathf.Lerp(bodyWeight, newBodyWeight, Time.deltaTime * lookSpeed);

            if (ikActive)
                animator.SetLookAtWeight(lookWeight, bodyWeight, headWeight, 0, clampWeight);
            else
                animator.SetLookAtWeight(lookWeight, bodyWeight, headWeight, 0, clampWeight);
        }
    }
}