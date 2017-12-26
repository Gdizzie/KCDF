using UnityEngine;
using System.Collections;

public class RagdollScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {
	
		GameObject.Find ("PlayerDependent").BroadcastMessage("SetPlayer", this.gameObject);
		//GameObject.Find ("CameraPointer").BroadcastMessage("SetOffset", Vector3.zero);
		GameObject.Find ("ObjectPool").BroadcastMessage("SetPlayer", this.gameObject);
		//GameObject.Find ("PlayerPos").SendMessage("SetPlayer", this.gameObject);
		//GameObject.Find ("PlayerPos").SendMessage("SetOffset", Vector3.zero);
		//StartCoroutine(RetargetCamera());
		
		//rigidbody.AddForce (Vector3.left * 1000, ForceMode.Impulse);
		
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/
	
	public IEnumerator RetargetCamera()
	{
		yield return new WaitForSeconds(0.1f);
		
	}	
	
	
}
