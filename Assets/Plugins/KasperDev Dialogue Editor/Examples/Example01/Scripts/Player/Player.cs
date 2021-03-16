using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotaSpeed = 5f;

    private Animator animator;
    private Rigidbody rb;
    private float vertical;
    private float horizontal;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        InputHander();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void InputHander()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
    }

    private void Movement()
    {
        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed;
        rb.velocity = movement;

        Vector3 direction = Vector3.RotateTowards(transform.forward, movement, rotaSpeed * Time.fixedDeltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);

        float animMove = Vector3.Magnitude(movement.normalized);
        animator.SetFloat("moveSpeed", animMove);
    }
}
