using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	
	public GameObject player;
	
	public Vector3 offset;
	
	// Use this for initialization
	void Start () {
		
		player = GameObject.Find ("Player");
	
	}
	
	// Update is called once per frame
	void LateUpdate () {		
	
		this.transform.position = new Vector3(player.transform.position.x + offset.x, player.transform.position.y + offset.y, player.transform.position.z + offset.z);	
	
		
	}
	
	void SetPlayer(GameObject newPlayer)
	{
		Debug.Log("SettingPlayer");
		player = newPlayer;
	}
	
	void SetOffset(Vector3 newOffset)
	{
		Debug.Log ("settingOffset");
		offset = newOffset;
	}
}
