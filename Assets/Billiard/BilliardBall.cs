using Billiard.Balls;
using UnityEngine;

namespace Billiard {
	public class BilliardBall : Ball {
		private bool isActive = true;

		public BilliardBall(Vector3 ballVelocity, Transform ballTrans) : base(ballTrans) {
			linearVelocity = ballVelocity;
		}


		public BilliardBall(Transform ballTrans) : base(ballTrans) {
		}


		public void SimulateBall(int subSteps, float sdt, float friction) {
			if (!isActive) {
				return;
			}

			Vector3 gravity = Vector3.zero;

			for (int step = 0; step < subSteps; step++) {
				// 更新位置
				// linearVelocity += gravity * sdt;
				// pos += linearVelocity * sdt;

				//角速度影响位置
				// if (Mathf.Abs((linearVelocity - angularVelocity * radius).magnitude) > 1e-3) {
				// 	// 滑动摩擦力
				// 	double frictionForce = ConstDefine.MuK * mass * 9.81; // f = μ * m * g
				// 	double frictionAcceleration = frictionForce / mass; // 摩擦产生的减速度
				//
				// 	// 更新线速度和角速度
				// 	linearVelocity -= frictionAcceleration * sdt;
				// 	angularVelocity += (frictionForce * radius / inertia) * sdt;
				// }
			}
		}


		public void DeActivateBall() {
			isActive = false;

			ballTransform.gameObject.SetActive(false);
		}
	}
}