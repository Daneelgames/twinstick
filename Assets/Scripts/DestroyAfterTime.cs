using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour
{
    public float t = 1f;
    void Start()
    {
        Destroy(gameObject, t);
    }
}