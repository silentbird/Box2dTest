using System;
using UnityEngine;

namespace Billiard.Balls {
	public class Ball {
		// 线速度
		public Vector3 linearVelocity;

		// 角速度
		public Vector3 angularVelocity;

		// 半径
		public readonly float radius;

		// 质量
		public readonly float mass = ConstDefine.Mass;

		// 转动惯量 I = 2/5 * m * r^2
		public double inertia => 0.4 * mass * Math.Pow(radius, 2);
		public Vector3 pos;

		public Transform ballTransform;

		public Ball(Transform ballTransform, float density = 1f) {
			this.ballTransform = ballTransform;

			pos = ballTransform.position;

			radius = ballTransform.localScale.x * 0.5f;

			mass = (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3f) * density;
		}


		public Ball(Vector3 pos, float mass = ConstDefine.Mass) {
			this.pos = pos;
			this.mass = mass;
		}


		public virtual void UpdateVisualPosition() {
			ballTransform.position = pos;
		}
	}
}