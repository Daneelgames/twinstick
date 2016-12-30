using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StepsList : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> stepsGravel;
    public List<AudioClip> stepsCarpet;
    public List<AudioClip> stepsWood;
    public List<AudioClip> stepsMetal;
    public List<AudioClip> stepsTile;
    public List<AudioClip> stepsGrass;
    public List<AudioClip> stepsSnow;
    public List<AudioClip> stepsMeat;


    public void PlayStep(string floor)
    {
        float randomVolume = Random.Range(0.75f, 1f);
        float randomPitch = Random.Range(0.75f, 1.25f);
        audioSource.pitch = randomPitch;

        switch (floor)
        {
            case "FloorGravel":
                audioSource.PlayOneShot(stepsGravel[Random.Range(0, stepsGravel.Count)], randomVolume);
                break;

            case "FloorCarpet":
                audioSource.PlayOneShot(stepsCarpet[Random.Range(0, stepsCarpet.Count)], randomVolume);
                break;

            case "FloorWood":
                audioSource.PlayOneShot(stepsWood[Random.Range(0, stepsWood.Count)], randomVolume);
                break;

            case "FloorMetal":
                audioSource.PlayOneShot(stepsMetal[Random.Range(0, stepsMetal.Count)], randomVolume);
                break;

            case "FloorTile":
                audioSource.PlayOneShot(stepsTile[Random.Range(0, stepsTile.Count)], randomVolume);
                break;

            case "FloorGrass":
                audioSource.PlayOneShot(stepsGrass[Random.Range(0, stepsGrass.Count)], randomVolume);
                break;

            case "FloorSnow":
                audioSource.PlayOneShot(stepsSnow[Random.Range(0, stepsSnow.Count)], randomVolume);
                break;

            case "FloorMeat":
                audioSource.PlayOneShot(stepsMeat[Random.Range(0, stepsMeat.Count)], randomVolume);
                break;
        }
    }
}