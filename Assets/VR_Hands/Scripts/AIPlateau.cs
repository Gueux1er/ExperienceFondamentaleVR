using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class AIPlateau : MonoBehaviour
{
    public bool isPicking { get; private set; }

    [SerializeField] float touchTolerance = 0.1f;
    Vector2 colSize;

    private void Start()
    {
        colSize = GetComponentInChildren<Collider>().bounds.size / 2;
    }

    public void OnPickUp()
    {
        isPicking = true;
    }

    public void OnPicking()
    {
        transform.localEulerAngles = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    public void OnDetached()
    {
        isPicking = false;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //if (!isPicking) return;

    //    if (collision.gameObject.GetComponentInParent<Throwable>() && !collision.gameObject.GetComponentInParent<Rigidbody>().isKinematic)
    //    {
    //        //Debug.Log("Touch object");
    //        //var targetPos = collision.transform.position;
    //        //var targetSize = collision.collider.bounds.size / 2;

    //        //var isOnTop = transform.position.y + colSize.y < (targetPos.y + targetSize.y + touchTolerance);
    //        //if (isOnTop)
    //        //{
    //        //    Debug.Log("TouchY");
    //        //    if ((transform.position.x > targetPos.x && transform.position.x - colSize.x < targetPos.x + targetSize.x) ||
    //        //        (transform.position.x < targetPos.x && transform.position.x + colSize.x > targetPos.x - targetSize.x))
    //        //    {
    //        collision.gameObject.GetComponentInParent<Rigidbody>().isKinematic = true;
    //        collision.transform.SetParent(transform, false);
    //        collision.transform.localEulerAngles = Vector3.zero;
    //        //    }
    //        //}
    //    }
    //}

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.parent == transform) return;

        if (collision.gameObject.GetComponentInParent<PickedObject>())
        {
            Debug.Log("Touch object");
            //var targetPos = collision.transform.position;

            //collision.gameObject.GetComponentInParent<PickedObject>().AttachToPlateau(colSize.y, this);

            //var isOnTop = transform.position.y + colSize.y < (targetPos.y + targetSize.y + touchTolerance);
            //if (isOnTop)
            //{
            //    Debug.Log("TouchY");
            //    if ((transform.position.x > targetPos.x && transform.position.x - colSize.x < targetPos.x + targetSize.x) ||
            //        (transform.position.x < targetPos.x && transform.position.x + colSize.x > targetPos.x - targetSize.x))
            //    {

            //    }
            //}
        }
    }
}
