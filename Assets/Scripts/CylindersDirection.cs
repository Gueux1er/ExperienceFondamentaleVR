using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylindersDirection : MonoBehaviour
{
    public Vector3 directionSpeed;
    float timer;
    float death = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(directionSpeed * Time.deltaTime);

        timer += 1 * Time.deltaTime;
        if (timer >= death)
        {
            Destroy(gameObject);
        }
    }
}
