using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour {
	private Button _btnLogin;

	private void Awake() {
		_btnLogin = transform.Find("BtnLogin").GetComponent<Button>();
		_btnLogin.onClick.AddListener(() => { SceneManager.LoadScene("Login"); });
		Debug.Log("LoginPanel Awake");
	}
}