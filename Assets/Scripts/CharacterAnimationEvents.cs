using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterAnimationEvents : MonoBehaviour
{

    public PlayerMovement pm;
    public MobMovement mm;
    public float mobStepCooldown = 0.2f;
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
        else if (mm)
        {
            StartCoroutine("ReduceSpeedWhileStepping", mobStepCooldown);
        }
        if (steps[leg])
        {
            steps[leg].StepRaycast();
        }
    }

    public void PlaySoundOnPlayer(int clipIndex)
    {
        GameManager.instance.gmAu.au.PlayOneShot(audioClips[clipIndex]);
    }

    IEnumerator ReduceSpeedWhileStepping(float t)
    {
        float mobSpeed = 0;
        if (pm)
            pm.SetMaxSpeed(0.6f);
        else if (mm)
        {
            mobSpeed = mm.speed;
            mm.StepSetSpeed(mobSpeed / 4);
        }
        yield return new WaitForSeconds(t);
        if (pm)
            pm.SetMaxSpeed(1f);
        else if (mm)
        {
            mm.StepSetSpeed(mobSpeed);
        }
    }

    public void MeleeAttack()
    {
        au.pitch = Random.Range(0.75f, 1.25f);
        au.PlayOneShot(meleeAttackClips[Random.Range(0, meleeAttackClips.Count)]);
    }
}
