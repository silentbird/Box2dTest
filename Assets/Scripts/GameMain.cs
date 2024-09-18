using UnityEngine;

public class GameMain : MonoBehaviour {
	private void Awake() {
		DontDestroyOnLoad(gameObject);
	}
}