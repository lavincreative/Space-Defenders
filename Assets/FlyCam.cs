using UnityEngine;
using System.Collections;

public class FlyCam : MonoBehaviour {

	public float speed = 50.0f;	// max speed of camera
	public float sensitivity = 0.25f;  // keep it from 0 to 1
	public bool inverted = false;

	private Vector3 lastMouse = new Vector3 (0, 0, 0);

	private Vector3 movementVectorAxis;
	public float moveSpeed = 8.0f;
	public float tiltAngle = 30.0f;
	public float moveSmooth = 2.0f;
//	private Vector3 lastAxis = new Vector3 (255, 255, 255);

	// smoothing
	public bool smooth = true;
	public float acceleration = 0.1f;
	private float actSpeed = 0.0f;	// kee it from 0 to 1
	private Vector3 lastDir = new Vector3();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		// Mouse Look

//		lastMouse = Input.mousePosition - lastMouse;
//		if( !inverted) lastMouse.y = -lastMouse.y;
//		lastMouse *= sensitivity;
//		lastMouse = new Vector3 (transform.eulerAngles.x + lastMouse.y, transform.eulerAngles.y + lastMouse.x, 0);
//		transform.eulerAngles = lastMouse;
//		lastMouse = Input.mousePosition;

		// Movement of the camera

		Vector3 dir = new Vector3 ();

		if(Input.GetKey(KeyCode.W)) dir.z += 1.0f;
		if(Input.GetKey(KeyCode.S)) dir.z -= 1.0f;
		if(Input.GetKey(KeyCode.A)) dir.x -= 1.0f;
		if(Input.GetKey(KeyCode.D)) dir.x += 1.0f;
		if (Input.GetKey (KeyCode.Space)) dir.y += 1.0f;
		if (Input.GetKey (KeyCode.C)) dir.y -= 1.0f;
		dir.Normalize ();

		// XBOX Commands
		// Movement of the camera
//		transform.Translate (Input.GetAxis ("Horizontal") * speed * Time.deltaTime, Input.GetAxis ("Vertical") * speed * Time.deltaTime, Input.GetAxis ("Player01_Triggers") * speed * Time.deltaTime);
		//Rotates the Camera
		transform.Rotate(Input.GetAxis("Horizontal") * Vector3.up * Time.deltaTime * speed);
		transform.Rotate(Input.GetAxis("Vertical") * Vector3.right * Time.deltaTime * speed);

		if (Input.GetKey (KeyCode.Joystick1Button4)) transform.Rotate (Vector3.forward * Time.deltaTime * speed);
		if (Input.GetKey (KeyCode.Joystick1Button5)) transform.Rotate (Vector3.back * Time.deltaTime * speed);

		if (Input.GetKey (KeyCode.Joystick1Button0)) dir.y += 1.0f;
		if (Input.GetKey (KeyCode.Joystick1Button1)) dir.y -= 1.0f;

		// Set Camera Rotation
		GetComponentInChildren<Camera>().transform.Rotate(Input.GetAxis ("Player01_HorizontalRotation") * Vector3.up * Time.deltaTime * speed);
		GetComponentInChildren<Camera>().transform.Rotate(Input.GetAxis ("Player01_VerticalRotation") * Vector3.right * Time.deltaTime * speed);
		if (Input.GetKey (KeyCode.Joystick1Button9)) GetComponentInChildren<Camera> ().transform.rotation = transform.rotation;

		dir.z = Input.GetAxis ("Player01_Triggers") * speed;
		dir.Normalize ();

		if (dir != Vector3.zero) {
			// some movement
			if (actSpeed < 1)
				actSpeed += acceleration * Time.deltaTime * 40;
			else
				actSpeed = 1.0f;

				lastDir = dir;
		} else {
			// should stop
			if (actSpeed > 0)
				actSpeed -= acceleration * Time.deltaTime * 20;
			else
				actSpeed = 0.0f;
		}

		if (smooth) {
			transform.Translate (lastDir * actSpeed * speed * Time.deltaTime);	
		} else {
			transform.Translate (dir * speed * Time.deltaTime);
		}
	}

	void OnTriggerEnter (Collider col) {
		if (col.tag == "Enemy") {
			Debug.Log ("Enemy enter");
			GetComponent<Rigidbody> ().MovePosition (new Vector3 (200, 200, 200));
		}
	}

	void OnGUI() {
		GUILayout.Box ("actSpeed: " + actSpeed.ToString());
	}
}
