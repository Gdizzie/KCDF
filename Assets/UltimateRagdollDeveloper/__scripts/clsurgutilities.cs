using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// REVISION 2012-11-27
/// RAGDOLLIFIER UTILITIES V4.0a
/// © THE ARC GAMES STUDIO 2012
/// DESIGNED WITH UNITY 3.5.6f4
/// 
/// This is an advanced utility class which hosts all global URG! methods
/// </summary>
public static class clsurgutils {
	
	/// <summary>
	/// Placeholder
	/// </summary>
	public static void metsummaryplaceholder() {
		//does nothing. setup here to work around the summary bug of the first function of the script.
	}

	/// <summary>
	/// crossfade an animated character from the current pose (might be animation or ragdoll) to the first frame of the indicated destination animation
	/// </summary>
	/// <param name="varpcharacter">
	/// the animated character's transform. this should be the transform that hosts the 'Skinned Mesh Renderer component'.
	/// </param>
	/// <param name="varpdestinationanimname">
	/// the actual string name of the animation, that must exist in the character's animation list, to transit to.
	/// </param>
	/// <param name="varptransitiontime">
	/// time in seconds for the transition animation to last
	/// </param>
	/// <param name="varpcontroller">
	/// OPTIONAL default null (WILL BE SEARCHED IF PASSED AS NULL)
	/// the root of the character will relocate to this parameter's position before a transition takes place
	/// IMPORTANT PARAMETER (IF NOT FOUND, CAN LEAD TO ARTIFACTS IN RAGDOLL TRANSITION)
	/// this is the 'root transform' of the ragdoll, the topmost transform of the 3d model, which normally hosts the character controller.
	/// </param>
	/// <param name="varpstateanimationname">
	/// OPTIONAL default "" (NO EFFECT IF "")
	/// transition animations clamp forever, so if they're used to change state (ragdoll=> 'get_up'=> 'stand'), user must manually manage timing of the state ('stand') Play or Crossfade  animation.
	/// specifying an animation name with this parameter, the transition animation will play from ragdoll, to transition, to state animation automatically.
	/// </param>
	/// <param name="varpgokinematic">
	/// OPTIONAL default true
	/// tells the routine if it needs to set kinematic rigidbodies or not (can be passed FALSE if the ragdoll is made kinematic before the call)
	/// </param>
	/// <param name="varpnewanimname">
	/// OPTIONAL default "transition"
	/// the name of the transition animation that will be created. passing the name of an existing animation, will erase that animation before creating a new transition
	/// </param>
	/// <param name="varpanimationsystem">
	/// OPTIONAL default null (WILL BE SEARCHED IF PASSED AS NULL)
	/// IMPORTANT PARAMETER (IF NOT FOUND, TRANSITION IS IMPOSSIBLE)
	/// the Animation component of the character. can be passed for performance reasons (the routine is otherwise forced to make a GetComponent call).
	/// </param>
	/// <param name="varprenderer">
	/// OPTIONAL default null (WILL BE SEARCHED IF PASSED AS NULL)
	/// IMPORTANT PARAMETER (IF NOT FOUND, TRANSITION IS IMPOSSIBLE)
	/// the SkinnedMeshRenderer component of the character. can be passed for performance reasons (the routine is otherwise forced to make a GetComponent call).
	/// </param>
	/// <param name="varpstatesmanager">
	/// OPTIONAL default null (WILL BE SEARCHED IF PASSED AS NULL)
	/// IMPORTANT PARAMETER (IF NOT FOUND, TRANSITION IS IMPOSSIBLE)
	/// the fundamental piece of the animation states management
	/// </param>
	public static int metcrossfadetransitionanimation(Transform varpcharacter, string varpdestinationanimname, float varptransitiontime, Transform varpcontroller = null, string varpstateanimationname = "", bool varpgokinematic = true, string varpnewanimname = "transition", Animation varpanimationsystem = null, SkinnedMeshRenderer varprenderer = null, clsurganimationstatesmanager varpstatesmanager = null) {
		//check the character
		if (varpcharacter == null) {
			return -1;
		}
		//don't even try if transition time is zero
		if (varptransitiontime == 0) {
			return -2;
		}
		if (varpanimationsystem == null) {
			varpanimationsystem = varpcharacter.root.GetComponentInChildren<Animation>();
			if (varpanimationsystem == null) {
				return -3;
			}
			//if we pass this as null, we assume it's not been referenced and thus stopped in the calling environment
			//all animations must stop playing for the ragdoll to take over
			varpanimationsystem.Stop();
		}
		if (varprenderer == null) {
			varprenderer = varpcharacter.root.GetComponentInChildren<SkinnedMeshRenderer>();
			if (varprenderer == null) {
				return -4;
			}
		}
		varpanimationsystem.animatePhysics = true;
		if (varpgokinematic) {
			//ensure that the character is kinematic
			clsurgutils.metgodriven(varpcharacter, true);
		}
		//setup the root normalizer variable (for possible future use)
		Vector3 varrootnormalizer = new Vector3();
		//normalize the character parent and the root position if the option is set. this is necessary to avoid 'jumping' between the transition pose and the actual destination animation (usually the ragdoll translates from the destination animation root pose)
		if (varpcontroller != null) {
			//define the vector with the current ragdoll position
			Vector3 varcharacterpairer = varpcharacter.position;
			varpcontroller.position = varcharacterpairer;
			//we assume that the original root position is always zero
			varpcharacter.localPosition = varrootnormalizer;
		}
		//get the transition clip
		AnimationClip varnewclip = metcreatetransitionanimation(varpcharacter, varpdestinationanimname, varptransitiontime, varpnewanimname, ref varrootnormalizer, varpstatesmanager);
		if (varnewclip != null) {
			//stop any currently playing animation
			varpanimationsystem.Stop();
			//remove the transition clip if it exists already
			AnimationState varoldclip = varpanimationsystem[varpdestinationanimname];
			if (varoldclip.name != varpdestinationanimname) {
				varpanimationsystem.RemoveClip(varpnewanimname);
			}
			//add the clip
			varpanimationsystem.AddClip(varnewclip, varpnewanimname);
			//determine the transition play mode
			if (varpstateanimationname != "") {
				//we have an end state animation so we chain it
				varpanimationsystem[varpnewanimname].wrapMode = WrapMode.Once;
				varpanimationsystem.CrossFade(varpnewanimname);
				varpanimationsystem.CrossFadeQueued(varpstateanimationname);
			}
			else {
				//just clamp the transition animation
				varpanimationsystem.CrossFade(varpnewanimname);
			}
			return 1;
		}
		else {
			Debug.LogError("Could not create transition");
		}
		return -5;
	}
	
