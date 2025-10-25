using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPMovement : MonoBehaviour
{

    public bool InvActive;
    [SerializeField]public GameObject InventoryCanvas;
    [SerializeField] Light Flashlight;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float interactDistance = 3f;
    private bool crouching;
    [SerializeField] CapsuleCollider playerCol;


    private CharacterController myCC;
    private GameObject mainCamera;
    private Camera playerCamera;
    private Vector3 velocity;
    private float yRotation = 0f;
    private bool crouchblock;

    void Start()
    {
        InvActive = false;

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera"); //finds main camera
        mainCamera.SetActive(true);    //disables main camera

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
        crouchblock = Physics.Raycast(transform.position, transform.up, 1.2f);

        if (Input.GetKey(KeyCode.C))
        {
            crouching = true;
        }

        else
        {
            crouching = false;
        }

        if (crouching)
        {
            playerCamera.transform.localPosition = new Vector3(0,0,0);
            moveSpeed = 2f;
            playerCol.height = 0.5f;
            myCC.height = .5f;
        }

        else if (!crouching & !crouchblock)
        {
            playerCamera.transform.localPosition = new Vector3(0, 0.8f, 0);
            moveSpeed = 5f;
            playerCol.height = 2f;
            myCC.height = 2f;
        }

        //Debug.Log(crouching);

        if (Input.GetKeyDown(KeyCode.F) & Flashlight.intensity > 0)
        {
            Flashlight.intensity = 0;
        }

        else if (Input.GetKeyDown(KeyCode.F) & Flashlight.intensity <= 0)
        {
            Flashlight.intensity = 0.65f;
        }
     
       if(Input.GetKeyDown(KeyCode.I) & InvActive == false)
       {
            InventoryCanvas.SetActive(true);
            InvActive = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
       }

        else if (Input.GetKeyDown(KeyCode.I) & InvActive == true)
        {
            InventoryCanvas.SetActive(false);
            InvActive = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }



      

        HandleMovement();  // calls movement code below
        HandleMouseLook(); // calls look code below
        HandleInteract();  // calls interact code below
    }

    void HandleMovement()
    {
        if (InvActive == false)
        {
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

    }
    // --- Ground check ---



    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (InvActive == false)
        {
            // Rotate player left/right
            transform.Rotate(Vector3.up * mouseX);

            // Rotate camera up/down
            yRotation -= mouseY;
            yRotation = Mathf.Clamp(yRotation, -75f, 75f); // limit looking up/down
            playerCamera.transform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
        }
           
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
