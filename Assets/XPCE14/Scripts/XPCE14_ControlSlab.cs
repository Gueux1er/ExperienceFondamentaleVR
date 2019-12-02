using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class XPCE14_ControlSlab : MonoBehaviour
{
    [System.Serializable]
    public struct Controler
    {
        public GameObject arrowObject;
        //public XPCE14_ControlSlab targetControlSlab;
    }
    public List<Controler> controlers;

    private Vector3 baseArrowScale;

    private void Awake()
    {
        baseArrowScale = controlers[0].arrowObject.transform.localScale;

        Active();
    }

    public void Desable()
    {
        for (int i = 0; i < controlers.Count; ++i)
        {
            controlers[i].arrowObject.transform.DOKill();
            controlers[i].arrowObject.transform.DOScale(0, 0.5f);
        }
    }

    public void Active()
    {
        for (int i = 0; i < controlers.Count; ++i)
        {
            if (!Physics.Raycast(
                new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z),
                (i == 0) ? Vector3.forward : (i == 1) ? Vector3.right : (i == 2) ? Vector3.back : Vector3.left + Vector3.up * (transform.position.y + 0.1f),
                1f))
            {
                controlers[i].arrowObject.SetActive(true);
                controlers[i].arrowObject.transform.DOKill();
                controlers[i].arrowObject.transform.localScale = baseArrowScale;
                controlers[i].arrowObject.transform.DOScale(0, 0.5f).From();
            }
        }
    }
}