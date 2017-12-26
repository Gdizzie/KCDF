using UnityEngine;
using System.Collections;

public class SegCleanupFailSafe : MonoBehaviour {
	
	
	GameManager gameManager;
	// Use this for initialization
	void Start () {
		
		gameManager = GameObject.Find("ScriptManager").GetComponent<GameManager>();
	
	}
	
	// Update is called once per frame
	/*void Update () {
		
		//this.transform.position = GameObject.Find("Player").transform.position;
	
	}*/
	
	void OnTriggerEnter(Collider other)
	{
		//Debug.Log("Collision");
		if(other.tag != "Static" && other.tag != "Untagged" && other.tag != "PlayerDeathCollider" && other.gameObject.layer != 9)
		{	
			//Debug.Log ("other: " + other.name);
			other.SendMessage("RespawnTerrain", SendMessageOptions.DontRequireReceiver);
		}
	}
}
