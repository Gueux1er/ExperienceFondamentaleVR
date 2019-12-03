using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedObject : MonoBehaviour
{
    AIPlateau targetPlateau;

    Transform defaultParent;
    Rigidbody rb;
    Vector2 colSize;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultParent = transform.parent;
        colSize = GetComponentInChildren<Collider>().bounds.size / 2;
    }

    public void AttachToPlateau(float targetYSize, AIPlateau plateau)
    {
        if (rb.isKinematic) return;

        targetPlateau = plateau;

        rb.isKinematic = true;
        transform.SetParent(plateau.transform, false);
        transform.localEulerAngles = Vector3.zero;
        transform.localPosition = new Vector3(transform.localPosition.x, colSize.y + targetYSize, transform.localPosition.z);
    }

    public void OnDetached()
    {
        transform.parent = defaultParent;
        rb.isKinematic = false;

        targetPlateau = null;
    }

    private void Update()
    {
        if (targetPlateau != null)
        {

        }
    }
}
