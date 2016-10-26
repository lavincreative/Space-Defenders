using UnityEngine;
using System.Collections;

public class CameraMapController : MonoBehaviour {

	private Vector3 pointA;
	public Transform[] pointB;
	public float time = 3.0f;

	private Vector3 posZ;
	public float speedRotation = 1.0f;

	private int nextPos;
	private int prePos;
	private int currentPos = 0;

	private bool MoveBlocked = false;

	public GameObject fadeOutImage;

	LevelManager levelManager;

	// Use this for initialization
	void Start () {
		levelManager = GameObject.Find ("LevelManager").GetComponent<LevelManager>();

		nextPos = 1;
		Debug.Log ("Current mission: " + currentPos.ToString ());
		Debug.Log ("Next mission: " + nextPos.ToString ());
	}
	
	// Update is called once per frame
	void FixedUpdate () {	
		if (!MoveBlocked) {
			pointA = transform.position;
			if (Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Joystick1Button5)) {								
				Debug.Log ("You press W");
				if (nextPos < pointB.Length) {	
					prePos = nextPos - 1;
					currentPos = nextPos;
					StartCoroutine (MoveCamera (transform, pointA, pointB [nextPos].position, time));
					nextPos++;
				}
			}
			if (Input.GetKeyDown (KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Joystick1Button4)) {			
				Debug.Log ("You press S");
				if (currentPos > 0) {	
					nextPos = prePos + 1;
					currentPos = prePos;
					StartCoroutine (MoveCamera (transform, pointA, pointB [prePos].position, time));
					prePos--;
				} 
			}
			if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0)) {
				Debug.Log ("Space pressed");
				fadeOutImage.SetActive (true);
				levelManager.LoadLevel ("Mission" + currentPos);
			}

			if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Joystick1Button1)) {
				Debug.Log ("Esc pressed");
				fadeOutImage.SetActive (true);
				levelManager.LoadLevel ("Start Menu");
			}
		}
	}

	IEnumerator MoveCamera(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time) {
		MoveBlocked = true;
		float i = 0.0f;
		float rate = 1.0f / time;

		while (i < 1.0f) {
			Quaternion targetRotation = Quaternion.LookRotation(pointB[currentPos].parent.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, i);
			// Other kind of camera movement
//			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speedRotation * Time.deltaTime);
			// Testing message
			i += Time.deltaTime * rate;
			thisTransform.position = Vector3.Lerp(startPos, endPos, i);
			yield return null;
		}			
		Debug.Log ("Current mission: " + currentPos.ToString ());
		MoveBlocked = false;
	}
}
