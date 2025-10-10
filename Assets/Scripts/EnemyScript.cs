using System.Collections;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private bool SeePlayer;
    [SerializeField] LayerMask PlayMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SeePlayer = Physics.Raycast(transform.position, transform.up, 8f, PlayMask);
        Debug.Log(SeePlayer);
        JumpscareCancel();
        Debug.DrawRay(transform.position, transform.up, Color.green, 8);
    }

    IEnumerator JumpscareCountdown()
    {
        if (SeePlayer)
        {
            yield return new WaitForSeconds(2.5f);
            Debug.Log("AAH! WOW!");
        }
    }

    void JumpscareCancel()
    {
        if (!SeePlayer)
        {
            StopCoroutine(JumpscareCountdown());
        }
    }
}
