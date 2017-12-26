using UnityEngine;
using System.Collections;

public class PoolControl : MonoBehaviour {
	
	
	public GameObject objectToSpawn;
	public GameObject parentToJoin;
	GameManager gameManager;
	
	// Use this for initialization
	void Start () {
		
		gameManager = GameObject.Find("ScriptManager").GetComponent<GameManager>();
	
		StartCoroutine(SpawnBeginSegs());
		
	}
	
	IEnumerator SpawnBeginSegs()
	{
		yield return new WaitForSeconds(2);
		for(int ii = 0; ii < 5; ii++)
		{
			gameManager.SendMessage("SpawnRandomSeg", objectToSpawn);	
			
			if(ii == 5)
			{
				gameManager.initSpawn = false;
				gameManager = null;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
