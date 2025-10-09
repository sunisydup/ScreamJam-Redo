using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPMovement : MonoBehaviour
{

    private bool InvActive;
    [SerializeField] GameObject InventoryCanvas;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float interactDistance = 3f;

    private CharacterController myCC;
    private GameObject mainCamera;
    private Camera playerCamera;
    private Vector3 velocity;
    private float yRotation = 0f;

    void Start()
    {
        InvActive = false;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera"); //finds main camera
        mainCamera.SetActive(false);    //disables main camera

        myCC = GetComponent<CharacterController>(); //sets up short had refernce (myCC) for character controller

        // Add a player camera if not found
        playerCamera = GetComponentInChildren<Camera>();  // checks to see if a playerCamera exists
        if (playerCamera == null)  // if not make one
        {
            GameObject camObj = new GameObject("PlayerCamera");
            camObj.transform.SetParent(transform);
            camObj.transform.localPosition = new Vector3(0, 0.8f, 0); // adjust height
            playerCamera = camObj.AddComponent<Camera>();
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
     
        Debug.Log(InvActive);
        if (Input.GetKeyDown(KeyCode.Tab) & (InvActive = false))
        {
            InventoryCanvas.SetActive(true);
            InvActive = true;

        }

        else if (Input.GetKeyDown(KeyCode.Tab) & (InvActive = true))
        {
            InventoryCanvas.SetActive(false);
            InvActive = false;
            
        }



        HandleMovement();  // calls movement code below
        HandleMouseLook(); // calls look code below
        HandleInteract();  // calls interact code below
    }

    void HandleMovement()
    {
        // --- Ground check ---
        if (myCC.isGrounded)
        {
            // Reset downward velocity when grounded
            if (velocity.y < 0)
                velocity.y = -2f;

            // Jump
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // --- Movement ---
        float x = Input.GetAxis("Horizontal"); // A/D
        float z = Input.GetAxis("Vertical");   // W/S

        Vector3 move = transform.right * x + transform.forward * z;
        myCC.Move(move * moveSpeed * Time.deltaTime);

        // --- Apply gravity ---
        velocity.y += gravity * Time.deltaTime;
        myCC.Move(velocity * Time.deltaTime);
    }



    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate player left/right
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera up/down
        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -75f, 75f); // limit looking up/down
        playerCamera.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
    }

    void HandleInteract()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                // Try to send "OnInteract" to the hit object
                hit.collider.gameObject.SendMessage("OnInteract", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

}
