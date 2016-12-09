using UnityEngine;
using System.Collections;

public class SetCharacterColor : MonoBehaviour {

    public SkinnedMeshRenderer mesh;
    public MeshRenderer meshRenderer;

	// Use this for initialization
	void Start () {
		if (mesh)
        	mesh.material.color = GameManager.instance._sm.charactersColor;
		if (meshRenderer)
			meshRenderer.material.color = GameManager.instance._sm.charactersColor;
	}
}
