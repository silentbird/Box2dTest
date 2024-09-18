using System;
using UnityEngine;

public class Circle : MonoBehaviour {
	private Rigidbody2D _circleRigid;

	private void Awake() {
		_circleRigid = GetComponent<Rigidbody2D>();
	}

	private void OnTriggerEnter(Collider other) {
		Debug.Log($"OnTriggerEnter:{other.gameObject.name}");
		_circleRigid.velocity = Vector2.zero;
	}

	private void OnTriggerExit(Collider other) {
		Debug.Log($"OnTriggerExit:{other.gameObject.name}");
	}

	private void OnCollisionEnter(Collision other) {
		Debug.Log($"OnCollisionEnter:{other.gameObject.name}");
		_circleRigid.velocity = Vector2.zero;
	}

	private void OnCollisionExit(Collision other) {
		Debug.Log($"OnTriggerExit:{other.gameObject.name}");
	}
}