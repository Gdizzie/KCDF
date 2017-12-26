using UnityEngine;
using System.Collections;

/// <summary>
/// 2012-10-04
/// ULTIMATE RAGDOLL GENERATOR V4.0
/// © THE ARC GAMES STUDIO 2011
/// DESIGNED WITH UNITY 3.5.6f4
/// 
/// Extended class to drive the demo scene vehicles
/// NOTE: the dismemberation flow starts with collision detection, hosted into the "_colliders=> bumper" child of the jeep, for ease of access to the trigger
/// </summary>
public class clscar : MonoBehaviour {
	//rev 4
	public int vargamcartype = 0;
	/// <summary>
	/// set to true to manually control the car with standard directions
	/// </summary>
	public bool vargamusercontrolled = false;
	/// <summary>
	/// maximum torque
	/// </summary>
	public float vargammotormax = 500;
	/// <summary>
	/// simple car with a single shift, will accelerate with maximum allowed torque and reach this value, to stop accelerating further
	/// </summary>
	public float vargamspeedmax = 15;
	/// <summary>
	/// maximum steering angle in degrees
	/// </summary>
	public float vargamsteermax = 20;
	/// <summary>
	/// maximum antitorque
	/// </summary>
	public float vargambrakemax = 150;
	/// <summary>
	/// car main rigidbody reference
	/// </summary>
	public Rigidbody vargamcarbody = null;

	private int varwheelscount = 0;
	private WheelCollider[] varwheels;

	private const float cnsspring = 2500;
	private const float cnsdamper = 200;
	private const float cnssuspension = 0.5f;

	void Start () {
		switch (vargamcartype) {
			case 0:
				//V1 ugly direction tweak because of bad model orientation, saves a TransformDirection call
				//left here for compatibility
				Vector3 varforward = -transform.up;
				GetComponent<ConstantForce>().force = varforward*3000;
				enabled = false;
				break;
			case 1:
				//V4 car manager
				if (vargamcarbody == null) {
					Debug.LogError("The car needs a rigidbody to function.");
					enabled = false;
				}
				varwheels = GetComponentsInChildren<WheelCollider>();
				varwheelscount = varwheels.Length;
				for (int varwheelcounter = 0; varwheelcounter < varwheelscount; varwheelcounter++) {
					JointSpring varspring = new JointSpring();
					varspring.spring = cnsspring;
					varspring.damper = cnsdamper;
					varspring.targetPosition = 0;
					varwheels[varwheelcounter].suspensionSpring = varspring;
					varwheels[varwheelcounter].suspensionDistance = cnssuspension;
					
					//full throttle
					varpower = 1;
					varsteering = 0;
					varbrake = 0;
				}
				vargamcarbody.centerOfMass = new Vector3(0, -0.05f, 0);
				varspeedmax = vargamspeedmax * vargamspeedmax;
				break;
			default:
				break;
		}
	}

	private float varpower, varsteering, varbrake, varspeedmax;
	void FixedUpdate() {
		if (vargamusercontrolled) {
			varpower = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);
			varsteering = Mathf.Clamp(Input.GetAxis("Horizontal"), -1, 1);
			varbrake = (-1 * Mathf.Clamp(Input.GetAxis("Vertical"), -1, 0));
		}
		//toggle pedal when maximum speed is reached
		if (vargamcarbody.velocity.sqrMagnitude > varspeedmax) {
			varpower = 0;
		}
		
		//wheels 2 and 3 are front wheels
		if (varwheels[2] != null) {
			varwheels[2].steerAngle = vargamsteermax * varsteering;
		}
		if (varwheels[3] != null) {
			varwheels[3].steerAngle = vargamsteermax * varsteering;
		}

		//wheels 0 and 1 are rear wheels
		if (varwheels[0] != null) {
			varwheels[0].motorTorque = vargammotormax * varpower;
			varwheels[0].brakeTorque = vargambrakemax * varbrake;
		}
		if (varwheels[1] != null) {
			varwheels[1].motorTorque = vargammotormax * varpower;
			varwheels[1].brakeTorque = vargambrakemax * varbrake;
		}
		
	}
}
