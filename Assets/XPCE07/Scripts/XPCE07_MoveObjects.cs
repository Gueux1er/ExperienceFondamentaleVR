using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCE07_MoveObjects : MonoBehaviour
{
    [SerializeField] Transform m_transformToMove;
    [SerializeField] float m_speed;
    bool m_moving;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            m_moving = true;
        if(m_moving)
        m_transformToMove.position += new Vector3(0, 0, Time.deltaTime* m_speed);
    }
}
