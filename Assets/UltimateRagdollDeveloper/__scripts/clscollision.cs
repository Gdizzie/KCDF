using UnityEngine;
using System.Collections;

/// <summary>
/// 2012-11-07
/// ULTIMATE RAGDOLL GENERATOR V4
/// © THE ARC GAMES STUDIO 2012
/// DESIGNED WITH UNITY 3.5.6f4
/// 
/// simple class to facilitate trigger and collision on demo scene objects
/// NOTE: this also manages jeep dismemberment
/// </summary>
public class clscollision : MonoBehaviour {
	/// <summary>
	/// Inspector slot of the passenger for car cases
	/// </summary>
	public Transform vargampassenger;
	/// <summary>
	/// used for scene performance benchmarking
	/// </summary>
	public bool vargamcandestroyjeep;
	
	private clsragdollify varlocalragdollifier; 
	private bool varengaged = false; //fake semaphore to avoid multiple triggers
	private clsdismemberator varD;
	private clscar varcar;
	private Transform[] varbones;
	
	void Start() {
		varD = transform.root.GetComponentInChildren<clsdismemberator>();
		varcar = transform.root.GetComponentInChildren<clscar>();
		if (varD != null) {
			varbones = varD.vargamskinnedmeshrenderer.bones;
		}
	}
	
	//used for cars
	void OnTriggerEnter() {
		if (!varengaged) {
			if (varD != null) {
				if (varcar != null) {
					if (vargamcandestroyjeep) {
						//determine the number of parts to break, based on our speed on trigger
						int varparts = varD.vargamboneindexes.Count;
						float varspeedratio = varcar.vargamcarbody.velocity.sqrMagnitude / (varcar.vargamspeedmax * varcar.vargamspeedmax);
						int varpartstobreak =  (int)(varparts * varspeedratio);
						int varbrokenparts = 0;
						for (int varbreakcounter = 0; varbreakcounter < varbones.Length; varbreakcounter++) {
							float varbreakchance = Random.Range(0, 0.99f);
							if (varbreakchance > (1- varspeedratio) && varbones[varbreakcounter].GetComponent<Collider>() != null) {
								clsurgutils.metdismember(varbones[varbreakcounter],null,varD);
								varbrokenparts++;
							} 
							if (varbrokenparts > varpartstobreak) {
								break;
							}
						}
					}
				}
			}
			if (vargampassenger != null) {
				varlocalragdollifier = vargampassenger.GetComponent<clsragdollify>();
				if (varlocalragdollifier != null) {
					varlocalragdollifier.metgoragdoll();
					Destroy(transform.root.GetComponent<ConstantForce>(),1);
					Destroy(vargampassenger.gameObject);
				}
			}
			//use up the object
			varengaged = true;
		}
	}
	
	/// <summary>
	/// remove physics from the host
	/// </summary>
	private void metdiscard() {
		Destroy(GetComponent<Rigidbody>());
		Destroy(GetComponent<Collider>());
	}
	
	//used for guardrail
	void OnCollisionEnter(Collision varpother) {
		if (!varengaged) {
			//we use this trick to basically destroy collision props physics after 6 seconds since playmode start
			varengaged = true;
			Invoke("metdiscard",5);
		}
	}
}
