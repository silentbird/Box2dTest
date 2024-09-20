using UnityEngine;

public class BallMovement : MonoBehaviour {
	public float mass = 1f; // 球的质量(kg)
	public float radius = 0.5f; // 球的半径(m)
	public float frictionCoeff = 0.1f; // 摩擦系数
	public Vector3 initialVelocity; // 初始线速度(m/s)
	public Vector3 initialAngularVelocity; // 初始角速度(rad/s)

	private Vector3 velocity; // 当前线速度
	private Vector3 angularVelocity; // 当前角速度
	private float gravity = 9.81f; // 重力加速度(m/s^2)
	private float momentOfInertia; // 转动惯量
	public static BallMovement Instance { get; private set; }

	void Start() {
		Instance = this;
		velocity = initialVelocity;
		angularVelocity = initialAngularVelocity;
		// 计算球体的转动惯量 (I = 2/5 * m * r^2)
		momentOfInertia = 0.4f * mass * radius * radius;

		ApplyImpulse(new Vector3(10f, 0f, 0f), new Vector3(0f, 0f, 0f));
	}

	private void FixedUpdate() {
		float deltaTime = Time.deltaTime;

		// 计算摩擦力
		float normalForce = mass * gravity;
		float frictionMagnitude = frictionCoeff * normalForce;
		Vector3 frictionForce = -velocity.normalized * frictionMagnitude;

		// 计算线加速度
		Vector3 linearAcceleration = frictionForce / mass;

		// 计算角加速度
		Vector3 torque = Vector3.Cross(Vector3.up * radius, frictionForce);
		Vector3 angularAcceleration = torque / momentOfInertia;

		// 更新线速度和角速度
		velocity += linearAcceleration * deltaTime;
		angularVelocity += angularAcceleration * deltaTime;

		// 如果线速度很小，认为球已停止
		if (velocity.magnitude < 0.01f) {
			velocity = Vector3.zero;
			angularVelocity = Vector3.zero;
		}

		// 更新位置
		transform.position += velocity * deltaTime;

		// 更新旋转
		Quaternion rotation = Quaternion.AngleAxis(angularVelocity.magnitude * Mathf.Rad2Deg * deltaTime, angularVelocity.normalized);
		transform.rotation = rotation * transform.rotation;

		// 确保角速度和线速度匹配（纯滚动条件）
		Vector3 velocityFromRotation = Vector3.Cross(angularVelocity, Vector3.up * radius);
		velocity = Vector3.Lerp(velocity, velocityFromRotation, 0.1f);
	}

	// 施加冲量（瞬时冲击）
	public void ApplyImpulse(Vector3 impulse, Vector3 applicationPoint) {
		velocity += impulse / mass;

		// 计算角冲量并更新角速度
		Vector3 angularImpulse = Vector3.Cross(applicationPoint - transform.position, impulse);
		angularVelocity += angularImpulse / momentOfInertia;
	}
}