namespace Billiard {
	public class ConstDefine {
		// 物理常量
		public const float MuK = 0.02f; // 动摩擦系数
		public const float Mass = 0.17f; // 台球质量 (kg)
		public const float Radius = 0.085f; // 台球半径 (m)
		public const float Inertia = 2.0f / 5.0f * Mass * Radius * Radius; // 台球的转动惯量
	}
}