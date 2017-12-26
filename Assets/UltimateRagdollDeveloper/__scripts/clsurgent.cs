using UnityEngine;
using System.Collections;

/// <summary>
/// 2012-10-25
/// URG-ENTITIES, CLASS FOR SPECIAL BODY PART OPERATIONS
/// © THE ARC GAMES STUDIO 2012
/// DESIGNED WITH UNITY 3.5.6f4
/// 
/// FINAL DESIGN STAGE
/// 
/// USAGE NOTE: URG has a hard coded reference for 'clsurgent' and 'clsurgentactuator' classes user can change them at will, but needs be sure that these always exist when creating URGent ragdolls
/// 
/// This is an advanced utility class that is responsible for body part operations and that stores the URGed structure to easily access and interact with ragdolled body parts
public class clsurgent : MonoBehaviour {
	/// <summary>
	/// the list of the nodes that constitute the ragdoll
	/// </summary>
	public clsurgentnodes vargamnodes = new clsurgentnodes();
	/// <summary>
	/// this is an arbitrary value that can be used in the clsurgetnactuator class as a case switch.
	/// it will allow for example differentiation between a 'player' ragdoll with vargamcasemanager = 0
	/// and a 'monster' ragdoll with vargamcasemanager = 1
	/// </summary>
	public int vargamcasemanager = 0;

	/*
	//commented out in rev.4 for proper animation states easing
	void Start() {
		animation.wrapMode = WrapMode.Loop;
	}	
	*/
	
	/// <summary>
	/// 'HUB' instance of the collision event, that can be issued by any of the actuators of this URGent ragdoll
	/// </summary>
	/// <param name='varpbodypart'>
	/// the actual transform of the original collision event
	/// </param>
	public void metcollsionentered(Transform varpbodypart) {
		//line commented for release polish. uncomment to monitor urg manager collider events
		//Debug.LogError("manager collider event" + varpbodypart.name + " " + varpbodypart.root.name);
	}
	
	/// <summary>
	/// 'HUB' instance of the trigger event, that can be issued by any of the actuators of this URGent ragdoll
	/// </summary>
	/// <param name='varpbodypart'>
	/// the actual transform of the original trigger event
	/// </param>
	public void metcollidertriggered(Transform varpbodypart) {
		//line commented for release polish. uncomment to monitor urg manager trigger events
		//Debug.LogError("manager trigger event" + varpbodypart.name + " " + varpbodypart.root.name);
	}
	
}
