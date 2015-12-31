using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartButtonSelected : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Button pressedButton = gameObject.GetComponent<Button> ();
		pressedButton.Select ();
	}

}
