using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {
	
	public int health;
	
	int origHealth;
	
	GameObject objectPool;
	
	public GameObject storedDamager;
	
	// Use this for initialization
	void Start () {
		
		storedDamager = this.gameObject;
		
		//objectPool = ObjectPool.instance.gameObject;
	
		if(health == 0)
		{
			Debug.Log ("EnemyHealth@ 0: " + this.name);	
		}
		else
		{
			origHealth = health;	
		} 
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
	
	public void TakeDamage(GameObject damager)
	{
		
		if(storedDamager.gameObject != damager.gameObject)
		{
			storedDamager = damager;
			
			if(objectPool != null)
			{
				
			}
			else
			{
				objectPool = GameObject.Find("ObjectPool");	
			}
			//health -= damage;
				
			if(damager.tag != "Player")
			{
				health -= 1;
			}
			else
			{
				health -= 3;	
			}
			
			if(health < 1)
			{
				DestroyEnemy();
			}
		}
	}
	
	void DestroyEnemy()
	{
		GameObject v;
		v = ObjectPool.instance.GetObjectForType("AnimatedSpriteFX", false);
			
		v.transform.position = this.transform.position;
			
		v.SendMessage("PlayFX", "EnemyDeath");
		//Destroy(this.gameObject);
		health = origHealth;
		/*if(this.transform.parent != ObjectPool.instance.transform)
		{
			this.transform.parent = ObjectPool.instance.transform;
		}*/
		this.gameObject.transform.parent = objectPool.transform;
		ObjectPool.instance.PoolObject(this.gameObject);	
	}
	
	public void RestoreHealth()
	{
		if(origHealth != 0)
		{
			storedDamager = this.gameObject;
			health = origHealth;	
		}
	}
	
	/*void CleanupObject()
	{
		ObjectPool.instance.PoolObject(this.gameObject);	
	}*/
}
