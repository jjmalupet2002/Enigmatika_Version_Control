using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator playerAnim;
    public Rigidbody playerRigid;
    public float runSpeed = 5f;
    public float backSpeed = 5f;  // Backward speed should be slower
    public float rotationSpeed = 100f;
    public Transform playerTrans;

    void FixedUpdate()
    {
        // Handle running forward
        if (Input.GetKey(KeyCode.W))
        {
            playerRigid.velocity = playerTrans.forward * runSpeed;
            playerAnim.SetTrigger("Run");
            playerAnim.ResetTrigger("Idle");
            playerAnim.ResetTrigger("Backwards");
            UnityEngine.Debug.Log("Moving Forward - Running");
        }
        // Handle moving backward
        else if (Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                // Perform 180-degree turn if turning keys are pressed
                playerAnim.SetTrigger("180");
                playerTrans.Rotate(0, 180f, 0);
                playerRigid.velocity = -playerTrans.forward * backSpeed;
                playerAnim.SetTrigger("Backwards");
                playerAnim.ResetTrigger("Idle");
                UnityEngine.Debug.Log("Performing 180-Degree Turn and Moving Backward");
            }
            else
            {
                // Move directly backward
                playerRigid.velocity = -playerTrans.forward * backSpeed;
                playerAnim.SetTrigger("Backwards");
                playerAnim.ResetTrigger("Idle");
                UnityEngine.Debug.Log("Moving Backward");
            }
        }
        else
        {
            playerRigid.velocity = Vector3.zero;
            playerAnim.SetTrigger("Idle");
            playerAnim.ResetTrigger("Run");
            playerAnim.ResetTrigger("Backwards");
            UnityEngine.Debug.Log("Idle - Not Moving");
        }
    }

    void Update()
    {
        // Handle turning left
        if (Input.GetKey(KeyCode.A))
        {
            playerTrans.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("TurnLeft"))
            {
                playerAnim.SetTrigger("Left");
                UnityEngine.Debug.Log("Turning Left");
            }
        }

        // Handle turning right
        if (Input.GetKey(KeyCode.D))
        {
            playerTrans.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            if (!playerAnim.GetCurrentAnimatorStateInfo(0).IsName("Turn180"))
            {
                playerAnim.ResetTrigger("Left");
                UnityEngine.Debug.Log("Turning Right");
            }
        }

        // Handle 180-degree turn
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerAnim.SetTrigger("180");
            playerTrans.Rotate(0, 180f, 0);
            UnityEngine.Debug.Log("Performing 180-Degree Turn");
        }
    }
}
