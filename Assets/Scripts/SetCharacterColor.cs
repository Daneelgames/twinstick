using UnityEngine;
using System.Collections;

public class SetCharacterColor : MonoBehaviour {

    public SkinnedMeshRenderer mesh;

	// Use this for initialization
	void Start () {
        mesh.material.color = GameManager.instance._sm.charactersColor;
	}
}
