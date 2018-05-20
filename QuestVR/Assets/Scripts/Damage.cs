using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

public class Damage : MonoBehaviour {

	public int lives = 3;
	VignetteModel.Settings originalVignetteSettings;
	bool isOriginalSet = false;
	Color color;
	bool isColorSet = false;
	public PostProcessingProfile profile;
	
	// Update is called once per frame
	void Update () {
		if (lives <= 0) {
			float fadeTime = this.GetComponent<Fading> ().BeginFade (1);
			Invoke ("reloadScene", fadeTime/1.5f);
		}
	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Enemy") {
			lives -= 1;
			//damageEffect (); <-- Needs a postprocessing profile
		}
	}

	void damageEffect(){
		VignetteModel.Settings vignetteSettings = profile.vignette.settings;
		if (isOriginalSet == false) {
			originalVignetteSettings = vignetteSettings;
			isOriginalSet = true;
		}
		if (lives <= 0) {
			vignetteSettings.intensity = 1.0f;
			vignetteSettings.smoothness = 1.0f;
			Invoke ("reloadScene", 0.25f);
		} else {
			if (isColorSet == false) {
				if (ColorUtility.TryParseHtmlString ("#F80505", out color)) {
					vignetteSettings.color = color;
					isColorSet = true;
				}
			}
			vignetteSettings.intensity += 0.15f;
			vignetteSettings.smoothness += 0.15f;
		}
		profile.vignette.settings = vignetteSettings;
	}

	void reloadScene() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		if (profile != null) {
			profile.vignette.settings = originalVignetteSettings;
		}
	}
}
