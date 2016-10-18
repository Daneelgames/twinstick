using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKLookControl : MonoBehaviour
{
    protected Animator animator;

    public bool ikActive = false;
    public Vector3 lookPos;

    public float lookWeight = 0f;
    public float bodyWeight = 0f;
    public float headWeight = 0f;
    public float clampWeight = 0f;
    public float lookSpeed = 0.25f;

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
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                newLookWeight = 1f;
                animator.SetLookAtPosition(lookPos);
            }
            lookWeight = Mathf.Lerp(lookWeight, newLookWeight, Time.deltaTime * lookSpeed);
            animator.SetLookAtWeight(lookWeight, bodyWeight, headWeight, 0, clampWeight);
        }
    }
}