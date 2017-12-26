using UnityEngine;
using System.Collections;

public class SetLayerScript : MonoBehaviour {
	
	//private static GameObject thisObject;
	
	// Use this for initialization
	void Start () {
		
		//thisObject = this.gameObject;
		GameObject.Find ("Gestures").GetComponent<TapRecog>().playerColliders.Add(this.gameObject);
	}
	
	void Awake()
	{
		//GameObject.Find ("Gestures").GetComponent<TapRecog>().playerColliders.Add(this.gameObject);	
	}
	
	void Update()
	{

	}
	
	public void SetLayer(int newLayer)
	{
		Debug.Log ("thisObject: " + this.gameObject.name + " SettingLayer: " + newLayer + " @: " + Time.time);
		this.gameObject.layer = newLayer;
		//Debug.Log ("thisObjectParent: " + thisObject);
		//Debug.Log ("Layer: " + this.gameObject.layer);
	}
}
