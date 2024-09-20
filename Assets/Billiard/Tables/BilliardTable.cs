using System.Collections.Generic;
using Billiard.Balls;
using UnityEngine;

namespace Billiard.Tables {
	public abstract class BilliardTable : MonoBehaviour {
		public TableType tableType = TableType.RectangleTable;
		public float xWidth = 1f;
		public float zWidth = 1f;

		[Range(0, 1)]
		public float friction;

		public readonly List<Hole> holes = new();
		public abstract void Init();

		public abstract bool HandleBallEnvironmentCollision(Ball ball, float restitution = 1f);

		public abstract bool IsBallOutsideOfTable(Vector3 ballPos, float ballRadius);

		public virtual bool IsBallInHole(Ball ball) {
			for (int i = 0; i < holes.Count; i++) {
				var hole = holes[i];
			}

			return false;
		}

		public abstract void MyUpdate();
	}

	public enum TableType {
		CircleTable,
		RectangleTable,
		PolygonalTable,
	}
}