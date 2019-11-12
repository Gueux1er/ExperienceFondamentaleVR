using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCrusher : MonoBehaviour
{

    public Camera cam;

    public float moveSpeed = 0.2f;
    public bool steppedMovement = false;
    public float steppCycle = 3.0f;
    public GameObject[] walls;

    private float timer = 0.0f;
    private bool moveStep = false;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= steppCycle)
        {
            moveStep = !moveStep;
            timer = 0.0f;
        }

        if (!steppedMovement || moveStep)
        {
            foreach (GameObject go in walls)
            {
                go.transform.Translate(go.transform.forward * moveSpeed * Time.deltaTime);
            }
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
        cam.gameObject.SetActive(false);
    }

    private void StartExperience()
    {
        cam.gameObject.SetActive(true);

    }

}
