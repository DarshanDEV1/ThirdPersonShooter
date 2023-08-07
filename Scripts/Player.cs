using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerAnimation
{
    Idle,
    Walk,
    Run,
    Jump,
    Fall,
    Ladder
}

public class Player : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float speed = 6f;
    float turnVelocity;
    float turnTime = 0.1f;
    [SerializeField] Transform cam;
    [SerializeField] Animator animator;
    private bool isGrounded;
    [SerializeField] float jumpForce = 9.0f;
    private float gravity = -9.81f;
    private Vector3 velocity;
    private bool isJumping = false;
    private bool isSprinting = false;
    private bool isClimbing = false;
    [SerializeField] PlayerAnimation[] playerAnimation;
    [SerializeField] GameManager _gameManager;
    private AudioSource playAudio;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;

        playAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        if (isClimbing)
        {
            ClimbLadder();
            return;
        }

        isGrounded = controller.isGrounded;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Only update rotation when there is movement input
        if (direction.magnitude >= 0.1f)
        {
            var playerRotation = GetPlayerRotation(direction);
            float targetAngle = playerRotation.targetAngle;
            float angle = playerRotation.refinedAngle;

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            float currentSpeed = isSprinting ? (speed * 2) + speed : speed;

            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            ChangeAnimation(new int[] { 1 });
        }
        else
        {
            ChangeAnimation(new int[] { 0 });
        }

        Sprinting();

        Grounded();

        Gravity();
    }

    private void Sprinting()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isSprinting = true;
            ChangeAnimation(new int[] { 2 });
        }
        else
        {
            isSprinting = false;
        }
    }

    private void Grounded()
    {
        if (isGrounded)
        {
            velocity.y = -2f;
            if (Input.GetButtonDown("Jump"))
            {
                isJumping = true;

                playAudio.clip = _gameManager._audioClips[1];
                playAudio.Play();

                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                ChangeAnimation(new int[] { 3 });
            }
            isJumping = false;
        }
        else
        {
            if (!isJumping)
            {
                ChangeAnimation(new int[] { 4 });
            }
        }
    }

    private void Gravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void ClimbLadder()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 climbDirection = new Vector3(0f, vertical, 0f);

        float climbSpeed = 6.0f;

        controller.Move(climbDirection * climbSpeed * Time.deltaTime);
    }

    private ReturnAngle GetPlayerRotation(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnTime);

        return new ReturnAngle { targetAngle = targetAngle, refinedAngle = angle };
    }

    private void ChangeAnimation(int[] arr)
    {
        for (int i = 0; i < playerAnimation.Length; i++)
        {
            foreach (int j in arr)
            {
                if (i == j)
                {
                    animator.SetBool(playerAnimation[i].ToString(), true);
                }
                else
                {
                    animator.SetBool(playerAnimation[i].ToString(), false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = true;
            // Disable gravity and reset velocity when entering ladder
            gravity = 0f;
            velocity.y = 0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            if (isClimbing)
                ChangeAnimation(new int[] { 5 });
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false;
            // Restore gravity when leaving ladder
            gravity = -9.81f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("DeathZone"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        else if (collision.collider.CompareTag("Collectables"))
        {
            _gameManager._currentScore++;
            _gameManager._score_Text.text = "Score : " + _gameManager._currentScore.ToString();
            playAudio.clip = _gameManager._audioClips[2];
            playAudio.Play();
            Destroy(collision.collider.gameObject);
        }
    }

}

[System.Serializable]
public struct ReturnAngle
{
    public float targetAngle;
    public float refinedAngle;
}
