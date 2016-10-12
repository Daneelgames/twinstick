using UnityEngine;
using System.Collections;

public class HoleInteractor : MonoBehaviour {

    public HealthController unitHealth;
    public WeaponController weapon;
    public Animator anim;


    public float fallTimeMax = 1f;
    public float fallTime = 1f;
    public bool objectOverHole = false;

    public void ToggleOverHole(bool overHole)
    {
        objectOverHole = overHole;
        fallTime = fallTimeMax;
    }

    void Update()
    {
        if (objectOverHole)
        {
            if (fallTime > 0)
                fallTime -= Time.deltaTime;
            else
            {
                ObjectFall();
                objectOverHole = false;
            }
        }
    }

    void ObjectFall()
    {

    }
}
