using UnityEngine;
using System.Collections;

public class OnBecameInvisibleCheck : MonoBehaviour {
	
	public bool onBecameInvisible;
	
	public GameObject target;
	
	public bool parent = true;
	
	public bool poolTarget = false;
	
	public bool sendTargetMessage = false;
	
	public string messageString;
	
	
	public bool onBecameVisible;
	
	public GameObject targetV;
	
	public bool parentV = true;
	
	public bool poolTargetV = false;
	
	public bool sendTargetMessageV = false;
	
	public string messageStringV;
	
	
	// Use this for initialization
	void Start () {
		
		if(!target && parent)
		{
			target = this.transform.parent.gameObject;	
		}
		
		if(!target && !parent)
		{
			target = this.gameObject;	
		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnBecameVisible()
	{
		
		if(onBecameVisible)
		{
			if(targetV || parentV)
			{
				if(sendTargetMessageV)
				{
					Debug.Log ("Sending");
					targetV.SendMessage(messageStringV);
				}
				if(poolTargetV)
				{
					
					ObjectPool.instance.PoolObject(targetV);	
				}
			}
		}
		
	}
	
	void OnBecameInvisible()
	{
		if(onBecameInvisible)
		{
			//Debug.Log ("BecameInvisible");
			if(target || parent)
			{
				if(sendTargetMessage)
				{
					target.SendMessage(messageString);
				}
				if(poolTarget)
				{
					
					ObjectPool.instance.PoolObject(target);	
				}
			}
		}
	}
}
