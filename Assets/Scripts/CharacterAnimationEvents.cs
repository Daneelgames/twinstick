﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterAnimationEvents : MonoBehaviour
{

    public PlayerMovement pm;
    public List<StepController> steps;
    public List<AudioClip> audioClips;
    public List<AudioClip> meleeAttackClips;
    public AudioSource au;

    public void Step(int leg) // 0 left; 1 right
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
        if (steps[leg])
        {
            steps[leg].Step();
        }
    }

    public void PlaySoundOnPlayer(int clipIndex)
    {
        GameManager.instance.gmAu.au.PlayOneShot(audioClips[clipIndex]);
    }

    IEnumerator ReduceSpeedWhileStepping(float t)
    {
        pm.SetMaxSpeed(0.6f);
        yield return new WaitForSeconds(t);
        pm.SetMaxSpeed(1f);
    }

    public void MeleeAttack()
    {
        au.pitch = Random.Range(0.75f, 1.25f);
        au.PlayOneShot(meleeAttackClips[Random.Range(0, meleeAttackClips.Count)]);
    }
}
