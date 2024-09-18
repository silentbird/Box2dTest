using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanel : MonoBehaviour {
	public Rigidbody2D circle;

	public void onBtnResetClick() {
		circle.transform.position = new Vector3(0, 0, 0);
		circle.velocity = new Vector2(Random.Range(-600, 600), Random.Range(0, 600));
	}
}