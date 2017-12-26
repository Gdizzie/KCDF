using UnityEngine;
using System.Collections;

public class DistanceCheck : MonoBehaviour {
	
	Camera camera;
	
	bool visible = false;
	
	// Use this for initialization
	void Start () {
		
		camera = Camera.main;
		StartCoroutine(CheckDistance());
		
	}
	
	// Update is called once per frame
	void Update () {
	
		//Debug.Log ("LessThanFCP: " + Vector3.Distance(camera.transform.position, transform.position));
		//Debug.Log ("FCP: " + camera.farClipPlane);
	}
	
	IEnumerator CheckDistance()
	{
		yield return new WaitForSeconds(1);
		
		if(Vector3.Distance(camera.transform.position, transform.position) < camera.farClipPlane && visible == false)
		{
			//Debug.Log ("LessThanFCP: " + Vector3.Distance(camera.transform.position, transform.position));	
			yield return new WaitForSeconds(1);
			SendMessage("FadeIn");
			visible = true;
		}
		else if(Vector3.Distance(camera.transform.position, transform.position) > camera.farClipPlane)
		{
			SendMessage("FadeOut");
			
		}
		StartCoroutine(CheckDistance());
	}
	
			
	
}
