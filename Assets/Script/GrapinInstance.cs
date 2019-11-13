using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapinInstance : MonoBehaviour
{

    public bool xMove = false;
    public bool yMove = false;
    public bool zMove = false;

    public bool xRotate = false;
    public bool yRotate = false;
    public bool zRotate = false;
    public float speed;

    void Start()
    {
        //transform.localScale +=  (Vector3.one * Random.Range(-0.1f, 0.1f));
        transform.position = new Vector3(transform.position.x, transform.position.y + Random.Range(-0.5f, 1f), transform.position.z);
        speed = Random.Range(20f, 30f);
        if (zMove)
        {
            speed = Random.Range(7f, 9f);
            transform.DOMoveZ(-transform.position.z, speed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
        else if(yMove)
        {
            speed = Random.Range(1f, 3f);
            transform.DOMoveY(transform.position.y + 3f, Random.Range(1f,3f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
        else if (xMove)
        {
            speed = Random.Range(8f, 10f);
            transform.DOMoveX(-transform.position.z, Random.Range(8f, 10f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }

    private void Update()
    {
        if (yRotate)
        {
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
        if (xRotate)
        {
            transform.Rotate(Vector3.right, speed * Time.deltaTime);
        }
        if (zRotate)
        {
            transform.Rotate(Vector3.forward, speed * Time.deltaTime);
        }
    }
}
