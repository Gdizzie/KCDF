using UnityEngine;
using System.Collections;

public class SegCleanup : MonoBehaviour {
	
	GameManager gameManager;
	
	bool canPool = false;
	bool cleaningUp = false;
	
	GameObject player;
	
	//GameObject parentParent;
	
	//MeshCollider meshCollider;
	//ConcaveCollider concaveCollider;
	
	// Use this for initialization
	void Start () {
	
		//parentParent = this.transform.parent.parent.gameObject;
		
		player = GameObject.Find ("Player");
		
		gameManager = GameObject.Find("ScriptManager").GetComponent<GameManager>();
		//meshCollider = this.GetComponent<MeshCollider>();
		//concaveCollider = this.GetComponent<ConcaveCollider>();
		
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
	
	public void SetPlayer(GameObject newPlayer)
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
		if(this.tag != "Static" && canPool && player.transform.position.x > this.transform.position.x)
		{
			yield return new WaitForSeconds(Random.Range(0, 3));
			//ObjectPoolController.Destroy(this.gameObject);
			RespawnTerrain();	
			//Debug.Log ("SentSpawn");
		}
	}*/
	
	void Cleanup()
	{
		cleaningUp = true;
		if(this.tag == "Segment")
		{
			gameManager.segCount -= 1;
		}
		
		if(this.transform.parent.tag == "Segment")
		{
			transform.parent.BroadcastMessage("ObjectCleanup", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			BroadcastMessage("ObjectCleanup", SendMessageOptions.DontRequireReceiver);
		}
		
		if(this.name == "ClonedObject" || this.transform.parent.name == "ClonedObject")
		{
			Destroy(gameObject);
			
		}
		else if(this.name == "MeshBakerMesh" || this.name == "Collider")
		{
			ObjectPool.instance.PoolObject(this.transform.parent.gameObject);
		}
		else
		{
			ObjectPool.instance.PoolObject(this.gameObject);
		}
		
	}
	
	void RespawnTerrain()
	{
		if(this.transform.parent.tag == "Segment")
		{
			transform.parent.BroadcastMessage("ObjectCleanup", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			BroadcastMessage("ObjectCleanup", SendMessageOptions.DontRequireReceiver);
		}
		if(this.name == "Collider" || this.name == "MeshBakerMesh")
		{
			ObjectPool.instance.PoolObject(this.transform.parent.gameObject);
			gameManager.SendMessage("SpawnRandomSeg", this.transform.parent.gameObject);
		}
		else if(this.name == "ClonedObject" || this.transform.parent.name == "ClonedObject")
		{
			if(this.name == "ClonedObject")
			{	
				gameManager.segCount -= 1;
				Destroy(gameObject);
			}
			else
			{
				Destroy(this.transform.parent.gameObject);	
			}
		}
		else
		{
			ObjectPool.instance.PoolObject(this.gameObject);
			gameManager.SendMessage("SpawnRandomSeg", this.gameObject);
		}
		
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
