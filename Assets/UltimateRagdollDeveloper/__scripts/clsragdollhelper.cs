using UnityEngine;
using System.Collections;

/// <summary>
/// 2012-11-06
/// ULTIMATE RAGDOLL GENERATOR V4
/// © THE ARC GAMES STUDIO 2012
/// DESIGNED WITH UNITY 3.4.0f5
/// 
/// helper class to trigger the ragdoll function in a gameobject with a ragdollifier attached
/// </summary>
public class clsragdollhelper : MonoBehaviour {
	private bool varcanragdoll = false;
	private clsragdollify varlocalragdollifier;
	
	void Start () {
		GetComponent<Animation>().wrapMode = WrapMode.Loop;
		varlocalragdollifier = GetComponent<clsragdollify>();
		if (varlocalragdollifier != null) {
			if (varlocalragdollifier.vargamragdoll != null)
				varcanragdoll = true;
		}
	}
	
	/*
	//V4: moved into camera manager
	void OnGUI() {
		if (varcanragdoll) {
			if(GUILayout.Button("Go ragdoll")) {
				varlocalragdollifier.metgoragdoll();
				Destroy(gameObject);
			}
		}
	}
	*/
	
	/// <summary>
	/// shortcut to the ragdollify component
	/// </summary>
	public void metgoragdoll() {
		if (varcanragdoll) {
			varlocalragdollifier.metgoragdoll();
			Destroy(gameObject);
		}
	}
}
