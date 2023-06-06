using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	// General Components
	private Rigidbody2D Rigid;
	private SpriteRenderer SpriteR;
	
	// Movement
	[SerializeField] public Vector2 Movement;
	private float MoveSpeed = 4.5f;
	
	// Control flow	
	public bool canMove = true;
	
	
	// Start is called before the first frame update
	void Start()
	{
		Rigid = GetComponent<Rigidbody2D>();
		SpriteR = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		SpriteFlipCheck();
	}
	
	private void FixedUpdate() {
		// Movement		
		if (canMove)
		{
			Vector2 targetSpeed = new Vector2(Movement.x * MoveSpeed, Movement.y * MoveSpeed);
			Vector2 speedDif = targetSpeed - Rigid.velocity;
			Vector2 actualSpeed = speedDif * new Vector2(12, 12); // Change the vector values to change acceleration
			Rigid.AddForce(actualSpeed);
		}
		else
		{
			Rigid.velocity = Vector2.zero;
		}
	}

	private void SpriteFlipCheck()
	{
		if (Movement.x < 0)
		{
			SpriteR.flipX = true;
		}
		else if (Movement.x > 0)
		{
			SpriteR.flipX = false;

		}
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		Movement = context.ReadValue<Vector2>();
	}
}
