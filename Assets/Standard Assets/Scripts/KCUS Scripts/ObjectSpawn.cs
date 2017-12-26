using UnityEngine;
using System.Collections;

public class ObjectSpawn : MonoBehaviour {
	
	public string PoolableObjectName;
	
	public GameObject spawnedObject;
	
	GameObject objectPool = null;
	
	bool spawnedObjectExisted;
	
	string lastSpawnedObjectName;
	
	int lastSpawnedObjectId;
	
	// Use this for initialization
	void Start () {
	
		//foreach(Transform child in transform)
		//{
			
			//Debug.Log ("Children: " + child.name);	
		//}
		
	}
	
	// Update is called once per frame
	/*void Update () {
	
		if(spawnedObjectExisted && spawnedObject == false)
		{
			//Debug.Log ("SpawnedObjectNull: " + lastSpawnedObjectName, gameObject);	
		}
		
	if(spawnedObject)
		{
			lastSpawnedObjectId = spawnedObject.GetInstanceID();
			lastSpawnedObjectName = spawnedObject.name;
			spawnedObjectExisted = true;
		}
		
	}*/
	
	void SpawnObject()
	{
		if(objectPool != null)
		{
			//Debug.Log ("PoolSet");	
		}
		else
		{
			
			objectPool = GameObject.Find ("ObjectPool");
			//Debug.Log ("PoolSetting: " + objectPool);	
		}
				
		//Debug.Log ("SpawningObject: " + PoolableObjectName);
		GameObject v;
		v = ObjectPool.instance.GetObjectForType(PoolableObjectName, false);
		
		if(v.transform.parent.name == "ObjectPool" || v.transform.parent.name == "CloneHolder")
		{
			//Debug.Log ("NewSOId: " + v.GetInstanceID());
			spawnedObject = v;
		}
		else
		{
			Debug.Log ("pooledObjectBeingUsed");
			v = ObjectPool.instance.GetObjectForType(PoolableObjectName, false);	
		}
				
		v.transform.parent = this.transform;
		
		v.transform.position = this.transform.position;
		
		v.transform.rotation = this.transform.rotation;
		
		if(v.tag == "Enemy" && v)
		{
			v.SendMessage("RestoreHealth",SendMessageOptions.DontRequireReceiver);		
		}
		else if(v.tag == "Gem")
		{
			v.SendMessage("ResetGem");	
		}
		
		
		
		
		
		
	}
	
	void CleanupObject()
	{
		if(!spawnedObject)
		{
			Debug.Log ("CleanupSpawnedObjMissing Name: " + this.name + " iD: " + this.GetInstanceID() + " lastSO: " + lastSpawnedObjectName + " iD: " + lastSpawnedObjectId);
		}
		else
		{
			//Debug.Log ("CleanupSpawnedObj Name: " + this.name + " SOName: " + spawnedObject.name + " SOtag: " + spawnedObject.tag + " iD: " + this.GetInstanceID());
		}
		if(spawnedObject)
		{
			if(spawnedObject.name == "ClonedObject")
			{
				Destroy(spawnedObject);	
			}//Debug.Log ("CleanupObject: " + spawnedObject);
			else
			{
				//Debug.Log ("PooledObject");
				/*if(spawnedObject.tag == "Enemy")
				{
					Debug.Log ("cleanedUpObject: " + spawnedObject);
					//spawnedObject.SendMessage("RestoreHealth");	
				}*/
				
				//ObjectPool.instance.PoolObject(spawnedObject);
				if(spawnedObject.transform.parent.name != "ObjectPool")
				{
					spawnedObject.transform.parent = objectPool.transform;
					ObjectPool.instance.PoolObject(spawnedObject);
				}
			}
		}
		//else
			
		
	}
}
