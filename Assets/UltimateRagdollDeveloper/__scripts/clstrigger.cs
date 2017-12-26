using UnityEngine;
using System.Collections;

/// <summary>
/// 2011-09-12
/// ULTIMATE RAGDOLL GENERATOR V1.6
/// © THE ARC GAMES STUDIO 2011
/// DESIGNED WITH UNITY 3.4.1f5
/// 
/// helper class to trigger the physics driven activation of a ragdolled kinematic character
/// 1- add a collider to the scene object
/// 2- (if the triggering object has no rigidbody) add a rigidbody to the same scene object, and set it as kinematic
/// 3- turn all rigidbodies into physic driven with a call to clsurgutils.metgodriven
/// </summary>
public class clstrigger : MonoBehaviour {
	
	void OnTriggerEnter(Collider varsource) {
		//trigger only with the car's 'bumper' collider
		if (varsource.name == "bumper") {
			//turn rigidbodies into physic driven
			clsurgutils.metgodriven(transform);
		}
		
	}
	
		
}
