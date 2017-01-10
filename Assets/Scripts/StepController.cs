using UnityEngine;
using System.Collections;

public class StepController : MonoBehaviour
{
    public StepsList sl;
    float cooldown = 0.2f;

    void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }

    public void Step()
    {
        if (cooldown <= 0)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.2f, 1 << 9))
            {
                cooldown = 0.3f;
                sl.PlayStep(hit.collider.tag);
            }
        }
    }
}