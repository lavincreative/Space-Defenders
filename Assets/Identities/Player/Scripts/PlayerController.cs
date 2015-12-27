using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	private Vector3 movementVectorAxis;
	public float speed = 8.0f;
	public float paddingX = 1f;
	public float paddingY = 1f;

	public float smooth = 2.0F;
	public float tiltAngle = 30.0F;

	float xmin, xmax;
	float ymin, ymax;

	// Use this for initialization
	void Start () {
		float distance = transform.position.z - Camera.main.transform.position.z;
		Vector3 leftmost = Camera.main.ViewportToWorldPoint (new Vector3 (0,0,distance));
		Vector3 rightmost = Camera.main.ViewportToWorldPoint (new Vector3 (1,0,distance));
		Vector3 downmost = Camera.main.ViewportToWorldPoint (new Vector3 (0,0,distance));
		Vector3 upmost = Camera.main.ViewportToWorldPoint (new Vector3 (0,1,distance));
		xmin = leftmost.x + paddingX;
		xmax = rightmost.x - paddingX;
		ymin = downmost.y;
		ymax = upmost.y - paddingY;
	}
	
	// Update is called once per frame
	void Update () {

		movementVectorAxis.y = Input.GetAxis ("Vertical") * speed * Time.deltaTime;
		movementVectorAxis.x = Input.GetAxis ("Horizontal") * speed * Time.deltaTime;
		transform.position += movementVectorAxis;

		float tiltAroundY = Input.GetAxis("Horizontal") * tiltAngle;
//		float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle;
		Quaternion target = Quaternion.Euler(transform.rotation.x, -tiltAroundY, transform.rotation.z);
//		transform.rotation = Quaternion.Euler(0, 0, 30);
		transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

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

		float newX = Mathf.Clamp (transform.position.x, xmin, xmax);
		float newY = Mathf.Clamp (transform.position.y, ymin, ymax);

		transform.position = new Vector3 (newX, newY, transform.position.z);
	}
}
