using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public GameObject spawner;
    public GameObject[] prefabs;
    private GameObject currentPrefab;
    private int prefabIndex = 0;

    private float spawnTimer = 8;
    private float spawnTmp = 0;

    [HideInInspector] public float speed = 30;

    public static SpawnerManager instance;

    public float multiplicator;

    private bool stickIsUp;
    private bool stickIsDown;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        
        if (OVRInput.GetDown(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.A)) //B
        {
            currentPrefab.GetComponent<PrefabController>().IncreaseScale();
        }
        if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Q)) //A
        {
            currentPrefab.GetComponent<PrefabController>().DecreaseScale();
        }


        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick) || Input.GetKeyDown(KeyCode.P)) //press joystick gauche
        {
            prefabIndex = (prefabIndex + 1) % 3;
        }


        if (OVRInput.GetDown(OVRInput.Button.Four)) //Y
        {
            //increase speed
            speed += 5;
        }
        if (OVRInput.GetDown(OVRInput.Button.Three)) //X
        {
            //decrease speed
            speed -= 5;
        }

        float v = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y;

        if (v >= 0.75f && !stickIsUp)
        {
            stickIsUp = true;
            spawner.transform.position += Vector3.up;
        }
        else
        {
            stickIsUp = false;
        }

        if (v <= -0.75f && !stickIsDown)
        {
            stickIsDown = true;
            spawner.transform.position -= Vector3.up;
        }
        else
        {
            stickIsDown = false;
        }

        //spawn each x seconds
        spawnTmp += Time.deltaTime;
        if (spawnTmp >= spawnTimer)
        {
            spawnTmp = 0f;
            SpawnPrefab();
        }

 
    }

    private void SpawnPrefab()
    {
        Debug.Log("spawn");
        currentPrefab = Instantiate(prefabs[prefabIndex], spawner.transform);
        currentPrefab.transform.forward = -currentPrefab.transform.position.normalized;
        //currentPrefab.transform.DOScale(Vector3.zero, 0.1f).SetDelay(spawnTimer);
    }
}
