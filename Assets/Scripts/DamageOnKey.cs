using UnityEngine;
using System.Collections;

public class DamageOnKey : MonoBehaviour {

	public HealthController h;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("g"))
		{
			h.Damage(1);
		}
	}
}
