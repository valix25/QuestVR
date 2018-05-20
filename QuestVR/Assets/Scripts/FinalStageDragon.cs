using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalStageDragon : MonoBehaviour {

	public float takeoffTime = 2.0f;
	public float afterTakeoffTime = 1.5f;
	public float heightIncrement = 0.4f;
	public float forwardFlyTime = 2.0f;
	public float forwardIncrement = 1.0f;
	public float glideTime = 5.0f;
	public float glideIncrement = 0.7f;

	private Animator anim;
	private bool hasTakenOff = false;
	private float takeoffTimer = 0.0f;
	private bool afterTakeoff = false;
	private float forwardFlyTimer = 0.0f;
	private bool finalGlide = false;
	private float glideTimer = 0.0f;
	private bool endScene = false;

	private string takeOffTrigger = "startFly";
	private string forwardFlyTrigger = "forwardFly";
	private string glideTrigger = "glide";
	private float startHeight;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (hasTakenOff == false) {
			Vector3 increaseHeight = new Vector3 (0.0f, heightIncrement, 0.0f);
			transform.position += increaseHeight;
			takeoffTimer += Time.deltaTime;
			if (takeoffTimer % 60 > takeoffTime) {
				anim.SetTrigger (takeOffTrigger);
				takeoffTimer = 0.0f;
				hasTakenOff = true;
			}
		}

		if (hasTakenOff == true && afterTakeoff == false) {
			Invoke ("afterTakeOffAction", afterTakeoffTime);
		}

		if (afterTakeoff == true && finalGlide == false) {
			Vector3 increaseForward = new Vector3 (0.0f, 0.0f, forwardIncrement);
			transform.position -= increaseForward;
			forwardFlyTimer += Time.deltaTime;
			if (forwardFlyTimer % 60 > forwardFlyTime) {
				forwardFlyTimer = 0.0f;
				finalGlide = true;
				timeToGlideIntoTheHorizon ();
			}
		}

		if (finalGlide == true && endScene == false) {
			Vector3 increaseForward = new Vector3 (0.0f, 0.0f, forwardIncrement);
			transform.position -= increaseForward;
			glideTimer += Time.deltaTime;
			if (glideTimer % 60 > glideTime) {
				endScene = true;
				fadeOutScene ();
			}
		}
	}

	void afterTakeOffAction(){
		anim.SetTrigger (forwardFlyTrigger);
		afterTakeoff = true;
	}

	void timeToGlideIntoTheHorizon(){
		anim.SetTrigger (glideTrigger);	
	}

	void fadeOutScene(){
		float fadeTime = this.GetComponent<Fading> ().BeginFade (1);
		Invoke ("quitGame", fadeTime/1.5f);
	}

	void quitGame() {
		Application.Quit ();
	}
}
