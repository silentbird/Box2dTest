using System.Collections;
using System.Collections.Generic;
using Billiard.Balls;
using UnityEngine;

public static class BallCollisionHandling {
	public static double ElasticityCoefficient = 1.0; // 完全弹性碰撞, 0.0为完全非弹性碰撞
	public static double FrictionCoefficient = 0.1; // 摩擦系数

	public static bool AreBallsColliding(Vector3 p1, Vector3 p2, float r1, float r2) {
		bool areColliding = true;

		//The distance sqr between the balls
		float dSqr = (p2 - p1).sqrMagnitude;

		float minAllowedDistance = r1 + r2;

		//The balls are not colliding (or they are exactly at the same pos)
		//Square minAllowedDistance because we are using distance Square, which is faster 
		//They might be at the same pos if we check if the ball is colliding with itself, which might be faster than checking if the other ball is not the same as the ball 
		if (dSqr == 0f || dSqr > minAllowedDistance * minAllowedDistance) {
			areColliding = false;
		}

		return areColliding;
	}


	public static void HandleBallBallCollision(Ball ball1, Ball ball2, float restitution) {
		//Check if the balls are colliding
		bool areColliding = AreBallsColliding(ball1.pos, ball2.pos, ball1.radius, ball2.radius);

		if (!areColliding) {
			return;
		}


		//Update positions

		//Direction from ball1 to ball2
		Vector3 dir = ball2.pos - ball1.pos;

		//The distance between the balls
		float d = dir.magnitude;

		dir = dir.normalized;

		//The distace each ball should move so they no longer intersect 
		float corr = (ball1.radius + ball2.radius - d) * 0.5f;

		//Move the balls apart along the dir vector
		ball1.pos += dir * -corr; //-corr because dir goes from ball1 to ball2
		ball2.pos += dir * corr;
		//Update velocities
		/*
		//The linearVelocity of the balls can only change in the direction of the penetration
		float v1 = Vector3.Dot(ball1.linearVelocity, dir);
		float v2 = Vector3.Dot(ball2.linearVelocity, dir);
		float m1 = ball1.mass;
		float m2 = ball2.mass;
		float new_v1 = (m1 * v1 + m2 * v2 - m2 * (v1 - v2) * restitution) / (m1 + m2);
		float new_v2 = (m1 * v1 + m2 * v2 - m1 * (v2 - v1) * restitution) / (m1 + m2);
		ball1.linearVelocity += dir * (new_v1 - v1);
		ball2.linearVelocity += dir * (new_v2 - v2);
		 */

		Vector3 collisionNormal = (ball2.pos - ball1.pos).normalized; // 计算碰撞法向量并单位化

		// 法向速度分量
		float v1n = Vector3.Dot(ball1.linearVelocity, collisionNormal);
		float v2n = Vector3.Dot(ball2.linearVelocity, collisionNormal);

		// 碰撞后的法向速度分量
		float v1nAfter = (v1n * (float)(ball1.mass - ElasticityCoefficient * ball2.mass) + 2 * ball2.mass * v2n) / (ball1.mass + ball2.mass);
		float v2nAfter = (v2n * (float)(ball2.mass - ElasticityCoefficient * ball1.mass) + 2 * ball1.mass * v1n) / (ball1.mass + ball2.mass);

		// 更新法向速度
		ball1.linearVelocity += collisionNormal * (v1nAfter - v1n);
		ball2.linearVelocity += collisionNormal * (v2nAfter - v2n);

		// 切向速度分量 (考虑球的角速度对接触点的影响)
		Vector3 relativeVelocityAtContact = GetRelativeVelocityAtContact(ball1, ball2, collisionNormal);

		// 根据切向分量调整角速度（假设摩擦力作用）
		Vector3 tangent = relativeVelocityAtContact - collisionNormal * Vector3.Dot(relativeVelocityAtContact, collisionNormal);
		if (tangent.magnitude > 0) {
			tangent.Normalize(); // 单位化切向分量
		}

		// 计算摩擦力影响
		Vector3 frictionImpulse = tangent * (float)(-relativeVelocityAtContact.magnitude * FrictionCoefficient);

		// 更新角速度（球的转动惯量会影响角速度变化）
		ball1.angularVelocity += frictionImpulse * (float)(-1 / ball1.inertia);
		ball2.angularVelocity += frictionImpulse * (float)(1 / ball2.inertia);
	}


	// 计算两个球在接触点的相对速度
	public static Vector3 GetRelativeVelocityAtContact(Ball ball1, Ball ball2, Vector3 collisionNormal) {
		// 球的接触点速度 = 线速度 + 角速度 * 半径（计算球表面的速度）
		Vector3 contactVelocity1 = ball1.linearVelocity + Vector3.Cross(ball1.angularVelocity, collisionNormal * ball1.radius);
		Vector3 contactVelocity2 = ball2.linearVelocity + Vector3.Cross(ball2.angularVelocity, collisionNormal * ball2.radius);

		// 接触点的相对速度
		return contactVelocity2 - contactVelocity1;
	}

//The walls are a list if edges ordered counter-clockwise
//The first point on the border also has to be included at the end of the list
	public static bool HandleBallWallEdgesCollision(Ball ball, List<Vector3> border, float restitution) {
		//We need at least a triangle (the start and end are the same point, thus the 4)
		if (border.Count < 4) {
			return false;
		}


		//Find closest point on the border and related data to the line segment the point is on
		Vector3 closest = Vector3.zero;
		Vector3 ab = Vector3.zero;
		Vector3 wallNormal = Vector3.zero;

		float minDistSqr = 0f;

		//The border should include both the start and end points which are at the same location
		for (int i = 0; i < border.Count - 1; i++) {
			Vector3 a = border[i];
			Vector3 b = border[i + 1];
			Vector3 c = UsefulMethods.GetClosestPointOnLineSegment(ball.pos, a, b);

			//Using the square is faster
			float testDistSqr = (ball.pos - c).sqrMagnitude;

			//If the distance is smaller or its the first run of the algorithm
			if (i == 0 || testDistSqr < minDistSqr) {
				minDistSqr = testDistSqr;

				closest = c;

				ab = b - a;

				wallNormal = ab.Perp();
			}
		}


		//Update pos
		Vector3 d = ball.pos - closest;

		float dist = d.magnitude;

		//Special case if we end up exactly on the border 
		//If so we use the normal of the line segment to push out the ball
		if (dist == 0f) {
			d = wallNormal;
			dist = wallNormal.magnitude;
		}

		//The direction from the closest point on the wall to the ball
		Vector3 dir = d.normalized;

		//If they point in the same direction, meaning the ball is to the left of the wall
		if (Vector3.Dot(dir, wallNormal) >= 0f) {
			//The ball is not colliding with the wall
			if (dist > ball.radius) {
				return false;
			}

			//The ball is colliding with the wall, so push it in again
			ball.pos += dir * (ball.radius - dist);
		}
		//Push in the opposite direction because the ball is outside of the wall (to the right)
		else {
			//We have to push it dist so it ends up on the wall, and then radius so it ends up outside of the wall
			ball.pos += dir * -(ball.radius + dist);
		}


		//Update vel

		//Collisions can only change linearVelocity components along the penetration direction
		float v = Vector3.Dot(ball.linearVelocity, dir);

		float vNew = Mathf.Abs(v) * restitution;

		//Remove the old linearVelocity and add the new linearVelocity
		ball.linearVelocity += dir * (vNew - v);

		return true;
	}
}