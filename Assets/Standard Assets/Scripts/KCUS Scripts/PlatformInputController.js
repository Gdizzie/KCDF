// This makes the character turn to face the current movement speed per default.
var autoRotate : boolean = true;
var maxRotationSpeed : float = 360;

private var motor : CharacterMotor;

private var charm : CharacterMotorMovement;

private var calibrationMatrix : Matrix4x4;

private var storedDir = Vector3.zero;

public var inversion : Vector3;

public var tiltLayout = "null";

//var directionVector : Vector3;// = new Vector3(dir.z + 1.5f, 0f, 0f);

// Use this for initialization
function Awake () {
	motor = GetComponent(CharacterMotor);
	charm = GetComponent(CharacterMotorMovement);
	
	
	
	//PlayerPrefs.Set
	
	if(PlayerPrefs.GetString("TiltLayout") == "LR")
	{
		tiltLayout = "LR";//PlayerPrefs.GetString("TiltLayout");
	}
	else if(PlayerPrefs.GetString("TiltLayout") == "FB")
	{
		tiltLayout = "FB";//PlayerPrefs.GetString("TiltLayout");	
	}
	else
	{
		PlayerPrefs.SetString("TiltLayout", "LR");	
		tiltLayout = "LR";
	}
}

function Start()
{

	if(PlayerPrefs.GetInt("Inverted") == 1)
	{
		inversion = Vector3(-1, 1, 1);
	}
	else if(PlayerPrefs.GetInt("Inverted") == 0)
	{
		inversion = Vector3(1, -1, -1);
	}
	else
	{
		PlayerPrefs.SetInt("Inverted", 0);
		inversion = Vector3(1, -1, -1);
	}
	//CalibrateAccelerometer();
}

function JumpSend()
{
	Debug.Log("JumpSend");
	this.motor.inputJump = true;
}


/*public function ReturnVelocity(): Vector3
{
	return charm.velocity;
}*/

// Update is called once per frame
function Update () {


	var dir : Vector3;
	var accel : Vector3;
	var directionVector = new Vector3();
	
	
	

	//dir.z = Input.acceleration.x;

	//Debug.Log("Dir.Z1: " + dir.y);	

	
	
	if(tiltLayout == "FB")
	{
		if(1 > 2)//Application.isEditor)
		{
			accel = Vector3(5, 5, 5);
			dir = Vector3(1, 1, 1);
		}
		else
		{
			accel = Input.acceleration;
			dir = FixAcceleration(accel);// Vector3.zero;
		}
		
		if (dir.sqrMagnitude > 1)
		{
			dir.Normalize();
		}
		
		dir *= Time.deltaTime;
		
		directionVector = new Vector3(dir.y * 400, 0, 0);//, Input.GetAxis("Vertical"), 0);
	
		directionVector.x += (dir.y * 145);// - 0.5f;
	}
	else
	{
		//dir.x = Input.acceleration.x;
	
		accel = Input.acceleration;
		dir = FixAcceleration(accel);
	
		if (dir.sqrMagnitude > 1)
		{
			dir.Normalize();
		}
		dir *= Time.deltaTime;
		
		directionVector = new Vector3((dir.x * 400) + 1.5f, 0f, 0f);//, Input.GetAxis("Vertical"), 0);
	}
		
//	if(this.GetComponent(CharacterMotor).grounded)
//	{
//		storedDir = Vector3.zero;
//		directionVector = new Vector3(dir.y * 400, 0, 0);//, Input.GetAxis("Vertical"), 0);
//	
//		directionVector.x += (dir.y * 145);// - 0.5f;
//	}
//	else
//	{
//		if(storedDir.y == Vector3.zero)
//		{
//			storedDir.y = dir.y;
//		}
//		directionVector = new Vector3(dir.y * 400 * 0.9f, 0, 0);//, Input.GetAxis("Vertical"), 0);
//	
//		//storedDir + dir * friction (0.2);
//	
//		directionVector.x += (dir.y * 145);// - (dir.y * 0.97f);// - 0.5f;
//	}
	
	//if(this.GetComponent(CharacterMotor).grounded)
	//{
		//storedDir = Vector3.zero;
		//directionVector = new Vector3(dir.y * 400, 0, 0);//, Input.GetAxis("Vertical"), 0);
	
		//directionVector.x += (dir.y * 145);// - 0.5f;
	//}
	/*else
	{
		if(storedDir.y == Vector3.zero)
		{
			storedDir.y = dir.y;
		}
		directionVector = new Vector3(dir.y * 400 * 0.9f, 0, 0);//, Input.GetAxis("Vertical"), 0);
	
		//storedDir + dir * friction (0.2);
	
		directionVector.x += (dir.y * 145);// - (dir.y * 0.97f);// - 0.5f;
	}*/
		
		
	//var dir = Vector3.zero;




	if(directionVector.x < 0.25f)
	{
		directionVector.x = 0.25f;
	} 
	

	
		
		//Debug.Log("Dir.Z: " + dir.z);
		
		//Debug.Log("DirectionVector: " + directionVector);
		
		
	if (directionVector != Vector3.zero) {
		// Get the length of the directon vector and then normalize it
		// Dividing by the length is cheaper than normalizing when we already have the length anyway
		var directionLength = directionVector.magnitude;
		directionVector = directionVector / directionLength;
		
		// Make sure the length is no bigger than 1
		//directionLength = Mathf.Min(1, directionLength);
		
		// Make the input vector more sensitive towards the extremes and less sensitive in the middle
		// This makes it easier to control slow speeds when using analog sticks
		//directionLength = directionLength * directionLength;
		
		// Multiply the normalized direction vector by the modified length
		directionVector = directionVector * directionLength;
	}
	
	
	
	// Rotate the input vector into camera space so up is camera's up and right is camera's right
	//directionVector = Camera.main.transform.rotation * directionVector;
	directionVector = GameObject.Find("ScriptManager").transform.rotation * directionVector;
	
	// Rotate input vector to be perpendicular to character's up vector
	//var camToCharacterSpace = Quaternion.FromToRotation(-Camera.main.transform.forward, transform.up);
	var camToCharacterSpace = Quaternion.FromToRotation(-GameObject.Find("ScriptManager").transform.forward, transform.up);
	directionVector = (camToCharacterSpace * directionVector);
	
	// Apply the direction to the CharacterMotor
	motor.inputMoveDirection = directionVector;
	motor.inputJump = Input.GetButton("Jump");
	
	// Set rotation to the move direction	
	if (autoRotate && directionVector.sqrMagnitude > 0.01) {
		var newForward : Vector3 = ConstantSlerp(
			transform.forward,
			directionVector,
			maxRotationSpeed * Time.deltaTime
		);
		newForward = ProjectOntoPlane(newForward, transform.up);
		transform.rotation = Quaternion.LookRotation(newForward, transform.up);
	}
}

