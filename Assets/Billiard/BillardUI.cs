using UnityEngine;
using UnityEngine.UI;

namespace Billiard {
	public class BillardUI : MonoBehaviour {
		public Button btnReset;

		private void Awake() {
			btnReset.onClick.AddListener(OnBtnResetClick);
		}

		private static void OnBtnResetClick() {
			// BilliardController.Instance.ResetSimulation();
			BallMovement.Instance.ApplyImpulse(new Vector3(0f, 0f, 10f), new Vector3(0f, 0f, 0f));
		}
	}
}