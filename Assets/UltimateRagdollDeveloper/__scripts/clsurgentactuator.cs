using UnityEngine;
using System.Collections;

/// <summary>
/// 2012-11-27
/// URG-ENTITIES ACTUATOR CLASS FOR SPECIAL BODY PART OPERATIONS
/// © THE ARC GAMES STUDIO 2012
/// DESIGNED WITH UNITY 3.5.6f4
/// 
/// FINAL DESIGN STAGE
/// 
/// Basic actuator class that will call the URGent source methods on trigger or collision, after processing own physic functions.
/// All URGent body parts will have this class attached to handle collision events, whenever the URGent option is enabled in URG! interface.
/// Naturally, the class can be separately added at will to any gameobject to perform any common physic event, shared with URG entities.
/// Behaviors are implemented by means of inheritance or by the clsurgent public variable vargamcasemanager, which is used in the actuators as a switch variable. Basically, any URGent gameobject host can have a different vargamcasemanager value to setup its specific behavior in clsurgent collision or trigger events.
/// 
/// USAGE NOTE: URG has a hard coded reference for 'clsurgent' and 'clsurgentactuator' classes user can edit them at will, but needs be sure that these two classes always exist when creating URGent ragdolls
/// 
/// NOTE: it is expected and suggested that the game model be ragdolled SEPARATED from its possible prefab parent. Notice in fact the references to vargamurgentsource.transform as the 'root' of the object, to avoid referencing an actual parent that might not be in the original 3D model
/// </summary>
public class clsurgentactuator : MonoBehaviour {
	/// <summary>
	/// the body part type, from body extensions available (head, spine, arms, legs)
	/// </summary>
	public clsurgutils.enumparttypes vargamparttype;
	/// <summary>
	/// the name of the part to which the instance is attached. can be used as a parameter for URGent calls.
	/// </summary>
	public string vargampartinstancename = "";
	/// <summary>
	/// the part index in the entities array. Useful value that allows iteration of the part through clsurgutils, with the URGent manager
	/// </summary>
	public int vargampartindex = 0;
	/// <summary>
	/// basic activator switch. Basically used to avoid unnecessary calls on objects that have alreaby been subjected to physic actions
	/// </summary>
	public bool vargamactuatorenabled = true;
	/// <summary>
	/// reference to the source root urgent manager class. critical value that stores the URGent manager.
	/// </summary>
	public clsurgent vargamurgentsource;
	/// <summary>
	/// reference to the transform of this part's ragdoll parent. Caches the part parent.
	/// </summary>
	public Transform vargamsource;
	
	//uncomment this section to manage urgent clicks on body parts
	/*
	void OnMouseDown() {
		metmanagekinematic();
	}
	*/
	
	private void metmanagekinematic() {
		//this condition is used when the script is attached to "scenery", for example a palisade or a fence, or a weapon in the character's hand
		//that needs to become physical and detach from its parent
		if (vargamurgentsource == null) {
			GetComponent<Rigidbody>().isKinematic = false;
			transform.parent = null;
		}
		else {
			//this instead is the code for the 'death' condition that will make the game object entirely ragdoll driven
			//for a start, stop ongoing animations. we assume that our urgent source is hosted together with the Animation component
			vargamurgentsource.GetComponent<Animation>().Stop();
			//this condition determines if the current part is head, spine or source, in which case the character is neutralized
			if (transform == vargamsource || vargamparttype == clsurgutils.enumparttypes.head || vargamparttype == clsurgutils.enumparttypes.spine) {
				clsurgutils.metdriveurgent(vargamurgentsource,null);
				vargamurgentsource.transform.GetComponent<Animation>().Stop();
				vargamurgentsource.transform.GetComponent<Animation>().animatePhysics = false;
				//if there's a character controller added to the parent gameobject, we destroy it to stop the movement
				//note the intentional search of the character controller in the root
				CharacterController varcontroller = transform.root.GetComponent<CharacterController>();
				if (varcontroller != null) {
					Vector3 varforce = varcontroller.velocity;
					Destroy(varcontroller);
					//this adds a scenic effect to the ragdoll, to simulate inertia (for a standard 75kg ragdoll weight)
					vargamurgentsource.vargamnodes.vargamspine[0].GetComponent<Rigidbody>().AddForce(varforce*7500);
				}
			}
			//otherwise we just drive the body part of the actuator
			else {
				clsurgutils.metdriveurgent(vargamurgentsource,this);
				transform.parent = vargamurgentsource.transform;
			}
		}
	}
	
