using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 move;
    public float forwardSpeed;
    public float maxSpeed;

    private int desiredLane = 1; // 0 left, 1 middle, 2 right
    [SerializeField]
    private float laneDistance = 3f; // the distance between two lanes

    private float gravity = -12f;
    public float jumpForce = 2;
    private Vector3 velocity;

    public bool isGrounded;
    public LayerMask groundLayer;
    public Transform groundCheck;

    public Animator animator;

    private bool isSliding = false;

    bool toggle = false;

    public float slideDuration = 1.5f;

    //jarak raycast
    public float raycastDistance = 0.2f; // Jarak Raycast ke bawah

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (!PlayerManager.isGameStarted)
            return;

        if (toggle)
        {
            toggle = false;
            if (forwardSpeed < maxSpeed)
                forwardSpeed += 0.1f * Time.fixedDeltaTime;
        } else
        {
            toggle = true;
            if (Time.timeScale < 2f)
                Time.timeScale += 0.005f * Time.fixedDeltaTime;
        }

        //apply gravity
        if (isGrounded && velocity.y < 0)
            velocity.y = -1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerManager.isGameStarted)
            return;

        //update animator
        animator.SetBool("isGameStarted", true);

        // Update movement
        move.z = forwardSpeed;

        // Check is the player is grounded
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, raycastDistance, groundLayer);
        animator.SetBool("isGrounded", isGrounded);

        if (controller.isGrounded)
        {
            move.y = -1;

            if (Input.GetKeyDown(KeyCode.UpArrow) || SweepManager.swipeUp)
            {
                Jump();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || SweepManager.swipeDown && !isSliding)
                StartCoroutine(Slide());
        }
        else
        {
            move.y += gravity * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.DownArrow) || SweepManager.swipeDown && !isSliding)
            {
                StartCoroutine(Slide());
                velocity.y = -10;
            }
        }

        controller.Move(move * Time.deltaTime);

        // Gather the input which lane we should be 
        if (Input.GetKeyDown(KeyCode.RightArrow) || SweepManager.swipeRight)
        {
            desiredLane++;
            if (desiredLane == 3)
                desiredLane = 2;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || SweepManager.swipeLeft)
        {
            desiredLane--;
            if (desiredLane == -1)
                desiredLane = 0;
        }

        // calculate where should be in future
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (desiredLane == 0)
            targetPosition += Vector3.left * laneDistance;
        else if (desiredLane == 2)
            targetPosition += Vector3.right * laneDistance;

        transform.position = targetPosition;
    }

    //private void FixedUpdate()
    //{
    //    if (!PlayerManager.isGameStarted)
    //        return;

    //    controller.Move(move * Time.fixedDeltaTime);

    //}

    private void Jump()
    {
        move.y = jumpForce;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("Obstacle"))
        {
            PlayerManager.gameOver = true;
            // FindObjectOfType<AudioManager>().PlaySound("GameOver");
        }
    }

    private IEnumerator Slide()
    {
        isSliding = true;
        animator.SetBool("isSliding", true);
        yield return new WaitForSeconds(0.25f / Time.timeScale);
        controller.center = new Vector3(0, -0.5f, 0);
        controller.height = 1;

        yield return new WaitForSeconds((slideDuration - 0.25f) / Time.timeScale);

        animator.SetBool("isSliding", false);
        controller.center = Vector3.zero;
        controller.height = 2;

        isSliding = false;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheck.position, 0.1f);
        }
    }




}