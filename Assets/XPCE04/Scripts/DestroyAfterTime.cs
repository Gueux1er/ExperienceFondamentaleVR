using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
	public float lifetime = 10;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Kill", lifetime);
    }

    // Update is called once per frame
    void Kill()
    {
        Destroy(gameObject);
    }
}
