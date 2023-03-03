using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public GameBehavior gameManager;

    public float moveSpeed = 10f;
    public float rotateSpeed = 10f;
    public float jumpVelocity = 5f;
    public float airJumpVelocity = 6f;
    public float distanceToGround = 0.1f;
    public float extraFallForce = -10f;
    public float decendingForce = 1.5f;
    public float bufferTime = 0.6f;
    public float coyoteTime = 0.6f;
    public LayerMask groundLayer;

    public Transform mainCamera;
    public GameObject bullet;
    public GameObject platform;
    public float bulletSpeed = 100f;

    private Vector3 bulletOffSet;
    private Transform position;
    public bool isGroundJumping = false;
    private bool isShooting = false;
    private bool firePlatform = false;
    private float vInput;
    private float hInput;
    private float lastJumpTime;
    private float LastGroundTime;
    public float fallSpeed;
    private Rigidbody rb;
    private CapsuleCollider col;

    public delegate void JumpingEvent();

    public event JumpingEvent playerJump;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        position = GameObject.Find("Player").transform;   
        gameManager = GameObject.Find("GameManager").GetComponent<GameBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        lastJumpTime -= Time.deltaTime;
        LastGroundTime -= Time.deltaTime;

        vInput = Input.GetAxis("Vertical") * moveSpeed;
        hInput = Input.GetAxis("Horizontal") * rotateSpeed;

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            isGroundJumping = true;
            lastJumpTime = bufferTime;

            playerJump();
        }

        if (Input.GetMouseButtonDown(0))
        {
            isShooting = true;
        }

        if (Input.GetMouseButtonDown(1) && !IsGrounded() )
        {
            firePlatform = true;
        }

    }

    void FixedUpdate() 
    {
        if (isGroundJumping)
        {
            lastJumpTime = 0f;
            isGroundJumping = false;
            rb.velocity = new Vector3(0, 0, 0);
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
        }


        if (isShooting)
        {
            bulletOffSet = position.TransformPoint(1f, 0f, 0f);
            Vector3 source = mainCamera.transform.eulerAngles;
            Vector3 target = bullet.transform.eulerAngles;
            target.y = source.y;
            bullet.transform.eulerAngles = target;
            GameObject newBullet = Instantiate(bullet, bulletOffSet, bullet.transform.rotation) as GameObject;
            Rigidbody bulletRB = newBullet.GetComponent<Rigidbody>();
            bulletRB.velocity = this.transform.forward* bulletSpeed;
            isShooting = false;
        }


        if (firePlatform)
        {
            GameObject newPlatform = Instantiate(platform, this.transform.position + new Vector3(0, -2.8f, 0), this.transform.rotation) as GameObject;
            gameManager.platformsRemaining -= 1;
            if (gameManager.platformsRemaining <= 0)
            {
                gameManager.platformTrigger = false;
            }
            firePlatform = false;
        }

        #region movement
        // rb.AddForce(Vector3.up * fallSpeed);
        Vector3 rotation = Vector3.up * hInput;
        Quaternion angleRot = Quaternion.Euler(rotation * Time.fixedDeltaTime);
        rb.MovePosition(this.transform.position + this.transform.forward * vInput * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * angleRot);
        #endregion 
    }

    public bool IsGrounded()
    {
        Vector3 capsuleBottom = new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z);
        bool grounded = Physics.CheckCapsule(col.bounds.center, capsuleBottom, distanceToGround, groundLayer, QueryTriggerInteraction.Ignore);
        return grounded;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Enemy")
        {
            gameManager.HP -= 1;
        }
    }
}
