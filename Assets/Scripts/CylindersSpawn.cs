using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylindersSpawn : MonoBehaviour
{

    public GameObject cylinderPrefab;

    float timerOne;
    float timerTwo;
    float timerThree;
    float timerFour;

    public float timerToSpawn = 5f;

    public Vector3 spawnOne;
    public Vector3 spawnTwo;
    public Vector3 spawnThree;
    public Vector3 spawnFour;

    public Vector3 movementDirection;

    public bool randomRotation = false;

    // Start is called before the first frame update
    void Start()
    {
        timerOne = Random.Range(0, 3);
        timerTwo = Random.Range(0, 3);
        timerThree = Random.Range(0, 3);
        timerFour = Random.Range(0, 3);
    }

    // Update is called once per frame
    void Update()
    {
        timerOne += 1 * Time.deltaTime;
        timerTwo += 1 * Time.deltaTime;
        timerThree += 1 * Time.deltaTime;
        timerFour += 1 * Time.deltaTime;

        if (timerOne >= timerToSpawn)
        {
            GameObject obstacle = Instantiate(cylinderPrefab, gameObject.transform);
            obstacle.transform.position = spawnOne;
            CylindersDirection cD = obstacle.GetComponent<CylindersDirection>();
            cD.directionSpeed = movementDirection;
            if (randomRotation == false)
            {
                obstacle.transform.eulerAngles = new Vector3(0,0,90f);
            }
            else
            {
                obstacle.transform.eulerAngles = new Vector3(Random.Range(0,60), Random.Range(0,60), 90f);
            }
            timerOne = Random.Range(0, 2);
        }
        if (timerTwo >= timerToSpawn)
        {
            GameObject obstacle = Instantiate(cylinderPrefab, gameObject.transform);
            obstacle.transform.position = spawnTwo;
            CylindersDirection cD = obstacle.GetComponent<CylindersDirection>();
            cD.directionSpeed = movementDirection;
            if (randomRotation == false)
            {
                obstacle.transform.eulerAngles = new Vector3(0, 0, 90f);
            }
            else
            {
                obstacle.transform.eulerAngles = new Vector3(Random.Range(0, 60), Random.Range(0, 60), 90f);
            }
            timerTwo = Random.Range(0, 2);
        }
        if (timerThree >= timerToSpawn)
        {
            GameObject obstacle = Instantiate(cylinderPrefab, gameObject.transform);
            obstacle.transform.position = spawnThree;
            CylindersDirection cD = obstacle.GetComponent<CylindersDirection>();
            cD.directionSpeed = movementDirection;
            if (randomRotation == false)
            {
                obstacle.transform.eulerAngles = new Vector3(0, 0, 90f);
            }
            else
            {
                obstacle.transform.eulerAngles = new Vector3(Random.Range(0, 60), Random.Range(0, 60), 90f);
            }
            timerThree = Random.Range(0, 2);
        }
        if (timerFour >= timerToSpawn)
        {
            GameObject obstacle = Instantiate(cylinderPrefab, gameObject.transform);
            obstacle.transform.position = spawnFour;
            CylindersDirection cD = obstacle.GetComponent<CylindersDirection>();
            cD.directionSpeed = movementDirection;
            if (randomRotation == false)
            {
                obstacle.transform.eulerAngles = new Vector3(0, 0, 90f);
            }
            else
            {
                obstacle.transform.eulerAngles = new Vector3(Random.Range(0, 60), Random.Range(0, 60), 90f);
            }
            timerFour = Random.Range(0, 2);
        }

    }
}
