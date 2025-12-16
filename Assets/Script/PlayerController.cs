
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	[Header("Movimiento")]
	[SerializeField, Min(0f)]
	private float moveSpeed = 7f;

	public Vector2 MoveInput => _moveInput;

	private Rigidbody2D _rb;
	private Vector2 _moveInput;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		var keyboard = Keyboard.current;
		if (keyboard == null)
		{
			_moveInput = Vector2.zero;
			return;
		}

		Vector2 dir = Vector2.zero;
		if (keyboard.wKey.isPressed)
		{
			dir.y += 1f;
		}
		if (keyboard.sKey.isPressed)
		{
			dir.y -= 1f;
		}
		if (keyboard.aKey.isPressed)
		{
			dir.x -= 1f;
		}
		if (keyboard.dKey.isPressed)
		{
			dir.x += 1f;
		}

		_moveInput = dir.sqrMagnitude > 1f ? dir.normalized : dir;
	}

	private void FixedUpdate()
	{
		_rb.linearVelocity = _moveInput * moveSpeed;
	}
}
