using UnityEngine;
using System.Collections;

public class ActiveIfHaveItems : MonoBehaviour {
	public string itemNeeded;
	public bool active = true;

	void Start()
	{
		if (StateManager.instance.HaveItem(itemNeeded))
		{
			print (active);
			gameObject.SetActive(active);
		}
	}
}