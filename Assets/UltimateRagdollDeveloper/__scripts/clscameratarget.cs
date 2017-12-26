using UnityEngine;
using System.Collections;

/// <summary>
/// 2012-11-06
/// ULTIMATE RAGDOLL GENERATOR V4
/// © THE ARC GAMES STUDIO 2012
/// DESIGNED WITH UNITY 3.5.6f4
/// 
/// advanced class for camera scene management
/// 
/// NOTE v4: the class is used as a scene manager
/// 
/// NOTE v1.8: the class is used as initializer for the "Fixed timestep" parameter of the
/// EDIT=> PROJECT SETTINGS=> TIME menu, that determines the frequency of physic updates for the game
/// It is suggested that the value be kept consistent throughout development to avoid
/// unexpected physic behavior
/// </summary>
public class clscameratarget : MonoBehaviour {
	/// <summary>
	/// inspector slot for a target
	/// </summary>
	public Transform vargamtarget = null;
	//V4: removed after introduction of scene manager
	//private Vector3 varpositionfixer = new Vector3();
	//private Vector3 varrotationfixer = new Vector3();

	/// <summary>
	/// inspector 3d scene positions for camera snapshotting function
	/// </summary>
	public Transform[] vargamscenarios;
	/// <summary>
	/// list of targets to combine with the scenarios array
	/// </summary>
	public Transform[] vargamtargets;
	
	private int varcurrentscenario = -2;
	private const int cnsbuttonwidthmenu = 250;
	private const int cnsbuttonwidth = 100;
	private const string cnsdemostagename = "__Demo stage stupid drivers";
	private const string cnssoldier = "Soldier";
	
	private GameObject varsoldier;
	void Awake() {
		//set the Fixed timestep to 50 calls
		Time.fixedDeltaTime = 0.01f;
		Time.timeScale = 0;
	}
	
	private void metwatchscenario(int varpscenario) {
		vargamtarget = null;
		//enabled = false;
		varcurrentscenario = varpscenario;
		Time.timeScale = 1;
		transform.position = vargamscenarios[varpscenario].position;
		transform.rotation = vargamscenarios[varpscenario].rotation;
		vargamtarget = vargamtargets[varpscenario];
	}

