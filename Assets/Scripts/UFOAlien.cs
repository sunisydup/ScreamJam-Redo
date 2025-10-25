using UnityEngine;

public class UFOAlien : MonoBehaviour
{
    public GameObject playerref;
    bool inrange;
    float distance;
    bool canattack;
    float atkcooldown;

    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        distance = Vector3.Distance(transform.position, playerref.transform.position);

        Debug.Log(distance);





        if (!inrange)
        {
            transform.Translate()
        }
    }
}