	/// <summary>
	/// this function creates a clip to transit from ragdoll to animation. the method relies on a properly compiled animation states manager class.
	/// NOTE: the animation is normally not reusable, since it stores the 'instantaneous' ragdoll pose, which is certainly different from subsequent ragdoll poses, so this function should be called as part of an animation creation process, for example with metcrossfadetransitionanimation
	/// </summary>
	/// <param name="varpsource">
	/// the source for the animation state synchronization. this should be the transform which name is specified in the animation state manager's vargamrootname variable, since it's needed for the correct transformation array transposition.
	/// NOTE: this object is generally the parent of the skinned mesh renderer host of the model, so it will automatically be set to that transform with the use of the animation states memorizer tool
	/// </param>
	/// <param name="varptargetanimname">
	/// the name of the target animation. basically the ragdoll will transition to the animation state with the name passed in.
	/// </param>
	/// <param name="varptransitiontime">
	/// the time in seconds that the transition animation will last
	/// </param>
	/// <param name="varpnewanimationname">
	/// the name that the new animation will be given
	/// </param>
	/// <param name="varprootnormalizer">
	/// will return the skinned mesh renderer's root bone original local position, to properly reset the character location (in ragdoll mode, the skm 'detachs' from the character)
	/// </param>
	/// <param name="varpanimationstatemanager">
	/// OPTIONAL: allows passage of the animation state manager to avoid a getcomponent call to the source root, for performance reasons
	/// </param>
	/// <returns>
	/// returns the new animation clip that was just created in case of success. null otherwise.
	/// </returns>
	public static AnimationClip metcreatetransitionanimation(Transform varpsource, string varptargetanimname, float varptransitiontime, string varpnewanimationname, ref Vector3 varprootnormalizer, clsurganimationstatesmanager varpanimationstatemanager = null) {
		//check if we passed the animation manager, otherwise fetch it from the root
		if (varpanimationstatemanager == null) {
			varpanimationstatemanager  = varpsource.root.GetComponentInChildren<clsurganimationstatesmanager>();
			if (varpanimationstatemanager == null ){
				Debug.Log("No animation states manager in the source");
				return null;
			}
		}
		//determine if the specified animation is memorized in the manager
		int varanimstateindex = varpanimationstatemanager.vargamstatenames.IndexOf(varptargetanimname);
		if (varanimstateindex < 0) {
			Debug.LogError("Animation state not memorized in manager");
			return null;
		}
		//check if the manager root name is the same as the source name. this is not a critical issue if the bones actually correspond, however it's an anomaly
		if (varpanimationstatemanager.vargamrootname != varpsource.name) {
			Debug.LogWarning("The animation states root is different than the source passed in. Unexpected behavior may occur [passed " + varpsource.name + " expected " + varpanimationstatemanager.vargamrootname + "]");
		}
		//declare temp variables
		AnimationClip vartransitionanimation = new AnimationClip();
		//Vector3 varsourcepos,vardestinationpos;
		Quaternion varsourcerot,vardestinationrot;
		
		//***position setup tends to alter animation stability. disabled in version 2 of the utility
		/*
		AnimationCurve varpositionxcurve = new AnimationCurve();
		AnimationCurve varpositionycurve = new AnimationCurve();
		AnimationCurve varpositionzcurve = new AnimationCurve();
		*/
		AnimationCurve varrotationxcurve = new AnimationCurve();
		AnimationCurve varrotationycurve = new AnimationCurve();
		AnimationCurve varrotationzcurve = new AnimationCurve();
		AnimationCurve varrotationwcurve = new AnimationCurve();
		//name the new clip
		vartransitionanimation.name = varpnewanimationname;
		//fetch the root normalizer
		varprootnormalizer = varpanimationstatemanager.vargamrootoriginallocalposition;

		//an extremely important step: get the source's parts to scan them and add animation curves. the source is normally the parent of the object that hosts the skinned mesh renderer
		Transform[] varsourceelements = varpsource.GetComponentsInChildren<Transform>();
		if (varsourceelements.Length != varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate.Length) {
			Debug.LogError("Source and state body parts length missmatch. Can't continue [" + varsourceelements.Length + " - " + varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate.Length +"]");
			return null;
		}
		for (int varcounter = 0; varcounter < varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate.Length; varcounter++) {
			//element 0 should be the skinned mesh renderer transform, which usually displaces from the target animation original root position
			if (varcounter == 0) {
				Vector3 varsourcepos,vardestinationpos;
				AnimationCurve varpositionxcurve = new AnimationCurve();
				AnimationCurve varpositionycurve = new AnimationCurve();
				AnimationCurve varpositionzcurve = new AnimationCurve();
				varsourcepos = varsourceelements[varcounter].localPosition;
				vardestinationpos = varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].propposition;
				if (varsourcepos != vardestinationpos) {
				    varpositionxcurve = AnimationCurve.Linear(0, varsourcepos.x, varptransitiontime, vardestinationpos.x);
				    varpositionycurve = AnimationCurve.Linear(0, varsourcepos.y, varptransitiontime, vardestinationpos.y);
				    varpositionzcurve = AnimationCurve.Linear(0, varsourcepos.z, varptransitiontime, vardestinationpos.z);
				
				    vartransitionanimation.SetCurve(varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].proppath, typeof(Transform), "localPosition.x", varpositionxcurve);
				    vartransitionanimation.SetCurve(varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].proppath, typeof(Transform), "localPosition.y", varpositionycurve);
				    vartransitionanimation.SetCurve(varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].proppath, typeof(Transform), "localPosition.z", varpositionzcurve);
				}
			}
			/*
			//***position setup tends to alter animation stability. disabled in version 2 of the utility
			//get the ragdoll and the animation position for the current part
			varsourcepos = varsourceelements[varcounter].localPosition;
			vardestinationpos = varanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].propposition;
			if (varsourcepos != vardestinationpos) {
		        varpositionxcurve = AnimationCurve.EaseInOut(0, varsourcepos.x, varptransitiontime, vardestinationpos.x);
		        varpositionycurve = AnimationCurve.EaseInOut(0, varsourcepos.y, varptransitiontime, vardestinationpos.y);
		        varpositionzcurve = AnimationCurve.EaseInOut(0, varsourcepos.z, varptransitiontime, vardestinationpos.z);

		        vartransitionanimation.SetCurve(varanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].proppath, typeof(Transform), "localPosition.x", varpositionxcurve);
		        vartransitionanimation.SetCurve(varanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].proppath, typeof(Transform), "localPosition.y", varpositionycurve);
		        vartransitionanimation.SetCurve(varanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].proppath, typeof(Transform), "localPosition.z", varpositionzcurve);
			}
			*/
			//get the ragdoll and the animation rotation for the current part
			varsourcerot = varsourceelements[varcounter].localRotation;
			vardestinationrot = varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].proprotation;
			//assign the new curves only if there's actual transformation of the part
			if (varsourcerot != vardestinationrot) {
		        varrotationxcurve = AnimationCurve.Linear(0, varsourcerot.x, varptransitiontime, vardestinationrot.x);
		        varrotationycurve = AnimationCurve.Linear(0, varsourcerot.y, varptransitiontime, vardestinationrot.y);
		        varrotationzcurve = AnimationCurve.Linear(0, varsourcerot.z, varptransitiontime, vardestinationrot.z);
		        varrotationwcurve = AnimationCurve.Linear(0, varsourcerot.w, varptransitiontime, vardestinationrot.w);

				vartransitionanimation.SetCurve(varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].proppath, typeof(Transform), "localRotation.x", varrotationxcurve);
		        vartransitionanimation.SetCurve(varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].proppath, typeof(Transform), "localRotation.y", varrotationycurve);
		        vartransitionanimation.SetCurve(varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].proppath, typeof(Transform), "localRotation.z", varrotationzcurve);
		        vartransitionanimation.SetCurve(varpanimationstatemanager.vargamanimationstates[varanimstateindex].propanimationstate[varcounter].proppath, typeof(Transform), "localRotation.w", varrotationwcurve);
			}
		}
		//normalize rotations
		vartransitionanimation.EnsureQuaternionContinuity();
		//setup standard wrap mode
		vartransitionanimation.wrapMode = WrapMode.ClampForever;
		//return the new animation
		return vartransitionanimation;
	}
	
	/// <summary>
	/// This procedure's objective is to set varpsource's joint connected body property, assigning
	/// varptarget's rigidbody to it, and eventually adding or modifying preexisting such elements
	/// </summary>
	/// <param name="varpsource">
	/// The source object, which hosts the joint component that will get varptargets' rigidbody reference
	/// </param>
	/// <param name="varptarget">
	/// The object that will effectively become the physical parent of varpsource' joint
	/// </param>
	/// <param name="varpreplace">
	/// If true, will add the required components if they don't exist, or replace existing components values
	/// if the component exist. If false and the components exist, will not replace existing component values
	/// </param>
	/// <returns>
	/// 0 if one or both of the bodies were null.
	/// 1 if procedure completed successfully.
	/// </returns>
	public static int metconnectbodies(Transform varpsource, Transform varptarget, bool varpreplace) {
		int varreturnvalue = 0;
		Joint varcurrentjoint;
		Rigidbody varcurrentbody;
		//check if the bodies are null since we can't proceed in such case
		if (varpsource != null && varptarget != null) {
			//fetch the components
			varcurrentjoint = varpsource.gameObject.GetComponent<Joint>();
			varcurrentbody = varptarget.gameObject.GetComponent<Rigidbody>();
			//check if the retrieved joint is null in which case we add it to the source
			if (varcurrentjoint == null) {
				varcurrentjoint = varpsource.gameObject.AddComponent<CharacterJoint>();
			}
			//check if the retrieved rigidbody is null in which case we add it to the target
			if (varcurrentbody == null) {
				varcurrentbody = varptarget.gameObject.AddComponent<Rigidbody>();
			}
			//check if we can replace, or if we can't replace but there's no connected body in our joint, or if the joint is already connected to the body
			if (varpreplace == true || (varpreplace == false && ((varcurrentjoint.connectedBody == null || varcurrentjoint.connectedBody == varcurrentbody)))) {
				//we perform the replacement only if actually different
				if (varcurrentjoint.connectedBody != varcurrentbody)
					varcurrentjoint.connectedBody = varcurrentbody;
				//succesful operation, set up here as functionality expansion placeholder
				varreturnvalue = 1;
			}
		}
		//return the current result
		return varreturnvalue;
	}
	
	/// <summary>
	/// Static instance of the clskinetify utility, that turns a kinematic ragdoll into a driven ragdoll.
	/// This procedure is particularly useful to animate 'roadside' objects or scenery objects that need to become physical when they
	/// receive a trigger (or otherwise a collision when implemented like so) using this class can save time and resources in those
	/// cases, since it avoids the use of a specific ragdoll prefab and a separate scripted gameobject
	/// NOTE: can be used to turn a driven ragdoll into a kinematic ragdoll, too, by passing the secondary optional parameter as true
	/// </summary>
	/// <param name="varpsource">
	/// the gameobject that will be used as root. NOTE: this object's rigidbody will not be included in the iteration.
	/// </param>
	/// <param name="varpkinematic">
	/// OPTIONAL: default false
	/// Determines the 'iskinematic' setting of the rigidbodies of the source
	/// </param>
	public static void metgodriven(Transform varpsource, bool varpkinematic = false) {
		//retrieve all the rigidbodies of the current gameobject
		Rigidbody[] varrigidbodies;
		varrigidbodies = varpsource.GetComponentsInChildren<Rigidbody>();
		//cycle the rigidbodies and turn them into physics driven objects
		foreach (Rigidbody varcurrentrigidbody in varrigidbodies) {
			//necessary to awake from sleep mode, to work around unity 3.5 optimizations
			varcurrentrigidbody.WakeUp();
			varcurrentrigidbody.isKinematic = varpkinematic;
		}
	}
	
	/// <summary>
	/// URG entities alternative to metgodriven. Will scan the urgent array and ragdollify all children of the parameter part.
	/// Accessing this procedure is convenient in terms of efficiency and cpu since it's 'cheaper' than getcomponentsinchildren, when
	/// the source is based on the actual URGent armature
	/// NOTE: can be used to URG drive a clsurgent gameobject, when the actuator is passed as null
	/// </summary>
	/// <param name="varpsource">
	/// The URG entities manager which holds the armature structure data for the actuator
	/// </param>
	/// <param name="varpactuator">
	/// OPTIONAL default null the actuator that will become driven, followed by its children. If this parameter is null, all the ragdoll will be URG driven.
	/// </param>
	public static void metdriveurgent(clsurgent varpsource, clsurgentactuator varpactuator = null) {
		if (varpsource == null) {
			Debug.LogError("Received a null parameter: " + varpsource + " - " + varpactuator);
			return;
		}
		//check if the actuator is null, of ir it's the source, in which case drive all body parts
		if (varpactuator == null || varpactuator.transform == varpsource.vargamnodes.vargamspine[0]) {
			metdrivebodypart(varpsource,enumparttypes.head,0);
			metdrivebodypart(varpsource,enumparttypes.arm_left,0);
			metdrivebodypart(varpsource,enumparttypes.arm_right,0);
			metdrivebodypart(varpsource,enumparttypes.leg_left,0);
			metdrivebodypart(varpsource,enumparttypes.leg_right,0);
			metdrivebodypart(varpsource,enumparttypes.spine,0);
		}
		//otherwise just call the drive procedure on the specified body part and index
		else {
			metdrivebodypart(varpsource,varpactuator.vargamparttype,varpactuator.vargampartindex);
		}
	}

	/// <summary>
	/// Auxilliary function to drive an urgent object, can be used to directly ragdollimbify a body part without the need
	/// of a collision or trigger effect
	/// NOTE: most likely the animation component will require the 'Animathe physics' check enabled
	/// </summary>
	/// <param name="varpsource">
	/// The URG entities manager which holds the armature structure data for the limb
	/// </param>
	/// <param name="varppart">
	/// The body part type
	/// </param>
	/// <param name="varppartindex">
	/// The source part index, which will be driven followed with its children
	/// </param>
	public static void metdrivebodypart(clsurgent varpsource, clsurgutils.enumparttypes varppart, int varppartindex) {
		//determine the body part
		if (varpsource != null) {
			Transform[] varcurrentbodypart = new Transform[0];
			switch (varppart) {
				case enumparttypes.spine:
					varcurrentbodypart = varpsource.vargamnodes.vargamspine;
					break;
				case enumparttypes.head:
					varcurrentbodypart = varpsource.vargamnodes.vargamhead;
					break;
				case enumparttypes.arm_left:
					varcurrentbodypart = varpsource.vargamnodes.vargamarmleft;
					break;
				case enumparttypes.arm_right:
					varcurrentbodypart = varpsource.vargamnodes.vargamarmright;
					break;
				case enumparttypes.leg_left:
					varcurrentbodypart = varpsource.vargamnodes.vargamlegleft;
					break;
				case enumparttypes.leg_right:
					varcurrentbodypart = varpsource.vargamnodes.vargamlegright;
					break;
				default:
					Debug.LogError("Unmanaged part type");
					break;
			}
			//cycle 'outwards' in the parts list, and set the part as physics driven
			for (int varcounter = varppartindex; varcounter < varcurrentbodypart.Length; varcounter++) {
				if (varcurrentbodypart[varcounter] != null) {
					if (varcurrentbodypart[varcounter].GetComponent<Rigidbody>() != null) {
						varcurrentbodypart[varcounter].GetComponent<Rigidbody>().isKinematic = false;
					}
				}
			}
		}
		else {
			Debug.LogError("Received a request to URG drive a null source");
		}
	}
	
	/// <summary>
	/// Basic function to determine a body part's path in respect to its source.
	/// NOTE: the path does NOT include the root.
	/// </summary>
	/// <param name="varpsource">
	/// The source transform
	/// </param>
	/// <returns>
	/// The string path that leads from varpsource's root to its transform
	/// </returns>
	public static string metbuildpartpath(Transform varpsource) {
		Transform varcurrentpart = varpsource;
		string varreturnvalue = "";
		string varseparator = "";
		while (varcurrentpart != null && (varcurrentpart.parent != null)) {
			varreturnvalue = varcurrentpart.name + varseparator + varreturnvalue;
			varcurrentpart = varcurrentpart.parent;
			if (varseparator == "")
				varseparator = "/";
				
		}
		return varreturnvalue;
	}
	
	/// <summary>
	/// The global enumerator that defines body parts
	/// </summary>
	public enum enumparttypes {
		head,
		spine,
		arm_left,
		arm_right,
		leg_left,
		leg_right,
	}
	
	/// <summary>
	/// determines if a segment intersects a plane
	/// </summary>
	/// <param name='varpplane'>
	/// the plane object to test against
	/// </param>
	/// <param name='varpplaneposition'>
	/// the plane position in space
	/// </param>
	/// <param name='varpsegmentpoint1'>
	/// the starting point of the segment
	/// </param>
	/// <param name='varpsegmentpoint2'>
	/// endpoin of the segment
	/// </param>
	/// <param name='varpintersection'>
	/// return value of the intersection, by reference
	/// </param>
	/// <returns>
	/// true if plane and segment intersect. value stored in varpintersection
	/// </returns>
	private static bool metsetsegmentplaneintersection(Plane varpplane, Vector3 varpplaneposition, Vector3 varpsegmentpoint1, Vector3 varpsegmentpoint2, ref Vector3 varpintersection) {
		float vardistance1, vardistance2, varintersectionfactor;
		
		vardistance1 = varpplane.GetDistanceToPoint(varpsegmentpoint1);
		vardistance2 = varpplane.GetDistanceToPoint(varpsegmentpoint2);

		if (vardistance1 * vardistance2 > 0) {
			return false;
		}

		Vector3 vardirection = varpsegmentpoint2 - varpsegmentpoint1;
		varintersectionfactor = Vector3.Dot(varpplane.normal, (varpplaneposition - varpsegmentpoint1)) / Vector3.Dot(varpplane.normal, (vardirection));
		varpintersection = varpsegmentpoint1 + (varintersectionfactor * vardirection);

		return true;
	}
	
	/// <summary>
	/// calculates the abc angle from the ab, cb segments
	/// </summary>
	/// <param name='varpa'>
	/// point a
	/// </param>
	/// <param name='varpb'>
	/// point b
	/// </param>
	/// <param name='varpc'>
	/// point c
	/// </param>
	/// <returns>
	/// the angle in degrees
	/// </returns>
	private static float metthreepointangle(Vector3 varpa, Vector3 varpb, Vector3 varpc) {
	    Vector2 ab;
		ab.x = varpb.x - varpa.x;
		ab.y = varpb.y - varpa.y;
	    Vector2 cb;
		cb.x = varpb.x - varpc.x;
		cb.y = varpb.y - varpc.y;
		float angba = Mathf.Atan2(ab.y, ab.x);
		float angbc = Mathf.Atan2(cb.y, cb.x);
		float rslt = angba - angbc;
		float rs = (rslt * 180) / Mathf.PI;
		
		//return Mathf.Abs(rs)
		return (rs > 0 ? 360 - rs : -rs);
	}
		
	/// <summary>
	/// creates a 3d sphere of a chosen color in space. useful for debug purposes.
	/// </summary>
	/// <param name='varpradius'>
	/// radius of the sphere
	/// </param>
	/// <param name='varpcolor'>
	/// color of the spere
	/// </param>
	/// <param name='varpposition'>
	/// world position of the sphere
	/// </param>
	/// <param name='varpname'>
	/// OPTIONAL default Placeholder name of the resulting gameobject
	/// </param>
	/// <param name='varpparent'>
	/// OPTIONAL default null transform of the object the sphere will be parented to
	/// </param>
	public static void metcreateplaceholder(float varpradius, Color varpcolor, Vector3 varpposition, string varpname = "Placeholder", Transform varpparent = null) {
		GameObject varplaceholder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		varplaceholder.name = varpname;
		varplaceholder.transform.position = varpposition;
		varplaceholder.transform.localScale = Vector3.one * varpradius;
		Material varmaterial = new Material(Shader.Find("Diffuse"));
		varmaterial.color = varpcolor;
		varplaceholder.GetComponent<Renderer>().material = varmaterial;
		if (varpparent != null)
			varplaceholder.transform.parent = varpparent;
	}
	
	/// <summary>
	/// creates a 3d mesh triangle of the specified color. useful for debug purposes
	/// </summary>
	/// <param name='varpcolor'>
	/// colod of the resulting mesh
	/// </param>
	/// <param name='varpp1'>
	/// world point 1 of the triangle
	/// </param>
	/// <param name='varpp2'>
	/// world point 2 of the triangle
	/// </param>
	/// <param name='varpp3'>
	/// world point 3 of the triangle
	/// </param>
	/// <param name='varpname'>
	/// optional name of the resulting gameobject
	/// </param>
	/// <param name='varpdouble'>
	/// OPTIONAL default true creates a double sided triangle
	/// </param>
	/// <param name='varpparent'>
	/// OPTIONAL default null parents the created gameobject to this transform
	/// </param>
	public static void metcreatetriangle(Color varpcolor, Vector3 varpp1, Vector3 varpp2, Vector3 varpp3, string varpname = "Placeholder", bool varpdouble = true, Transform varpparent = null) {
		GameObject varnewtriangle = new GameObject(varpname);
		MeshFilter varmeshfilter = varnewtriangle.AddComponent<MeshFilter>();
		varnewtriangle.AddComponent<MeshRenderer>();
		Mesh mesh = varmeshfilter.mesh;
		mesh.Clear();
		mesh.vertices = new Vector3[] { varpp1, varpp2, varpp3};
		mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};
		mesh.normals = new Vector3[] {Vector3.up, Vector3.up, Vector3.up};
		if (varpdouble) {
			mesh.triangles = new int[] {0, 1, 2, 0 ,2 ,1};
		}
		else {
			mesh.triangles = new int[] {0, 1, 2};//, 0 ,2 ,1};
		}
		varnewtriangle.GetComponent<Renderer>().material = new Material(Shader.Find("Self-Illumin/Diffuse"));
		varnewtriangle.GetComponent<Renderer>().material.color = varpcolor;
		if (varpparent != null)
			varnewtriangle.transform.parent = varpparent;
	}
	
	/// <summary>
	/// creates a 3d mesh plane gameobject and returns it. useful for debug purposes.
	/// </summary>
	/// <param name='varpname'>
	/// name of the resulting gameobject
	/// </param>
	/// <param name='varpwidth'>
	/// width of the new plane
	/// </param>
	/// <param name='varpheight'>
	/// height of the new plane
	/// </param>
	public static GameObject metcreateplane(string varpname, float varpwidth = 1, float varpheight = 1) {
		GameObject varreturnvalue = new GameObject(varpname);
		MeshFilter varmeshfilter = varreturnvalue.AddComponent<MeshFilter>();
		varreturnvalue.AddComponent<MeshRenderer>();
		Mesh varmesh = varmeshfilter.mesh;
		varmesh.Clear();
		float varpside1 = varpwidth /2, varpside2 = varpheight /2;
		varmesh.vertices = new Vector3[] { new Vector3(varpside1,varpside2,0), new Vector3(varpside1,-varpside2,0), new Vector3(-varpside1,-varpside2,0), new Vector3(-varpside1,varpside2,0)};
		varmesh.uv = new Vector2[] {new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1)};
		varmesh.normals = new Vector3[] {new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0)};
		varmesh.triangles = new int[] {2, 1, 0, 0, 3, 2};
		varreturnvalue.GetComponent<Renderer>().material = new Material(Shader.Find("Diffuse"));
		varreturnvalue.GetComponent<Renderer>().material.color = Color.red;
		return varreturnvalue;
	}
	
	/// <summary>
	/// separates a body part of a clsdismemberator compiled gameobject
	/// </summary>
	/// <param name="varpbonetodetach">
	/// the body part to separate. this object and all its children will be disconnected from the original gameobject.
	/// </param>
	/// <param name="varpstumpmaterial">
	/// if not null, will cause triangulation of the separation surface, in correspondence of the extremities of the varpbonetodetach
	/// </param>
	/// <param name="varpdismemberator">
	/// OPTIONAL: dismemberator host of varpbonetodetach, saves a GetComponent call when passed
	/// </param>
	/// <param name="varpparentparticle">
	/// OPTIONAL: gameobject that will be spawned at the varpbonetodetach position in the original host
	/// </param>
	/// <param name="varpchildparticle">
	/// OPTIONAL: gameobject that will be spawned at the varpbonetodetach position in the new host
	/// </param>
	/// <param name="varpcinematiccut">
	/// OPTIONAL: if true, will fetch all rigidbodies of the child and transform them into physics driven (iskinematic = off)
	/// </param>
	public static bool metdismember(Transform varpbonetodetach, Material varpstumpmaterial, clsdismemberator varpdismemberator = null, GameObject varpparentparticle = null, GameObject varpchildparticle = null, bool varpcinematiccut = true) {
		if (varpbonetodetach == null) {
			Debug.LogError("Null body part. Aborting.");
			return false;
		}
		if (varpdismemberator == null) {
			varpdismemberator = varpbonetodetach.root.GetComponentInChildren<clsdismemberator>();
		}
		if (varpdismemberator == null) {
			Debug.LogError("Null DM. Aborting.");
			return false;
		}

		int varbonecounter = varpdismemberator.vargamboneindexes.IndexOf(varpbonetodetach);
		if (varbonecounter < 0) {
			Debug.LogError("Bone not indexed. Aborting.");
			return false;
		}
		int varparentboneindex = varpdismemberator.vargamboneindexes.IndexOf(varpbonetodetach.parent);
		if (varparentboneindex < 0) {
			//Debug.LogWarning("Can't cut a root bone.");
			return false;
		}
		
		bool varcreatestump = false;
		if (varpstumpmaterial != null) {
			varcreatestump = true;
		}
		
		varpdismemberator.vargamparallelcutcounter++;
		
		GameObject varparentstump = new GameObject(varpbonetodetach.name + "_stump");
		GameObject varchildstump = null;
		Transform varparentstumptransform = varparentstump.transform;
		Transform varchildstumptransform = null;
		SkinnedMeshRenderer varoriginalrenderer = varpdismemberator.vargamskinnedmeshrenderer;
		SkinnedMeshRenderer varnewrenderer = null;
		Mesh varoriginalmesh = varoriginalrenderer.sharedMesh;
		Transform[] varoriginalbones = varoriginalrenderer.bones;
		Mesh varnewparentmesh = new Mesh();
		Mesh varnewchildmesh = null;
		Vector3[] varoriginalvertices = varoriginalmesh.vertices;
		Vector3[] varoriginalnormals = varoriginalmesh.normals;
		Vector2[] varoriginaluvs = varoriginalmesh.uv;
		BoneWeight[] varoriginalboneweights = varoriginalmesh.boneWeights;
		int[] varoriginaltriangles = varoriginalmesh.triangles;
		Matrix4x4[] varoriginalbindposes = varoriginalmesh.bindposes;
		Material[] varoriginalmaterials = varoriginalrenderer.materials;
		
		Transform[] varnewchildbones = new Transform[varoriginalbones.Length];
		varoriginalbones.CopyTo(varnewchildbones,0);
		Matrix4x4[] varnewchildbindposes = new Matrix4x4[varoriginalbindposes.Length];
		varoriginalbindposes.CopyTo(varnewchildbindposes,0);
		
		#region *** setup the parent stump
		//make it so that the parent stump is identical to the current bone
		varparentstump.layer = varoriginalbones[varparentboneindex].gameObject.layer;
		varparentstumptransform.position = varpbonetodetach.position;
		varparentstumptransform.rotation = varpbonetodetach.rotation;
		varparentstumptransform.localScale = varpbonetodetach.lossyScale;
		#endregion
		
		#region *** physics setup
		int[] varcloneboneindexes = varpdismemberator.vargambonerelationsindexes[varbonecounter].propchildrenside.propindexes;
		int vartotalbonestoclone = varcloneboneindexes.Length;
		int varcloneboneindex = -1;
		int varcloneparentboneindex = -1;
		GameObject varclonepart = null;
		Transform varclonetransform = null;
		Rigidbody varcurrentbody = null;
		Rigidbody varclonebody = null;
		Collider varcurrentcollider = null;
		CharacterJoint varcurrentjoint = null;
		List<int> varbonetocutpatch = new List<int>();
		int varseparationindex = 0;
		Vector3 varscale;
		//this cycle runs through all of the varpbonetocut children, to perform a 'clone' of the relative gameobjects
		for (int vardetachcounter = -1; vardetachcounter < vartotalbonestoclone; vardetachcounter++ ) {
			
			varclonepart = new GameObject();
			varclonetransform = varclonepart.transform;
			varcurrentjoint = null;
			//special case, counter -1 is reserved for the varpbonetodetach
			if (vardetachcounter == -1) {
				varcloneboneindex = varbonecounter;
				varchildstump = varclonepart;
				varchildstumptransform = varclonetransform;
				varnewrenderer = varclonepart.AddComponent<SkinnedMeshRenderer>();
				varnewchildmesh = new Mesh();
				varscale = varoriginalbones[varcloneboneindex].lossyScale;
			}
			else {
				varcloneboneindex = varcloneboneindexes[vardetachcounter];
				//varcloneparentboneindex = varpdismemberator.vargamboneindexes.IndexOf(varoriginalbones[varcloneboneindex].parent);
				varcloneparentboneindex = varpdismemberator.vargamboneindexesparents[varcloneboneindex];
				varclonetransform.parent = varnewchildbones[varcloneparentboneindex];
				varcurrentjoint = varoriginalbones[varcloneboneindex].GetComponent<CharacterJoint>();
				varscale = varoriginalbones[varcloneboneindex].localScale;
			}
			
			clsurgentactuator actuator = varoriginalbones[varcloneboneindex].GetComponent<clsurgentactuator>();
			if (actuator != null)
				actuator.vargamactuatorenabled = false;
			
			//transforms cloning
			varclonepart.name = varoriginalbones[varcloneboneindex].name;
			varclonepart.layer = varoriginalbones[varcloneboneindex].gameObject.layer;
			varclonetransform.position = varoriginalbones[varcloneboneindex].position;
			varclonetransform.rotation = varoriginalbones[varcloneboneindex].rotation;
			varclonetransform.localScale = varscale;
			
			//rigidbodies cloning
			varclonebody = null;
			varcurrentbody = varoriginalbones[varcloneboneindex].GetComponent<Rigidbody>();
			if (varcurrentbody != null ) {
				varclonebody = varclonepart.AddComponent<Rigidbody>();
				varclonebody.drag = 1;
				varclonebody.angularDrag = 1;
				varclonebody.mass = varcurrentbody.mass / 2f;
				varclonebody.isKinematic = true;
				varclonebody.constraints = varcurrentbody.constraints;
			}
			
			//collider fixing
			varcurrentcollider = varoriginalbones[varcloneboneindex].GetComponent<Collider>();
			if (varcurrentcollider != null) {
				System.Type varcurrenttype = varcurrentcollider.GetType();
				if (varcurrenttype == typeof(SphereCollider)) {
					SphereCollider varcurrentspherecollider = varoriginalbones[varcloneboneindex].GetComponent<SphereCollider>();
					SphereCollider varnewscollider = varclonepart.AddComponent<SphereCollider>();
					varnewscollider.radius = varcurrentspherecollider.radius;
					varnewscollider.center = varcurrentspherecollider.center;
					varnewscollider = varcurrentspherecollider;
				}
				else if (varcurrenttype == typeof(CapsuleCollider)) {
					CapsuleCollider varcurrentcapsulecollider = varoriginalbones[varcloneboneindex].GetComponent<CapsuleCollider>();
					CapsuleCollider varnewccollider = varclonepart.AddComponent<CapsuleCollider>();
					varnewccollider.height = varcurrentcapsulecollider.height;
					varnewccollider.direction = varcurrentcapsulecollider.direction;
					varnewccollider.radius = varcurrentcapsulecollider.radius;
					varnewccollider.center = varcurrentcapsulecollider.center;
				}
				else if (varcurrenttype == typeof(BoxCollider)) {
					BoxCollider varcurrentboxcollider = varoriginalbones[varcloneboneindex].GetComponent<BoxCollider>();
					BoxCollider varnewbcollider = varclonepart.AddComponent<BoxCollider>();
					varnewbcollider.center = varcurrentboxcollider.center;
					varnewbcollider.size = varcurrentboxcollider.size;
					varnewbcollider = varcurrentboxcollider;
				}
				else if (varcurrenttype == typeof(MeshCollider)) {
					MeshCollider varcurrentmeshcollider = varoriginalbones[varcloneboneindex].GetComponent<MeshCollider>();
					MeshCollider varnewmcollider = varclonepart.AddComponent<MeshCollider>();
					varnewmcollider.sharedMesh = varcurrentmeshcollider.sharedMesh;
					varnewmcollider.convex = varcurrentmeshcollider.convex;
					varnewmcollider = varcurrentmeshcollider;
				}
				else if (varcurrenttype == typeof(WheelCollider)) {
					WheelCollider varcurrentwheelcollider = varoriginalbones[varcloneboneindex].GetComponent<WheelCollider>();
					BoxCollider varnewwcollider = varclonepart.AddComponent<BoxCollider>();
					varnewwcollider.center = varcurrentwheelcollider.center;
					varnewwcollider.size = new Vector3(varcurrentwheelcollider.radius/3, varcurrentwheelcollider.radius*2, varcurrentwheelcollider.radius*2);
					if (varcurrentbody == null) {
						varclonebody = varclonepart.AddComponent<Rigidbody>();
						varclonebody.mass = varcurrentwheelcollider.mass;
						varclonebody.isKinematic = true;
					}
				}
				else if (varcurrenttype == typeof(Collider)) {
					//parent has not collider, so we're cutting a physics-less bone
					Debug.LogError("Missing collider on cut.");
					return false;
				}
				else {
					Debug.LogError("Can't manage collider type " + varoriginalbones[varcloneboneindex].GetComponent<Collider>().GetType().ToString());
					return false;
				}
			}
			
			//rigidbodies cloning
			if (varcurrentjoint != null) {
				if (varpdismemberator.vargamboneindexescharacterjointconnect[varcloneboneindex] > -1) {
					varcurrentjoint = varclonepart.AddComponent<CharacterJoint>();
					//varcurrentjoint.connectedBody = varnewchildbones[varpdismemberator.vargamboneindexescharacterjointconnect[varcloneboneindex]].rigidbody;
					Rigidbody varexistingbody = varnewchildbones[varpdismemberator.vargamboneindexescharacterjointconnect[varcloneboneindex]].GetComponent<Rigidbody>();
					if (varexistingbody == null) {
						varcurrentjoint.connectedBody = varnewchildbones[varbonecounter].GetComponent<Rigidbody>();
					}
					else {
						varcurrentjoint.connectedBody = varnewchildbones[varpdismemberator.vargamboneindexescharacterjointconnect[varcloneboneindex]].GetComponent<Rigidbody>();
					}
				}
			}
			
			//add the varpbonetodetach gameobject, that we've cloned this pass, to the cut parts cache, that we'll use afterwards to destroy it
			varpdismemberator.vargamcutpartscache.Add(varoriginalbones[varcloneboneindex]);
			//replace the child bones array slot of the current cloned bone, with the new gameobject transform
			varnewchildbones[varcloneboneindex] = varclonetransform;
		}
		//dependency checkup. cycle through all depending rigidbodies of varpbonetodetach and substitute the connectedbody, to avoid bodyless disconnection of original bodyparts, upon separation
		int varconnectors = varpdismemberator.vargamrigidbodyconnections[varbonecounter].propindexes.Length;
		if (varconnectors > 1) {
			//we have at least a dependency. retrieve varpbonetodetach's connectedbody rigidbody
			int varconnectionindex = varpdismemberator.vargamrigidbodyconnections[varbonecounter].propindexes[0];
			//recycle varcurrentbody variable
			varcurrentbody = varoriginalbones[varconnectionindex].GetComponent<Rigidbody>();
			if (varcurrentbody != null) {
				//we have a parent rigidbody, so we replace it in all of our children rigidbodies
				for (int varconnectorcounter = 1; varconnectorcounter < varpdismemberator.vargamrigidbodyconnections[varbonecounter].propindexes.Length; varconnectorcounter++) {
					varconnectionindex = varpdismemberator.vargamrigidbodyconnections[varbonecounter].propindexes[varconnectorcounter];
					//perform the mandatory checks on all parts, since separation can by all means eliminate them from the renderer
					if (varoriginalbones[varconnectionindex] != null) {
						//recycle varcurrentjoint variable
						varcurrentjoint = varoriginalbones[varconnectionindex].GetComponent<CharacterJoint>();
						if (varcurrentjoint != null) {
							varcurrentjoint.connectedBody = varcurrentbody;
						}
					}
				}
			}
		}
		#endregion
		
		#region *** triangles setup
		//set the patch number of triangles
		int varseparationverticescount = varpdismemberator.vargamboneseparationvertices[varbonecounter].propindexes.Length;
		int varpatchtriangles = 0;
		int varparenttrianglesindex = 0;
		int varparenttriangleslength = 0;
		int varchildtrianglesindex = 0;
		int[] varnewparenttriangles = new int[0];
		int[] varnewchildtriangles = new int[0];
		if (varseparationverticescount > 0) {
			//there's separation vertices for this cut so we need to allocate the proper data
			varpatchtriangles = (varseparationverticescount-2)*3;
		}
		//declare the arrays that will host the new triangles, adding the patch triangles
		varparenttrianglesindex = varoriginaltriangles.Length;
		varparenttriangleslength = varparenttrianglesindex;
		varchildtrianglesindex = varpdismemberator.vargambonefulltrianglesindexes[varbonecounter].propindexes.Length*3;
		varnewparenttriangles = new int[varparenttrianglesindex + varpatchtriangles];
		varnewchildtriangles = new int[varchildtrianglesindex + varpatchtriangles];
		varoriginaltriangles.CopyTo(varnewparenttriangles,0);

		int vartriangleindex;
		int vartrianglechildindex;
		
		int varindex2, varindex3;
		for (int varstumptrianglecounter = 0; varstumptrianglecounter < varpdismemberator.vargambonefulltrianglesindexes[varbonecounter].propindexes.Length; varstumptrianglecounter++) {
			vartriangleindex = varpdismemberator.vargambonefulltrianglesindexes[varbonecounter].propindexes[varstumptrianglecounter];
			varindex2 = vartriangleindex+1;
			varindex3 = vartriangleindex+2;

			varnewparenttriangles[vartriangleindex] = 0;
			varnewparenttriangles[varindex2] = 0;
			varnewparenttriangles[varindex3] = 0;
			
			vartrianglechildindex = varstumptrianglecounter * 3;
			varnewchildtriangles[vartrianglechildindex] = varoriginaltriangles[vartriangleindex];
			varnewchildtriangles[vartrianglechildindex+1] = varoriginaltriangles[varindex2];
			varnewchildtriangles[vartrianglechildindex+2] = varoriginaltriangles[varindex3];
		}
		/*
		//debug
		for (int vartrianglecounter = 0; vartrianglecounter < varnewchildtriangles.Length; vartrianglecounter+=3) {
			clsurgutils.metcreatetriangle(Color.blue, varoriginalvertices[varnewchildtriangles[vartrianglecounter]], varoriginalvertices[varnewchildtriangles[vartrianglecounter+1]], varoriginalvertices[varnewchildtriangles[vartrianglecounter+2]], "_DismemberatorDeletableDebugTriangle" + vartrianglecounter);
		}
		*/
		#endregion
		
		#region *** Materials management
		int varmaterialscount = varoriginalmaterials.Length;
		int varstumpmaterialindex = -1;
		Material[] varnewparentmaterials = new Material[0];
		Material[] varnewchildmaterials;
		bool varaddedstumpmaterial = false;
		int[] varstumpmaterialsubmeshtriangles = new int[0];
		if (varpstumpmaterial != null) {
			//check if the stump material is already in the original materials array
			for (int varmaterialscounter = 0; varmaterialscounter < varmaterialscount; varmaterialscounter++) {
				if (varoriginalmaterials[varmaterialscounter].name == varpstumpmaterial.name || varoriginalmaterials[varmaterialscounter].name == varpstumpmaterial.name + " (Instance)") {
					varstumpmaterialindex = varmaterialscounter;
					varstumpmaterialsubmeshtriangles = varoriginalmesh.GetTriangles(varstumpmaterialindex);
					break;
				}
			}
			if (varstumpmaterialindex < 0) {
				//we need to add the stump material to the original materials array
				varnewparentmaterials = new Material[varmaterialscount+1];
				varoriginalmaterials.CopyTo(varnewparentmaterials,0);
				//assign the stump material to the new material slot
				varnewparentmaterials[varmaterialscount] = varpstumpmaterial;
				varstumpmaterialindex = varmaterialscount;
				varaddedstumpmaterial = true;
			}
			else {
				varnewparentmaterials = varoriginalmaterials;
			}
		}
		else {
			varnewparentmaterials = varoriginalmaterials;
		}
		varnewchildmaterials = new Material[varnewparentmaterials.Length];
		varnewparentmaterials.CopyTo(varnewchildmaterials,0);
		#endregion
		
		#region *** mesh separation
		int vardetachindex;
		//to divide the new mesh from the original mesh, we need to manipulate the child bones and child bindposes arrays
		//so we extract the parent side bone indexes of the varpbonetodetach, and scroll through those to assign the child stump
		//transform and child stump bindpose to all the parent bones in the child bone array
		//we also use these iterations to extract all existing patch triangle indices lists, to add them to the triangle patch arrays of parent and child meshes
		int[] varrelatedboneindexes = varpdismemberator.vargambonerelationsindexes[varbonecounter].propparentside.propindexes;
		int varcurrenttriangleindex;
		int[] vartemptriangles;
		int vartemptrianglesindexer = 0;
		for (int vardetachcounter = 0; vardetachcounter < varrelatedboneindexes.Length; vardetachcounter++) {
			//fetch the actual bone index from the propindexes
			vardetachindex = varrelatedboneindexes[vardetachcounter];
			//assign the child stump transform and bindpose for the child arrays
			varnewchildbones[vardetachindex] = varchildstumptransform;
			varnewchildbindposes[vardetachindex] = varoriginalbindposes[varbonecounter];
			
			//rev 4: single material leftover. calculate the starting index of the patch triangles, to define where the one-material parent triangles end
			//int varseparationindices = varpdismemberator.vargamboneseparationpatchtriangleindexes[vardetachindex].propindexes.Length;
		}
		//calculate the starting index of the patch triangles, to define where the multi material parent triangles end
		varparenttriangleslength = varpdismemberator.vargamboneseparationsubmeshhelper.Length * 3;

		//perform the bone, bindpose, and cut patch triangle reassignment for the parent side arrays, scanning the children side
		varrelatedboneindexes = varpdismemberator.vargambonerelationsindexes[varbonecounter].propchildrenside.propindexes;
		for (int vardetachcounter = 0; vardetachcounter < varrelatedboneindexes.Length; vardetachcounter++) {
			vardetachindex = varrelatedboneindexes[vardetachcounter];

			varoriginalbones[vardetachindex] = varparentstumptransform;
			varoriginalbindposes[vardetachindex] = varoriginalbindposes[varbonecounter];
			
			//index all the existing childrenside patch triangles and add them to the new child patch array
			varseparationindex = varpdismemberator.vargamboneseparationpatchtriangleindexes[vardetachindex].propindexes.Length;
			if (varseparationindex > 0) {
				//Debug.LogWarning(varoriginalbones[vardetachindex].name + " has a child cut");
				//declare an array that will store the existing cut patch triangles that we must transfer to the new child mesh
				vartemptriangles = new int[varseparationindex*3];
				vartemptrianglesindexer = 0;
				for (int vartriangleindexcounter = 0; vartriangleindexcounter < varseparationindex; vartriangleindexcounter++) {
					varcurrenttriangleindex = varpdismemberator.vargamboneseparationpatchtriangleindexes[vardetachindex].propindexes[vartriangleindexcounter];
					
					//rev 4: single material leftover
					//these triangles exist in our varpbonetodetach so we must transfer them to the new child object mesh
					//vartemptriangles[vartemptrianglesindexer] = varoriginaltriangles[varcurrenttriangleindex];
					//vartemptriangles[vartemptrianglesindexer+1] = varoriginaltriangles[varcurrenttriangleindex+1];
					//vartemptriangles[vartemptrianglesindexer+2] = varoriginaltriangles[varcurrenttriangleindex+2];
					//these triangles are currently part of our parent mesh, so we meed to delete them since they're being transferred into the new child object mesh
					//varnewparenttriangles[varcurrenttriangleindex] = 0;
					//varnewparenttriangles[varcurrenttriangleindex+1] = 0;
					//varnewparenttriangles[varcurrenttriangleindex+2] = 0;

					//these triangles exist in our varpbonetodetach so we must transfer them to the new child object mesh
					vartemptriangles[vartemptrianglesindexer] = varstumpmaterialsubmeshtriangles[varcurrenttriangleindex];
					vartemptriangles[vartemptrianglesindexer+1] = varstumpmaterialsubmeshtriangles[varcurrenttriangleindex+1];
					vartemptriangles[vartemptrianglesindexer+2] = varstumpmaterialsubmeshtriangles[varcurrenttriangleindex+2];
					//these triangles are currently part of our parent mesh, so we meed to delete them since they're being transferred into the new child object mesh
					varstumpmaterialsubmeshtriangles[varcurrenttriangleindex] = 0;
					varstumpmaterialsubmeshtriangles[varcurrenttriangleindex+1] = 0;
					varstumpmaterialsubmeshtriangles[varcurrenttriangleindex+2] = 0;
					vartemptrianglesindexer+=3;
				}
				//add the temp triangles to the triangle patch of the varpbonetodetach
				varbonetocutpatch.AddRange(vartemptriangles);
				//reset the indexes of the current bone, since its triangles have been transferred from the parent mesh into a child mesh, and we will not consider them again
				varpdismemberator.vargamboneseparationpatchtriangleindexes[vardetachindex].propindexes = new int[0];
			}
		}
		//attach the cut bone stump to the parent
		varparentstump.transform.parent = varoriginalbones[varparentboneindex];
		/*
		//debug
		for (int varboneindexcounter = 0; varboneindexcounter < varoriginalbones.Length; varboneindexcounter++) {
			Debug.Log("Parent bone " + varoriginalbones[varboneindexcounter].name, varoriginalbones[varboneindexcounter].gameObject);
		}
		
		for (int varboneindexcounter = 0; varboneindexcounter < varnewchildbones.Length; varboneindexcounter++) {
			Debug.Log("Child bone " + varnewchildbones[varboneindexcounter].name, varnewchildbones[varboneindexcounter].gameObject);
		}
		for (int vartrianglecounter = 0; vartrianglecounter < varbonetocutpatch.Count; vartrianglecounter+=3) {
			clsurgutils.metcreatetriangle(Color.red, varoriginalvertices[varbonetocutpatch[vartrianglecounter]], varoriginalvertices[varbonetocutpatch[vartrianglecounter+1]], varoriginalvertices[varbonetocutpatch[vartrianglecounter+2]], "_DismemberatorDeletableDebugTriangle" + vartrianglecounter);
		}
		*/
		
		#endregion
		
		int[] varnewchildpatchtriangles = new int[0];
		int[] varnewparentpatchtriangles = new int[0];
		//determine if the cut patch is expected or not
		if (varcreatestump) {
			//create a cut patch
			#region *** create the new vertices and normals arrays so that correct uv can be applied
			//the vertices loop is an unordered list of indexes. we must translate that into a sequential list of vertices indexes
			//since we need to add the relevant dismemberator vertices to the vertices, uv and normals arrays
			int varloopvertices = varpdismemberator.vargamboneseparationvertices[varbonecounter].propindexes.Length;
			int varstart, varend, varnewdimension;
			varstart = varoriginalvertices.Length;
			varend = varloopvertices;
			varnewdimension = varstart + varend;
			Vector3 varnewparentnormalvector = varpdismemberator.vargamoriginalbonepositions[varbonecounter] - varpdismemberator.vargamoriginalbonepositions[varparentboneindex];
			Vector3 varnewchildnormalvector = varpdismemberator.vargamoriginalbonepositions[varparentboneindex] - varpdismemberator.vargamoriginalbonepositions[varbonecounter];
			Vector3[] varnewvertices = new Vector3[varnewdimension];
			Vector2[] varnewuvs = new Vector2[varnewdimension];
			Vector3[] varnewparentnormals = new Vector3[varnewdimension];
			Vector3[] varnewchildnormals = new Vector3[varnewdimension];
			BoneWeight[] varnewboneweights = new BoneWeight[varnewdimension];
			int[] varnewverticesindices = new int[varloopvertices];
			int varcurrentvertexindex, varextendedvertexindex;
			varoriginalvertices.CopyTo(varnewvertices,0);
			varoriginaluvs.CopyTo(varnewuvs,0);
			varoriginalnormals.CopyTo(varnewparentnormals,0);
			varoriginalnormals.CopyTo(varnewchildnormals,0);
			varoriginalboneweights.CopyTo(varnewboneweights,0);
			for (int varnewvertexcounter = 0; varnewvertexcounter < varloopvertices; varnewvertexcounter++) {
				varcurrentvertexindex = varpdismemberator.vargamboneseparationvertices[varbonecounter].propindexes[varnewvertexcounter];
				varextendedvertexindex = varnewvertexcounter + varstart;
				varnewvertices[varextendedvertexindex] = varoriginalvertices[varcurrentvertexindex];
				varnewboneweights[varextendedvertexindex] = varoriginalboneweights[varcurrentvertexindex];
				varnewparentnormals[varextendedvertexindex] = varnewparentnormalvector;
				varnewchildnormals[varextendedvertexindex] = varnewchildnormalvector;
				varnewverticesindices[varnewvertexcounter] = varextendedvertexindex;
			}
			//varnewvertices now holds the original vertices plus a 'copy' of the original loop vertices
			//and verticesindices now holds a list of the sequential 'new' vertices indexes
			varpdismemberator.vargamboneseparationverticesuvhelper[varbonecounter].propuvcoordinates.CopyTo(varnewuvs, varstart);
			#endregion

			#region *** recalculate parent triangles and setup child triangles
			varnewchildpatchtriangles = new int[varpatchtriangles];
			varnewparentpatchtriangles = new int[varpatchtriangles];
	 		/*
			this section makes use of the specific vargamboneseparationvertices which holds the clockwise sorted vertices indexes of
			each bone separation from its parent, in regards to the bone's triangles. the triangle patch will be identical for parent
			and child stump, only sorted inversely, and will pivot around the first vertex. this gives a number of triangles
			equal to the vargamboneseparationvertices indexes minus two
			*/
			int varvertexindex0 = varnewverticesindices[0];
			int varvertexindex1;
			int varvertexindex2;
			int varvertexindex0aux, varvertexindex1aux, varvertexindex2aux;
			//Debug.Log("Separation vertices " + varseparationverticescount);
			int vartriangleoffsetter = 0;
			for (int varseparationverticescounter = 2; varseparationverticescounter < varseparationverticescount; varseparationverticescounter++) {
				varvertexindex0aux = varseparationverticescounter-2+vartriangleoffsetter;
				varvertexindex1aux = varseparationverticescounter-1+vartriangleoffsetter;
				varvertexindex2aux = varseparationverticescounter+vartriangleoffsetter;
				
				varvertexindex1 = varnewverticesindices[varseparationverticescounter-1];
				varvertexindex2 = varnewverticesindices[varseparationverticescounter];
				varnewparentpatchtriangles[varvertexindex0aux] = varvertexindex0;
				varnewparentpatchtriangles[varvertexindex1aux] = varvertexindex1;
				varnewparentpatchtriangles[varvertexindex2aux] = varvertexindex2;
				
				varnewchildpatchtriangles[varvertexindex0aux] = varvertexindex0;
				varnewchildpatchtriangles[varvertexindex1aux] = varvertexindex2;
				varnewchildpatchtriangles[varvertexindex2aux] = varvertexindex1;
				vartriangleoffsetter+=2;
			}
			/*
			//debug: draw the varpbonetodetach cut patch triangles
			for (int varvertexcounter = 0; varvertexcounter < varnewchildpatchtriangles.Length; varvertexcounter+=3) {
				metcreatetriangle(Color.green, varnewvertices[varnewchildpatchtriangles[varvertexcounter]], varnewvertices[varnewchildpatchtriangles[varvertexcounter+1]], varnewvertices[varnewchildpatchtriangles[varvertexcounter+2]],"xxxpatchchild" + varvertexcounter);
			}
			*/
			//this is a critical step, where the triangles array for the parent is filled with the recently added patch from the cut
			varnewparentpatchtriangles.CopyTo(varnewparenttriangles, varparenttrianglesindex);
			varnewchildpatchtriangles.CopyTo(varnewchildtriangles, varchildtrianglesindex);
			//submesh zero always contains all the original triangles (minus the ones we removed with the cut), so we setup the new parent submesh
			int[] varnewparenttrianglessubmesh = new int[varparenttriangleslength];
			System.Array.Copy(varnewparenttriangles, varnewparenttrianglessubmesh, varparenttriangleslength);
			#endregion
			
			#region *** set mesh elements
			//replace the varpbonetodetach with the parent stump, in the original model
			varoriginalbones[varbonecounter] = varparentstump.transform;
			//parent mesh elements
			varnewparentmesh.vertices = varnewvertices;
			varnewparentmesh.normals = varnewparentnormals;
			varnewparentmesh.uv = varnewuvs;
			varnewparentmesh.boneWeights = varnewboneweights;
			varnewparentmesh.triangles = varnewparenttriangles;
			varnewparentmesh.bindposes = varoriginalbindposes;
			//child mesh elements
			varnewchildmesh.vertices = varnewvertices;
			varnewchildmesh.normals = varnewchildnormals;
			varnewchildmesh.uv = varnewuvs;
			varnewchildmesh.boneWeights = varnewboneweights;
			varnewchildmesh.triangles = varnewchildtriangles;
			varnewchildmesh.bindposes = varnewchildbindposes;
			#endregion
		}
		else {
			//we only perform the cuts and don't require triangulation
			#region *** set mesh elements
			//replace the varpbonetodetach with the parent stump, in the original model
			varoriginalbones[varbonecounter] = varparentstump.transform;
			//parent mesh elements
			varnewparentmesh.vertices = varoriginalvertices;
			varnewparentmesh.normals = varoriginalnormals;
			varnewparentmesh.uv = varoriginaluvs;
			varnewparentmesh.boneWeights = varoriginalboneweights;
			varnewparentmesh.triangles = varnewparenttriangles;
			varnewparentmesh.bindposes = varoriginalbindposes;
			//child mesh elements
			varnewchildmesh.vertices = varoriginalvertices;
			varnewchildmesh.normals = varoriginalnormals;
			varnewchildmesh.uv = varoriginaluvs;
			varnewchildmesh.boneWeights = varoriginalboneweights;
			varnewchildmesh.triangles = varnewchildtriangles;
			varnewchildmesh.bindposes = varnewchildbindposes;
			#endregion
		}		
		
		#region *** set the submeshes for proper texture mapping		
		//create a dynamic data structure to hold the submeshes and 'overwrite' them where necessary
		//int varoriginalsubmeshcount = varoriginalmesh.subMeshCount;
		int varsubmeshcount = varoriginalmesh.subMeshCount;
		//check if we added the cut material in this step, so that we increase the parent submeshcount
		if (varaddedstumpmaterial) {
			varsubmeshcount++;
		}
		int[][] varsubmeshmatrix = new int[varsubmeshcount][];
		int[][] varsubmeshmatrixchild = new int[varsubmeshcount][];
		for (int varsubmeshcounter = 0; varsubmeshcounter < varsubmeshcount; varsubmeshcounter++) {
			//init the parent by fetching all the original triangles of the submesh
			if (varsubmeshcounter == varstumpmaterialindex) {
				//we're working on the cut patch submesh, so we must consider the existing submesh varstumpmaterialsubmeshtriangles (from possibly existing cuts),
				//and the new submesh varnewparentpatchtriangles (from the varpbonetodetach bone separation dismemberator helper)
				varsubmeshmatrix[varsubmeshcounter] = new int[varstumpmaterialsubmeshtriangles.Length + varnewparentpatchtriangles.Length];
				varstumpmaterialsubmeshtriangles.CopyTo(varsubmeshmatrix[varsubmeshcounter],0);
				varnewparentpatchtriangles.CopyTo(varsubmeshmatrix[varsubmeshcounter],varstumpmaterialsubmeshtriangles.Length);
				
				varbonetocutpatch.AddRange(varnewchildpatchtriangles);
				varsubmeshmatrixchild[varsubmeshcounter] = varbonetocutpatch.ToArray();
			}
			else {
				//retrieve the original submesh array for the parent
				varsubmeshmatrix[varsubmeshcounter] = varoriginalmesh.GetTriangles(varsubmeshcounter);
				//init the child with a totally empty submesh triangles array (IMPORTANT: this makes the assumption that 'new int' produces a zero filled array)
				varsubmeshmatrixchild[varsubmeshcounter]  = new int[varsubmeshmatrix[varsubmeshcounter].Length];
			}
		}

		//using the submeshes arrays, we compare each submesh triangle with the varnewparenttriangles array, to determine which submesh triangles no longer exist
		//IMPORTANT NOTE: we use the -original- triangles length as iterator, but then use varnewparenttriangles to check for deleted vertices, since varnewparenttriangles will have more triangles,
		//but they'll always be PATCH triangles, and never original, and it's the one triangle array that stores the recently deleted triangles by the cut
		//PARENT submesh setup. iterate from the first triangle to the last original triangle (ignore the added triangles of the cut patches)
		if (varnewparenttriangles.Length > 0) {
			//parent triangles length is > 0 so we're separating a cutter (and not a closed loop)
			for (int varnewparenttrianglescounter = 0; varnewparenttrianglescounter < varparenttriangleslength; varnewparenttrianglescounter+=3) {
				if (varnewparenttriangles[varnewparenttrianglescounter] == 0 && varnewparenttriangles[varnewparenttrianglescounter+1] == 0) {
					int vartrianglesubmesh = 		(int)varpdismemberator.vargamboneseparationsubmeshhelper[varnewparenttrianglescounter/3].x;
					int vartrianglesubmeshindex = 	(int)varpdismemberator.vargamboneseparationsubmeshhelper[varnewparenttrianglescounter/3].y * 3;
					//the current triangle has been cut, so we must zero it in the relevant submesh
					varsubmeshmatrix[vartrianglesubmesh][vartrianglesubmeshindex]   = 0;
					varsubmeshmatrix[vartrianglesubmesh][vartrianglesubmeshindex+1] = 0;
					varsubmeshmatrix[vartrianglesubmesh][vartrianglesubmeshindex+2] = 0;
					//since the current triangle has been cut from the parent, it might be present in the child triangles. if it's not, it means they are 'old' cut triangles, and will be 'zero' in the original triangle too
					varsubmeshmatrixchild[vartrianglesubmesh][vartrianglesubmeshindex]   = varoriginaltriangles[varnewparenttrianglescounter];
					varsubmeshmatrixchild[vartrianglesubmesh][vartrianglesubmeshindex+1] = varoriginaltriangles[varnewparenttrianglescounter+1];
					varsubmeshmatrixchild[vartrianglesubmesh][vartrianglesubmeshindex+2] = varoriginaltriangles[varnewparenttrianglescounter+2];
				}
			}
		}
		
		//we actually init the subMeshCount value of the new meshes here
		varnewparentmesh.subMeshCount = varsubmeshcount;
		varnewchildmesh.subMeshCount = varsubmeshcount;
		//set the modified triangles, stored in varsubmeshmatrix, into the corresponding new submeshes
		//Debug.Log("Materials " + varsubmeshcount + " cut submesh index " + varstumpmaterialindex);
		for (int varsubmeshcounter = 0; varsubmeshcounter < varsubmeshcount; varsubmeshcounter++) {
			varnewparentmesh.SetTriangles(varsubmeshmatrix[varsubmeshcounter], varsubmeshcounter);
			varnewchildmesh.SetTriangles(varsubmeshmatrixchild[varsubmeshcounter], varsubmeshcounter);
		}

		//it's a mesh necessity to call settriangles to make persistent changes to the mesh, so we need to index the patch submeshes to be able to manage the vargamboneseparationpatchtriangleindexes index array,
		//and 'cut' the leftover patch triangles when the parent of a child cut bone is cut itself. this is to avoid former cut patch triangles to 'stay attached' to the original mesh, and float in midair,
		//after their parent is cut
		#region *** *** store the parent patch triangles indexes in the dismemberator vargamboneseparationpatchtriangleindexes indexer. this is required subsequently, if we perform another cut, of a parent of this bone, to dispose of these new triangles we added (which the dismemberator doesn't know of)
		//IMPORTANT: the vargamboneseparationpatchtriangleindexes is an INDEXER, which means it'll hold, for each triangle of the patch, only the triangle start index, in the triangles array of the mesh. a true triangles array holds an actual vertex index, instead.
		int varnewparentpatchindex = varstumpmaterialsubmeshtriangles.Length;
		varpdismemberator.vargamboneseparationpatchtriangleindexes[varbonecounter].propindexes = new int[varnewparentpatchtriangles.Length/3];
		for (int varnewparentpatchindexcounter = 0; varnewparentpatchindexcounter < varpdismemberator.vargamboneseparationpatchtriangleindexes[varbonecounter].propindexes.Length; varnewparentpatchindexcounter++) {
			varpdismemberator.vargamboneseparationpatchtriangleindexes[varbonecounter].propindexes[varnewparentpatchindexcounter] = varnewparentpatchindex;
			varnewparentpatchindex+=3;
		}
		#endregion
		
		/*
		//debug
		//draw the parent stump material submesh triangles
		GameObject varstumpsubmesh = new GameObject("_DismemberatorDeletableDebugGameobjectParent" + varpbonetodetach.name);
		int[] varstumptriangles = varnewparentmesh.GetTriangles(varstumpmaterialindex);
		for (int varcounter = 0; varcounter < varstumptriangles.Length; varcounter+=3) {
			metcreatetriangle(Color.yellow, varnewvertices[varstumptriangles[varcounter]], varnewvertices[varstumptriangles[varcounter+1]], varnewvertices[varstumptriangles[varcounter+2]], "index_" + varcounter, true, varstumpsubmesh.transform);
		}
		*/
		/*
		//draw the child stump material submesh triangles
		GameObject varstumpsubmesh = new GameObject("_DismemberatorDeletableDebugGameobjectChild" + varpbonetodetach.name);
		int[] varstumptriangles = varnewchildmesh.GetTriangles(varstumpmaterialindex);
		for (int varcounter = 0; varcounter < varstumptriangles.Length; varcounter+=3) {
			metcreatetriangle(Color.yellow, varnewvertices[varstumptriangles[varcounter]], varnewvertices[varstumptriangles[varcounter+1]], varnewvertices[varstumptriangles[varcounter+2]], "index_" + varcounter, true, varstumpsubmesh.transform);
		}
		*/
		#endregion
		
		#region *** set renderer elements
		//parent renderer elements
		varpdismemberator.vargamskinnedmeshrenderer.bones = varoriginalbones;
		varpdismemberator.vargamskinnedmeshrenderer.sharedMesh = varnewparentmesh;
		varpdismemberator.vargamskinnedmeshrenderer.materials = varnewparentmaterials;
		//child renderer elements
		varnewrenderer.bones = varnewchildbones;
		varnewrenderer.sharedMesh = varnewchildmesh;
		varnewrenderer.materials = varnewchildmaterials;
		#endregion

		#region *** concurrency fix
		//manage parallel cuts
		varpdismemberator.vargamparallelcutcounter--;
		if (varpdismemberator.vargamparallelcutcounter == 0) {
			//we are the only cut in progress right now, or we are the last of a sequence, so we can destroy the original bones safely
			for (int vardestroycounter = 0; vardestroycounter < varpdismemberator.vargamcutpartscache.Count; vardestroycounter++) {
				//check for null to avoid concurrency Destroy errors
				if (varpdismemberator.vargamcutpartscache[vardestroycounter].gameObject != null) {
					GameObject.Destroy(varpdismemberator.vargamcutpartscache[vardestroycounter].gameObject);
				}
			}
			//reset the cut parts cache
			varpdismemberator.vargamcutpartscache.Clear();
		}
		#endregion

		
		//instantiate parent and/ or child particles if not null
		if (varpparentparticle != null) {
			GameObject varparentparticle = GameObject.Instantiate(varpparentparticle, varparentstumptransform.position, varparentstumptransform.rotation) as GameObject;
			varparentparticle.transform.parent = varparentstumptransform;
		}
		if (varpchildparticle != null) {
			GameObject varchildparticle = GameObject.Instantiate(varpchildparticle, varchildstumptransform.position, Quaternion.Inverse(varchildstumptransform.rotation)) as GameObject;
			varchildparticle.transform.parent = varchildstumptransform;
		}
		
		//turn all of the stump child rigidbodies into physic driven if the corresponding parameter is true
		if (varpcinematiccut) {
			Rigidbody[] varstumpbodies = varchildstump.GetComponentsInChildren<Rigidbody>();
			for (int varchildcounter = 0; varchildcounter < varstumpbodies.Length; varchildcounter++) {
				varstumpbodies[varchildcounter].isKinematic = false;
				varstumpbodies[varchildcounter].velocity = Vector3.zero;
				varstumpbodies[varchildcounter].angularVelocity = Vector3.zero;
			}
			if (varchildstump.GetComponent<Rigidbody>() != null) {
				varchildstump.GetComponent<Rigidbody>().velocity = Vector3.zero;
				varchildstump.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			}
		}
		return true;
	}
	
	/// <summary>
	/// Checks the intersection between the passed plane and an object, based on distance, and performs a cut of the
	/// heaviest of the bones caught in the contactpoint-distance sphere. This means that, if the dismember catches
	/// an arm and a shoulder bone, the shoulder bone will be cut, and the arm and all its children will detach.
	/// NOTE: this function is in experimental and is a less polished alternative than metdismember
	/// </summary>
	/// <param name="varptarget">
	/// The object that will be check for cuts by the cutting plane
	/// </param>
	/// <param name="varpbonetodetach">
	/// The bone to detach, together with its children, from the main gameobject
	/// NOTE: pass null if a generic cut is performed
	/// </param>
	/// <param name="varpstumpmaterial">
	/// if this parameter is not null, hollow areas after a cut will be filled with new triangles made with this material and its related texture
	/// </param>
	/// <param name="varpcontactpoint">
	/// The point where the dismembering slash originates
	/// </param>
	/// <param name="varpslashdirection">
	/// The direction of the cut. This is basically the 'forward' of the slashing plane.
	/// </param>
	/// <param name="varpdistance">
	/// The maximum distance allowed to determine if the cut interacts with any of the target object bones.
	/// </param>
	/// <param name="varpcinematiccut">
	/// OPTIONAL default true will cycle the cut gameobject and set all its rigidbodies to physic driven. false leaves the original state unchanged.
	/// </param>
	/// <returns>
	/// true if there was a cut
	/// false if there was no cut
	/// </returns>
	public static bool metdismemberrigged(GameObject varptarget, Transform varpbonetodetach, Material varpstumpmaterial, Vector3 varpcontactpoint, Vector3 varpslashdirection, float varpdistance = 1, bool varpcinematiccut = true) {
		bool varintersection;
		//for a start, determine if the contact point and slash direction and distance actually cut any bone
		float vardistancesquare = varpdistance * varpdistance;
		Plane varcutplane = new Plane(varpslashdirection, varpcontactpoint);
		Transform[] varbones = new Transform[0]; //Transform[] varbones = var varfixer.GetComponentsInChildren<Transform>();
		
		if (varptarget == null) {
			Debug.LogError("Need a target to be able to cut");
		}
		
		clsdismemberator vardismemberator = varptarget.GetComponent<clsdismemberator>();
		if (vardismemberator == null) {
			vardismemberator = varptarget.transform.root.GetComponentInChildren<clsdismemberator>();
			if (vardismemberator == null) {
				Debug.LogWarning("No dismemberator class found in target. Unable to cut.");
				return false;
			}
		}
		
		//the first step is determination of the 'fixer', usually the armature, which is the parent of the skinned mesh renderer, or mesh filter object
		//of course we additionally verify if there's a proper mesh object to retrieve
		Transform varfixer = vardismemberator.vargamfixer;
		if (varfixer == null) {
			Debug.LogError("No fixer");
			return false;
		}
		if (vardismemberator.vargamskinnedmeshrenderer.sharedMesh == null) {
			Debug.LogError("No original mesh");
			return false;
		}
		//shortcut to the skinned mesh renderer bones
		varbones = vardismemberator.vargamskinnedmeshrenderer.bones;
		
		//limit the bone cut check cycle if we have a specific bone to detach
		int varstartingindex = 0, varendingindex = varbones.Length;
		if (varpbonetodetach) {
			varstartingindex = vardismemberator.vargamboneindexes.IndexOf(varpbonetodetach);
			if (varstartingindex < 0) {
				Debug.LogError("No index for parameter bone");
				return false;
			}
			varendingindex = varstartingindex+1;
		}
		Vector3 varcutpoint = new Vector3();
		//cycle through the bones to perform the actual cutting, which may or may not happen for the current bone
		//TODO: optimize by means of an external determination routine, based on rays or colliders
		for (int varbonecounter = varstartingindex; varbonecounter < varendingindex; varbonecounter++) {
			//verify if the current bone can actually be cut from somewhere
			if (varbones[varbonecounter].parent == null) {
				Debug.LogWarning("Skipping " + varbones[varbonecounter] + "] no parent");
				continue;
			}
			//verify if this bone is under cut distance
			if ((varbones[varbonecounter].position - varpcontactpoint).sqrMagnitude > vardistancesquare && varpbonetodetach == null) {
				Debug.LogWarning("Skipping " + varbones[varbonecounter] + "] too distant");
				continue;
			}
			//verify if the cut bone has a rigidbody, otherwise can't perform a cut
			if (varbones[varbonecounter].GetComponent<Rigidbody>() == null) {
				Debug.LogWarning("Skipping " + varbones[varbonecounter] + "] no parent rigidbody");
				continue;
			}

			//determine if there's an intersection between the segment formed by this bone and its parent, and the cut plane
			if (varpbonetodetach == null) {
				varintersection = metsetsegmentplaneintersection(varcutplane, varpcontactpoint, varbones[varbonecounter].position, varbones[varbonecounter].parent.position, ref varcutpoint);
				if (!varintersection) {
					Debug.LogWarning("Skipping candidate [" + varbones[varbonecounter].name + " parent " + varbones[varbonecounter].parent.name+ "] no intersection with plane");
					continue;
				}
			}
			else {
				varintersection = varbones[varbonecounter];
			}
			
			
			//the second step, calculation of the displaced cut plane. vertices are always memorized in the sharedmeshobject as per their static, model bound coordinates (normally zero-coordinate based),
			//so we need to remap our cut plane from its real world coordinates and normal into the sharedmesh coordinates
			//Debug.LogWarning("Fixer " + varfixer.name + ". I'm being cut " + varbones[varcounter].parent.name, varbones[varcounter].gameObject);
			
			//instance of the 'fixer' objects that will project the cut point and plane in respect to the bone being cut
			Vector3 varfixerplanenormal, varfixerplaneposition;
			
			int varparentboneindex = vardismemberator.vargamboneindexes.IndexOf(varbones[varbonecounter].parent);
			//we didn't find an index for the parent. should never happen, but we skip this bone otherwise.
			if (varparentboneindex<0) {
				Debug.LogWarning("We have a valid candidate [" + varbones[varbonecounter].name + "] but its parent is not ragdoll indexed. Can't cut");
				continue;
			}
			Debug.LogWarning("Good candidate n."+ varbonecounter +" [" + varbones[varbonecounter].name + " parent " + varbones[varbonecounter].parent.name+ "] parent bone n." + varparentboneindex + " [" + varbones[varparentboneindex].name + "]" );
			
			//extremely important: calculate fixer plane position and normal. the challenge here is that the vertices are in 'bind pose' while our current bone and cut plane position and rotation might be
			//completely different, and world bound instead of fixer bound
			if (varbones[varbonecounter].position == varbones[varbonecounter].parent.position) {
				Debug.LogWarning("Skipped candidate [" + varbones[varbonecounter].name + "] because it shared position with its parent");
				continue;
			}
			
			//instance the faux bone that will be used to make the transformations of the cut plane and normal
			GameObject varfauxbone = new GameObject(varbones[varbonecounter].name + "_faux");
			varfauxbone.transform.position = vardismemberator.vargamoriginalbonepositions[varbonecounter];
			varfauxbone.transform.eulerAngles = vardismemberator.vargamoriginalboneangles[varbonecounter];
			
			varfixerplaneposition = varbones[varbonecounter].InverseTransformPoint(varcutpoint);
			varfixerplaneposition = varfauxbone.transform.TransformPoint(varfixerplaneposition);
			varfixerplanenormal = varbones[varbonecounter].InverseTransformDirection(varpslashdirection);
			varfixerplanenormal = varfauxbone.transform.TransformDirection(varfixerplanenormal);
			
			//we have the cut plane and normal so we can destroy the faux bone
			GameObject.Destroy(varfauxbone);
			
			//metcreateplaceholder(0.06f, Color.yellow, varfixerplaneposition,"xxxfixerplaneposition_" + varbonecounter);
			//metcreateplaceholder(0.06f, Color.yellow, varfixerplaneposition+varfixerplanenormal,"xxxfixerplanenormal_" + varbonecounter);
			
			//we are definitely cutting, at this point
			
			//the cut creates two stumps. the parent stump will be a clone of the current bone, while the child stump is a copy of the parent bone
			//in both cases we -could- adapt colliders, but in release 1 we just copy the original colliders
			
			//parent stump creation, this is the gameobject that will replace the bone that we cut, in the original mesh
			GameObject varparentstump = new GameObject(varbones[varbonecounter].parent.name + "_stump");
			//halve the mass of the parent rigidbody. this should produce realistic enough results to avoid creating a ratio based weight split
			float varrigidbodymass;
			if (varbones[varparentboneindex].GetComponent<Rigidbody>() != null) {
				varrigidbodymass = varbones[varparentboneindex].GetComponent<Rigidbody>().mass / 2;
				varbones[varparentboneindex].GetComponent<Rigidbody>().mass = varrigidbodymass;
			}
			else {
				varrigidbodymass = varbones[varbonecounter].GetComponent<Rigidbody>().mass / 2;
			}
			//setup the stump, copying from the current bone
			varparentstump.layer = varbones[varparentboneindex].gameObject.layer;
			//make it so that the parent stump is identical to the current bone
			varparentstump.transform.position = varbones[varbonecounter].position;
			varparentstump.transform.rotation = varbones[varbonecounter].rotation;
			
			//child stump creation. we don't reinvent the wheel here: just create a 'clone' (this method is faster than instantiate) of the parent bone
			//and assign rotation, position, collider and rigidbody to suit the need
			GameObject varchildstump = new GameObject(varbones[varbonecounter].name + "_stump");
			varchildstump.layer = varbones[varparentboneindex].gameObject.layer;
			Rigidbody varnewbody = varchildstump.AddComponent<Rigidbody>();
			varnewbody.drag = 1;
			varnewbody.angularDrag = 1;
			varnewbody.mass = varrigidbodymass;
			//make it so that the child stump is identical to the parent of the current bone
			varchildstump.transform.position = varbones[varparentboneindex].position;
			varchildstump.transform.rotation = varbones[varparentboneindex].rotation;
			varchildstump.transform.localScale = varbones[varparentboneindex].lossyScale;
			
			//add a skinned mesh renderer to the child stump, this is a very important step that will allow the child stump to work as a physical object that inherits the stump parts of the original mesh
			SkinnedMeshRenderer varnewrenderer = varchildstump.AddComponent<SkinnedMeshRenderer>();
			//copy the original bones into the new renderer
			varnewrenderer.bones = vardismemberator.vargamskinnedmeshrenderer.bones;
			
			//stump materials assignment
			int varpatchmaterialindex = -1;
			//verify if the stump material is already in the original mesh, and add it if necessary
			int varauxindex = vardismemberator.vargamskinnedmeshrenderer.materials.Length;
			bool varmatfound = false;
			if (varpstumpmaterial != null) {
				for (int varmatindex = 0; varmatindex < vardismemberator.vargamskinnedmeshrenderer.materials.Length; varmatindex++ ) {
					if (vardismemberator.vargamskinnedmeshrenderer.materials[varmatindex].name == varpstumpmaterial.name + " (Instance)" || vardismemberator.vargamskinnedmeshrenderer.materials[varmatindex].name == varpstumpmaterial.name) {
						varmatfound = true;
						varpatchmaterialindex = varmatindex;
						break;
					}
				}
				//stump material is not present in the original mesh materials array, so we must add it
				if (varmatfound == false) {
					//create a new materials array, with an extra slot
					Material[] varmaterials = new Material[varauxindex+1];
					vardismemberator.vargamskinnedmeshrenderer.materials.CopyTo(varmaterials,0);
					varmaterials[varauxindex] = varpstumpmaterial;
					//add the stump material to the original mesh renderer materials instance
					vardismemberator.vargamskinnedmeshrenderer.materials = varmaterials;
				}
				//the materials array is reassigned back later in the routine
			}
			
			//=> collider fixing start
			System.Type varcurrenttype = varbones[varparentboneindex].GetComponent<Collider>().GetType();
			if (varcurrenttype == typeof(SphereCollider)) {
				SphereCollider varcurrentcollider = varbones[varparentboneindex].GetComponent<SphereCollider>();
				SphereCollider varnewscollider = varchildstump.AddComponent<SphereCollider>();
				varnewscollider.radius = varcurrentcollider.radius;
				varnewscollider.center = varcurrentcollider.center;
			}
			else if (varcurrenttype == typeof(CapsuleCollider)) {
				CapsuleCollider varcurrentcollider = varbones[varparentboneindex].GetComponent<CapsuleCollider>();
				CapsuleCollider varnewccollider = varchildstump.AddComponent<CapsuleCollider>();
				varnewccollider.height = varcurrentcollider.height;
				varnewccollider.direction = varcurrentcollider.direction;
				varnewccollider.radius = varcurrentcollider.radius;
				varnewccollider.center = varcurrentcollider.center;
			}
			else if (varcurrenttype == typeof(BoxCollider)) {
				BoxCollider varcurrentcollider = varbones[varparentboneindex].GetComponent<BoxCollider>();
				BoxCollider varnewbcollider = varchildstump.AddComponent<BoxCollider>();
				varnewbcollider.center = varcurrentcollider.center;
				varnewbcollider.size = varcurrentcollider.size;
			}
			else if (varcurrenttype == typeof(MeshCollider)) {
				MeshCollider varcurrentcollider = varbones[varparentboneindex].GetComponent<MeshCollider>();
				MeshCollider varnewmcollider = varchildstump.AddComponent<MeshCollider>();
				varnewmcollider.sharedMesh = varcurrentcollider.sharedMesh;
				varnewmcollider.convex = varcurrentcollider.convex;
			}
			else if (varcurrenttype == typeof(Collider)) {
				//parent has not collider, so we're cutting a physics-less bone
			}
			else {
				Debug.LogError("Can't manage collider type " + varbones[varbonecounter].parent.GetComponent<Collider>().GetType().ToString());
				return false;
			}
			//<= collider fixing end
			
			//setup and fix the meshes
			//Plane varlocalcutplane = new Plane(varfixerplanenormal, varfixerplaneposition);
			
			GameObject varnewplane = new GameObject("plocalplane");//metcreateplane("plocalplane");
			varnewplane.transform.position = varfixerplaneposition;
			varnewplane.transform.LookAt(varfixerplaneposition+varfixerplanenormal);
			
			//shortcut to the original sharedmesh
			Mesh varoriginalmesh = vardismemberator.vargamskinnedmeshrenderer.sharedMesh;
			float vardistp1 = 0, vardistp2 = 0, vardistp3 = 0, vardistpbone = 0;
			//int varvertexindex1, varvertexindex2, varvertexindex3 = 0;
			Vector3[] varoriginalvertices = varoriginalmesh.vertices;
			int[] varoriginaltriangles = varoriginalmesh.triangles;
			BoneWeight[] varoriginalboneweights = varoriginalmesh.boneWeights;
			Vector3[] varoriginalnormals = varoriginalmesh.normals;
			Vector2[] varoriginaluvs = varoriginalmesh.uv;
			
			//detach the triangles
			//zero the parent bone triangles, which are subject to intersection calculations (a part of them might 'stay' with the parent stump)
			//at the same time, verify if the vertices of this part are 'split' by the cut plane, to add them to the stump vertices compressor, and use them for triangulation
			//determine if the normal of the cutting plane is facing towards or facing away from the current bone (positive facing away, negative facing towards
			//vardistpbone = varlocalcutplane.GetDistanceToPoint(vardismemberator.vargamoriginalbonepositions[varbonecounter]);
			vardistpbone = (varfixerplanenormal.x*vardismemberator.vargamoriginalbonepositions[varbonecounter].x+varfixerplanenormal.y*vardismemberator.vargamoriginalbonepositions[varbonecounter].y+varfixerplanenormal.z*vardismemberator.vargamoriginalbonepositions[varbonecounter].z + 
							(-varfixerplanenormal.x*varfixerplaneposition.x-varfixerplanenormal.y*varfixerplaneposition.y-varfixerplanenormal.z*varfixerplaneposition.z));
			if (vardistpbone < 0) {
				vardistpbone = 1;
			}
			else {
				vardistpbone = -1;
			}

			//Debug.LogWarning("Total vertices " + varoriginalvertices.Length);
			//Debug.LogWarning("Total triangles " + varoriginaltriangles.Length);
			//the first new vertex we create is the one reserved to the 'cut' position, which will be used as a triangulator base.
			//IMPORTANT: notice that we don't modify the vertices array here
			int varfirstnewvertex = varoriginalvertices.Length;
			//last new vertex will hold the index of the 'last new vertex' of the last cut performed. used for uv and normal preservation.
			int varlastnewvertex = 0;
			//needs be used to compensate uv and normal mapping
			int[] varcurrentbonestrianglesindexes = new int[0];
			int varindexeslength = vardismemberator.vargambonefulltrianglesindexes[varparentboneindex].propindexes.Length;
			int varcurrentbonetrianglecount; // = vardismemberator.vargambonefulltrianglesindexes[varparentboneindex].propindexes.Length;
			int varindexerscroller = 0;
			//we already have a cut, so we must compensate the full triangles indexes array with the existing patch triangles.
			if (varpatchmaterialindex > -1) {
				int[] varpatchtriangles = varoriginalmesh.GetTriangles(varpatchmaterialindex);
				//determine the last new vertex index. need to do this from the 'bottom' of the gettriangles array, since 'settriangles' adds the new triangles on the 'front' of the array
				int varexistingpatchlengthindex = varpatchtriangles.Length-1;
				varlastnewvertex = varpatchtriangles[varexistingpatchlengthindex];
				for (int varlastnewvertexscanner = 1; varlastnewvertexscanner < 3; varlastnewvertexscanner++) {
					if (varpatchtriangles[varexistingpatchlengthindex-varlastnewvertexscanner] < varlastnewvertex) {
						varlastnewvertex = varpatchtriangles[varexistingpatchlengthindex-varlastnewvertexscanner];
					}
				}
				
				int varpatchindexercount = varpatchtriangles.Length/3;
				//create a new triangles indexes array that holds all the current bone indexes... plus the patch indexes (they might not be for this bone, but they should be just a few, so we don't compute the vertex boneweights because of the overhead that'd require
				varcurrentbonetrianglecount = varindexeslength + varpatchindexercount;
				varcurrentbonestrianglesindexes = new int[varcurrentbonetrianglecount];
				vardismemberator.vargambonefulltrianglesindexes[varparentboneindex].propindexes.CopyTo(varcurrentbonestrianglesindexes,0);
				//Debug.Log("Starting patch index = " + (varoriginaltriangles.Length - varpatchtriangles.Length));
				varindexerscroller = varoriginaltriangles.Length - varpatchtriangles.Length;
				for (int varpatchindexer = 0; varpatchindexer < varpatchindexercount; varpatchindexer++) {
					//Debug.Log(varpatchindexer + " " + varpatchtriangles[varindexerscroller]);
					varcurrentbonestrianglesindexes[varindexeslength+varpatchindexer] = varindexerscroller;
					varindexerscroller+=3;
				}
				//re-set the indexer scroller
				varindexerscroller = varoriginaltriangles.Length - varpatchtriangles.Length;
			}
			else {
				varcurrentbonetrianglecount = varindexeslength;
				varcurrentbonestrianglesindexes = new int[varcurrentbonetrianglecount];
				vardismemberator.vargambonefulltrianglesindexes[varparentboneindex].propindexes.CopyTo(varcurrentbonestrianglesindexes,0);
				varindexerscroller = varoriginaltriangles.Length;
			}
			//Debug.LogError("indexer scroller " + varindexerscroller);
			//this is the array that will hold all the original triangles minus the current bone vargambonefulltrianglesindexes
			int[] varnewparenttriangles = new int[varoriginaltriangles.Length];
			//this is the array that will hold all of the current bone triangles, plus the parent triangles that are 'this side' of the cut plane.
			int[] varnewchildtriangles;
			//this is the list that will be filled with triangles we transfer from the cut, and from the current bone to the child stump mesh renderer, with varchildstumptriangles
			List<int> varchildstumptrianglestemp = new List<int>();
			
			varoriginaltriangles.CopyTo(varnewparenttriangles,0);
			int vartriangleindex, varvertex1, varvertex2, varvertex3;
			List<int> varparentpatchtrianglelist = new List<int>();
			List<int> varchildpatchtrianglelist = new List<int>();
			int varfirstvertex;
			int varsecondvertex;
			int varparentsidevertices;
			
			//cycle through all of the current bone's triangles, and 'separate' vertices.
			//after this, varnewparenttriangles will have all parent triangles minus the ones 'below' the cut plane
			//varchildstumptrianglestemp will have all the child triangles
			//and varchildpatchtrianglelist, varparentpatchtrianglelist will have the loop vertices
			if (varpbonetodetach) {
				int varchildtriangles = vardismemberator.vargambonefulltrianglesindexes[varbonecounter].propindexes.Length;
				int[] vartriangle = new int[3];
				varcurrentbonestrianglesindexes = new int[varchildtriangles];
				vardismemberator.vargambonefulltrianglesindexes[varbonecounter].propindexes.CopyTo(varcurrentbonestrianglesindexes,0);
				
				for (int varcutbonetriangle = 0; varcutbonetriangle < varchildtriangles; varcutbonetriangle++) {
					vartriangleindex = varcurrentbonestrianglesindexes[varcutbonetriangle];
	
					varvertex1 = varoriginaltriangles[vartriangleindex];
					varvertex2 = varoriginaltriangles[vartriangleindex+1];
					varvertex3 = varoriginaltriangles[vartriangleindex+2];
					
					//zero out the triangle on the parent, since it was 'moved' to the child
					varnewparenttriangles[vartriangleindex] = 0;
					varnewparenttriangles[vartriangleindex+1] = 0;
					varnewparenttriangles[vartriangleindex+2] = 0;
					//add the zeroed triangle to the child stump
					vartriangle[0] = varvertex1;
					vartriangle[1] = varvertex2;
					vartriangle[2] = varvertex3;
					varchildstumptrianglestemp.AddRange(vartriangle);
				}
				//add the current bone loop to the child patch triangle list
				varchildpatchtrianglelist.AddRange(vardismemberator.vargamboneseparationvertices[varbonecounter].propindexes);
				//and to the parent patch
				varparentpatchtrianglelist.AddRange(vardismemberator.vargamboneseparationvertices[varbonecounter].propindexes);
				for (int x = 0; x < varchildpatchtrianglelist.Count; x++) {
					metcreateplaceholder(0.02f, Color.red, varoriginalvertices[varchildpatchtrianglelist[x]],"xxxchildpatchloop"+x);
				}
			}
			else {
				for (int varcutbonetriangle = 0; varcutbonetriangle < varcurrentbonetrianglecount; varcutbonetriangle++) {
					vartriangleindex = varcurrentbonestrianglesindexes[varcutbonetriangle];
	
					varvertex1 = varoriginaltriangles[vartriangleindex];
					varvertex2 = varoriginaltriangles[vartriangleindex+1];
					varvertex3 = varoriginaltriangles[vartriangleindex+2];
					
					//calculate distance from the plane for each vertex of the current triangle of the parent bone, normalizing it by the current bone distance side fixer, so that plane distance checks are always cast against a normal facing away from the current bone
					//vardistp1 = varlocalcutplane.GetDistanceToPoint(varmodifiedvertices[varvertex1]) * vardistpbone;
					//vardistp2 = varlocalcutplane.GetDistanceToPoint(varmodifiedvertices[varvertex2]) * vardistpbone;
					//vardistp3 = varlocalcutplane.GetDistanceToPoint(varmodifiedvertices[varvertex3]) * vardistpbone;
					//we explode this inline so that the calculation takes one third of the equivalent getdistancetopoint calls
					vardistp1 = (varfixerplanenormal.x*varoriginalvertices[varvertex1].x+varfixerplanenormal.y*varoriginalvertices[varvertex1].y+varfixerplanenormal.z*varoriginalvertices[varvertex1].z + 
								(-varfixerplanenormal.x*varfixerplaneposition.x-varfixerplanenormal.y*varfixerplaneposition.y-varfixerplanenormal.z*varfixerplaneposition.z)) * vardistpbone;
					vardistp2 = (varfixerplanenormal.x*varoriginalvertices[varvertex2].x+varfixerplanenormal.y*varoriginalvertices[varvertex2].y+varfixerplanenormal.z*varoriginalvertices[varvertex2].z + 
								(-varfixerplanenormal.x*varfixerplaneposition.x-varfixerplanenormal.y*varfixerplaneposition.y-varfixerplanenormal.z*varfixerplaneposition.z)) * vardistpbone;
					vardistp3 = (varfixerplanenormal.x*varoriginalvertices[varvertex3].x+varfixerplanenormal.y*varoriginalvertices[varvertex3].y+varfixerplanenormal.z*varoriginalvertices[varvertex3].z + 
								(-varfixerplanenormal.x*varfixerplaneposition.x-varfixerplanenormal.y*varfixerplaneposition.y-varfixerplanenormal.z*varfixerplaneposition.z)) * vardistpbone;
	
					//reset the triangle vertices aux variables (equals to distp1 > 0 && distp2 > 0)
					varfirstvertex = varvertex1;
					varsecondvertex = varvertex2;
					varparentsidevertices = 0;
					//check if any of the vertexes is on the "child side" of the plane, which means that the triangle is cut
					if (vardistp1 <= 0 || vardistp2 <= 0 || vardistp3 <= 0 || vartriangleindex >= varindexerscroller) {
						//determine if the vertices of the current triangle are 'on the parent side' of the cutting plane, in which case, they must be added to the
						//skinned mesh renderer of the child stump, to be weighted and thus animated
						if (vardistp1 <= 0 && vardistp2 <= 0 && vardistp3 > 0) {
							varfirstvertex = varvertex1;
							varsecondvertex = varvertex2;
							varparentsidevertices = -2;
						}
						else if (vardistp1 <= 0 && vardistp2 > 0 && vardistp3 <= 0) {
							varfirstvertex = varvertex1;
							varsecondvertex = varvertex3;
							varparentsidevertices = -3;
						}
						else if (vardistp1 <= 0 && vardistp2 > 0 && vardistp3 > 0) {
							varfirstvertex = varvertex2;
							varsecondvertex = varvertex3;
							varparentsidevertices = 2;
						}
						else if (vardistp1 > 0 && vardistp2 <= 0 && vardistp3 <= 0) {
							varfirstvertex = varvertex2;
							varsecondvertex = varvertex3;
							varparentsidevertices = -4;
						}
						else if (vardistp1 > 0 && vardistp2 <= 0 && vardistp3 > 0) {
							varfirstvertex = varvertex1;
							varsecondvertex = varvertex3;
							varparentsidevertices = 3;
						}
						else if (vardistp1 > 0 && vardistp2 > 0 && vardistp3 <= 0) {
							varfirstvertex = varvertex1;
							varsecondvertex = varvertex2;
							varparentsidevertices = 4;
						}
						else if (vardistp1 > 0 && vardistp2 > 0 && vardistp3 > 0) {
							varparentsidevertices = 5;
						}
						
						if (varparentsidevertices > 1) {
							//the new triangle vertex couple is parent side
							switch (varparentsidevertices) {
								case 2:
									varparentpatchtrianglelist.Add(varfirstnewvertex);
									varparentpatchtrianglelist.Add(varfirstvertex);
									varparentpatchtrianglelist.Add(varsecondvertex);
									break;
								case 3:
									varparentpatchtrianglelist.Add(varfirstvertex);
									varparentpatchtrianglelist.Add(varfirstnewvertex);
									varparentpatchtrianglelist.Add(varsecondvertex);
									break;
								case 4:
									varparentpatchtrianglelist.Add(varfirstvertex);
									varparentpatchtrianglelist.Add(varsecondvertex);
									varparentpatchtrianglelist.Add(varfirstnewvertex);
									break;
								case 5:
									varparentpatchtrianglelist.Add(varvertex1);
									varparentpatchtrianglelist.Add(varvertex2);
									varparentpatchtrianglelist.Add(varvertex3);
									break;
							}
						} else if (varparentsidevertices < -1) {
							switch (varparentsidevertices) {
								case -2:
									varchildpatchtrianglelist.Add(varfirstvertex);
									varchildpatchtrianglelist.Add(varsecondvertex);
									varchildpatchtrianglelist.Add(varfirstnewvertex);
									break;
								case -3:
									varchildpatchtrianglelist.Add(varfirstvertex);
									varchildpatchtrianglelist.Add(varfirstnewvertex);
									varchildpatchtrianglelist.Add(varsecondvertex);
									break;
								case -4:
									varchildpatchtrianglelist.Add(varfirstnewvertex);
									varchildpatchtrianglelist.Add(varfirstvertex);
									varchildpatchtrianglelist.Add(varsecondvertex);
									break;
							}
						} else if (varparentsidevertices == 0) {
							varchildstumptrianglestemp.AddRange(new []{varvertex1, varvertex2, varvertex3});
						}
						//zero out the triangle on the parent, since it was 'moved' to the child
						varnewparenttriangles[vartriangleindex] = 0;
						varnewparenttriangles[vartriangleindex+1] = 0;
						varnewparenttriangles[vartriangleindex+2] = 0;
					}
					
				}
			}
			
			//setup the rigidbodies
			float varcurrentbodyside, varcurrentboneside;
			if (varnewrenderer.bones[varbonecounter].GetComponent<Rigidbody>()) {
				varnewrenderer.bones[varbonecounter].GetComponent<Rigidbody>().drag = 1;
				varnewrenderer.bones[varbonecounter].GetComponent<Rigidbody>().angularDrag = 1;
				if (varnewbody) {
					//check for joints that depend on the current rigidbody that's been cut, to replace them with a proper parent
					CharacterJoint varcurrentjointfixer = null;
					Transform varconnectedbody = null;

					//verify additional cut bones
					int varcurrentchildboneindex;
					for (int varjointcounter = 0; varjointcounter < vardismemberator.vargambonerelationsindexes[varparentboneindex].propchildrenside.propindexes.Length; varjointcounter++) {
						varcurrentchildboneindex = vardismemberator.vargambonerelationsindexes[varparentboneindex].propchildrenside.propindexes[varjointcounter];
						//Debug.Log("Checking bone " + varbones[varcurrentchildboneindex].name);
						varcurrentjointfixer = varbones[varcurrentchildboneindex].GetComponent<CharacterJoint>(); //varbones[vardismemberator.vargamrigidbodyconnections[varcurrentchildboneindex].propindexes[varconnectedbodies]].GetComponent<CharacterJoint>();
						varcurrentboneside = 	(varpslashdirection.x*varbones[varcurrentchildboneindex].position.x+varpslashdirection.y*varbones[varcurrentchildboneindex].position.y+varpslashdirection.z*varbones[varcurrentchildboneindex].position.z + 
												(-varpslashdirection.x*varpcontactpoint.x-varpslashdirection.y*varpcontactpoint.y-varpslashdirection.z*varpcontactpoint.z)) * vardistpbone;
						//Debug.Log("		Bone side " + varcurrentboneside);
						if (varcurrentjointfixer != null) {
							if (varcurrentjointfixer.connectedBody != null) {
								varconnectedbody = varcurrentjointfixer.connectedBody.transform;
								//Debug.Log(varconnectedbody.name + " " + varcurrentjointfixer.connectedBody.transform.name);
								//determine the cut plane side of the connected body
								varcurrentbodyside = 	(varpslashdirection.x*varconnectedbody.transform.position.x+varpslashdirection.y*varconnectedbody.transform.position.y+varpslashdirection.z*varconnectedbody.transform.position.z + 
														(-varpslashdirection.x*varpcontactpoint.x-varpslashdirection.y*varpcontactpoint.y-varpslashdirection.z*varpcontactpoint.z)) * vardistpbone;
								//Debug.Log("		" + varcurrentboneside + " " + varcurrentbodyside);
								if (varcurrentbodyside > 0 && varcurrentboneside <=0) {
									//connected body parent side, bone child side, attach to the child stump
									//Debug.LogWarning("we have a bone child side [" + varbones[varcurrentchildboneindex].name + "], and its connected body parent side [" + varconnectedbody.name + "]");
									varcurrentjointfixer.connectedBody = varnewbody;
									//varbones[varcurrentchildboneindex].parent = varchildstump.transform;
								}
								else if (varcurrentbodyside <=0 && varcurrentboneside >0) {
									//connected body child side, bone parent side, attach to the parent stump
									//Debug.LogWarning("we have a bone parent side [" + varbones[varcurrentchildboneindex].name + "], and its connected body child side [" + varconnectedbody.name + "]");
									varcurrentjointfixer.connectedBody = varbones[varparentboneindex].GetComponent<Rigidbody>(); //varnewbody;
									
								}
							}
						}
						//Debug.LogWarning(varbones[varcurrentchildboneindex].name + " " + varbones[varcurrentchildboneindex].parent + " " + varbones[varparentboneindex].name);
						if (varbones[varcurrentchildboneindex].parent != null) {
							if (varcurrentboneside <= 0) {
								if (varbones[varcurrentchildboneindex].parent == varbones[varparentboneindex]) {
									//Debug.LogError("Sibling");
									varbones[varcurrentchildboneindex].parent = varchildstump.transform;
								}
								else {
									varcurrentboneside = 	(varpslashdirection.x*varbones[varcurrentchildboneindex].parent.position.x+varpslashdirection.y*varbones[varcurrentchildboneindex].parent.position.y+varpslashdirection.z*varbones[varcurrentchildboneindex].parent.position.z + 
															(-varpslashdirection.x*varpcontactpoint.x-varpslashdirection.y*varpcontactpoint.y-varpslashdirection.z*varpcontactpoint.z)) * vardistpbone;
									if (varcurrentboneside > 0) {
										//Debug.LogError("Topmost child of a split sibling");
										varbones[varcurrentchildboneindex].parent = varchildstump.transform;
									}
								}
							}
						}
					}
				}
			}
			

			//this array will be compiled with the transforms of the bones that are 'children side' of the cut plane, and will be cast against the cut vertices to determine if their boneindexes need be moved or not
			Transform[] varchildsidebones = new Transform[0];
			Transform[] varparentsidebones = new Transform[0];
			//child side and parent side bones helper lists
			List<Transform> varchildsidebonescompiler = new List<Transform>();
			List<Transform> varparentsidebonescompiler = new List<Transform>();
			float varside;
			int varcurrentindex;
			for (int varsidecounter = 0; varsidecounter < vardismemberator.vargambonerelationsindexes[varparentboneindex].propchildrenside.propindexes.Length; varsidecounter++) {
				varcurrentindex = vardismemberator.vargambonerelationsindexes[varparentboneindex].propchildrenside.propindexes[varsidecounter];
				varside = (	varpslashdirection.x*varbones[varcurrentindex].position.x +
							varpslashdirection.y*varbones[varcurrentindex].position.y +
							varpslashdirection.z*varbones[varcurrentindex].position.z + 
							(-varpslashdirection.x*varpcontactpoint.x-varpslashdirection.y*varpcontactpoint.y-varpslashdirection.z*varpcontactpoint.z)) * vardistpbone;
				if (varside < 0) {
					//Debug.LogWarning("is child side " + varbones[varcurrentindex].name);
					varchildsidebonescompiler.Add(varbones[varcurrentindex]);
				}
				else {
					//Debug.LogWarning("is parent side " + varbones[varcurrentindex].name);
					varparentsidebonescompiler.Add(varbones[varcurrentindex]);
				}
			}
			if (varchildsidebonescompiler.Count > 0) {
				varchildsidebones = new Transform[varchildsidebonescompiler.Count];
				varchildsidebonescompiler.CopyTo(varchildsidebones);
			}
			if (varparentsidebonescompiler.Count > 0) {
				varparentsidebones = new Transform[varparentsidebonescompiler.Count];
				varparentsidebonescompiler.CopyTo(varparentsidebones);
			}
			
			
			//triangulate
			//this is the second variant, where we perform triangulation adding an additional vertex
			//allocate new meshes
			Mesh varnewparentmesh = new Mesh(); //(Mesh)Object.Instantiate(varoriginalmesh);
			Mesh varnewchildmesh = new Mesh();//(Mesh)Object.Instantiate(varoriginalmesh);
			
			
			//this is the actual first step in the triangulation process. we can skip, starting from here, if we avoid the creation of cut patches.
			//skipping is based on the presence of the stump material. if it's null, we will not create patch submeshes
			if (varpstumpmaterial != null) {
				//first of all we dump the varparentpatchtrianglelist and child into compressors, to determine the number of vertices that we need to add (otherwise we have problems setting up the UVs
				HashSet<int> varcompressedparentpatchvertices = new HashSet<int>();
				//Debug.LogWarning("first new vertx? " + varfirstnewvertex + " last new vertex? " + varlastnewvertex + " existing patch vertex count " + varexistingpatchvertexcount);
				if (varlastnewvertex == 0) {
					varcompressedparentpatchvertices.UnionWith(varparentpatchtrianglelist);
				}
				else {
					for (int varparentpatchtriangletricker = 0; varparentpatchtriangletricker < varparentpatchtrianglelist.Count; varparentpatchtriangletricker++ ) {
						//Debug.LogWarning("Vertex " + varparentpatchtrianglelist[varparentpatchtriangletricker]);
						if (varparentpatchtrianglelist[varparentpatchtriangletricker] < varlastnewvertex || varparentpatchtrianglelist[varparentpatchtriangletricker] == varfirstnewvertex) {
							//Debug.LogWarning("		Added");
							varcompressedparentpatchvertices.Add(varparentpatchtrianglelist[varparentpatchtriangletricker]);
						}
					}
				}
	
				HashSet<int> varcompressedchildpatchvertices = new HashSet<int>();
				varcompressedchildpatchvertices.UnionWith(varchildpatchtrianglelist);
				//we now need to resize the vertex and triangle arrays. the "+1" is the spot reserved to the only real new vertex, the cut point. The other vertices will be duplicated.
				int varnewparentverticesnumber = varoriginalvertices.Length + varcompressedparentpatchvertices.Count + 1;
				int varnewchildverticesnumber = varoriginalvertices.Length + varcompressedchildpatchvertices.Count + 1;
				int varauxcounter = 0;
	
				//vertices
				Vector3[] varnewparentverticesarray = new Vector3[varnewparentverticesnumber];
				Vector3[] varnewchildverticesarray = new Vector3[varnewchildverticesnumber];
				varoriginalvertices.CopyTo(varnewparentverticesarray,0);
				varoriginalvertices.CopyTo(varnewchildverticesarray,0);
				//insert the real new vertex
				varnewparentverticesarray[varfirstnewvertex] = varfixerplaneposition;
				varnewchildverticesarray[varfirstnewvertex] = varfixerplaneposition;
	
				//triangles
				varnewchildtriangles = new int[varchildstumptrianglestemp.Count];
				varchildstumptrianglestemp.CopyTo(varnewchildtriangles,0);
	
				//uvs
				Vector3 varfixedside = Vector3.zero;
				//we use the "real new vertex" to determine the 'angle' of each of the compressed parent and child patch triangle vertices
				varfixedside = varfixerplaneposition;
				varfixedside.y += 1;
				varfixedside = Vector3.up;
				float varanglestep = (varcompressedparentpatchvertices.Count > 0 ? 360 / varcompressedparentpatchvertices.Count : 0);
				float varthisangle = 0;
				float varthisanglex = 0;
				float varthisangley = 0;
				Vector2[] varnewparentverticesuvs = new Vector2[varnewparentverticesnumber];
				Vector2[] varnewchildverticesuvs = new Vector2[varnewchildverticesnumber];
				varoriginaluvs.CopyTo(varnewparentverticesuvs,0);
				varoriginaluvs.CopyTo(varnewchildverticesuvs,0);
				int varsortedstumpscount, varinsertindex;
				List<int> varsortedstumpvertices = new List<int>();
				float[] varsortedstumpverticesangles;
				//assign the uv to the new vertex
				varnewparentverticesuvs[varfirstnewvertex] = new Vector2(0.5f, 0.5f);
				varnewchildverticesuvs[varfirstnewvertex] = new Vector2(0.5f, 0.5f);
	
				//boneweights
				BoneWeight[] varnewparentverticesboneweights = new BoneWeight[varnewparentverticesnumber];
				BoneWeight[] varnewchildverticesboneweights = new BoneWeight[varnewchildverticesnumber];
				varoriginalboneweights.CopyTo(varnewparentverticesboneweights,0);
				varoriginalboneweights.CopyTo(varnewchildverticesboneweights,0);
				//copy the 'first' vertex boneweight available into the new position. this is not vital to calculate and the copy should be enough
				varnewparentverticesboneweights[varfirstnewvertex].boneIndex0 = varparentboneindex;
				varnewparentverticesboneweights[varfirstnewvertex].weight0 = 1;
				varnewchildverticesboneweights[varfirstnewvertex].boneIndex0 = varparentboneindex;
				varnewchildverticesboneweights[varfirstnewvertex].weight0 = 1;
	
				//normals
				Vector3[] varnewparentverticesnormals = new Vector3[varnewparentverticesnumber];
				Vector3[] varnewchildverticesnormals = new Vector3[varnewchildverticesnumber];
				varoriginalnormals.CopyTo(varnewparentverticesnormals,0);
				varoriginalnormals.CopyTo(varnewchildverticesnormals,0);
				//create the normal vector for the parent stump
				Vector3 varnormalreferenceparent =  (vardismemberator.vargamoriginalbonepositions[varbonecounter] - vardismemberator.vargamoriginalbonepositions[varparentboneindex]).normalized;
				//create the normal vector for the child stump
				Vector3 varnormalreferencechild = (vardismemberator.vargamoriginalbonepositions[varparentboneindex] - vardismemberator.vargamoriginalbonepositions[varbonecounter]).normalized;
				//setup the normals
				varnewparentverticesnormals[varfirstnewvertex] = varnormalreferenceparent;
				varnewchildverticesnormals[varfirstnewvertex] = varnormalreferencechild;
				
				//cycle through the varcompressedparentpatchvertices array, to project the stump vertices onto the cut plane, and create a sorted-by-clockwise-angle list varsortedstumpvertices
				int varcurrentnewvertex, varlastcompressedvertexindex;
				Vector3 varoriginalvertexvector, varprojectedvertex;
				Vector2 ab, cb;
				float angba, angbc, rslt, rs;
				int[] varsortedcompressedpatchvertices = new int[0];
				int[] vartempcompressor = new int[0];
				varauxcounter = 0;
				
				//parent reformula start
				varsortedstumpverticesangles = new float[varcompressedparentpatchvertices.Count];
				foreach (int varcurrentcompressedvertexindex in varcompressedparentpatchvertices) {
					//this is sort of a replication, but we need the condition to avoid crashing the routine in "varoriginalvertices[varcurrentcompressedvertexindex]" when the current vertex index equals the "real new vertex", that still doesn't exist in the vertices array
					if (varcurrentcompressedvertexindex == varfirstnewvertex) {
						//Debug.Log("Angle " + varauxcounter + " for " + varcurrentcompressedvertexindex + " : 0");
						varsortedstumpverticesangles[varauxcounter] = 0;
						varsortedstumpvertices.Insert(0, varauxcounter);						
						varauxcounter++;
						continue;
					}
					//cache the current vertex coordinates, retrieving them from the original mesh, and using the vartstumpvertices array and counter
					varoriginalvertexvector = varoriginalvertices[varcurrentcompressedvertexindex];
					//calculate the vector3 that's the projection of the current original vertex onto the cut plane, and store it into the varstumpverticescutplaneprojections array
					varprojectedvertex = varnewplane.transform.InverseTransformPoint(varoriginalvertexvector) - (Vector3.Dot(varnewplane.transform.InverseTransformPoint(varoriginalvertexvector), varnewplane.transform.InverseTransformDirection(varfixerplanenormal.normalized)) * varnewplane.transform.InverseTransformDirection(varfixerplanenormal.normalized));
					
					//metcreateplaceholder(0.03f, Color.black, varprojectedvertex, "projected "+ varauxcounter + "-" + varcurrentcompressedvertexindex);
					//calculate the 'angle' of the current stump vertex in respect to the "real new vertex" and its vertical projection
					//we do it inline to speed up execution by 60%
					//varthisangle = metthreepointangle(varfixedside, Vector3.zero, varprojectedvertex);
					ab.x = -varfixedside.x;
					ab.y = -varfixedside.y;
					cb.x = -varprojectedvertex.x;
					cb.y = -varprojectedvertex.y;
					angba = Mathf.Atan2(ab.y, ab.x);
					angbc = Mathf.Atan2(cb.y, cb.x);
					rslt = angba - angbc;
					rs = (rslt * 180) / Mathf.PI;
					varthisangle = (rs > 0 ? 360 - rs : -rs);
	
					//Debug.Log("Angle " + varauxcounter + " for " + varcurrentcompressedvertexindex + " :" + varthisangle );
					//store the angle to achieve a varcompressedparentpatchvertices - varsortedstumpverticesangles 1 : 1 relation
					varsortedstumpverticesangles[varauxcounter] = varthisangle;
	
					varsortedstumpscount = varsortedstumpvertices.Count;
					varinsertindex = -1;
					for (int varsortedstumpcounter = 0; varsortedstumpcounter < varsortedstumpscount; varsortedstumpcounter++) {
						if (varthisangle < varsortedstumpverticesangles[varsortedstumpvertices[varsortedstumpcounter]]) {
							varinsertindex = varsortedstumpcounter;
							break;
						}
					}
					if (varinsertindex > -1) {
						varsortedstumpvertices.Insert(varinsertindex, varauxcounter);						
						//Debug.LogWarning("Inserting " + varcurrentangle + " in " + varinsertindex);
					}
					else {
						varsortedstumpvertices.Add(varauxcounter);
						//Debug.LogWarning("Adding " + varcurrentangle);
					}
					varauxcounter++;
				}
				varsortedcompressedpatchvertices = new int[varcompressedparentpatchvertices.Count];
				vartempcompressor = new int[varcompressedparentpatchvertices.Count];
				varcompressedparentpatchvertices.CopyTo(vartempcompressor,0);
				for (int varsortedvertexcounter = 0; varsortedvertexcounter < varsortedcompressedpatchvertices.Length; varsortedvertexcounter++) {
					varsortedcompressedpatchvertices[varsortedvertexcounter] = vartempcompressor[varsortedstumpvertices[varsortedvertexcounter]];
					//metcreateplaceholder(0.04f, Color.magenta, (varsortedcompressedparentpatchvertices[varsortedvertexcounter] == varfirstnewvertex ? varfixerplaneposition : varoriginalvertices[varsortedcompressedparentpatchvertices[varsortedvertexcounter]]), "sorted"+varsortedvertexcounter+"-"+varsortedcompressedparentpatchvertices[varsortedvertexcounter]);
				}
				varcompressedparentpatchvertices.Clear();
				varcompressedparentpatchvertices.UnionWith(varsortedcompressedpatchvertices);
				
				//use the parent patch compressor to add the new vertices to the new parent vertices array, and to substitute the original vertices in the parent patch triangle list
				varcurrentnewvertex = varfirstnewvertex + 1;
				varlastcompressedvertexindex = 0;
				varauxcounter = 0;
				foreach (int varcurrentcompressedvertexindex in varcompressedparentpatchvertices) {
					//Debug.LogError("current vertx " + varcurrentcompressedvertexindex);
					//skip the real new vertex, since it doesn't exist in the original vertices array
					if (varcurrentcompressedvertexindex == varfirstnewvertex) {
						varlastcompressedvertexindex = varcurrentcompressedvertexindex;
						continue;
					}
					//copy the original vertex into the new position
					varnewparentverticesarray[varcurrentnewvertex] = varoriginalvertices[varcurrentcompressedvertexindex];
					//scan the parent patch triangles and substitute the original vertex index with the new vertex index
					for (int varpatchvertexcounter = 0; varpatchvertexcounter < varparentpatchtrianglelist.Count; varpatchvertexcounter++) { //varparentpatchtrianglelist.Count; varparentpatchvertexcounter++) {
						if (varparentpatchtrianglelist[varpatchvertexcounter] == varcurrentcompressedvertexindex) {
							varparentpatchtrianglelist[varpatchvertexcounter] = varcurrentnewvertex;
						}
					}
					//setup the UV calculating the 'ratio' of the current patch vertex based on a projection into the standard normalized uv rectangle (coordinates 0,0; 1,0; 1,1; 0,1)
					varthisangle = varauxcounter * varanglestep;
					if (varthisangle <= 90) {
						varthisanglex = varthisangle / 90;
						varthisangley = 0;
					}
					else if (varthisangle <= 180) {
						varthisanglex = 1;
						varthisangley = (varthisangle-90) / 90;
					}
					else if (varthisangle <= 270) {
						varthisanglex = (270-varthisangle)/90;
						varthisangley = 1;
					}
					else if (varthisangle <= 360) {
						varthisanglex = 0;
						varthisangley = (360-varthisangle)/90;
					}
					//assign the calculated uv to the current vertex in the uv array
					//Debug.Log(varcurrentnewvertex + " angle step " + varanglestep + " current angle " + varthisangle + " coordinate x " + varthisanglex + " y " + varthisangley);
					if (varlastcompressedvertexindex != varfirstnewvertex && varoriginalvertices[varcurrentcompressedvertexindex] == varoriginalvertices[varlastcompressedvertexindex]) {
						//the current vertex coordinates are the same of the last vertex (they're sorted so it means that the two are cohincident), so we copy the uvs
						varnewparentverticesuvs[varcurrentnewvertex] = varnewparentverticesuvs[varcurrentnewvertex-1];
					}
					else {
						//assign the new uv
						varnewparentverticesuvs[varcurrentnewvertex] = new Vector2(varthisanglex, varthisangley);
					}
	
					//copy the original vertex boneweight into the new position
					varnewparentverticesboneweights[varcurrentnewvertex] = varoriginalboneweights[varcurrentcompressedvertexindex];
					//setup the normal
					varnewparentverticesnormals[varcurrentnewvertex] = varnormalreferenceparent;
					//setup the last varcurrentcompressedvertexindex (important: this must happen on 'continue', too)
					varlastcompressedvertexindex = varcurrentcompressedvertexindex;
					//increase the counters
					varcurrentnewvertex++;
					varauxcounter++;
				}
				//parent reformula end
	
	// *** child reformula start
				varauxcounter = 0;
				varsortedstumpvertices.Clear();
				varsortedstumpverticesangles = new float[varcompressedchildpatchvertices.Count];
				foreach (int varcurrentcompressedvertexindex in varcompressedchildpatchvertices) {
					//this is sort of a replication, but we need the condition to avoid crashing the routine in "varoriginalvertices[varcurrentcompressedvertexindex]" when the current vertex index equals the "real new vertex", that still doesn't exist in the vertices array
					if (varcurrentcompressedvertexindex == varfirstnewvertex) {
						//Debug.Log("Angle " + varauxcounter + " for " + varcurrentcompressedvertexindex + " : 0");
						varsortedstumpverticesangles[varauxcounter] = 0;
						varsortedstumpvertices.Insert(0, varauxcounter);						
						varauxcounter++;
						continue;
					}
					//cache the current vertex coordinates, retrieving them from the original mesh, and using the vartstumpvertices array and counter
					varoriginalvertexvector = varoriginalvertices[varcurrentcompressedvertexindex];
					//calculate the vector3 that's the projection of the current original vertex onto the cut plane, and store it into the varstumpverticescutplaneprojections array
					varprojectedvertex = varnewplane.transform.InverseTransformPoint(varoriginalvertexvector) - (Vector3.Dot(varnewplane.transform.InverseTransformPoint(varoriginalvertexvector), varnewplane.transform.InverseTransformDirection(varfixerplanenormal.normalized)) * varnewplane.transform.InverseTransformDirection(varfixerplanenormal.normalized));
					
					//metcreateplaceholder(0.03f, Color.black, varprojectedvertex, "projected "+ varauxcounter + "-" + varcurrentcompressedvertexindex);
					//calculate the 'angle' of the current stump vertex in respect to the "real new vertex" and its vertical projection
					//we do it inline to speed up execution by 60%
					ab.x = -varfixedside.x;
					ab.y = -varfixedside.y;
					cb.x = -varprojectedvertex.x;
					cb.y = -varprojectedvertex.y;
					angba = Mathf.Atan2(ab.y, ab.x);
					angbc = Mathf.Atan2(cb.y, cb.x);
					rslt = angba - angbc;
					rs = (rslt * 180) / Mathf.PI;
					varthisangle = (rs > 0 ? 360 - rs : -rs);
	
					//Debug.Log("Angle " + varauxcounter + " for " + varcurrentcompressedvertexindex + " :" + varthisangle );
					//store the angle to achieve a varcompressedchildpatchvertices - varsortedstumpverticesangles 1 : 1 relation
					varsortedstumpverticesangles[varauxcounter] = varthisangle;
	
					varsortedstumpscount = varsortedstumpvertices.Count;
					varinsertindex = -1;
					for (int varsortedstumpcounter = 0; varsortedstumpcounter < varsortedstumpscount; varsortedstumpcounter++) {
						if (varthisangle < varsortedstumpverticesangles[varsortedstumpvertices[varsortedstumpcounter]]) {
							varinsertindex = varsortedstumpcounter;
							break;
						}
					}
					if (varinsertindex > -1) {
						varsortedstumpvertices.Insert(varinsertindex, varauxcounter);						
						//Debug.LogWarning("Inserting " + varcurrentangle + " in " + varinsertindex);
					}
					else {
						varsortedstumpvertices.Add(varauxcounter);
						//Debug.LogWarning("Adding " + varcurrentangle);
					}
					varauxcounter++;
				}
				varsortedcompressedpatchvertices = new int[varcompressedchildpatchvertices.Count];
				vartempcompressor = new int[varcompressedchildpatchvertices.Count];
				varcompressedchildpatchvertices.CopyTo(vartempcompressor,0);
				for (int varsortedvertexcounter = 0; varsortedvertexcounter < varsortedcompressedpatchvertices.Length; varsortedvertexcounter++) {
					varsortedcompressedpatchvertices[varsortedvertexcounter] = vartempcompressor[varsortedstumpvertices[varsortedvertexcounter]];
					//metcreateplaceholder(0.04f, Color.magenta, (varsortedcompressedchildpatchvertices[varsortedvertexcounter] == varfirstnewvertex ? varfixerplaneposition : varoriginalvertices[varsortedcompressedchildpatchvertices[varsortedvertexcounter]]), "sorted"+varsortedvertexcounter+"-"+varsortedcompressedchildpatchvertices[varsortedvertexcounter]);
				}
				varcompressedchildpatchvertices.Clear();
				varcompressedchildpatchvertices.UnionWith(varsortedcompressedpatchvertices);
				
				//use the child patch compressor to add the new vertices to the new child vertices array, and to substitute the original vertices in the child patch triangle list
				varcurrentnewvertex = varfirstnewvertex + 1;
				varlastcompressedvertexindex = 0;
				varauxcounter = 0;
				foreach (int varcurrentcompressedvertexindex in varcompressedchildpatchvertices) {
					//skip the real new vertex, since it doesn't exist in the original vertices array
					if (varcurrentcompressedvertexindex == varfirstnewvertex) {
						varlastcompressedvertexindex = varcurrentcompressedvertexindex;
						continue;
					}
					//copy the original vertex into the new position
					varnewchildverticesarray[varcurrentnewvertex] = varoriginalvertices[varcurrentcompressedvertexindex];
					//scan the child patch triangles and substitute the original vertex index with the new vertex index
					for (int varpatchvertexcounter = 0; varpatchvertexcounter < varchildpatchtrianglelist.Count; varpatchvertexcounter++) { //varchildpatchtrianglelist.Count; varchildpatchvertexcounter++) {
						if (varchildpatchtrianglelist[varpatchvertexcounter] == varcurrentcompressedvertexindex) {
							varchildpatchtrianglelist[varpatchvertexcounter] = varcurrentnewvertex;
						}
					}
					//setup the UV calculating the 'ratio' of the current patch vertex based on a projection into the standard normalized uv rectangle (coordinates 0,0; 1,0; 1,1; 0,1)
					varthisangle = varauxcounter * varanglestep;
					if (varthisangle <= 90) {
						varthisanglex = varthisangle / 90;
						varthisangley = 0;
					}
					else if (varthisangle <= 180) {
						varthisanglex = 1;
						varthisangley = (varthisangle-90) / 90;
					}
					else if (varthisangle <= 270) {
						varthisanglex = (270-varthisangle)/90;
						varthisangley = 1;
					}
					else if (varthisangle <= 360) {
						varthisanglex = 0;
						varthisangley = (360-varthisangle)/90;
					}
					//assign the calculated uv to the current vertex in the uv array
					//Debug.Log(varcurrentnewvertex + " angle step " + varanglestep + " current angle " + varthisangle + " coordinate x " + varthisanglex + " y " + varthisangley);
					if (varlastcompressedvertexindex != varfirstnewvertex && varoriginalvertices[varcurrentcompressedvertexindex] == varoriginalvertices[varlastcompressedvertexindex]) {
						//the current vertex coordinates are the same of the last vertex (they're sorted so it means that the two are cohincident), so we copy the uvs
						varnewchildverticesuvs[varcurrentnewvertex] = varnewchildverticesuvs[varcurrentnewvertex-1];
					}
					else {
						//assign the new uv
						varnewchildverticesuvs[varcurrentnewvertex] = new Vector2(varthisanglex, varthisangley);
					}
	
					//copy the original vertex boneweight into the new position
					varnewchildverticesboneweights[varcurrentnewvertex] = varoriginalboneweights[varcurrentcompressedvertexindex];
					//setup the normal
					varnewchildverticesnormals[varcurrentnewvertex] = varnormalreferencechild;
					//setup the last varcurrentcompressedvertexindex (important: this must happen on 'continue', too)
					varlastcompressedvertexindex = varcurrentcompressedvertexindex;
					//increase the counters
					varcurrentnewvertex++;
					varauxcounter++;
				}
	//*** child reformula end
				
	//*** finalize changes
				//parent mesh components
				varnewparentmesh.vertices = varnewparentverticesarray;
				varnewparentmesh.normals = varnewparentverticesnormals;
				varnewparentmesh.uv = varnewparentverticesuvs;
				//varnewparentmesh.uv2 = varoriginaluv2s; //varnewuv2;
				
				varnewparentmesh.boneWeights = varnewparentverticesboneweights;
				//assign the modified triangle array back to the parent mesh. this array is basically the original, minus the triangles that were found 'child side' of the cut plane. those triangles are simply '0-0-0',
				//so the triangle array dimension is not changed (faster cutting than resizing the original array)
				varnewparentmesh.triangles = varnewparenttriangles;//varnewparenttrianglesarray;
				
				//add the stump material to the renderer of the stump. this is always necessary
				varnewrenderer.materials = vardismemberator.vargamskinnedmeshrenderer.materials;
				//child mesh components
				varnewchildmesh.vertices = varnewchildverticesarray;
				varnewchildmesh.normals = varnewchildverticesnormals;
				varnewchildmesh.uv = varnewchildverticesuvs;
				varnewchildmesh.boneWeights = varnewchildverticesboneweights;
				varnewchildmesh.triangles = varnewchildtriangles;
				
				//set the triangles of the current submeshes to add the new or modified triangles of the cut patch
				if (varpatchmaterialindex > -1) {
					//we already have the material in the materials array, so we must resize the current triangles array with the same material, and add the new cut's patch triangles to it
					varnewparentmesh.subMeshCount = varoriginalmesh.subMeshCount;
					varnewchildmesh.subMeshCount = varoriginalmesh.subMeshCount;
				}
				else {
					varpatchmaterialindex = varoriginalmesh.subMeshCount;
					varnewparentmesh.subMeshCount = varpatchmaterialindex+1;
					varnewchildmesh.subMeshCount = varpatchmaterialindex+1;
				}
				
				//'transfer' the modified triangle array into the relative submeshes. since the triangle array dimension has not been changed,
				//this equals to a simple subdivision of the triangle array: triangles array = submesh1 + submesh2 + ... + submeshn
				//submeshes array themselves don't change in dimension
				int varsubmeshlength;
				int varsubmeshlengthlast = 0;
				int[] varcurrentsubmesh;
				for (int varsubmeshcounter = 0; varsubmeshcounter < varpatchmaterialindex; varsubmeshcounter++) {
					varsubmeshlength = varoriginalmesh.GetTriangles(varsubmeshcounter).Length;
					varcurrentsubmesh = new int[varsubmeshlength];
					for (int varsubmeshtrianglecounter = 0; varsubmeshtrianglecounter < varsubmeshlength; varsubmeshtrianglecounter++) {
						varcurrentsubmesh[varsubmeshtrianglecounter] = varnewparenttriangles[varsubmeshtrianglecounter + varsubmeshlengthlast];
					}
					varnewparentmesh.SetTriangles(varcurrentsubmesh, varsubmeshcounter);
					varsubmeshlengthlast = varsubmeshlength;
				}
				
				//actually set the triangles of the parent stump submesh
				varnewparentmesh.SetTriangles(varparentpatchtrianglelist.ToArray(),varpatchmaterialindex);
				//set the triangles of the child stump submesh
				varnewchildmesh.SetTriangles(varchildpatchtrianglelist.ToArray(),varpatchmaterialindex);
			}
			else {
				/*
				//boneweights
				BoneWeight[] varnewparentverticesboneweights = new BoneWeight[varnewparentverticesnumber];
				BoneWeight[] varnewchildverticesboneweights = new BoneWeight[varnewchildverticesnumber];
				varoriginalboneweights.CopyTo(varnewparentverticesboneweights,0);
				varoriginalboneweights.CopyTo(varnewchildverticesboneweights,0);
				//copy the 'first' vertex boneweight available into the new position. this is not vital to calculate and the copy should be enough
				varnewparentverticesboneweights[varfirstnewvertex].boneIndex0 = varparentboneindex;
				varnewparentverticesboneweights[varfirstnewvertex].weight0 = 1;
				varnewchildverticesboneweights[varfirstnewvertex].boneIndex0 = varparentboneindex;
				varnewchildverticesboneweights[varfirstnewvertex].weight0 = 1;
				*/
				
				//parent mesh components
				varnewparentmesh.vertices = varoriginalmesh.vertices;
				varnewparentmesh.normals = varoriginalmesh.normals;
				varnewparentmesh.uv = varoriginalmesh.uv;
				varnewparentmesh.uv2 = varoriginalmesh.uv2;
				varnewparentmesh.boneWeights = varoriginalmesh.boneWeights; //varnewparentverticesboneweights;
				varnewparentmesh.triangles = varnewparenttriangles;

				//add the original materials to the renderer of the child stump
				varnewrenderer.materials = vardismemberator.vargamskinnedmeshrenderer.materials;

				//child triangles
				varnewchildtriangles = new int[varchildstumptrianglestemp.Count];
				varchildstumptrianglestemp.CopyTo(varnewchildtriangles,0);

				//child mesh components
				varnewchildmesh.vertices = varoriginalmesh.vertices;
				varnewchildmesh.normals = varoriginalmesh.normals;
				varnewchildmesh.uv = varoriginalmesh.uv;
				varnewchildmesh.uv2 = varoriginalmesh.uv2;
				varnewchildmesh.boneWeights = varoriginalmesh.boneWeights; //varnewchildverticesboneweights;
				varnewchildmesh.triangles = varnewchildtriangles;

				//'transfer' the modified triangle array into the relative submeshes. since the triangle array dimension has not been changed,
				//this equals to a simple subdivision of the triangle array: triangles array = submesh1 + submesh2 + ... + submeshn
				//submeshes array themselves don't change in dimension
				int varsubmeshes = varoriginalmesh.subMeshCount;
				varnewparentmesh.subMeshCount = varsubmeshes;
				varnewchildmesh.subMeshCount = varsubmeshes;
				int varsubmeshlength;
				int varsubmeshlengthlast = 0;
				int[] varcurrentsubmesh;
				for (int varsubmeshcounter = 0; varsubmeshcounter < varsubmeshes; varsubmeshcounter++) {
					varsubmeshlength = varoriginalmesh.GetTriangles(varsubmeshcounter).Length;
					varcurrentsubmesh = new int[varsubmeshlength];
					for (int varsubmeshtrianglecounter = 0; varsubmeshtrianglecounter < varsubmeshlength; varsubmeshtrianglecounter++) {
						varcurrentsubmesh[varsubmeshtrianglecounter] = varnewparenttriangles[varsubmeshtrianglecounter + varsubmeshlengthlast];
					}
					varnewparentmesh.SetTriangles(varcurrentsubmesh, varsubmeshcounter);
					varsubmeshlengthlast = varsubmeshlength;
				}
			
			}
			
			Transform[] varnewbones = new Transform[vardismemberator.vargamskinnedmeshrenderer.bones.Length];
			Matrix4x4[] varnewbonesbindposes = new Matrix4x4[varoriginalmesh.bindposes.Length];
			vardismemberator.vargamskinnedmeshrenderer.bones.CopyTo(varnewbones, 0);
			varoriginalmesh.bindposes.CopyTo(varnewbonesbindposes, 0);
			Transform[] varparentbones = vardismemberator.vargamskinnedmeshrenderer.bones;
			Matrix4x4[] varparentbonebindposes = varoriginalmesh.bindposes;

			//disconnect the parent side bones of the child stump
			int vardetachindex;
			//bone reassignment
			//child side
			//"reset" all parent side bone positions to the child stump
			for (int vardetachcounter = 0; vardetachcounter < varnewbones.Length; vardetachcounter++) {
				varnewbones[vardetachcounter] = varchildstump.transform;
				varnewbonesbindposes[vardetachcounter] = varnewbonesbindposes[varparentboneindex];
			}
			//setup back only those bones that have been modified
			for (int vardetachcounter = 0; vardetachcounter < varchildsidebones.Length; vardetachcounter++) {
				vardetachindex = vardismemberator.vargamboneindexes.IndexOf(varchildsidebones[vardetachcounter]);
				if (vardetachindex == -1) {
					//Debug.Log("Skipping " + varchildsidebones[vardetachcounter].name);
					continue;
				}
				varnewbones[vardetachindex] = varchildsidebones[vardetachcounter];
				varnewbonesbindposes[vardetachindex] = varparentbonebindposes[vardetachindex];
				
				varparentbones[vardetachindex] = varparentstump.transform;
				varparentbonebindposes[vardetachindex] = varparentbonebindposes[varparentboneindex];
			}
			
			//reassign to the renderer child side
			varnewrenderer.bones = varnewbones;
			varnewchildmesh.bindposes = varnewbonesbindposes;
			//reassign to the mesh parent side
			vardismemberator.vargamskinnedmeshrenderer.bones = varparentbones;
			varnewparentmesh.bindposes = varparentbonebindposes;
			
			//replace the original shared mesh of the parent with the modified one. this will automatically accomodate references to the triangles, bones and vertexes array
			//NOTE: this should be the very last step in the cutting process, since changes are made final and moved onto the current gameobject in the scene
			vardismemberator.vargamskinnedmeshrenderer.sharedMesh = varnewparentmesh;
			//assign the new mesh object to the child stump renderer created during the process
			varnewrenderer.sharedMesh = varnewchildmesh;

			//turn all of the stump child rigidbodies into physic driven if the corresponding parameter is true
			if (varpcinematiccut) {
				Rigidbody[] varstumpbodies = varchildstump.GetComponentsInChildren<Rigidbody>();
				for (int varchildcounter = 0; varchildcounter < varstumpbodies.Length; varchildcounter++) {
					varstumpbodies[varchildcounter].isKinematic = false;
				}
			}

			//introduce multi threading
			//http://forum.unity3d.com/threads/90676-Multithreading-Solutions
			//using System.Threading;
			
			
		}
		return true;
	}
	
	/// <summary>
	/// utility function to display messages in behalf of Debug.Log
	/// </summary>
	/// <param name="varpmessage">
	/// The message to be sent to the console
	/// </param>
	/// <param name="varplevel">
	/// The level of the message. 0 = log, 1 = warning, 2 = error, 4 = warning if verbose
	/// </param>
	/// <param name="varpverbose">
	/// OPTIONAL default false. Ignores log level messages unless true.
	/// </param>
	/// <param name="varptarget">
	/// OPTIONAL default null. if specified as a transform or gameobject, will be used to give focus to the log entry.
	/// </param>
	public static void metprint(string varpmessage, int varplevel, bool varpverbose = false, Object varptarget = null) {
		switch (varplevel) {
		case 0:
			if (varpverbose==true)
				Debug.Log(varpmessage, varptarget);
			break;
		case 1:
			Debug.LogWarning(varpmessage, varptarget);
			break;
		case 2:
			Debug.LogError(varpmessage, varptarget);
			break;
		case 4:
			if (varpverbose==true)
				Debug.LogWarning(varpmessage, varptarget);
			break;
		default:
			Debug.Log(varpmessage, varptarget);
			break;
		}
	}
	
}
