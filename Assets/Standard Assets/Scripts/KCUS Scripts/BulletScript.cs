using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {
	
	public float timer = -1;
	
	float origTimer = -1;
	
	GameObject player;
	
	Vector3 velocity;
	
	bool canDamage = true;
	
	int damage = 1;
	
	// Use this for initialization
	void Start () {
	
		player = GameObject.Find ("Player");
		
	}
	
	// Update is called once per frame
	void Update () {
	
		if(timer != -1)
		{
			if(origTimer == -1)
			{
				origTimer = timer;
			}
			//velocity = new Vector3(velocity.x, velocity.y, 0);
			timer -= 1*Time.deltaTime;	
		}
		
		if(timer < 0 && timer > -1)
		{
			
			//Debug.Log ("Collision: " + other.tag);
			
			//Debug.Log ("Shot: " + this.gameObject);
			BulletDestroy(true);	
		}
		
		//Debug.Log ("Timer: " + timer);
		
		//Mathf.Clamp(velocity.x, 18, 26);
		
		if(velocity.x < 16)
		{
			velocity = new Vector3(16, velocity.y, 0);	
		}
		
		this.transform.Translate(new Vector3((velocity.x * 3) * Time.deltaTime, (velocity.y * 3) * Time.deltaTime, 0));
		
		
		
		//Debug.Log ("VelocityX: " + velocity.x);
		
		
	}
	
	public void SetVelocity(Vector3 speed)
	{
		velocity = speed;	
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag != "Player" && other.tag != "PlayerShot" && other.gameObject.layer != 9)//ITEM LAYER
		{
			//Debug.Log ("BulletHit:" + other);
			if(other.tag == "Enemy" && canDamage)
			{
				//EnemyScript enemyScript;
				
				//enemyScript = other.GetComponent<EnemyScript>();
				
				//enemyScript.TakeDamage(damage, this.gameObject);
				
				canDamage = false;
				other.SendMessageUpwards("TakeDamage", this.gameObject, SendMessageOptions.DontRequireReceiver);	
				damage -= 1;
				BulletDestroy(true);
				//this.GetComponent<Collider>().enabled = false;
				//Debug.Log ("SendTakeDamage: " + other.name);
				
				
			}
			else
			{
				BulletDestroy(true);
			}
		}
	}
	
	void BulletDestroy(bool burstAnim)
	{
		//Debug.Log ("BulletDestroy");
		
		if(burstAnim)
		{
			GameObject v;
		
			v = ObjectPool.instance.GetObjectForType("AnimatedSpriteFX", false);
		
			v.transform.position = this.transform.position;
			
			v.SendMessage("PlayFX", "ShotBurst");
			
			//v.SendMessage("ResetAnimation");
		}
		else
		{
			//this.BroadcastMessage("ResetAnimation");	
		}
			
		timer = origTimer;
		canDamage = true;
		damage = 1;
		ObjectPool.instance.PoolObject(this.gameObject);
	}
	
	void OnBecameInvisible()
	{
		//BulletDestroy(false);
		//ObjectPool.instance.PoolObject(this.gameObject);	
	}
}
