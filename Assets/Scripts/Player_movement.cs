using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public CharacterController controller;
    public float walkSpeed = 12f;
    public float runSpeed = 24f; // Double the walk speed for running
    public float movementSpeed; // Current movement speed
    public float jumpHeight = 5f;
    public float gravity = -4f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public Transform playerCamera; // Assuming the camera is a child of the player object

    public float objectOffset = 0.5f;
    public Transform AttachPoint; // Reference to the player arm
    private GameObject carriedObject;
    public float pickupRange = 3f;

    public float destructionRadius = 5f;

    public GameObject MusicManager;
    private AkEvent MusicEvent;

    Vector3 velocity;
    bool isGrounded;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Check if the player is holding the "Shift" key to run
        movementSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        controller.Move(move * movementSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Object Pickup
        if (Input.GetButtonDown("Fire2")) //Right Mouse Button
        {
            if (carriedObject == null)
            {
                TryPickupObject();
            }
            else
            {
                DropObject();
            }
        }

        if (carriedObject != null && Input.GetButtonDown("Fire1")) //Left Mouse Button
        {
            TriggerEffect();
            AkSoundEngine.PostEvent("Play_SonicBoom", this.gameObject);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void TryPickupObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, pickupRange))
        {
            if (hit.collider.CompareTag("Pickupable"))
            {
                carriedObject = hit.collider.gameObject;
                carriedObject.GetComponent<Rigidbody>().isKinematic = true; // Disable physics

                // Calculate the offset in the left direction relative to the camera
                Vector3 offset = -playerCamera.up * objectOffset;

                // Snap the object to the attach point position with the offset
                carriedObject.transform.position = AttachPoint.position + offset;

                // Attach the object to the player's arm
                carriedObject.transform.parent = AttachPoint;

                AkSoundEngine.SetRTPCValue("BassVolume", 1, MusicManager);
            }
        }
    }

    void DropObject()
    {
        carriedObject.GetComponent<Rigidbody>().isKinematic = false; // Enable physics
        carriedObject.transform.parent = null; // Detach the object
        carriedObject = null;
    }

    void TriggerEffect()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits something
        if (Physics.Raycast(ray, out hit, destructionRadius))
        {
            // Check if the hit object has the "Destructible" tag
            if (hit.collider.CompareTag("Destructible"))
            {
                Destroy(hit.collider.gameObject);
                //remember this!!!!!!!!
                AkSoundEngine.PostEvent("Play_Glass_Shatter", this.gameObject); 
                //remember this!!!!!!!!
            }
        }
    }
}
