using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DragonState {
	Idle, Mad, Calmed
}

public class Dragon : MonoBehaviour {

	public DragonState state; // For testing
	private DragonState currentState;
	private playerControl dragon;

	void Awake () {
		state = DragonState.Idle;
		dragon = GetComponent<playerControl> ();
	}
	
	void Update () {
		SetState (state); // For testing
	}

	public void SetState(DragonState newState) {
		
		if (newState != currentState) {
			currentState = newState;
			switch (state) {
			case DragonState.Mad:
				dragon.ClawAttack ();
				break;
			case DragonState.Calmed:
				dragon.Idle02 ();
				break;
			}
		}
	}

	public void Scream () {
		dragon.Scream ();
	}

//	public void Attack () {
//		dragon.ClawAttack ();
//	}
}
