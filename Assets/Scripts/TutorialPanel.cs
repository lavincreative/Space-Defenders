using UnityEngine;
using System.Collections;

public class TutorialPanel : MonoBehaviour {

	private AudioSource music;

	// Use this for initialization
	void Start () {
		music = GameObject.Find ("MusicPlayer").GetComponent<AudioSource>();
		music.Stop ();
		Time.timeScale = 0;
	}

	public void AcceptButton() {		
		Time.timeScale = 1;
		music.Play ();
		gameObject.SetActive (false);
//		Destroy (this.gameObject);
	}
}
