using UnityEngine;
using System.Collections;

public class SpriteFaceCam : MonoBehaviour
{
    public bool blockY = false;
    void Update()
    {
        if (Camera.main)
        {
            if (blockY)
                transform.LookAt(Camera.main.transform.position);
            else
                transform.LookAt(Camera.main.transform.position, Vector3.up);
        }
    }
}
