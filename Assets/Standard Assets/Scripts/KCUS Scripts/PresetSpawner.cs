using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PresetSpawner : MonoBehaviour {
	
	public List<GameObject> presets;
	
	GameObject spawnedPreset;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
	
	public void SpawnPreset()
	{
		int random;
		random = Random.Range(0, presets.Count);	
		//RANDOM 0 - 2 WILL ONLY PROVIDE 0
		//Debug.Log ("randomPreset: " + random + " count: " + presets.Count);
		
		if(presets.Count > 0 && random < presets.Count && random > -1)
		{
			spawnedPreset = presets[random];
			
			spawnedPreset.BroadcastMessage("SpawnObject");
			
			//return presets[random];
		}
		else
		{
			//return null;	
		}
	}
	
	public void ObjectCleanup()
	{
		if(spawnedPreset)
		{
			spawnedPreset.BroadcastMessage("CleanupObject", SendMessageOptions.DontRequireReceiver);
			spawnedPreset = null;
		}
	}
}
