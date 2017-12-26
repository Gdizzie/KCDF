﻿using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {
	
	Vector3 startPos;
	
	protected Animator animator; 
	
	GameManager gameManager;
	
	GameObject objectPool;
	
	public bool canDie = true;
	
	public bool knockOut;
	
	clsragdollify ragDollifier;
	
	GameObject cameraPointer;
	
	//GameObject playerDepe
	
	GameObject playerPos;
	
	bool ragdollExecuted = false;
	
	CharacterMotor cMotor;
	
	GameObject root;
	
	int playerLayer = 11;
	
	int playerInvincibleLayer = 12;
	
	//public int distanceInt = 0;
	
	private float distanceFloat = 0f;
	
	public string distanceString;
	
	// Use this for initialization
	void Start () {
		
		root = this.transform.root.gameObject;
		
		playerPos = GameObject.Find ("PlayerPos");
		
		cameraPointer = GameObject.Find ("CameraPointer");
		
		ragDollifier = this.GetComponent<clsragdollify>();
		
		gameManager = GameObject.Find ("ScriptManager").GetComponent<GameManager>();
	
		animator = GetComponent<Animator>();
		
		
		//this.animation["Run"].speed = 1.5f;
		
		startPos = GameObject.Find ("SpawnPoint").transform.position;
		
		
		
	}
	
	
	
	
	// Update is called once per frame
	void Update () {
		
		distanceFloat = Vector3.Distance(startPos, this.transform.position);
		
		//distanceInt = Mathf.RoundToInt(distanceFloat);
		
		distanceString = Mathf.RoundToInt(distanceFloat).ToString();
		
		//Debug.Log(Mathf.RoundToInt(distanceFloat));
		//distanceInt = Co distanceFloat
		
		
		if(knockOut)
		{
			ExecuteRagdoll();
			knockOut = false;
		}
		
		//GetComponent<Ch
		
		//Vector3 veloc = Vector3.zero;
		
		//veloc = SendMessage("ReturnVelocity", veloc);
		
		//transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(veloc, Vector3.forward), Time.deltaTime * 10f);

			
		if(this.transform.position.z != startPos.z && this.name != "Collider")
		{
			this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, startPos.z);
		}
	
	}
	
	/*void StartDashAttack()
	{
		CharacterMotor cMotor  = transform.root.gameObject.GetComponent<CharacterMotor>();
		cMotor.
			SetVelocity(new Vector3(cMotor.movement.velocity.x +, 1 * 20, cMotor.movement.velocity.z));
	}*/
	
	void StopDashAttack()
	{
			
	}
	
	void ExecuteRagdoll()
	{
		//Debug.Log ("ExecuteRagdoll: " + this.name);
		
		if(ragdollExecuted == false)
		{
			
			
			ragdollExecuted = true;
			ragDollifier.metgoragdoll();
			//Debug.Log ("ThisObject: ", Object);
			//cameraPointer.SendMessage("SetPlayer", ragDollifier.vargamragdoll.gameObject);
			Destroy(this.gameObject.transform.root.gameObject);
		}
	}
	
	void Animate(string state)
	{
		animator.SetBool(state, true);	
	}
	
	void Cleanup()
	{
		//Debug.Log ("Cleanup");
		
		//Debug.Log ("ResetPos");
		Start ();
		this.GetComponent<CharacterMotor>().player = this.gameObject;
		GameObject.Find ("PlayerDependent").BroadcastMessage("SetPlayer", this.gameObject);
		cameraPointer.SendMessage("SetOffset", new Vector3(8, 3, 0));
		animator = GetComponent<Animator>();
		
		
		//player.SendMessage("SetPlayer", this.gameObject);
		
		this.transform.position = GameObject.Find("SpawnPoint").transform.position;	
	}
	
	void OnTriggerEnter(Collider other)
	{
		
		CharacterMotor cMotor  = transform.root.gameObject.GetComponent<CharacterMotor>();
		if(cMotor.dash.dashingActive)
		{
			//Debug.Log ("DashCollision: " + other.name);	
		}
		
		//Debug.Log ("PlayerCollision: " + other.tag + " Layer: " + other.gameObject.layer);
		if(other.tag == "Gem")
		{
			//rigidbody.AddForce (Vector3.up * 100000, ForceMode.Impulse);
			
			//transform.root.gameObject.SendMessage("Animate", "jumping");
			//CharacterMotor cController = this.transform.root.gameObject.GetComponent<CharacterMotor>();
			
			//cController.SimpleMove(Vector3.up * 1000);
			//cController.rigidbody.AddForce(Vector3.up * 1000, ForceMode.VelocityChange);
			
			//cController.SetVelocity(new Vector3(cController.movement.velocity.x, Vector3.up.y * 30, cController.movement.velocity.z));
			
			
			GameObject v;
		
			v = ObjectPool.instance.GetObjectForType("AnimatedSpriteFX", false);
		
			v.transform.position = this.transform.position;
			
			v.SendMessage("PlayFX", "GemCollect");
			
			//v.transform.parent = this.transform;
			
			other.SendMessage("GemCollect");
		}
		else if(other.tag == "Enemy" && this.name != "Player")
		{
			if(this.name == "JumpCollider")
			{
				Debug.Log ("JumpImpact");
				//CharacterMotor cMotor  = transform.root.gameObject.GetComponent<CharacterMotor>();
				other.SendMessageUpwards("TakeDamage", this.gameObject, SendMessageOptions.DontRequireReceiver);
				
				root.SendMessage("Animate", "jumping");
				if(cMotor.dash.dashingActive)
				{
					cMotor.Dash(new Vector3(16, 20, cMotor.movement.velocity.z), 0.1f, 0.1f, false);
				}
				else
				{
					cMotor.SetVelocity(new Vector3(cMotor.movement.velocity.x + 6f, 1 * 20, cMotor.movement.velocity.z));
				}
			}
			else
			{
				
			
				Debug.Log ("ColliderParent: " + this.transform.parent.name + " Name: " + this.name);
				if(!animator)
				{
					animator = transform.root.gameObject.GetComponent<Animator>();
				}
				cMotor = transform.root.gameObject.GetComponent<CharacterMotor>();
				
				if(cMotor.dash.dashingActive)
				{
					
					Debug.Log ("DashImpact");
					Animate("jumping");
					other.SendMessageUpwards("TakeDamage", this.gameObject, SendMessageOptions.DontRequireReceiver);	
					cMotor.Dash(new Vector3(13.5f, 20, cMotor.movement.velocity.z), 0.1f, 0.1f, false);
					
				}
				else
				{
					if(canDie)
					{
						knockOut = true;
					}
					else if(this.name != "Player")
					{
						Debug.Log ("SendRagdolExec: " + this.gameObject.layer);
						this.transform.root.gameObject.SendMessage("ExecuteRagdoll");	
					}
				}
				
				
				
				
				
				
				/*
				objectPool = GameObject.Find("ObjectPool");
				objectPool.BroadcastMessage("Cleanup");
				
				GameObject.Find ("CloneHolder").BroadcastMessage("Cleanup", SendMessageOptions.DontRequireReceiver);
				
				//GameObject v = new GameObject();
				
				//v.name = "CloneHolder";
				
				this.transform.root.SendMessage("Cleanup");
				gameManager.SendMessage("Cleanup");
				*/
				//Debug.Log ("HazardHit: " + this.name);
			}
		}
		else if(other.tag == "Hazard")
		{
			Debug.Log ("SendRagdolExec: " + this.gameObject.layer);
			this.transform.root.gameObject.SendMessage("ExecuteRagdoll");	
		}
		
	}
	
	void OnTriggerStay(Collider other)
	{
		CharacterMotor cMotor  = transform.root.gameObject.GetComponent<CharacterMotor>();
		/*if(cMotor.dash.dashingActive)
		{
			Debug.Log ("DashCollision: " + other.name + " tag: " + other.tag);	
		}*/
		
		if(other.tag == "Enemy" && this.name != "Player")
		{
			if(this.name == "JumpCollider")
			{
				Debug.Log ("JumpImpact");
				
				other.SendMessageUpwards("TakeDamage", this.gameObject, SendMessageOptions.DontRequireReceiver);
				
				root.SendMessage("Animate", "jumping");
				if(cMotor.dash.dashingActive)
				{
					cMotor.Dash(new Vector3(16, 20, cMotor.movement.velocity.z), 0.1f, 0.1f, false);
				}
				else
				{
					cMotor.SetVelocity(new Vector3(cMotor.movement.velocity.x + 6f, 1 * 20, cMotor.movement.velocity.z));
				}
			}
			else
			{
				
				Debug.Log ("ColliderParent: " + this.transform.parent.name + " Name: " + this.name);
				if(!animator)
				{
					animator = transform.root.gameObject.GetComponent<Animator>();
				}
				cMotor = transform.root.gameObject.GetComponent<CharacterMotor>();
				
				if(cMotor.dash.dashingActive)
				{
					
					Debug.Log ("DashImpact");
					Animate("jumping");
					other.SendMessageUpwards("TakeDamage", this.gameObject, SendMessageOptions.DontRequireReceiver);	
					cMotor.Dash(new Vector3(13.5f, 20, cMotor.movement.velocity.z), 0.1f, 0.1f, false);
					
				}
				else
				{
					if(canDie)
					{
						knockOut = true;
					}
					else if(this.name != "Player")
					{
						Debug.Log ("SendRagdolExec: " + this.gameObject.layer);
						this.transform.root.gameObject.SendMessage("ExecuteRagdoll");	
					}
				}
				
				
				
				
				
				/*
				objectPool = GameObject.Find("ObjectPool");
				objectPool.BroadcastMessage("Cleanup");
				
				GameObject.Find ("CloneHolder").BroadcastMessage("Cleanup", SendMessageOptions.DontRequireReceiver);
				
				//GameObject v = new GameObject();
				
				//v.name = "CloneHolder";
				
				this.transform.root.SendMessage("Cleanup");
				gameManager.SendMessage("Cleanup");
				*/
				//Debug.Log ("HazardHit: " + this.name);
			}
		}
		
	}
	
}