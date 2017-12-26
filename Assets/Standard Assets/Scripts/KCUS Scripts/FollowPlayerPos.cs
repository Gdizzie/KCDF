using UnityEngine;
using System.Collections;

public class FollowPlayerPos : MonoBehaviour {
	
	GameObject playerPos;
	
	public Vector3 offset;
	
	// Use this for initialization
	void Start () {
		
		playerPos = GameObject.Find ("PlayerPos");
	
	}
	
	// Update is called once per frame
	void Update () {		
	
		this.transform.position = new Vector3(playerPos.transform.position.x + offset.x, playerPos.transform.position.y + offset.y, playerPos.transform.position.z + offset.z);	
	
		
	}
	
	void SetPlayerPos(GameObject newPlayerPos)
	{
		playerPos = newPlayerPos;
	}
	
	void SetOffset(Vector3 newOffset)
	{
		offset = newOffset;
	}
}
