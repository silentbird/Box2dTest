using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Billiard;
using UnityEngine.UI;

namespace Billiard {
	//Simulate billiard balls with different size
	//Based on: https://matthias-research.github.io/pages/tenMinutePhysics/
	public class BilliardController : MonoBehaviour {
		public GameObject ballPrefabGO;

		public BilliardTable billiardTable;

		public Button resetButton;

		//Simulation properties
		private readonly int subSteps = 5;

		private readonly int numberOfBalls = 20;

		//How much velocity is lost after collision between balls [0, 1]
		//Is usually called e
		//Elastic: e = 1 means same velocity after collision (if the objects have the same size and same speed)
		//Inelastic: e = 0 means no velocity after collions (if the objects have the same size and same speed) and energy is lost
		private readonly float restitution = 0.8f;

		private List<BilliardBall> allBalls;

		private void Awake() {
			resetButton.onClick.AddListener(ResetSimulation);
		}

		private void Start() {
			ResetSimulation();

			billiardTable.Init();
		}


		private void ResetSimulation() {
			allBalls = new List<BilliardBall>();

			// Vector2 mapSize = new((billiardTable as Rectangle).xWidth, (billiardTable as Rectangle).zWidth);
			// SetupBalls.AddRandomBallsWithinRectangle(ballPrefabGO, numberOfBalls, allBalls, ballRadius, ballRadius, mapSize, Vector3.zero);
			float ballRadius = 8.5f / 2; // Standard billiard ball radius in meters
			float rowSpacing = ballRadius * 2;
			for (int row = 0; row < 5; row++) {
				for (int col = 0; col <= row; col++) {
					float z = col * rowSpacing - row * ballRadius;
					float x = Mathf.Cos(Mathf.Deg2Rad * 30) * row * rowSpacing;
					var pos = new Vector3(x, 0, z);
					SetupBalls.AddBall(ballPrefabGO, allBalls, pos, ballRadius * 2);
				}
			}


			BilliardMaterials.GiveBallsRandomColor(ballPrefabGO, allBalls);
			var mainBall = SetupBalls.AddBall(ballPrefabGO, allBalls, new Vector3(-70, 0, 0), ballRadius * 2);
			mainBall.vel = new Vector3(100, 0, 0);

			//Give each ball a velocity
			// foreach (BilliardBall b in allBalls)
			// {
			//     float maxVel = 40f;
			//
			//     float randomVelX = Random.Range(-maxVel, maxVel);
			//     float randomVelZ = Random.Range(-maxVel, maxVel);
			//
			//     Vector3 randomVel = new Vector3(randomVelX, 0f, randomVelZ);
			//
			//     b.vel = randomVel;
			// }
		}


		private void Update() {
			//Update the transform with the position we simulate in FixedUpdate
			foreach (BilliardBall ball in allBalls) {
				ball.UpdateVisualPosition();
			}
		}


		private void FixedUpdate() {
			float sdt = Time.fixedDeltaTime / (float)subSteps;

			for (int i = 0; i < allBalls.Count; i++) {
				BilliardBall thisBall = allBalls[i];

				thisBall.SimulateBall(subSteps, sdt);

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