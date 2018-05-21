using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Dragon : MonoBehaviour {

	public NavMeshAgent navAgent;
	public GameObject player;
	public int SwordNumber = 1;
	public float timeToFloat = 3.0f;

	private Animator anim;
	private string screamTrigger = "Scream";
	private string sleepTrigger = "Sleep";
	private string walkTrigger = "Walk";
	private string attackTrigger = "Attack";
	private string takeOffTrigger = "TakeOff";
	private string floatTrigger = "Float";
	private bool followPlayer = false;
	private bool stopToAttack = false;
	private float timerForFloating = 0.0f;
	private bool isFloating = false;
	private bool isEndSceneLoading = false;

	void Start() {
		anim = GetComponent<Animator> ();
	}

	void Update() {
		navAgent.SetDestination (player.gameObject.transform.position);
		navAgent.isStopped = !followPlayer;

		if (followPlayer) {
			if (computeDistance () < navAgent.stoppingDistance + 0.25f) {
				anim.SetTrigger (attackTrigger);
				anim.ResetTrigger (walkTrigger);
				navAgent.isStopped = true;
			} else {
				anim.SetTrigger (walkTrigger);
				anim.ResetTrigger (attackTrigger);
			}
		}

		if (SwordNumber <= 0 && isFloating == false) {
			print("Should fly");
			anim.SetTrigger (floatTrigger);
			isFloating = true;
		}

		if (isFloating) {
			timerForFloating += Time.deltaTime;
			if (timerForFloating % 60 > timeToFloat && isEndSceneLoading == false) {
				float fadeTime = this.GetComponent<Fading> ().BeginFade (1);
				Invoke ("loadEndScene", fadeTime / 1.5f);
				isEndSceneLoading = true;
			}
		}
	}

	public void startScream(){
		anim.SetTrigger (screamTrigger);
	}

	public void startWalking(){
		anim.SetTrigger (walkTrigger);
		followPlayer = true;
	}

	float computeDistance(){
		return Vector2.Distance (new Vector2 (transform.position.x, transform.position.z),
			new Vector2 (player.transform.position.x, player.transform.position.z));
	}

	public void backToSleep(){
		anim.SetTrigger (sleepTrigger);
		anim.ResetTrigger (walkTrigger);
		anim.ResetTrigger (attackTrigger);
		followPlayer = false;
	}

	void loadEndScene(){
		SceneManager.LoadScene ("FinalScene");
	}
}	
