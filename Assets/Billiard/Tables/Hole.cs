using UnityEngine;

namespace Billiard.Tables {
	public class Hole {
		public Vector3 pos;
		public float radius;

		public Hole(Vector3 pos, float radius) {
			this.pos = pos;
			this.radius = radius;
		}
	}
}