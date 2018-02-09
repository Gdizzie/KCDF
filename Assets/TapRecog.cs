using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TapRecog : MonoBehaviour {
	
	GameManager gameManager;
	
	GameObject objectPool;
	
	public GameObject player;
	
	public GameObject playerPrefab;
	
	//[HideInInspector]
	public List<GameObject> playerColliders;
	
	CharacterMotor cMotor;
	
	int playerLayer = 11;
	
	int playerInvincibleLayer = 12;
	
	MenuManager menuManager;

	bool dashing = false;

	PlayerScript playerScript;

	//GameObject testText;
	
	// Use this for initialization
	void Start () {
	
		gameManager = GameObject.Find("ScriptManager").GetComponent<GameManager>();
		
		menuManager = GameObject.Find ("ScriptManager").GetComponent<MenuManager>();
		
		player = GameObject.Find ("Player");
		//testText = GameObject.Find ("TestText");
		playerScript = player.GetComponent<PlayerScript>();
	}
	
	void OnTap(TapGesture gesture) { 
		
		Debug.Log( "Tap gesture detected at " + gesture.Position + 
            ". It was sent by " + gesture.Recognizer.name );	
		/* your code here */ 
	
	}
	
	
	
	void OnFingerDown(FingerDownEvent e) { 
		
	
		if(menuManager.paused)
			return;
		
		
		//Debug.Log ("FingerDown at: " + e.Position);
		
		if(e.Position.y > Screen.height/1.1f && gameManager.initSpawn == false && e.Position.x < Screen.width / 1.5f)
		{
			/*if(player)
			{
				player.SendMessage("CalibrateAccelerometer", SendMessageOptions.DontRequireReceiver);
			}*/
			
			
			
			objectPool = GameObject.Find("ObjectPool");
			objectPool.BroadcastMessage("Cleanup");
			
			GameObject.Find ("CloneHolder").BroadcastMessage("Cleanup", SendMessageOptions.DontRequireReceiver);
			
			playerColliders.Clear();
			
			
			//GameObject v = new GameObject();
			
			//v.name = "CloneHolder";
			
			//player.gameObject.SetActive(true);
			
			if(!player)
			{
				Destroy(GameObject.FindGameObjectWithTag("Ragdoll").gameObject);
				player = Instantiate(playerPrefab, GameObject.Find("SpawnPoint").transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;
				menuManager.SetPlayer(player);
				playerScript = player.GetComponent<PlayerScript>();
			}
				
			player.SendMessage ("Cleanup");
			gameManager.SendMessage("Cleanup");
			
			GameObject.Find ("ScriptManager").SendMessage("RestoreCalibrationMatrix");
		}
		
		if(player)
		{
			
			/*if(e.Position.x >Screen.width/2 && e.Position.y < Screen.height/1.1f && e.Position.y > Screen.height/2)
			{
				
				//Debug.Log ("PlayerSpeed: " + player.GetComponent<CharacterController>().velocity);
				
				GameObject v;
				
				v = ObjectPool.instance.GetObjectForType("PlayerShot", false);
				
				
				
				v.transform.rotation = Quaternion.Euler(0, 0, 0);
				
				v.transform.position = new Vector3(player.transform.position.x + 0.9f, player.transform.position.y + 1.6f, player.transform.position.z);
				
				if(player.GetComponent<CharacterController>().isGrounded)
				{
					v.SendMessage("SetVelocity", player.GetComponent<CharacterController>().velocity);
				}
				else
				{
					v.SendMessage("SetVelocity", new Vector3(player.GetComponent<CharacterController>().velocity.x, 0, 0));
				}
				
				
				
				/*
				objectPool = GameObject.Find("ObjectPool");
				objectPool.BroadcastMessage("Cleanup");
				
				
				player.SendMessage ("Cleanup");
				gameManager.SendMessage("Cleanup");
				
			}
			else */
			if(e.Position.x > Screen.width/2 && e.Position.y < Screen.height/1.5f)
			{
				//**DASH**
				//player.BroadcastMessage("SetLayer", playerInvincibleLayer);
				
				
				//playerColliders
				//Debug.Log ("TAP");
				//SetPlayerInvincible();
				cMotor = player.GetComponent<CharacterMotor>();
				if (cMotor.dash.dashingActive == false)
				{
					GameObject v;
					v = ObjectPool.instance.GetObjectForType("AnimatedSpriteFX", false);
					v.transform.position = player.transform.position;
					v.SendMessage("PlayFX", "EnemyDeath");

					playerScript.dashAudioSource.Play();

					cMotor.Dash(new Vector3(cMotor.movement.velocity.x + 55, 0.2f, cMotor.movement.velocity.z), 0.17f, 0.25f, true);
					dashing = true;
				}
				
				
			}
			else if(e.Position.x < Screen.width/2 && e.Position.y < Screen.height/1.1f)
			{
				//cMotor.movement.maxAirAcceleration = 20;
				player.SendMessage("JumpReceive");
				player.SendMessage("Animate", "jumping");
				//testText.GetComponent<TextMesh>().text = "TouchFired";
			}
			
			
		}
		/*Debug("Down gesture detected at " + e.Position + 
            ". It was sent by " + e.Recognizer.name );		
		/* your code here */ 
	
	}
	
	void SetPlayerInvincible()
	{
		foreach (GameObject go in playerColliders)
		{
			go.GetComponent<SetLayerScript>().SetLayer(playerInvincibleLayer);	
		}
	}
	
	void SetPlayerVincible()
	{
		//playerColliders
		foreach (GameObject go in playerColliders)
		{
			//go = GameObject.FindGameObjectsWithTag("PlayerCollider");
			go.GetComponent<SetLayerScript>().SetLayer(playerLayer);	
		}
	}
	
	void OnFingerUp(FingerUpEvent e) 
	{
		if(menuManager.paused)
			return;
		
		if(dashing)
		{
			cMotor = player.GetComponent<CharacterMotor>();
			cMotor.DashStop();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
