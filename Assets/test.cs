using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//rotate the cube at a contant rate
		transform.Rotate(new Vector3(200,200,200) * Time.deltaTime);
	}
}
