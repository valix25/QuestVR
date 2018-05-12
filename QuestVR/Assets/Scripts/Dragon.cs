using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DragonState {
	Idle, Weak, Dead
}

public class Dragon : MonoBehaviour {

	public DragonState state;
	private DragonState currentState;
	private playerControl dragon;

	void Awake () {
		state = DragonState.Idle;
		dragon = GetComponent<playerControl> ();
	}
	
	void Update () {
		
	}

	public void SetState(DragonState newState) {
		
		if (newState != state) {
			state = newState;
			switch (state) {
			case DragonState.Weak:
				dragon.Idle02 ();
				break;
			case DragonState.Dead:
				dragon.Die ();
				break;
			}
		}
	}

	public void Scream () {
		dragon.Scream ();
	}

	public void Attack () {
		dragon.ClawAttack ();
	}
}
