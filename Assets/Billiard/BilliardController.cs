using System.Collections.Generic;
using Billiard.Tables;
using UnityEngine;

namespace Billiard {
	//Simulate billiard balls with different size
	//Based on: https://matthias-research.github.io/pages/tenMinutePhysics/
	public class BilliardController : MonoBehaviour {
		public static BilliardController Instance { get; private set; }

		public GameObject ballPrefabGO;

		public BilliardTable billiardTable;
		public Transform ballsPlace;
		public Transform mainBallPlace;

		//Simulation properties
		private readonly int subSteps = 5;


		//How much velocity is lost after collision between balls [0, 1]
		//Is usually called e
		//Elastic: e = 1 means same velocity after collision (if the objects have the same size and same speed)
		//Inelastic: e = 0 means no velocity after collision (if the objects have the same size and same speed) and energy is lost
		private readonly float restitution = 0.8f;

		private readonly List<BilliardBall> allBalls = new();
		private BilliardBall mainBall;

		private void Awake() {
			Instance = this;
		}

		private void Start() {
			ResetSimulation();

			billiardTable.Init();
		}


		public void ResetSimulation() {
			if (allBalls.Count > 0) {
				allBalls.ForEach(ball => Destroy(ball.ballTransform.gameObject));
				allBalls.Clear();
			}

			float ballRadius = ConstDefine.Radius / 2; // Standard billiard ball radius in meters
			float ballCalibre = ballRadius * 2;


			BilliardMaterials.GiveBallsRandomColor(ballPrefabGO, allBalls);
			mainBall = SetupBalls.AddBall(ballPrefabGO, allBalls, new Vector3(0, ballRadius, 0), ballCalibre, mainBallPlace);

			// 初始条件
			float linearVelocity = 0.0f; // 线速度 (m/s)
			float angularVelocity = 0.0f; // 角速度 (rad/s)
			float force = 10.0f; // 向右的瞬时力 (N)
			float timeStep = 0.01f; // 时间步长 (秒)
			// 力作用在台球表面，产生线速度和角速度
			float acceleration = force / ConstDefine.Mass; // 线加速度 a = F/m
			linearVelocity = acceleration * timeStep; // 初始线速度

			// 计算力矩和角加速度
			float torque = force * ConstDefine.Radius; // 力矩 τ = F * R
			float angularAcceleration = torque / ConstDefine.Inertia; // 角加速度 α = τ / I
			angularVelocity = angularAcceleration * timeStep; // 初始角速度
			mainBall.linearVelocity = new Vector3(0, 0, linearVelocity);
			mainBall.angularVelocity = new Vector3(0, 0, angularVelocity);
			Debug.Log($"初始线速度: {linearVelocity} m/s");
			Debug.Log($"初始角速度: {angularVelocity} rad/s");


			for (int row = 0; row < 5; row++) {
				for (int col = 0; col <= row; col++) {
					float x = col * ballCalibre - row * ballRadius;
					float z = Mathf.Cos(Mathf.Deg2Rad * 30) * row * ballCalibre;
					var pos = new Vector3(x, ballRadius, z);
					SetupBalls.AddBall(ballPrefabGO, allBalls, pos, ballCalibre, ballsPlace);
					return;
				}
			}
		}


		private void Update() {
			//Update the transform with the position we simulate in FixedUpdate
			foreach (BilliardBall ball in allBalls) {
				ball.UpdateVisualPosition();
			}
		}


		private void FixedUpdate() {
			float sdt = Time.fixedDeltaTime / subSteps;

			for (int i = 0; i < allBalls.Count; i++) {
				BilliardBall thisBall = allBalls[i];

				thisBall.SimulateBall(subSteps, sdt, billiardTable.friction);

				//Check collision with the other balls after this ball in the list of all balls
				for (int j = i + 1; j < allBalls.Count; j++) {
					BilliardBall ballOther = allBalls[j];

					//HandleBallCollision(ball, ballOther, restitution);
					BallCollisionHandling.HandleBallBallCollision(thisBall, ballOther, restitution);
				}

				//thisBall.HandleSquareCollision(wallLength);
				billiardTable.HandleBallEnvironmentCollision(thisBall);
			}
		}
	}
}