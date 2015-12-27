using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	private Vector3 movementVectorAxis;
	public float speed = 8.0f;
	public float paddingX = 1f;
	public float paddingY = 1f;

	public float smooth = 2.0f;
	public float tiltAngle = 30.0f;

	float xmin, xmax;
	float ymin, ymax;

	public GameObject projectile;
	public float projectileSpeed;
	public float firingRate = 0.2f;

	// Use this for initialization
	void Start () {
		RestrictPositionShip ();
	}

	void RestrictPositionShip ()
	{
		float distance = transform.position.z - Camera.main.transform.position.z;
		Vector3 leftmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance));
		Vector3 rightmost = Camera.main.ViewportToWorldPoint (new Vector3 (1, 0, distance));
		Vector3 downmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0, distance));
		Vector3 upmost = Camera.main.ViewportToWorldPoint (new Vector3 (0, 1, distance));
		xmin = leftmost.x + paddingX;
		xmax = rightmost.x - paddingX;
		ymin = downmost.y;
		ymax = upmost.y - paddingY;
	}

	void Fire(){
		GameObject beam = Instantiate (projectile, transform.position, Quaternion.identity) as GameObject;	
		beam.GetComponent<Rigidbody2D> ().velocity = new Vector3 (0, projectileSpeed, 0);
	}

	// Update is called once per frame
	void Update () {
		// Keyboard controller FIRE
		if (Input.GetKeyDown (KeyCode.Space)) {
			InvokeRepeating ("Fire", 0.000001f, firingRate);
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			CancelInvoke ("Fire");
		}

		// Command "XBOX" controller FIRE
		if (Input.GetKeyDown (KeyCode.Joystick1Button0)) {
			InvokeRepeating ("Fire", 0.000001f, firingRate);
		}
		if (Input.GetKeyUp (KeyCode.Joystick1Button0)) {
			CancelInvoke ("Fire");
		}

		// Smoothed movement of player ship getting Vertical and Horizontal values from Input Parameters
		movementVectorAxis.y = Input.GetAxis ("Vertical") * speed * Time.deltaTime;
		movementVectorAxis.x = Input.GetAxis ("Horizontal") * speed * Time.deltaTime;
		transform.position += movementVectorAxis;

		// Giving smoothed Y (horizontal) rotation when player ship goes left and right
		float tiltAroundY = Input.GetAxis("Horizontal") * tiltAngle;
		Quaternion target = Quaternion.Euler(transform.rotation.x, -tiltAroundY, transform.rotation.z);
		transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

		// Restricting the player's position to gamespace (Starting with RestrictPositionShip() method) 
		float newX = Mathf.Clamp (transform.position.x, xmin, xmax);
		float newY = Mathf.Clamp (transform.position.y, ymin, ymax);
		transform.position = new Vector3 (newX, newY, transform.position.z);

//		// Arrow keys from keyboard
//		if (Input.GetKey (KeyCode.LeftArrow)) {			
////			transform.position += new Vector3 (-speed * Time.deltaTime, 0, 0);
//			transform.position += Vector3.left * speed * Time.deltaTime;
//		} else if (Input.GetKey(KeyCode.RightArrow)) {			
////			transform.position += new Vector3 (speed * Time.deltaTime, 0, 0);
//			transform.position += Vector3.right * speed * Time.deltaTime;
//		}
//
//		if (Input.GetKey (KeyCode.UpArrow)) {			
////			transform.position += new Vector3 (0, speed * Time.deltaTime, 0);
//			transform.position += Vector3.up * speed * Time.deltaTime;	
//		} else if (Input.GetKey (KeyCode.DownArrow)) {			
////			transform.position += new Vector3 (0, -speed * Time.deltaTime, 0);
//			transform.position += Vector3.down * speed * Time.deltaTime;
//		}
	}
}
