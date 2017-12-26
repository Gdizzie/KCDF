using UnityEngine;
using System.Collections;

public class CCMBSegCleanup : MonoBehaviour {
	
	GameManager gameManager;
	
	bool canPool = false;
	bool cleaningUp = false;
	
	GameObject sequenceParent;
	
	GameObject player;
	
	// Use this for initialization
	void Start () {
	
		player = GameObject.Find("Player");
		sequenceParent = this.transform.parent.parent.gameObject;
		gameManager = GameObject.Find("ScriptManager").GetComponent<GameManager>();
		
	}
	
	// Update is called once per frame
	void Update () {
	
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
	
	IEnumerator OnBecameInvisible()
	{
		//Debug.Log ("SeqInvisible");
		if(this.tag != "Static" && canPool && player.transform.position.x > this.transform.position.x)
		{
			yield return new WaitForSeconds(Random.Range(0, 3));
			//ObjectPoolController.Destroy(this.gameObject);
			RespawnTerrain();
			//Debug.Log ("SentSeqSpawn");
		}
	}
	
	void Cleanup()
	{
		//DOUBLECHECK
		cleaningUp = true;
		ObjectPool.instance.PoolObject(sequenceParent);	
		gameManager.segCount -= 1;
		//gameManager.sequences.Add(sequenceParent);
	}
	
	void RespawnTerrain()
	{
		ObjectPool.instance.PoolObject(sequenceParent);
		gameManager.SendMessage("SpawnRandomSeg", sequenceParent);
		canPool = false;	
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
