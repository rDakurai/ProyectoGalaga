
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	[Header("Movimiento")]
	[SerializeField, Min(0f)]
	private float moveSpeed = 7f;

	[Header("Disparo")]
	[SerializeField]
	private GameObject bulletPrefab;

	[SerializeField, Min(0f)]
	private float cannonOffsetX = 0.1f;

	[SerializeField, Min(0f)]
	private float cannonOffsetY = 0.15f;

	[SerializeField, Min(0f)]
	private float bulletSpeed = 25f;

	[SerializeField, Min(0f)]
	private float fireRate = 0.15f;

	public Vector2 MoveInput => _moveInput;

	private Rigidbody2D _rb;
	private Vector2 _moveInput;
	private float _nextFireTime;

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

		// Disparo con tecla P
		if (keyboard.pKey.isPressed && Time.time >= _nextFireTime)
		{
			Shoot();
			_nextFireTime = Time.time + fireRate;
		}
	}

	private void FixedUpdate()
	{
		_rb.linearVelocity = _moveInput * moveSpeed;
	}

	private void Shoot()
	{
		if (bulletPrefab == null)
		{
			return;
		}

		// Reproducir sonido de disparo
		PlayerAudioManager.PlayShoot();

		// Cañón superior derecho
		Vector3 upperRightPos = transform.position + new Vector3(cannonOffsetX, cannonOffsetY, 0f);
		GameObject upperRightBullet = Instantiate(bulletPrefab, upperRightPos, Quaternion.identity);
		BulletComportamiento upperBehavior = upperRightBullet.GetComponent<BulletComportamiento>();
		if (upperBehavior != null)
		{
			upperBehavior.SetSpeed(bulletSpeed);
			upperBehavior.SetDirection(Vector2.right);
		}

		// Cañón inferior derecho
		Vector3 lowerRightPos = transform.position + new Vector3(cannonOffsetX, -cannonOffsetY, 0f);
		GameObject lowerRightBullet = Instantiate(bulletPrefab, lowerRightPos, Quaternion.identity);
		BulletComportamiento lowerBehavior = lowerRightBullet.GetComponent<BulletComportamiento>();
		if (lowerBehavior != null)
		{
			lowerBehavior.SetSpeed(bulletSpeed);
			lowerBehavior.SetDirection(Vector2.right);
		}
	}
}
