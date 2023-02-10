using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 10f;
    public float jumpVelocity = 5f;
    public float distanceToGround = 0.1f;
    public LayerMask groundLayer;

    public Transform mainCamera;
    public GameObject bullet;
    public GameObject platform;
    public float bulletSpeed = 100f;

    private Vector3 bulletOffSet;
    private Transform position;
    private bool isJumping = false;
    private bool isShooting = false;
    private bool firePlatform = false;
    private float vInput;
    private float hInput;
    private Rigidbody rb;
    private CapsuleCollider col;
    private bool groundCheck;
    private bool jumpcheck;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        position = GameObject.Find("Player").transform;   

    }

    // Update is called once per frame
    void Update()
    {
        vInput = Input.GetAxis("Vertical") * moveSpeed;
        hInput = Input.GetAxis("Horizontal") * rotateSpeed;

        if(Input.GetButtonDown("Jump") && IsGrounded())
        {
            isJumping = true;
        }


        if (Input.GetMouseButtonDown(0))
        {
            isShooting = true;
        }


        if (Input.GetMouseButtonDown(1) && !IsGrounded())
        {
            firePlatform = true;
        }

    }

    void FixedUpdate() 
    {
        if(isJumping)
        {
            rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
            isJumping = false;
        }


        if(isShooting)
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


        if(firePlatform)
        {
            GameObject newPlatform = Instantiate(platform, this.transform.position + new Vector3(0, -1.5f, 0), this.transform.rotation) as GameObject;
            firePlatform = false;
        }

        Vector3 rotation = Vector3.up * hInput;
        Quaternion angleRot = Quaternion.Euler(rotation * Time.fixedDeltaTime);
        rb.MovePosition(this.transform.position + this.transform.forward * vInput * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * angleRot);

    }

    private bool IsGrounded(){
        Vector3 capsuleBottom = new Vector3(col.bounds.center.x, col.bounds.min.y, col.bounds.center.z);
        bool grounded = Physics.CheckCapsule(col.bounds.center, capsuleBottom, distanceToGround, groundLayer, QueryTriggerInteraction.Ignore);
        return grounded;
    }
}
