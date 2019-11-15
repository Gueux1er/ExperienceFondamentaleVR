using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class XPCE14_ControlSlab : MonoBehaviour
{
    [System.Serializable]
    public struct Controler
    {
        public GameObject arrowObject;
        public XPCE14_ControlSlab targetControlSlab;
    }
    public List<Controler> controlers;

    private Vector3 baseArrowScale;
    

    private void Awake()
    {
        baseArrowScale = controlers[0].arrowObject.transform.localScale;
        
        for (int i = 0; i < controlers.Count; ++i)
        {
            controlers[i].arrowObject.SetActive(false);
        }
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
            controlers[i].arrowObject.SetActive(true);
            controlers[i].arrowObject.transform.DOKill();
            controlers[i].arrowObject.transform.localScale = baseArrowScale;
            controlers[i].arrowObject.transform.DOScale(0, 0.5f).From();
        }
    }
}
