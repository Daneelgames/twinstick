using UnityEngine;
using System.Collections;

public class FadeAudioOnG : MonoBehaviour {

public Stateful auSo;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("g"))
		{
			auSo.BgmSourceInactve();
		}
	}
}
