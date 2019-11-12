﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundPlayer : MonoBehaviour
{
    GameObject obj;
    public GameObject player;
    public float speed = 20;
    
    // Start is called before the first frame update
    void Start()
    {
        obj = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        obj.transform.RotateAround(player.transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
