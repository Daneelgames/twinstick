using UnityEngine;
using System.Collections;

public class CameraZoneController : MonoBehaviour
{
    public GameObject camAnchor;
    public bool followX = false;
    public bool followY = false;
    public bool active = false;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            GameManager.instance.SetActiveCamera(this);
        }
    }

    void FixedUpdate()
    {
        if (active)
        {
            if (followX)
            {
                // look at player on x
            }
            if (followY)
            {
                // look at player on y
            }
        }
    }
}