using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameSettings : MonoBehaviour {

	public Slider volumeOptionsSlider;
	public Text volumePercentage;
	private float volume = 1f;

	public Dropdown resolutionOptionsDropdown;
	public Dropdown qualityOptionsDropdown;
	private int quality;

	LevelManager levelManager;

	// Use this for initialization
	void Start () {
		levelManager = GameObject.Find ("LevelManager").GetComponent<LevelManager>();

		volumeOptionsSlider = volumeOptionsSlider.GetComponent<Slider> ();
		volumePercentage = volumeOptionsSlider.transform.FindChild("Handle Slide Area").FindChild("Handle").FindChild("Volume Percentage").GetComponent<Text>();

		resolutionOptionsDropdown = resolutionOptionsDropdown.GetComponent<Dropdown> ();
		qualityOptionsDropdown = qualityOptionsDropdown.GetComponent<Dropdown> ();

		if (PlayerPrefs.HasKey("Master Volume")) {
			volumeOptionsSlider.value = PlayerPrefs.GetFloat("Master Volume");
		} else {
			volumeOptionsSlider.value = volume;
			AudioListener.volume = volume;
		}

		for (int i = 0; i < Screen.resolutions.Length; i++) {
			resolutionOptionsDropdown.options.Add (new Dropdown.OptionData (Screen.resolutions [i].width + "X" + Screen.resolutions [i].height));
			if (Screen.currentResolution.ToString ().Contains (Screen.resolutions [i].width + " x " + Screen.resolutions [i].height)) {
				resolutionOptionsDropdown.value = i;
			}
		}	
			
		qualityOptionsDropdown.value = QualitySettings.GetQualityLevel();
	}
	
	// Update is called once per frame
	void Update () {		
		AudioListener.volume = volumeOptionsSlider.value;
		volumePercentage.text = (volumeOptionsSlider.value * 100).ToString();
	}		

	public void ViewInLiveResolution() {
		Screen.SetResolution(Screen.resolutions[resolutionOptionsDropdown.value].width,Screen.resolutions[resolutionOptionsDropdown.value].height,true);
	}

	public void ViewInLiveQuality() {
		QualitySettings.SetQualityLevel(qualityOptionsDropdown.value, true);
	}

	public void ApplyChanges(){
		AudioListener.volume = volumeOptionsSlider.value;
		PlayerPrefs.SetFloat ("Master Volume", volumeOptionsSlider.value);
	}

	public void Quit() {
		Application.Quit();
	}

	public void BackMainMenu() {		
//		LevelManager man = GameObject.Find ("LevelManager").GetComponent<LevelManager> ();
		Time.timeScale = 1;
//		man.LoadLevel ("Start Menu");
		levelManager.LoadLevel ("Start Menu");
//		Destroy(gameObject);
	}
}
