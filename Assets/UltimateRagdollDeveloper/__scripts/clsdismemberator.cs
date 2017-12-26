using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 2012-10-27
/// URG ADVANCED FUNCTIONS BONE SEPARATION DATA STRUCTURES
/// 
/// This class is injected with its 
/// © THE ARC GAMES STUDIO 2012
/// DESIGNED WITH UNITY 3.5.6f4
/// 
/// START THE APPLICATION FROM UNITY'S MENU "GAMEOBJECT=> CREATE OTHER=> ULTIMATE RAGDOLL"
/// REFER TO THE README FILE FOR USAGE SPECIFICATIONS
/// 
/// NOTE: all hidden elements are public for modular acces' sake, but tinkering with them
/// will have unexpected and most certainly play mode consequences
/// </summary>
public class clsdismemberator : MonoBehaviour {
	/// <summary>
	/// reference to the skinned mesh renderer host from which all the data is compiled
	/// </summary>
	public SkinnedMeshRenderer vargamskinnedmeshrenderer;
	/// <summary>
	/// reference to the material that can be passed to cutting routines.
	/// NOTE: this variable is here for convenience only, it is not used automatically by cutting routines.
	/// </summary>
	public Material vargamstumpmaterial = null;
	/// <summary>
	/// reference to the gameobject that can be spawned parent side after a cut
	/// NOTE: this variable is here for convenience only, it is not used automatically by cutting routines.
	/// </summary>
	public GameObject vargamparticleparent = null;
	/// <summary>
	/// reference to the gameobject that can be spawned child side after a cut
	/// NOTE: this variable is here for convenience only, it is not used automatically by cutting routines.
	/// </summary>
	public GameObject vargamparticlechild = null;
	[HideInInspector]
	/// <summary>
	/// mesh pivot host. its transform is used for all trigonometric translations
	/// </summary>
	public Transform vargamfixer;
	[HideInInspector]
	/// <summary>
	/// shortcut to the fixer's euler angle in respect to the origin
	/// </summary>
	public Vector3 vargamfixereuler;
	[HideInInspector]
	/// <summary>
	/// shortcut to the original bone positions for cut plane comparisons
	/// </summary>
	public Vector3[] vargamoriginalbonepositions = new Vector3[0];
	[HideInInspector]
	/// <summary>
	/// shortcut to the original bone angles for direction determination
	/// </summary>
	public Vector3[] vargamoriginalboneangles = new Vector3[0];
	/*
	[HideInInspector]
	/// <summary>
	/// original bone rotations in quaternions for ease of computing. currently unused. available for decomment in dismemberator editor.
	/// </summary>
	public Quaternion[] vargamoriginalbonerotations = new Quaternion[0];
	*/
	[HideInInspector]
	/// <summary>
	/// stores the dependencies between the current bone and its joint-rigidbody connections, to properly disconnect a bone when its cut, and it's used by other bones as a joint connectedbody
	/// IMPORTANT: setup is unusual but convenient for the operations to be performed
	/// propindexes[0] is always equal to the -bone's- joint connectedbody's transform bone index, eventual other indexes are of depending bones.
	/// </summary>
	public clsdismemberatorindexer[] vargamrigidbodyconnections;
	[HideInInspector]
	/// <summary>
	/// an actual 'triangle like' array that holds the vertex indexes in groups of three, for each triangle that's connected to each bone by means of vertex-boneweight-to-bone, and triangles-from-such-vertex
	/// </summary>
	public clsdismemberatorindexer[] vargambonetriangles;
	[HideInInspector]
	/// <summary>
	/// a dismemberator bone indexer, holding the first index of each if its child triangles in the triangles array of the skinned mesh
	/// this means that, in a 9 vertex mesh with three bones, indexes 0, 1 and 2
	/// if there's three triangles and the triangle array is
	/// 1st tri|     2nd tri|    3rd tri
	/// 0, 1, 2,     6, 7, 8,    3, 4, 5
	/// and bone 1 is bound to vertices 7 and 4 (so the boneweight for these vertices is 7 and 4)
	/// the variable vargambonetrianglesindexes will hold, at index 1 [the value of the bone] a propindexes array with values 3 and 6 (which in the triangles array correspond to vertices 6 and 3 respectively, and from which we calculate +1
	/// and +2 for the additional vertices of the same triangle)
	/// </summary>
	public clsdismemberatorindexer[] vargambonetrianglesindexes;
	[HideInInspector]
	/// <summary>
	/// an extended version of the vargambonetrianglesindexes list, which holds -all- triangles of this particular bone, meaning it holds all of its child bone triangles indexes too (it's compiled with a compressor
	/// so there are no index duplicates).
	/// </summary>
	public clsdismemberatorindexer[] vargambonefulltrianglesindexes;
	[HideInInspector]
	/// <summary>
	/// holds the vertices indexes for all those vertices that are weighted to each bone
	/// </summary>
	public clsdismemberatorindexer[] vargambonevertices;
	[HideInInspector]
	/// <summary>
	/// a complex data structure to hold the vertex indexes of a bone where at least a boneindex of that vertex is in the 'parent side' of the armature (will store all the vertices of the children of the current bone),
	/// with a specific additional index that refers to the 'boneWeightN'. compiled to achieve superior performance detaching bone weights during stump transfer after a cut
	/// </summary>
	public clsdismemberatorvertexindexer[] vargamboneverticesfullindex;
	/*
	[HideInInspector]
	/// <summary>
	/// this structure holds the vertex array, and for each a boneindex0-normal-bound clockwise vertex indexer of its neighbours, to allow creation of cut loops
	/// NOTE: currently unused. available for decomment in dismemberator editor.
	/// </summary>
	public clsdismemberatorindexer[] vargamverticesneighbours;
	*/
	[HideInInspector]
	/// <summary>
	/// the loop of clockwise vertex indices that constitute the bone's detachment gap, to be used by the bone detachment routine
	/// NOTE: these are not suitable for normal cut operations, since they're not dynamic
	/// </summary>
	public clsdismemberatorindexer[] vargamboneseparationvertices;
	[HideInInspector]
	/// <summary>
	/// companion array to vargamboneseparationvertices, will hold the precompiled uv mapping of the separation vertices loops, to save on computing time during separations
	/// </summary>
	public clsdismemberatorseparationverticesuvhelper[] vargamboneseparationverticesuvhelper;
	[HideInInspector]
	/// <summary>
	/// dynamic data: for each cut performed, will store the triangle[] indexes of the related patch triangles, so that the patch triangles can be removed from the triangle array
	/// if a subsequent parent cut is performed (otherwise, those triangles 'float in midair')
	/// </summary>
	public clsdismemberatorindexer[] vargamboneseparationpatchtriangleindexes;
	[HideInInspector]
	/// <summary>
	/// a list that stores the bone index value of the skinned mesh renderer for each transform, to be retrieved thanks to the 'indexof' method of the List class.
	/// </summary>
	public List<Transform> vargamboneindexes;
	[HideInInspector]
	/// <summary>
	/// shortcut array that holds the bones[] index of each bone's parent, -1 in case the bone has no parent, and -2 in case the parent is not indexed
	/// </summary>
	public int[] vargamboneindexesparents;
	[HideInInspector]
	/// <summary>
	/// shortcut array that holds the bones[] index for the eventual characterjoint 'connectedbody', and -1 if there's no characterjoint, -2 if the connectedbody is not indexed and -3 if there's no connectedbody
	/// NOTE: this information runs parallel to vargamrigidbodyconnections[N].propindexes[0] and is compiled for ease of access
	/// </summary>
	public int[] vargamboneindexescharacterjointconnect;
	[HideInInspector]
	/// <summary>
	/// a straightforward indexer that holds the indexes of all parents and children of each bone. NOTE: does not include the current bone amongst the indexes
	/// </summary>
	public clsdismemberatorbonerelationsindexes[] vargambonerelationsindexes;
	[HideInInspector]
	/// <summary>
	/// dynamic data: this is an important variable holds a meta coordinate: for each mesh.triangles[] index, the .x coordinate specifies the submesh that triangle is in, and the .y indicates the .submesh[] array index.
	/// it is used to replicate mesh.triangles[] modifications after a cut, into the submeshes arrays.
	/// </summary>
	public Vector2[] vargamboneseparationsubmeshhelper;
	[HideInInspector]
	/// <summary>
	/// dynamic data: auxilliary variable that's used during the cutting process, indicates if the current cut call is happening in parallel with another cut (which for example happens during multi frame cuts), and is used internally
	/// </summary>
	public int vargamparallelcutcounter = 0;
	[HideInInspector]
	/// <summary>
	/// dynamic data: essential helper variable, stores the transform elements of the original parts that were cut, and that will be ysed by the last parallelcut, to destroy them.; 
	/// </summary>
	public List<Transform> vargamcutpartscache;
}

[System.Serializable]
public class clsdismemberatorindexer {
	public int[] propindexes;
	
	public clsdismemberatorindexer() {
		propindexes = new int[0];
	}
}

[System.Serializable]
public class clsdismemberatorvertexindexer {
	public int[] propverticesindexes;
	public clsdismemberatorindexer[] propvertices;
	
	public clsdismemberatorvertexindexer() {
		propvertices = new clsdismemberatorindexer[0];
	}
}

[System.Serializable]
public class clsdismemberatorbonerelationsindexes {
	public clsdismemberatorindexer propparentside;
	public clsdismemberatorindexer propchildrenside;
	
	public clsdismemberatorbonerelationsindexes() {
		propparentside = new clsdismemberatorindexer();
		propchildrenside = new clsdismemberatorindexer();
	}
}

[System.Serializable]
public class clsdismemberatorseparationverticesuvhelper {
	public Vector2[] propuvcoordinates;
	
	public clsdismemberatorseparationverticesuvhelper() {
		propuvcoordinates = new Vector2[0];
	}
}