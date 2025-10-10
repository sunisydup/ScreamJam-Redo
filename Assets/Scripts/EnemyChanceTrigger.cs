using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyChanceTrigger : MonoBehaviour
{
    [SerializeField] GameObject EnemyObj;
    private float RNGChance;

    [SerializeField] Quaternion EnemyRot;
    [SerializeField] Vector3 EnemyPos;
    [SerializeField] int EnemyLikelyness;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RNGChance = Mathf.Clamp(RNGChance, 0, 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator RNG()
    {
        RNGChance = Random.Range(0, 100);
        Debug.Log(RNGChance);
        EnemyRot = transform.rotation = Quaternion.Euler(0, 0, -90);
        EnemyPos = new Vector3(-5, 1.2f, 0.7f);

        if (RNGChance > EnemyLikelyness)
        {
            EnemyObj.transform.position = EnemyPos;
            EnemyObj.transform.rotation = EnemyRot;
        }

        else
        {
            EnemyObj.transform.position = new Vector3(0, 0, -50);
        }


            yield return new WaitForSeconds(3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(RNG());
        }
    }
}


