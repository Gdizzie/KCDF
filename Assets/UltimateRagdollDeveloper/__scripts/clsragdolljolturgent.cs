using UnityEngine;
using System.Collections;

/// <summary>
/// 2012-05-26
/// ULTIMATE RAGDOLL GENERATOR V4
/// © THE ARC GAMES STUDIO 2012
/// DESIGNED WITH UNITY 3.5.6f4
/// 
/// This is an utility class that will use the urgent class to apply a force and rotation to body parts
/// thanks to the urgent class, it is possible to specify which limbs will receive the jolt
/// </summary>
public class clsragdolljolturgent : MonoBehaviour {
	public float vargamforcemin = -1f, vargamforcemax = 0.5f, vargamtorquemin = -200f, vargamtorquemax = 100f;
	public ForceMode vargamforcemode = ForceMode.VelocityChange;
	public bool vargamjoltspine = true, vargamjolthead = true, vargamjoltarmleft = true, vargamjoltarmright = true, vargamjoltlegleft = true, vargamjoltlegright = true;

	void Start () {
		metjolturgent(vargamforcemin, vargamforcemax, vargamtorquemin, vargamtorquemax, ForceMode.VelocityChange, vargamjoltspine, vargamjolthead, vargamjoltarmleft, vargamjoltarmright, vargamjoltlegleft, vargamjoltlegright);
	}
	
	private void metjolturgent(float varpforcemin, float varpforcemax, float varptorquemin, float varptorquemax, ForceMode varpforcemode, bool varpjoltspine, bool varpjolthead, bool varpjoltarmleft, bool varpjoltarmright, bool varpjoltlegright, bool varpjoltlegleft) {
		clsurgent varurgent = gameObject.GetComponentInChildren<clsurgent>();

		if (varurgent == null) {
			return;
		}
		
		Vector3 varrandomforce = new Vector3(Random.Range(varpforcemin, varpforcemax), Random.Range(varpforcemin, varpforcemax), Random.Range(varpforcemin, varpforcemax));
		Vector3 varrandomtorque = new Vector3(Random.Range(varptorquemin, varptorquemax), Random.Range(varptorquemin, varptorquemax), Random.Range(varptorquemin, varptorquemax));
		if (varpjoltspine) {
			for (int varpartcounter = 0; varpartcounter < varurgent.vargamnodes.vargamspine.Length; varpartcounter++) {
				varurgent.vargamnodes.vargamspine[varpartcounter].GetComponent<Rigidbody>().AddForce(varrandomforce, varpforcemode);
				varurgent.vargamnodes.vargamspine[varpartcounter].GetComponent<Rigidbody>().AddTorque(varrandomtorque, varpforcemode);
			}
		}
		if (varpjolthead) {
			for (int varpartcounter = 0; varpartcounter < varurgent.vargamnodes.vargamhead.Length; varpartcounter++) {
				varurgent.vargamnodes.vargamhead[varpartcounter].GetComponent<Rigidbody>().AddForce(varrandomforce, varpforcemode);
				varurgent.vargamnodes.vargamhead[varpartcounter].GetComponent<Rigidbody>().AddTorque(varrandomtorque, varpforcemode);
			}
		}
		if (varpjoltarmleft) {
			for (int varpartcounter = 0; varpartcounter < varurgent.vargamnodes.vargamarmleft.Length; varpartcounter++) {
				varurgent.vargamnodes.vargamarmleft[varpartcounter].GetComponent<Rigidbody>().AddForce(varrandomforce, varpforcemode);
				varurgent.vargamnodes.vargamarmleft[varpartcounter].GetComponent<Rigidbody>().AddTorque(varrandomtorque, varpforcemode);
			}
		}
		if (varpjoltarmright) {
			for (int varpartcounter = 0; varpartcounter < varurgent.vargamnodes.vargamarmright.Length; varpartcounter++) {
				varurgent.vargamnodes.vargamarmright[varpartcounter].GetComponent<Rigidbody>().AddForce(varrandomforce, varpforcemode);
				varurgent.vargamnodes.vargamarmright[varpartcounter].GetComponent<Rigidbody>().AddTorque(varrandomtorque, varpforcemode);
			}
		}
		if (varpjoltlegleft) {
			for (int varpartcounter = 0; varpartcounter < varurgent.vargamnodes.vargamlegleft.Length; varpartcounter++) {
				varurgent.vargamnodes.vargamlegleft[varpartcounter].GetComponent<Rigidbody>().AddForce(varrandomforce, varpforcemode);
				varurgent.vargamnodes.vargamlegleft[varpartcounter].GetComponent<Rigidbody>().AddTorque(varrandomtorque, varpforcemode);
			}
		}
		if (varpjoltlegright) {
			for (int varpartcounter = 0; varpartcounter < varurgent.vargamnodes.vargamlegright.Length; varpartcounter++) {
				varurgent.vargamnodes.vargamlegright[varpartcounter].GetComponent<Rigidbody>().AddForce(varrandomforce, varpforcemode);
				varurgent.vargamnodes.vargamlegright[varpartcounter].GetComponent<Rigidbody>().AddTorque(varrandomtorque, varpforcemode);
			}
		}
	}
}
