using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BreakableObject : MonoBehaviour
{
    public List<Rigidbody> fragments;
    public float explosiveForce = 350.0f;

    public void Break()
    {
        foreach (Rigidbody j in fragments)
        {
            j.AddForce(Random.Range(-explosiveForce, explosiveForce), Random.Range(-explosiveForce, explosiveForce), Random.Range(-explosiveForce, explosiveForce));
            j.AddTorque(Random.Range(-explosiveForce, explosiveForce), Random.Range(-explosiveForce, explosiveForce), Random.Range(-explosiveForce, explosiveForce));
        }
    }
}