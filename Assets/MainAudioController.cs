using UnityEngine;
using System.Collections;

public class MainAudioController : MonoBehaviour {

	public AudioSource au;

	public void Play(AudioClip clip)
	{
		au.clip = clip;
		au.Play();
	}
}
