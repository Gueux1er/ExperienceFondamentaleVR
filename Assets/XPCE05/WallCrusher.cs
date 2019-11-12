using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCrusher : MonoBehaviour
{

    public Camera cam;

    public float moveSpeed = 0.2f;
    public bool steppedMovement = false;
    public float stepCycle = 3.0f;
    public GameObject[] walls;

    private float timer = 0.0f;
    private bool moveStep = false;

    private List<Vector3> positions = new List<Vector3>();

    private void Start()
    {
        foreach(GameObject go in walls)
        {
            positions.Add(go.transform.position);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= stepCycle)
        {
            moveStep = !moveStep;
            timer = 0.0f;
        }

        if (!steppedMovement || moveStep)
        {
            foreach (GameObject go in walls)
            { 
                if (go.name == "top")
                {
                    go.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
                }
                else
                {
                    go.transform.Translate(go.transform.forward * moveSpeed * Time.deltaTime);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Initialize();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            Debug.Log("aaaaah");

            StopExperience();
        }
    }

    private void StopExperience()
    {
        Initialize(); 
    }

    private void Initialize()
    {
        for (int i = 0; i < positions.Count; ++i)
        {
            walls[i].transform.position = positions[i];
        }
    }

}