	void OnGUI() {
		switch(varcurrentscenario) {
			case 0:
				GUILayout.Label("All effects displayed at once.");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					Application.LoadLevel(cnsdemostagename);
				}
				break;
			case 1:
				GUILayout.Label("Simple ragdoll functionality.\nThe soldier game model was ragdolled into a prefab in editor mode.\nAt the press of the 'Go ragdoll' button below,\nthe ragdolled prefab is spawned and posed like the original gameobject, which is destroyed.");
				if (varsoldier != null) {
					if (GUILayout.Button("Go ragdoll", GUILayout.Width(cnsbuttonwidthmenu))) {
						clsragdollhelper varhelper = varsoldier.transform.root.GetComponentInChildren<clsragdollhelper>();
						if (varhelper != null) {
							varhelper.metgoragdoll();
						}
						else {
							Debug.LogError("Soldier has no helper. Please make sure that it's reverted to its original prefab state.");
						}
					}
				}
				else {
					GUILayout.Space(24);
				}
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					Application.LoadLevel(cnsdemostagename);
				}
				break;
			case 2:
				GUILayout.Label("Basic ragdoll functionality.\nThe ratman was ragdolled in edit mode, with the 'Kinematic' checkbox flagged in URG! options.\nAdditionally, a trigger script and a trigger collider were added to it.\nWhen the jeep enters the collider, the collider script turns the ratman rigidbodies\nfrom 'Kinematic' to 'Physic driven', and the ratman becomes a ragdoll.");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					Application.LoadLevel(cnsdemostagename);
				}
				break;
			case 3:
				GUILayout.Label("Advanced ragdoll:\nThe ratman was ragdolled in edit mode, with the 'URGent' checkbox flagged in URG! options.\nAfterwards, the 'Animate Physics' flag was set in its Animation component\n, and an animation was set to 'Play automatically'.\nThanks to the urgent actuators, the collision event generated from the weight\n that hits the ratman issues a call that turns all its rigidbodies into physic driven.");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					Application.LoadLevel(cnsdemostagename);
				}
				break;
			case 4:
				GUILayout.Label("Advanced statified ragdoll:\nAny standard or URGent ragdoll can receive the URG Animation states class.\nIn edit mode, the ASM compiler creates data structures that are used\nin play mode, to create ragdoll to animation or animation to animation transitions.\nIn this example, there's a simple script that checks if the character has received an impact and,\nwhen the character becomes still, transitions to one 'stand_up' animation, which is consequently played.");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					Application.LoadLevel(cnsdemostagename);
				}
				break;
			case 5:
				GUILayout.Label("Advanced dismemberable character:\nThe 'Big D' is a next-gen utility and URG exclusive that allows separation of any 'Transform',\nalong with its mesh triangles, from the main gameobject.\nThe compiled class is installed in edit mode with the Dismemberator utility\nand is used afterwards in a call with a single line of code.\nSeparation can optionally instance cut triangles with an user defined\nmaterial, and parent and child separation gameobjects (for example particles) if so desired.\nAdditionally, this feature doesn't affect animations, so it becomes possible\nto keep an animation running, and seamlessly detach any gameobject part.");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					Application.LoadLevel(cnsdemostagename);
				}
				break;
			case 6:
				GUILayout.Label("Linear ragdoll for partial characters:\nWith certain unusual ragdolls (multiple limbs, like spiders or ropes, fences, chains, etc.),\nthe normal ragdolling procedure might not complete properly.\nThanks to the 'Fake Limbs' and 'Connect' URG! functions\nit becomes possible to create separate linear ragdolls,\nand connect them with a single click, to make the final gameobject perform\nas close as a 'full' ragdoll as possible.");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					Application.LoadLevel(cnsdemostagename);
				}
				break;
			case 7:
				GUILayout.Label("Dismember for arbitrary game models:\nDismemberator functionality is handily available for any sort of SkinnedMeshRendered gameobject.\nTanks to the 'Big D', as long as an object has 3D bones and is properly skinned,\nit will be possible to dynamically detach any of its parts in realtime.\nIn this example, the vehicles host the Big D class, and when they impact,\na simple but clever routine decides which parts to disconnect, retrieving their transforms.\nFor each of these is then issued a single instruction call to the dismemberator utility.");
				if (GUILayout.Button("<= Back", GUILayout.Width(cnsbuttonwidth))) {
					Application.LoadLevel(cnsdemostagename);
				}
				break;
			default:
				GUILayout.Label("Welcome to URG! demo scene.\nPlease press the buttons for a demonstration of the available functions.\n\nNOTE: the scene is meant to be taxing to the system for benchmarking purposes of the\ndismemberator utility, but all URG utilities are optimized and fast.\nReal game scenarios can handle dozens of URG objects on screen with great performance.");
				GUILayout.Space(10);
				if (GUILayout.Button("Run complete scene", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(0);
				}
				if (GUILayout.Button("Simple use: ragdoll prefab", GUILayout.Width(cnsbuttonwidthmenu))) {
					if (varsoldier == null) {
						varsoldier = GameObject.Find(cnssoldier);
					}
					metwatchscenario(1);
				}
				if (GUILayout.Button("Basic use: kinematic ragdoll", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(2);
				}
				if (GUILayout.Button("Advanced use: URGent ragdoll", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(3);
				}
				if (GUILayout.Button("Advanced use: Animation States", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(4);
				}
				if (GUILayout.Button("Advanced use: Dismemberator", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(5);
				}
				if (GUILayout.Button("Special use: Linear ragdoll", GUILayout.Width(cnsbuttonwidthmenu))) {
					metwatchscenario(6);
				}
				if (GUILayout.Button("Special use: Arbitrary separation", GUILayout.Width(cnsbuttonwidthmenu))) {
					//find the separable jeep and activate its separable attribute
					bool varcanwatch = true;
					GameObject varjeepd = GameObject.Find("Jeep_D");
					if (varjeepd == null) {
						varcanwatch = false;
					}
					else {
						clscollision varcollision = varjeepd.GetComponentInChildren<clscollision>();
						varcollision.vargamcandestroyjeep = true;
						if (varcollision == null) {
							varcanwatch = false;
						}
					}
					if (!varcanwatch) {
						Debug.LogError("Please make sure that the separable jeep is in the scene to watch this scenario.");
					}
					else {
						metwatchscenario(7);
					}
				}
				break;
		}
	}
	
	void FixedUpdate () {
		if (vargamtarget != null) {
			transform.LookAt(vargamtarget);
			/*
			//V4: removed after introduction of scene manager
			varpositionfixer = transform.position;
			varpositionfixer.x = vargamtarget.position.x;
			varpositionfixer.z = vargamtarget.position.z;
			varrotationfixer = transform.eulerAngles;
			varrotationfixer.y = 0;
			varrotationfixer.z = 0;

			transform.position = varpositionfixer;
			transform.eulerAngles = varrotationfixer;
			*/
		}
		
	}
}
