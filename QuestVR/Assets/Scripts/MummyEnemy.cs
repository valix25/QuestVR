using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MummyEnemy : MonoBehaviour {

	[Header("NavMesh")]
	public GameObject target;
	public GameObject targetCore;
	public float coreDistance = 2.0f;
	public float resetHitTime = 1.0f;
	public float resetNavAgentTime = 0.75f;
	public GameObject deathEffect;
	public float mummyDeathTime = 1.25f;
	public GameObject weapon;
	public NavMeshAgent navAgent;
	public GameObject spawningOrigin;

	[Header("Base Animations")]
	public string mummyType;
	public string runTrigger;
	public string attackTrigger;
	public string dieTrigger;
	public string getDamageTrigger;
	public string idleTrigger;

	[Header("Extra Animations")]
	public string attack1Trigger;
	public string attack2Trigger;
	public string threatenTrigger;
	public string getDamage1Trigger;

	private Animator anim;
	private bool wasHit = false;
	private float hitTimer = 0.0f;
	private int mummyLives = 2;
	private float navAgentTimer = 0.0f;

	void Start() {
		anim = GetComponent<Animator> ();
		anim.SetTrigger (runTrigger);
	}
	
	// Update is called once per frame
	void Update () {
		navAgent.SetDestination (target.gameObject.transform.position);
		if (wasHit) {
			hitTimer += Time.deltaTime;
			if (hitTimer > resetHitTime) {
				wasHit = false;
				hitTimer = 0.0f;
			}
		}

		if (navAgent.isStopped) {
			navAgentTimer += Time.deltaTime;
			if (navAgentTimer > resetNavAgentTime) {
				navAgent.isStopped = false;
				navAgentTimer = 0.0f;
			}
		}

		if (computeDistance () < navAgent.stoppingDistance + 0.25f) {
			// float randomValue = Random.Range (0.0f, 1.0f);
			float randomValue = 0.2f;
			if (randomValue < 0.33f) {
				anim.SetTrigger (attack1Trigger);
			} else if (randomValue < 0.66f) {
				anim.SetTrigger (attackTrigger);
			} else {
				anim.SetTrigger (attack2Trigger);
			}
			navAgent.isStopped = true;

			if (computeDistanceCore () < coreDistance && wasHit == false) {
				print ("Lives -1");
				reducePlayerLives ();
				wasHit = true;
			}

		} else {
			if (navAgent.isStopped == false) {
				anim.SetTrigger (runTrigger);
			}
		}
	}

	float computeDistance(){
		return Vector2.Distance (new Vector2 (transform.position.x, transform.position.z),
			new Vector2 (target.transform.position.x, target.transform.position.z));
	}

	float computeDistanceCore() {
		return Vector3.Distance (weapon.transform.position, targetCore.transform.position);
	}

	void reducePlayerLives() {
		target.GetComponent<Damage> ().lives -= 1;
	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Laser") {
			mummyLives -= 1;
			Destroy (col.gameObject);
			if (mummyLives == 1) {
				navAgent.isStopped = true;
				anim.SetTrigger (threatenTrigger);
				if (threatenTrigger == "Gd") {
					navAgentTimer += resetNavAgentTime / 1.6f;
				}
			} else {
				navAgent.isStopped = true;
				anim.SetTrigger (dieTrigger);
				Invoke ("MummyDie", mummyDeathTime);
			}
		}
		if (col.gameObject.tag == "Fireball") {
			navAgent.isStopped = true;
			anim.SetTrigger (dieTrigger);
			Invoke ("MummyDie", mummyDeathTime);
		}
	}

	void MummyDie() {
		if (spawningOrigin != null) {
			spawningOrigin.GetComponent<Spawning> ().currentEnemyCounter -= 1;
		}
		Destroy (gameObject);
		if(deathEffect != null) {
			// Invoke ("playDeathEffect", 0.8f);
			playDeathEffect();
		}
	}

	void playDeathEffect() {
		GameObject explosionObj = Instantiate(deathEffect, transform.position, transform.rotation);
		Destroy (explosionObj, 1.5f);
	}
}
