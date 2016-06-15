using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

	// factor for cube movement speed
	float speed = 10f;
	private float slowMoTimeScale;
	private float factor = 4f;

	void Start() {
		slowMoTimeScale = Time.timeScale / factor;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.S)) {
			slowMo();
		}

		// get input from keyboard i.e. the arrow keys
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		// add the input values to the Player so as to move it
		transform.position += new Vector3(h,0,v) * Time.deltaTime * speed * 1 / Time.timeScale;
	}

	void slowMo() {
		// assing new time scale value
		Time.timeScale = slowMoTimeScale;
		// reduce this to the same proportion as timescale to ensure smooth simulation
		Time.fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;
		StartCoroutine(WaitRefreshLive(5.0f));
	}

	IEnumerator WaitRefreshLive(float waitTime) {
		yield return new WaitForSeconds (waitTime);
		Time.timeScale = 1;
	}
}
