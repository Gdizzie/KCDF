using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeqCleanup : MonoBehaviour {
	
	GameManager gameManager;
	
	bool canPool = false;
	bool cleaningUp = false;
	
	GameObject sequenceParent;
	
	GameObject player;
	
	public List<GameObject> presets;
	
	GameObject spawnedPreset;
	
	// Use this for initialization
	void Start () {
	
		player = GameObject.Find("Player");
		sequenceParent = this.transform.parent.gameObject;
		gameManager = GameObject.Find("ScriptManager").GetComponent<GameManager>();
		
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
	
	void SetPlayer(GameObject newPlayer)
	{
		player = newPlayer;
	}
	
	void OnBecameVisible()
	{
		//yield return new WaitForSeconds(3);
		
		if(cleaningUp)
		{
			cleaningUp = false;
		}
		canPool = true;
	}
	
	/*IEnumerator OnBecameInvisible()
	{
		if(!player)
		{
			player = GameObject.Find ("NalaPrefab(Clone)");	
		}
		//Debug.Log ("SeqInvisible");
		if(this.tag != "Static" && canPool && player.transform.position.x > this.transform.position.x)
		{
			yield return new WaitForSeconds(Random.Range(0, 3));
			//ObjectPoolController.Destroy(this.gameObject);
			RespawnTerrain();
			//Debug.Log ("SentSeqSpawn");
		}
	}*/
	
	public GameObject PresetProvider()
	{
		int random;
		random = Random.Range(0, presets.Count);	
		//RANDOM 0 - 2 WILL ONLY PROVIDE 0
		//Debug.Log ("randomPreset: " + random + " count: " + presets.Count);
		
		if(presets.Count > 0 && random < presets.Count && random > 0)
		{
			spawnedPreset = presets[random];
			return presets[random];
		}
		else
		{
			return null;	
		}
	}
	
	void Cleanup()
	{
		
		if(this.tag == "Segment" && this.transform.parent.tag != "Sequence")
		{
			
			gameManager.segCount -= 1;
		}
		
		this.transform.parent.BroadcastMessage("ObjectCleanup",SendMessageOptions.DontRequireReceiver);
		
		//Debug.Log ("objectName: " + this.name + " parentName: " + this.transform.parent.name);
		if(this.transform.parent.name == "ClonedObject")
		{
			Destroy(this.transform.parent.gameObject);	
		}
		else
		{
			cleaningUp = true;
			if(spawnedPreset)
			{
				//spawnedPreset.BroadcastMessage("CleanupObject");
			}
			ObjectPool.instance.PoolObject(sequenceParent);	
			
			if(this.transform.parent.tag == "Sequence")
			{
				gameManager.sequences.Add(sequenceParent);
			}
		}
	}
	
	void RespawnTerrain()
	{
		this.transform.parent.BroadcastMessage("ObjectCleanup",SendMessageOptions.DontRequireReceiver);
		//Debug.Log ("objectName: " + this.name + " parentName: " + this.transform.parent.name);
		if(this.transform.parent.name == "ClonedObject")
		{
			gameManager.segCount -= 1;
			Destroy(this.transform.parent.gameObject);	
		}
		else
		{
			/*if(spawnedPreset)
			{
				spawnedPreset.BroadcastMessage("CleanupObject", SendMessageOptions.DontRequireReceiver);
			}*/
			ObjectPool.instance.PoolObject(sequenceParent);
			gameManager.SendMessage("SpawnRandomSeg", sequenceParent);
			canPool = false;	
		}
	}
	
	/*void OnTriggerEnter(Collider other)
	{
		Debug.Log("Collision");
		if(other.tag != "Static")
		{	
			ObjectPoolController.Destroy(other.gameObject);
		}
	}*/
}
