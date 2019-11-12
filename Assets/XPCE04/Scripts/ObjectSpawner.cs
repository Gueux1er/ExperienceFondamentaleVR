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
    }

    // Update is called once per frame
    void Update()
    {
        if (isObjectReady == true) {
            Instantiate(prefab, this.transform.position, Quaternion.identity);
            isObjectReady = false;
            StartCoroutine(ReadyingObject(delay));

        }
    }

    IEnumerator ReadyingObject(float time)
    {
        yield return new WaitForSeconds(time * Time.deltaTime);
 
        // Code to execute after the delay
        isObjectReady = true;
    }
}
