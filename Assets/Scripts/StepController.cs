using UnityEngine;
using System.Collections;

public class StepController : MonoBehaviour
{
    public StepsList sl;
    float cooldown = 0.15f;

    void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }

    public void StepRaycast()
    {
        if (cooldown <= 0)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.3f, 1 << 9))
            {
                cooldown = 0.15f;
                sl.PlayStep(hit.collider.tag);
            }
        }
    }
}