function ProjectOntoPlane (v : Vector3, normal : Vector3) {
	return v - Vector3.Project(v, normal);
}

function ConstantSlerp (from : Vector3, to : Vector3, angle : float) {
	var value : float = Mathf.Min(1, angle / Vector3.Angle(from, to));
	return Vector3.Slerp(from, to, value);
}

//Used to calibrate the initial iPhoneIput.acceleration input
function CalibrateAccelerometer () {
	Debug.Log("Calibrating");
    var accelerationSnapshot : Vector3 = Input.acceleration;
    var rotateQuaternion : Quaternion = Quaternion.FromToRotation(new Vector3(0.0, 0.0, 1.0), accelerationSnapshot);
 
    //create identity matrix ... rotate our matrix to match up with down vec
    var matrix : Matrix4x4 = Matrix4x4.TRS(Vector3.zero, rotateQuaternion, inversion);
 
    //get the inverse of the matrix
    calibrationMatrix = matrix.inverse;
    
    GameObject.Find("ScriptManager").SendMessage("StoreCalibrationMatrix", matrix.inverse);
    
   	//AccelMatrix.instance.calibrationMatrix = matrix.inverse;
}
 
//  Get the 'calibrated' value from the iPhone Input
function FixAcceleration (accelerator : Vector3) {
    var fixedAcceleration : Vector3 = calibrationMatrix.MultiplyVector(Vector3(accelerator.x, accelerator.y + 0.5f, accelerator.z));
    return fixedAcceleration;
}

function SetMatrix(matrix : Matrix4x4)
{
	calibrationMatrix = matrix;
}

public function SwapInversion()
{
	if(PlayerPrefs.GetInt("Inverted") == 0)
	{
		PlayerPrefs.SetInt("Inverted", 1);
		inversion = Vector3(-1, 1, 1);
	}
	else if(PlayerPrefs.GetInt("Inverted") == 1)
	{
		PlayerPrefs.SetInt("Inverted", 0);
		inversion = Vector3(1, -1, -1);
	}
}

function SetTilt(tilt : String)
{
	if(tilt == "FB")
	{
		tiltLayout = "FB";
		PlayerPrefs.SetString("TiltLayout", "FB");
	}
	else
	{
		tiltLayout = "LR";
		PlayerPrefs.SetString("TiltLayout", "LR");
	}
}

// Require a character controller to be attached to the same game object
@script RequireComponent (CharacterMotor)
@script AddComponentMenu ("Character/Platform Input Controller")
