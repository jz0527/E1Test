using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerInput : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashCooldown = 0.1f;

    private bool canDoubleJump = false;
    private bool isOnGround = false;
    private float recentDashTime;

    private Vector2 movement = Vector2.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move(movement.x, movement.y);
    }

    void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
        Debug.Log("Movement input: " + movement);
    }

    void OnJump()
    {
        if (isOnGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canDoubleJump = true;
        }
        else if (canDoubleJump)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            canDoubleJump = false;
        }
    }

    void OnDash()
    {
        if (Time.time - recentDashTime >= dashCooldown)
        {
            Vector3 dashDirection = new Vector3(movement.x, 0f, movement.y).normalized;
            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
            recentDashTime = Time.time;
        }
    }

    private void Move(float x, float z)
    {
        Vector3 velocity = new Vector3(x * speed, rb.linearVelocity.y, z * speed);
        rb.linearVelocity = velocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
        {
            isOnGround = true;
            canDoubleJump = false;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isOnGround = false;
    }
}
