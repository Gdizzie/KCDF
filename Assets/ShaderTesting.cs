using UnityEngine;
using System.Collections;

public class ShaderTesting : MonoBehaviour {
	
	//public Material shader;
	
	float cutOffValue = 100;
	
	public bool fallingOff = true;
	
	public float cutOffTarget;
	
	public float revealOffset;
	
	// Use this for initialization
	void Start () {
	
		//cutOffTarget = 0.5f;
		
	}
	
	// Update is called once per frame
	/*void Update () {
				
		this.renderer.material.SetFloat("_Cutoff", cutOffValue);
		
		if(cutOffValue < cutOffTarget && fallingOff)
		{
			cutOffValue += 0.01f;
		}
		else if(cutOffValue < 0)
		{
			fallingOff = false;	
		}
		
		if(fallingOff == false && cutOffValue > 0)
		{
			cutOffValue -= 0.01f;	
		}
		else if(cutOffValue < 0)
		{
			fallingOff = true;	
		}
		
	}*/
	
	void Update()
	{
		//revealOffset = cutOffValue / cutOffTarget;
		
		this.GetComponent<Renderer>().material.SetTextureOffset("_Mask", new Vector2(revealOffset, 0));
			
	}
	
	void AddMeter(float amount)
	{
			
	}
}
