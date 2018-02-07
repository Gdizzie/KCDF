using UnityEngine;
using System.Collections;

public class GemScript : MonoBehaviour {
	
	GameObject crunchtimeMeterPos; 
	
	Vector3 origPos;
	
	GameObject objectPool;
	
	GameManager gameManager;
	
	private bool gemLerping = false;

	public AudioSource collectSound;

	bool collected = false;

	// Use this for initialization
	void Start () {
		
		gameManager = GameObject.Find ("ScriptManager").GetComponent<GameManager>();
	
		origPos = this.transform.position;
		crunchtimeMeterPos = GameObject.Find ("GemCollectPos");
		
	}
	
	// Update is called once per frame
	/*void Update () {
		
		//this.transform.Rotate(new Vector3(0, 0, 1));
	
	}*/
	
	public void GemCollect()
	{
		if(!collected)
		{
			collected = true;
			if(objectPool != null)
			{
				
			}
			else
			{
				objectPool = GameObject.Find("ObjectPool");	
			}
			//GameObject v;
			
			//v = ObjectPool.instance.GetObjectForType("GemCollectFX", false);
			
			//v.transform.position = this.transform.position;
			collectSound.Play();
			StartCoroutine(GemLerp());
		}
	}
	
	public IEnumerator GemLerp()
	{
		if(gemLerping == false)
		{
			gemLerping = true;
			float i = 0f;
			float rate;
			float smoothRate = 0.9f;
				
			//rate = 1f/Time.time;
			rate = 1f/smoothRate;
			
			while(i < 0.4)
			{
				i += Time.deltaTime * rate;		
				transform.position = Vector3.Lerp(this.transform.position, new Vector3(crunchtimeMeterPos.transform.position.x + 0.6f, crunchtimeMeterPos.transform.position.y, crunchtimeMeterPos.transform.position.z + 0.3f), i);
				yield return null;
			}	
			
			if(this.tag == "SuperGem")
			{
				gameManager.AddGems(10);
			}
			else
			{
				gameManager.AddGems(1);
			}
			
			if(this.transform.parent.name != "StaticGems" && this.name != "ClonedObject")
			{
				//this.gameObject.transform.parent = objectPool.transform;
				//ObjectPool.instance.PoolObject(this.gameObject);
				this.GetComponent<Renderer>().enabled = false;
			//	Debug.Log ("GemFinishLerp");
			}
			else
			{
				if(this.name == "ClonedObject")
				{
					this.GetComponent<Renderer>().enabled = false;
					//Debug.Log ("ClonedGem");
					//this.gameObject.SetActive(false);
					//ObjectPool.instance.PoolObject(this.gameObject);
					//Destroy(this.gameObject);	
				}
				else
				{
					this.transform.position = origPos;
					gemLerping = false;
				}
			}
		}
		
	}
	
	void ResetGem()
	{
		this.GetComponent<Renderer>().enabled = true;
		gemLerping = false;
		collected = false;
	}
		
}
