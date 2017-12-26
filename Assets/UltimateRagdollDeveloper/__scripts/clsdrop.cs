using UnityEngine;
using System.Collections;

/// <summary>
/// 2013-01-07
/// ULTIMATE RAGDOLL GENERATOR V4
/// © THE ARC GAMES STUDIO 2012
/// DESIGNED WITH UNITY 4.0.0f7
/// 
/// essential class to separate a kinematic gameobject from its parent
/// </summary>
public class clsdrop : MonoBehaviour {

	void Start () {
		if (GetComponent<Rigidbody>() != null && GetComponent<Rigidbody>().isKinematic == true) {
			GetComponent<Rigidbody>().isKinematic = false;
		}
		transform.parent = null;
	}
}
