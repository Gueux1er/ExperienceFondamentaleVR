using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabController : MonoBehaviour
{
    public GameObject shape;

    private float multiplicator = 1;

    private float endLength;

    void Start()
    {
        endLength = transform.position.magnitude + 1;
    }
    void Update()
    {
        shape.transform.localScale = Vector3.one * SpawnerManager.instance.multiplicator;
        shape.transform.position = new Vector3(shape.transform.position.x, shape.transform.localScale.x/2, shape.transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward * 100 * SpawnerManager.instance.speed, Time.deltaTime * SpawnerManager.instance.speed);

        if (transform.position.magnitude >= endLength)
            Destroy(gameObject);
    }

    public void IncreaseScale()
    {
        SpawnerManager.instance.multiplicator += 1f;
    }

    public void DecreaseScale()
    {
        SpawnerManager.instance.multiplicator -= 1f;
    }
}
