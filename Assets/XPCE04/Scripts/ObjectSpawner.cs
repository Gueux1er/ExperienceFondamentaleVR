using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public static bool isObjectReady = true;
    public Transform prefab;
    public float delay = 5;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(prefab, this.transform.position, Quaternion.identity);
        StartCoroutine(ReadyingObject());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ReadyingObject()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(delay);
            GameObject go = Instantiate(prefab, this.transform.position, Quaternion.identity).gameObject;
        }
    }
}
