using UnityEngine;
using System.Collections;

public class FXScript : MonoBehaviour {

	 private tk2dSpriteAnimator anim;
	
	// Use this for initialization
	void Start () {
		
		anim = this.GetComponent<tk2dSpriteAnimator>();
	
	}
	
	// Update is called once per frame
	/*void Update () {
		
		
	
	}*/
	
	void PlayFX(string clip)
	{
		//Debug.Log (clip);
		
		if(!anim)
		{
			
			anim = this.GetComponent<tk2dSpriteAnimator>();	
			anim.Play(clip);
			StartCoroutine(FXControl(clip));
		}
		else
		{
			anim.Play(clip);
			StartCoroutine(FXControl(clip));
		}
	}
	
	IEnumerator FXControl(string clip)
	{
		while(anim.IsPlaying(clip))
		{
			yield return null;	
		}
		
		if(this.name != "ClonedObject")
		{
			ObjectPool.instance.PoolObject(this.gameObject);
		}
		else
		{
			Destroy(this.gameObject);	
		}
	}
}
