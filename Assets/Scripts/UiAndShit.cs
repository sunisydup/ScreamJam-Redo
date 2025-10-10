using UnityEngine;

public class UiAndShit : MonoBehaviour
{
    public int bandagecount;
    FPMovement Fmove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Fmove = GetComponent<FPMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Bandage"))
        {
            bandagecount++;
            Destroy(collision.gameObject);
        }
    }
    public void Unpause()
    {
        Fmove.InvActive = false;
        Fmove.InventoryCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
