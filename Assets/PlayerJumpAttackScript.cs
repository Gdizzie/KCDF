using UnityEngine;
using System.Collections;

public class PlayerJumpAttackScript : MonoBehaviour {
	
	CharacterMotor cMotor;
	
	GameObject root;
	
	// Use this for initialization
	void Start () {
		
		root = this.transform.root.gameObject;
		
		
	}
	
	// Update is called once per frame
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Enemy")
		{
			Debug.Log ("JumpImpact");
			CharacterMotor cMotor  = transform.root.gameObject.GetComponent<CharacterMotor>();
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
	}
	
	
}
