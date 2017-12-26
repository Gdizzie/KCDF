using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	public string gemString;
	
	public int gemCount;
	
	public GameObject player;
	
	Vector3 lastSegPos;
	
	public GameObject firstLastSeg;
	
	bool cleaningUp = false;
	
	public float gapMax;
	public float gapMin;
	
	GameObject storedLastSeg;
	
	public List<GameObject> sequences;
	
	public int segCount = 0;
	
	public bool initSpawn = true;
	
	public GameObject minLevelObject;
	
	Vector3 minLevel = new Vector3(-999, -999, -999);
	
	bool forced = false;
	
	public bool dashCrunchtime = false;
	
	
	//GameObject player;
	
	//public List<GameObject> platformList;
	
	//List<GameObject> objectPool = new List<GameObject>();
	
	// Use this for initialization
	void Start () {
		
		gemCount = 0;
		
		gemString = gemCount.ToString();
		
		if(minLevelObject)
		{
			minLevel = minLevelObject.transform.position;
		}
		
		//minLevel.position = minLevel? minLevel.position :minLevel.position = new Vector3(-999, -999, -999);
		/*{
			
		}
		else
		{
				
		}*/
			
		
		//segCount = 20;
		/*foreach(GameObject go in platformList)
		{
				
		}*/
		
		//objectPool = GetComponent<ObjectPool>();
		
		//Debug.Log ("objectPool: " + objectPool.Count);
		
		//sequences = new List<GameObject>[6];
		
		//sequences = GameObject.FindGameObjectsWithTag("Sequence");
		
		/*for(int i = 0; i < sequences.length; i++)
		{
			ObjectPool.instance.PoolObject(sequences[i]	
		}*/
		
		lastSegPos = firstLastSeg.GetComponentInChildren<ExitReturn>().ReturnPos();
		storedLastSeg = firstLastSeg;
		storedLastSeg.tag = "Segment";
		StartCoroutine(SpawnBeginSegs());
	}
	
	// Update is called once per frame
	void Update () {
		
		
		
		if(lastSegPos.x < 134.7f)
		{
			Debug.Log ("FailLastSegPos: " + storedLastSeg.name + " @ : " + storedLastSeg.transform.position);		
		}
			
		//Debug.Log ("segCount: "+ segCount);
		//Debug.Log ("initSpawn: " + initSpawn);
		if(segCount < 15 && initSpawn == false)
		{
			//Debug.Log ("segCount: "+ segCount);
			//Debug.Log ("ForceSpawnSeg: " + storedLastSeg.tag);
			forced = true;
			SpawnRandomSeg(storedLastSeg);	
		}
		else
		{
			forced = false;	
		}
	
	}
	
	void Cleanup()
	{
		
		cleaningUp = true;
		
		gemCount = 0;
		gemString = gemCount.ToString();
		
		lastSegPos = firstLastSeg.GetComponentInChildren<ExitReturn>().ReturnPos();
		
		storedLastSeg = firstLastSeg;
		
		storedLastSeg.tag = "Segment";
		
		StartCoroutine(SpawnBeginSegs());
		//int count = 0;
		//Debug.Log ("OjPref: " + ObjectPool.instance.pooledObjects);
		/*for(int i = 0; i < ObjectPool.instance.pooledObjects[0].Count; i++)
		{
			//count++;
			
			if(ObjectPool.instance.pooledObjects[0][i].activeSelf)
			{
			
				ObjectPool.instance.PoolObject(ObjectPool.instance.pooledObjects[0][i]);
				
			}
			
		    //GameObject v = ObjectPool.instance.pooledObjects[go][0];
			
			//ObjectPool.instance.PoolObject(v);	
		}*/
	}
	
	
	IEnumerator SpawnBeginSegs()
	{
		//Debug.Log ("INITSPAWN");
		initSpawn = true;
		
		yield return new WaitForSeconds(2);
		
		segCount = 0;
		
		for(int ii = 0; ii < 20; ii++)
		{
			//Debug.Log ("INITSPAWNINGGGG");
			SpawnRandomSeg(storedLastSeg);	
			
			if(cleaningUp && ii == 19)
			{
				cleaningUp = false;	
				
			}
			
			if(ii >= 19)
			{
				initSpawn = false;
				firstLastSeg.tag = "Static";
				
				GameObject.Find ("ObjectPool").BroadcastMessage("SetPlayer", this.gameObject);
				//Debug.Log ("initSpawn2: " + initSpawn);
			}
		}
		
		
		
	}
	
	
	void SpawnRandomSeg(GameObject lastSeg)
	{
		if(lastSeg.tag == "Sequence" && initSpawn == false)
		{
			//Debug.Log ("Sequence");
			sequences.Add(lastSeg);	
		}
		
		
		//Debug.Log ("SpawningRandomSeg: " + initSpawn);
		if(storedLastSeg.tag == "Segment" || storedLastSeg.tag == "Sequence")
		{
			int random = Random.Range(0, 63);
			GameObject v;
			if(random < 20)
			{
				v = ObjectPool.instance.GetObjectForType("FlatSegPrefab", false);	
				//Debug.Log ("SpawnFlatSeg");
				if(lastSeg.tag != "Segment" || initSpawn || forced)
				{
					//Debug.Log ("tag: " + lastSeg.tag);
					segCount += 1;	
				}
			}
			else if(random < 30)
			{
				//Debug.Log ("SeqSpawnSLS: " + storedLastSeg.transform.position + "minLevel: " + minLevel);
				if(sequences.Count > 0 && storedLastSeg.transform.position.y > minLevel.y + 15)//BUFFER AMOUNT
				{
					//NEED TO CHECK FOR USED SEQUENCE OR NOT
					//Debug.Log ("shortSeqs.Length: " + sequences.Count);
					
					random = Random.Range(0, sequences.Count);
					
					//string randomSeq = "ShortSeq" + random;
					
					//Debug.Log ("randomSeq: " + random);
					
					//Debug.Log ("randomSeq: " + sequences[random].name);
					v =  ObjectPool.instance.GetObjectForType(sequences[random].name, true);//sequences[0];//Random.Range(0, sequences.Length)];
					
					if(v)
					{
						v.BroadcastMessage("SpawnPreset",SendMessageOptions.DontRequireReceiver);
					}
					
					/*GameObject w;
					
					if(v)
					{
						w = v.GetComponentInChildren<SeqCleanup>().PresetProvider();
						if(w != null)
						{
							//Debug.Log ("SpawningPreset");
							w.BroadcastMessage("SpawnObject");
						}
					}*/
					
					
					
						
					sequences.RemoveAt(random);
					if(lastSeg.tag == "Segment" && initSpawn == false && forced == false)
					{
						//Debug.Log ("tag: " + lastSeg.tag);
						segCount -= 1;	
					}
				}
				else
				{
					if(lastSeg.tag != "Segment" || initSpawn || forced)
					{
						segCount += 1;	
						//Debug.Log ("tag: " + lastSeg.tag);
					}
					v = ObjectPool.instance.GetObjectForType("UpSegPrefab", false);		
					//v = null;
				}
			}
			else if(random < 41)
			{
				if(lastSeg.tag != "Segment" || initSpawn || forced)
				{
					//Debug.Log ("tag: " + lastSeg.tag);
					segCount += 1;
				}
				v = ObjectPool.instance.GetObjectForType("UpSegPrefab", false);	
			}
			else if(random < 46)
			{
				
				if(segCount%2 == 0)
				{
					//Debug.Log ("SpawnUpHillSeg");
					v = ObjectPool.instance.GetObjectForType("UpHillSegPrefab", false);	
				}
				else
				{
					//Debug.Log ("SpawnUpHillSeg");
					v = ObjectPool.instance.GetObjectForType("DownHillSegPrefab", false);		
				}
				if(lastSeg.tag != "Segment" || initSpawn || forced)
				{
					//Debug.Log ("tag: " + lastSeg.tag);
					segCount += 1;	
				}
			}
			else if(random < 55)
			{
				if(storedLastSeg.transform.position.y > minLevel.y + 15)//BUFFER AMOUNT
				{
					v = ObjectPool.instance.GetObjectForType("DownSegPrefab", false);	
					//Debug.Log ("SpawnDownSeg");
					if(lastSeg.tag != "Segment" || initSpawn || forced)
					{
						//Debug.Log ("tag: " + lastSeg.tag);
						segCount += 1;	
					}
				}
				else
				{
					v = ObjectPool.instance.GetObjectForType("UpSegPrefab", false);	
					//Debug.Log ("SpawnUpSeg");
					if(lastSeg.tag != "Segment" || initSpawn || forced)
					{
						//Debug.Log ("tag: " + lastSeg.tag);
						segCount += 1;	
					}
				}
			}
			else if(random < 60)
			{
				if(lastSeg.tag != "Segment" || initSpawn || forced)
				{
					//Debug.Log ("tag: " + lastSeg.tag);
					segCount += 1;
				}
				//LOG SPAWN
				//Debug.Log ("BridgeSegSpanw");
				v = ObjectPool.instance.GetObjectForType("BridgeSegPrefab", false);	
				//v.BroadcastMessage("SpawnPreset");
			}
			else
			{
				if(lastSeg.tag == "Segment" && initSpawn == false && forced == false)
				{
					segCount -= 1;	
				}
				//Debug.Log ("SpawnEndSeg");
				v = ObjectPool.instance.GetObjectForType("FlatEndSegPrefab", false);	
			}
			/*else
			{
				v = ObjectPool.instance.GetObjectForType("", true);	
			}*/
			
			
			
			//Debug.Log ("Random1: " + random);//
			
			if(v)
			{
				v.transform.position = new Vector3(lastSegPos.x, lastSegPos.y, 0);
				if(v.tag != "Sequence")
				{
					random = Random.Range (0, 10);
				
					if(random > 3)
					{
						v.BroadcastMessage("SpawnPreset", SendMessageOptions.DontRequireReceiver);	
					}
					
				}
			}
			else
			{
				//Debug.Log("NullSegFlatSegSpawn");
				v = ObjectPool.instance.GetObjectForType("FlatSegPrefab", false);
				v.transform.position = new Vector3(lastSegPos.x, lastSegPos.y, 0);
				if(lastSeg.tag != "Segment" || initSpawn || forced)
				{
					segCount += 1;
					//Debug.Log ("CatchNullSeg");
				}
			}
			
			storedLastSeg = v;
			
			if(v.GetComponentInChildren<ExitReturn>())
			{
				lastSegPos = v.GetComponentInChildren<ExitReturn>().ReturnPos();
			}
			else
			{
				Debug.Log ("FailER");
			}
				
			
			//break;
			
		}
		else if(storedLastSeg.tag == "StartSegment")
		{
			int random = Random.Range(0, 45);
			GameObject v;

			if(random < 20)
			{
				if(lastSeg.tag != "Segment" || initSpawn || forced)
				{
					//Debug.Log ("tag: " + lastSeg.tag);
					segCount += 1;
				}
				//Debug.Log ("FlatSegSpawn");
				v = ObjectPool.instance.GetObjectForType("FlatSegPrefab", false);	
			}
			else if(random < 30)
			{
				if(lastSeg.tag != "Segment" || initSpawn || forced)
				{
					//Debug.Log ("tag: " + lastSeg.tag);
					segCount += 1;
				}
				//Debug.Log ("BridgeSegSpawn");
				v = ObjectPool.instance.GetObjectForType("BridgeSegPrefab", false);	
				//v.BroadcastMessage("SpawnPreset");
			}
			else if(random < 40)
			{
				if(storedLastSeg.transform.position.y > minLevel.y + 15)//BUFFER AMOUNT
				{
					v = ObjectPool.instance.GetObjectForType("DownSegPrefab", false);	
					//Debug.Log ("SpawnDownSeg");
					if(lastSeg.tag != "Segment" || initSpawn || forced)
					{
						//Debug.Log ("tag: " + lastSeg.tag);
						segCount += 1;	
					}
				}
				else
				{
					
					v = ObjectPool.instance.GetObjectForType("UpSegPrefab", false);	
					
					//Debug.Log ("SpawnUpSeg");
					if(lastSeg.tag != "Segment" || initSpawn || forced)
					{
						//Debug.Log ("tag: " + lastSeg.tag);
						segCount += 1;	
					}
				}
			}
			else 
			{
				if(lastSeg.tag != "Segment" || initSpawn || forced)
				{
					//Debug.Log ("tag: " + lastSeg.tag);
					segCount += 1;
				}
				if(segCount%2 == 0)
				{
					//Debug.Log ("UpHillSegSpawn");
					v = ObjectPool.instance.GetObjectForType("UpHillSegPrefab", false);	
				}
				else
				{
					//Debug.Log ("DownHillSegSpawn");
					v = ObjectPool.instance.GetObjectForType("DownHillSegPrefab", false);		
				}
			}
			
			//Debug.Log ("Random: " + random);//
			
			random = Random.Range (0, 10);

			if(random > 3)
			{
				v.BroadcastMessage("SpawnPreset", SendMessageOptions.DontRequireReceiver);
			}
			
			v.transform.position = new Vector3(lastSegPos.x, lastSegPos.y, 0);
			
			storedLastSeg = v;
			
			lastSegPos = v.GetComponentInChildren<ExitReturn>().ReturnPos();
			
			//break;
		}
		else if(storedLastSeg.tag == "EndSegment")
		{
			//int random = Random.Range(0, 33);
			GameObject v;
			//Debug.Log ("SpawnStartSeg");
			
			if(lastSeg.tag == "Segment" && initSpawn == false && forced == false)
			{
				//Debug.Log ("tag: " + lastSeg.tag);
				segCount -= 1;	
			}
			
			v = ObjectPool.instance.GetObjectForType("FlatStartSegPrefab", false);	
			
			v.transform.position = new Vector3(lastSegPos.x, lastSegPos.y, 0) + new Vector3(Random.Range (gapMin, gapMax), 0, 0);
			
			storedLastSeg = v;
			
			lastSegPos = v.GetComponentInChildren<ExitReturn>().ReturnPos();
			
			//break;
			
		}
	}
	
	void SpawnTerrain(string poolableObject)
	{
		GameObject v;
		
		v = ObjectPool.instance.GetObjectForType("poolableObject", true);
		
		v.transform.position = new Vector3(lastSegPos.x, lastSegPos.y, 0);
		
		storedLastSeg = v;
		
		lastSegPos = v.GetComponent<ExitReturn>().ReturnPos();
	}
	
	public void AddGems(int amount)
	{
		gemCount += 1;
		gemString = gemCount.ToString();
	}
	
}
