using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviour {
	public float gravity = -9.8f;

	public float moveSpeed = 10;
	public float stepHeight = 0.4f;
	public float groundSlope = 45;
	public float jumpPower = 15;

	const float epsilon = 0.01f; // float.Epsilon;

	[Header("Debug")]
	public bool IsGrounded = false;
	public bool hasGrapple = false;
	public Vector2 grapplePoint;
	public float grappleDistance;

	public Vector2 velocity;
	public Vector2 position {
		get {
			var p = transform.position;
			return p;
		}

		set {
			var p = transform.position;
			p.x = value.x;
			p.y = value.y;
			transform.position = p;
		}
	}

	CapsuleCollider2D _capsule;
	List<RaycastHit2D> _castHit = new List<RaycastHit2D>();

	ContactFilter2D _contactFilter;
	float _groundSlopeY;

	void Start() {
		Application.targetFrameRate = 60;
		_capsule = GetComponent<CapsuleCollider2D>();

		_contactFilter = new ContactFilter2D();
		_contactFilter.SetLayerMask(~(1 << gameObject.layer));
		_contactFilter.useLayerMask = true;
	}

	void Update() {

		//Physics2D.CapsuleCast(position, _capsule.size, _capsule.direction, 0, new Vector2(1,1), _contactFilter, _castHit);
		//foreach (var h in _castHit) {
		//	Debug.DrawRay(h.point, h.normal, Color.green);
		//	Debug.DrawRay(h.point, h.centroid, Color.blue);
		//	Debug.DrawLine(position, h.point, Color.red);
		//}

		_groundSlopeY = Mathf.Cos(groundSlope);

		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			if (!hasGrapple) {
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				var plane = new Plane(Vector3.forward, 0);

				float d;
				if (plane.Raycast(ray, out d)) {
					hasGrapple = true;
					grapplePoint = ray.GetPoint(d);
					grappleDistance = (grapplePoint - position).magnitude;
				}
			}
		}
		
		if (!Input.GetKey(KeyCode.Mouse0)) {
			hasGrapple = false;
		}

		if (!hasGrapple) {
			
			if (IsGrounded) {
				hasGrapple = false;
				velocity.x = 0; // TODO slide on Ice

				// Jump
				if (Input.GetKeyDown(KeyCode.W)) {
					IsGrounded = false;
					velocity.y += jumpPower;
				}
			}

			// Move Left/Right
			if (Input.GetKey(KeyCode.A)) {
				if (velocity.x > - moveSpeed) {
					velocity.x = Mathf.Max(velocity.x - moveSpeed, -moveSpeed);
				}
			} else if (Input.GetKey(KeyCode.D)) {
				if (velocity.x < moveSpeed) {
					velocity.x = Mathf.Min(velocity.x + moveSpeed, moveSpeed);
				}
			}
		}
	}

	bool _Move(Vector2 moveDir, int maxIteration) {
		var distance = moveDir.magnitude;
		if (distance < epsilon) return false;

		var normalizeDir = moveDir.normalized;

		bool hit = false;
		var newOffset = moveDir;
		var newVelocity = velocity;

		var origin = position + _capsule.offset;
		Physics2D.CapsuleCast(origin, _capsule.size, _capsule.direction, 0, normalizeDir, _contactFilter, _castHit, distance);

		var minDistance = distance;


		foreach (var h in _castHit) {
			if (h.distance >= minDistance) continue;
			
			newOffset = normalizeDir * Mathf.Max(0, h.distance - 0.1f);

			minDistance = h.distance;
			hit = true;
			
			// slide
			newVelocity = velocity - Vector2.Dot(velocity, h.normal) * h.normal;

			//Debug.DrawRay(position, Vector2.Dot(velocity, h.normal) * h.normal, Color.blue);
			//Debug.DrawRay(position, velocity, Color.green);
			//Debug.DrawRay(position, newVelocity, Color.yellow);

			if (h.normal.y > _groundSlopeY) {
				IsGrounded = true;
				velocity.y = 0;
				newOffset.y += epsilon;
			}
		}
	
		if (newOffset.magnitude >= epsilon) {
			position += newOffset;
		}

		velocity = newVelocity;

		if (hit && maxIteration > 1) {
			var remainDistance = distance - minDistance;
			if (remainDistance > epsilon) {
				_Move(velocity.normalized * remainDistance, maxIteration - 1);
			}
		}

		return hit;
	}

	private void FixedUpdate() {
		var t = Time.fixedDeltaTime;
		velocity.y += gravity * t;

//		Debug.DrawRay(transform.position, velocity);

		if (hasGrapple) {
			var nl = (grapplePoint - position).normalized;
			var dot = Vector2.Dot(velocity, nl);
			if (dot < 0.8f) {
				velocity -= dot * nl;
			}
		}

		if (IsGrounded && velocity.x != 0) {
			_Move(new Vector2(0,  stepHeight), 1);
			_Move(velocity * t, 3);
			_Move(new Vector2(0, -stepHeight * 3), 1);
		} else {
			_Move(velocity * t, 3);
		}

		if (IsGrounded) {
			hasGrapple = false;
		}
	}

	private void OnDrawGizmos() {
		if (hasGrapple) {
			Gizmos.color = Color.blue;
			Gizmos.DrawCube(grapplePoint, Vector3.one * 0.2f);
			Gizmos.DrawLine(grapplePoint, position + _capsule.offset);
		}

		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = new Color(1, 1, 1);
		var cap = GetComponent<CapsuleCollider2D>();

		Gizmos.DrawCube(new Vector3(cap.offset.x, cap.offset.y, 0), new Vector3(cap.size.x, cap.size.y, 0));
	}
}
