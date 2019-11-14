using System.Linq;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CustomHandController : MonoBehaviour
{
    Hand currentHand;

    GameObject[] objList;
    GameObject oldObj;

    // Start is called before the first frame update
    void Awake()
    {
        currentHand = GetComponent<Hand>();
    }

    private void Start()
    {
        objList = FindObjectsOfType<GameObject>().Where
            (o => o.activeSelf && o.GetComponent<MeshRenderer>() != null && o.GetComponent<MeshRenderer>().material.HasProperty("_Color")).ToArray();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(currentHand.GetGrabStarting() != GrabTypes.None)
        {
            //find random material
            var mesh = objList[Random.Range(0, objList.Length)].GetComponent<MeshRenderer>();
            while (mesh.gameObject == oldObj)
            {
                mesh = objList[Random.Range(0, objList.Length)].GetComponent<MeshRenderer>();
                if (oldObj != mesh.gameObject) oldObj = mesh.gameObject;
            }
            var color = mesh.material.color;
            mesh.material.color = new Color(Random.Range(0, 1f), color.r, color.g); ;
        }
    }
}