	void OnCollisionEnter(Collision varpsource) {
		//this switch is used to streamline the urgent integration into multiple object, without the need to create additional scripts that inherit clsurgent
		//vargamcasemanager is a public variable which value is set into the inspector, for the clsurgent host
		switch (vargamurgentsource.vargamcasemanager) {
			//example cases, used in demo scenes. additional cases may be added at wish
			case -2:
				//ignore non terrain collisions
				if (!vargamactuatorenabled || (varpsource.gameObject.tag != "missile" && varpsource.gameObject.tag != "terrain")) {
					return;
				}
				clsdismemberator varD = vargamurgentsource.GetComponentInChildren<clsdismemberator>();
				if (varD != null) {
					float varroll = Random.Range(0,0.99f);
					if (varroll > 0.75f) {
						clsurgutils.metdismember(transform, varD.vargamstumpmaterial, varD, varD.vargamparticleparent, varD.vargamparticlechild);
					}
				}
				else {
					Debug.LogError("No Dismemberator Class in source D host.");
				}
				break;
			case -1:
				//lines commented for release polish. uncomment to monitor actuator collision events (attention to non null URGent source)
				//Debug.LogError("actuator collider event "  + transform.name + " " + vargamurgentsource.transform.name + " " + varpsource.transform.name, varpsource.transform);
				//vargamurgentsource.metcollsionentered(transform);
				if (vargamactuatorenabled) {
					//here is the spot for routine code: decrease hitpoints, drop weapon, etc.
					//in this example code, for instance, we react to a collision from 'missile', to activate our desired effect
					if (varpsource.gameObject.tag == "missile") {
						//turn off the actuator since it performed its conversion
						vargamactuatorenabled = false;
						//drive the host
						clsurgutils.metdriveurgent(vargamurgentsource);
						//apply the original force
						GetComponent<Rigidbody>().AddForceAtPosition(varpsource.impactForceSum,varpsource.contacts[0].point, ForceMode.VelocityChange);
					}
				}
				break;
			default:
				break;
		}
	}
	
	void OnTriggerEnter(Collider varpother) {
		//lines commented for release polish. uncomment to monitor actuator trigger events (attention to non null URGent source)
		//Debug.LogError("actuator trigger event " + transform.name + " " + vargamurgentsource.transform.name);
		//vargamurgentsource.metcollidertriggered(transform);
		
		//NOTE: what follows is an variant of the OnCollisionEnter manager (triggers don't have access to the same information as collisions)
		switch (vargamurgentsource.vargamcasemanager) {
			//example cases, used in demo scenes. additional cases may be added at wish
			case -2:
				//ignore non terrain collisions
				if (!vargamactuatorenabled || varpother.tag != "missile") {
					return;
				}
				metmanagekinematic();
				clsdismemberator varD = vargamurgentsource.GetComponentInChildren<clsdismemberator>();
				if (varD != null) {
					float varroll = Random.Range(0,0.99f);
					if (varroll > 0.75f) {
						clsurgutils.metdismember(transform, varD.vargamstumpmaterial, varD, varD.vargamparticleparent, varD.vargamparticlechild);
					}
					else {
					}
				}
				else {
					Debug.LogError("No Dismemberator Class in source D host.");
				}
				break;
			//example case
			case -1:
				//lines commented for release polish. uncomment to monitor actuator collision events (attention to non null URGent source)
				//Debug.LogError("actuator trigger event "  + transform.name + " " + vargamurgentsource.transform.name + " " + varpother.transform.name, varpother.transform);
				//vargamurgentsource.metcollsionentered(transform);
				if (vargamactuatorenabled) {
					//routine code: decrease hitpoints, drop weapon, etc.
					
					//this example code, for instance, would require an object tagged 'missile' to activate the collision effects
					if (varpother.gameObject.tag == "missile") {
						//Destroy(varpsource.collider); //can be used to avoid multiple collisions, for example with bullets
						if (GetComponent<Rigidbody>().isKinematic == true) {
							metmanagekinematic();
						}
						//turn off the actuator since it performed its conversion
						vargamactuatorenabled = false;
					}
				}
				break;
			default:
				break;
		}
	}
}
