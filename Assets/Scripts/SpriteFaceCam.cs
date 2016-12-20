using UnityEngine;
using System.Collections;

public class SpriteFaceCam : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		transform.rotation.SetLookRotation(Camera.main.transform.position * Time.unscaledTime);
	}
}
