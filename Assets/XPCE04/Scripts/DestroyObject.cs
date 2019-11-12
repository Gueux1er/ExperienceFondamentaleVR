using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public GameObject despawner;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {

    	if (other.CompareTag("Despawner") == despawner) { 
    		Debug.Log("hello Despawner");
    		// ObjectSpawner.isObjectReady = true;
    		Destroy(this.gameObject); 
    	}
    }
}
