using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	public float secondsEndFade = 2f;
	public bool isPlaying = false;

	public void LoadLevel(string name){
		Debug.Log ("New Level load: " + name);
//		GameObject go = new GameObject ("LevelManager");
//		LevelManager instance = go.AddComponent<LevelManager> ();
		StartCoroutine (InnerLoad (name));
	}

	IEnumerator InnerLoad(string name) {
		yield return new WaitForSeconds (secondsEndFade);
		//load transition scene
		Object.DontDestroyOnLoad(this.gameObject);

		SceneManager.LoadScene ("Loading Scene");

		//wait one frame (for rendering, etc.)
		yield return null;

		//load the target scene
		SceneManager.LoadScene (name);
		if (isPlaying) {
			Destroy (this.gameObject);
		}
	}
}
