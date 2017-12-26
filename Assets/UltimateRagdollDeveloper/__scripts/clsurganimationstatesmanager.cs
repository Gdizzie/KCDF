using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 2012-06-26
/// ULTIMATE RAGDOLL GENERATOR V4.0
/// © THE ARC GAMES STUDIO 2012
/// DESIGNED WITH UNITY 3.5.6f4
/// 
/// ANIMATION STATES MANAGER CLASS
/// 
/// This is an utility class that holds the data to manage animation states. it's based on two separate lists because dictionaries arent' serialized by the engine.
/// The class is compiled by the Animation States Manager editor
public class clsurganimationstatesmanager : MonoBehaviour {
	/// <summary>
	/// root name of the target character. it's normally the name of the parent transform of the skinned mesh renderer
	/// </summary>
	public string vargamrootname;
	[HideInInspector]
	/// <summary>
	/// original position of the ragdoll root. very important to sync the ragdoll and character after a transition
	/// </summary>
	public Vector3 vargamrootoriginallocalposition;
	[HideInInspector]
	/// <summary>
	/// this list will hold the memorized state names
	/// </summary>
	public List<string> vargamstatenames;
	[HideInInspector]
	/// <summary>
	/// this list will hold the parts of the memorized state names
	/// </summary>
	public List<clsanimationstatesnapshot> vargamanimationstates;
	
	/// <summary>
	/// remove an animation state
	/// </summary>
	/// <param name='varpname'>
	/// animation state to remove
	/// </param>
	public void metremove(string varpname) {
		int varindex;
		varindex = vargamstatenames.IndexOf(varpname);
		if (varindex > -1) {
			vargamstatenames.RemoveAt(varindex);
			vargamanimationstates.RemoveAt(varindex);
		}
	}
	
	/// <summary>
	/// add an animation state (removes the same index automatically)
	/// </summary>
	/// <param name='varpname'>
	/// name of the animation
	/// </param>
	/// <param name='varpsnapshot'>
	/// snapshot of the animation state
	/// </param>
	public void metadd(string varpname, clsanimationstatesnapshot varpsnapshot) {
		metremove(varpname);
		vargamstatenames.Add(varpname);
		vargamanimationstates.Add(varpsnapshot);
	}
}

[System.Serializable]
public class clsanimationstatesnapshot {
	public clsurganimationstatespart[] propanimationstate;
}

[System.Serializable]
public class clsurganimationstatespart {
	public string proppath;
	public Vector3 propposition;
	public Quaternion proprotation;
}