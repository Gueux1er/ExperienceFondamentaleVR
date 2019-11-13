using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSpawnerFixe : MonoBehaviour
{

    public static bool isObjectReady = true;
	public Transform[] prefabs;
	public int objectToSpawn;
	public bool activeOrNot = false;

	public float delay = 5;

	// Start is called before the first frame update
	void Start()
	{
	    StartCoroutine(ReadyingObject());
	}

	    // Update is called once per frame
	void Update()
	{
	    if (Input.GetKeyDown(KeyCode.R)) { SceneManager.LoadScene("XPCE04_02"); }
	    if (Input.GetKeyDown("1")) { objectToSpawn = 0; }
	    if (Input.GetKeyDown("2")) { objectToSpawn = 1; }
	    if (Input.GetKeyDown("3")) { objectToSpawn = 2; }
	    if (Input.GetKeyDown(KeyCode.W)) { activeOrNot = !activeOrNot; }
	}

	IEnumerator ReadyingObject()
	{
	    for (; ; )
	    {
	        
	        
	        yield return new WaitForSeconds(delay);
	        if (activeOrNot == true) {
	        	GameObject go = Instantiate(prefabs[objectToSpawn], this.transform.position, this.transform.rotation).gameObject;
	        	Debug.Log(this.transform.rotation.y);
	        }
	        
	    }
	} 
}

