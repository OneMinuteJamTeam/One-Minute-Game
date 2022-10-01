using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public enum PlayerNumber
    {
        PlayerOne,
        PlayerTwo,
    }

    [Header("Settings")]
    [SerializeField]
    protected float moveSpeed = 5.0f;
    [SerializeField]
    protected float rotationSpeed = 100.0f;
    [SerializeField]
    protected PlayerNumber playerNumber;

    private Rigidbody _rb;
    private Vector3 lastSpeedDirection;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        HandleInput();
        HandleRotation();
    }

    private void HandleInput()
    {
        Debug.Log("handle input called");
        if(playerNumber == PlayerNumber.PlayerOne && InputManager.Instance.IsMovingPlayer1)
        {
            // Handle Input P1
            _rb.velocity = new Vector3(InputManager.Instance.MoveDirectionPlayer1.x * moveSpeed, 0.0f, InputManager.Instance.MoveDirectionPlayer1.y * moveSpeed);

            lastSpeedDirection = new Vector3(InputManager.Instance.MoveDirectionPlayer1.x,0,InputManager.Instance.MoveDirectionPlayer1.y);
        }
        else if (playerNumber == PlayerNumber.PlayerTwo && InputManager.Instance.IsMovingPlayer2) {
            // Handle Input P2
            _rb.velocity = new Vector3(InputManager.Instance.MoveDirectionPlayer2.x * moveSpeed, 0.0f, InputManager.Instance.MoveDirectionPlayer2.y * moveSpeed);

            lastSpeedDirection = new Vector3(InputManager.Instance.MoveDirectionPlayer2.x, 0, InputManager.Instance.MoveDirectionPlayer2.y);
        }
    }

    private void HandleRotation() {
        Quaternion toRot = Quaternion.LookRotation(lastSpeedDirection, transform.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, rotationSpeed * Time.deltaTime);
    }
}
