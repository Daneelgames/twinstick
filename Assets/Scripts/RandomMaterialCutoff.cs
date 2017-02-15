using UnityEngine;
using System.Collections;

public class RandomMaterialCutoff : MonoBehaviour
{
    public Renderer rn;
    public float alphaMin;
    public float alphaMax;
    public float rate;
	public float speed;
    float newAlpha;
    void Start()
    {
        newAlpha = rn.material.GetFloat("_Cutoff");
        InvokeRepeating("SetNewAlpha", rate, rate);
    }
    // Update is called once per frame
    void Update()
    {
		float curAlpha = Mathf.Lerp(rn.material.GetFloat("_Cutoff"), newAlpha, speed);		
		rn.material.SetFloat("_Cutoff", curAlpha);
    }
	
	void SetNewAlpha()
	{
		newAlpha = Random.Range(alphaMin, alphaMax);
	}
}