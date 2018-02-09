using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	
	bool paused = false;
	
	GameObject player;
	
	private float distanceFloat = 0f;
	
	public string distanceString;
	
	GameObject spawnPoint;
	
	public string currentLayout;
	
	public bool inverted;
	
	public Matrix4x4 calibrationMatrix;
	
	// Use this for initialization
	void Start () {
		
		player = GameObject.Find("Player");
		
		StartCoroutine(InitialCalibrationMatrix());
		/*if(PlayerPrefs.GetInt("CalibrationSet") != 1)
		{
			player.SendMessage("CalibrateAccelerometer");
			PlayerPrefs.SetInt("CalibrationSet", 1);
		}
		else if(PlayerPrefs.GetInt("CalibrationSet") == 1)
		{
			player.SendMessage("SetMatrix", calibrationMatrix);
		
		}*/
		
		if(PlayerPrefs.GetInt("Inverted") == 0)
		{
			inverted = false;	
		}
		else if(PlayerPrefs.GetInt("Inverted") == 1)
		{
			inverted = true;	
		}
		else
		{
			inverted = false;	
		}
		
		spawnPoint = GameObject.Find("SpawnPoint");
		
		SetLayout();
		
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(player)
		{
			distanceFloat = Vector3.Distance(spawnPoint.transform.position, player.transform.position);
		
			distanceString = Mathf.RoundToInt(distanceFloat).ToString();
		}
		
		
		if(Input.GetKeyDown(KeyCode.P))
		{
			PauseGame();
		}
	

		
		
	}
	
	public void PauseGame()
	{
		if(paused == false)
		{
			Debug.Log ("Pause");	
			paused = true;
			Time.timeScale = 0.00000001f;
		}else if(paused)
		{
			paused = false;
			Time.timeScale = 1;
		}
	}
	
	public void SetPlayer(GameObject newPlayer)
	{
		player = newPlayer;
	}
	
	
	
	public void SwapLayout()
	{
		if(currentLayout == "[sprite \"TiltIconLR\"]")
		{
			currentLayout = "[sprite \"TiltIconFB\"]";
			player.SendMessage("SetTilt", "FB");
		}
		else
		{
			currentLayout = "[sprite \"TiltIconLR\"]";  //"[sprite " + "TiltIconLR" + "]";
			player.SendMessage("SetTilt", "LR");
		}
	}
	
	public void SetLayout()
	{
		if(currentLayout == "[sprite \"TiltIconLR\"]")
		{
			player.SendMessage("SetTilt", "LR");
		}
		else
		{
			player.SendMessage("SetTilt", "FB");
		}
	}
	
	public void SwapTiltInversion()
	{
		if(inverted)
		{
			inverted = false;	
		}
		else
		{
			inverted = true;	
		}
		player.SendMessage("SwapInversion");	
	}
	
	IEnumerator InitialCalibrationMatrix()
	{
		yield return new WaitForSeconds(0.3f);
		//calibrationMatrix = matrix;
		player.SendMessage("CalibrateAccelerometer");
	}
	
	public void SetCalibrationMatrix()
	{
		player.SendMessage("CalibrateAccelerometer");	
	}
	
	public void RestoreCalibrationMatrix()
	{
		player.SendMessage("SetMatrix", calibrationMatrix);
	}
	
	public void StoreCalibrationMatrix(Matrix4x4 matrix)
	{
		calibrationMatrix = matrix;
	}
}
