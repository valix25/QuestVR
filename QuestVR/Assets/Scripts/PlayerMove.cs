using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

	[SerializeField] GameObject playerControl;

	public GameObject centralAnchor;
	public GameObject arms;

	public bool activeWalk = false;
	public float referenceX;
	public float currentX;
	public int differenceX;

	public int speed = 0;

	void Update () {
		Move ();
		Translate ();
	}

	public void Move(){
		if (OVRInput.GetDown (OVRInput.Button.PrimaryTouchpad) && !activeWalk) {
			activeWalk = true;
			referenceX = OVRInput.GetLocalControllerRotation (OVRInput.Controller.RTrackedRemote).eulerAngles.x;
			if (referenceX < 91)
				referenceX += 360.0f;
		} else if (OVRInput.GetDown (OVRInput.Button.PrimaryTouchpad) && activeWalk) {
			activeWalk = false;
			speed = 0;
		}
		if (activeWalk) {
			currentX = OVRInput.GetLocalControllerRotation (OVRInput.Controller.RTrackedRemote).eulerAngles.x;
			differenceX = (int)(referenceX - currentX);
			//speed = 0; Attention, need upper-lower boundaries because otherwise it will default to the next opt without upper boudary
			if (differenceX >= 70 && differenceX < 120) {
				if (speed != 16) {
					speed = 16;
					Debug.Log ("VR: Speed set to " + speed);
				}
			} else if (differenceX >= 50 && differenceX < 70) {
				if (speed != 8) {
					speed = 8;
					Debug.Log ("VR: Speed set to " + speed);
				}
			} else if (differenceX >= 30 && differenceX < 50) {
				if (speed != 5) {
					speed = 5;
					Debug.Log ("VR: Speed set to " + speed);
				}
			} else if (differenceX >= 0 && differenceX < 30) {
				if (speed != 1) {
					speed = 1;
					Debug.Log ("VR: Speed set to 1");
				}
			} else if (differenceX < 0 && differenceX > -30) {
				if (Mathf.Abs (speed) != 4) {
					speed = -4;
					Debug.Log ("VR: Speed set to 1");
				}

			} else if (differenceX < -30 && differenceX > -100) {
				if (Mathf.Abs (speed) != 8) {
					speed = -8;
					Debug.Log ("VR: Speed set to 1");
				}
			}
			else {
				speed = 0;
				Debug.Log ("VR: Default case Speed set to " + speed);
			}
		} else if (!activeWalk) {
			if (speed != 0)
				speed = 0;
		}
	}
	public void Translate(){
		Vector3 move = Vector3.zero;
		move += centralAnchor.transform.forward;
		move = move.normalized * Time.deltaTime * speed;
		playerControl.transform.Translate (move, Space.World);
	}
}
