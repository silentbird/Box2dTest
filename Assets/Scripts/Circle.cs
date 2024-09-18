using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Circle : MonoBehaviour {
	private Rigidbody2D _circleRigid;

	private void Awake() {
		_circleRigid = GetComponent<Rigidbody2D>();
		_circleRigid.velocity = new Vector2(Random.Range(-600, 600), Random.Range(0, 600));
	}

	private void FixedUpdate() {
	}

	private void OnCollisionEnter2D(Collision2D other) {
	}

	private void OnCollisionExit2D(Collision2D other) {
	}
}