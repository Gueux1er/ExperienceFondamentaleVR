using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ce script doit être mis sur une surface.
// Pour chaque collider dans la liste des trous, 
// on va mettre à jour le shader associé à la surface qui va s'occuper de rendre ou non les pixels en fonction

public class HolesController : MonoBehaviour
{
  const int MAX_HOLES = 100;
  public SphereCollider[] holes;
  public Material holesMaterial;

  Vector4[] holesPositionBuffer = new Vector4[MAX_HOLES]; // les shaders ne permettent pas d'avoir des tableaux de V3
  float[] holesRadiusBuffer = new float[MAX_HOLES];

  void Start()
  {
    // initialise les tableaux du shader (car si on a que deux trous au début, on ne pourra pas resize plus tard du côté du shader)
    holesMaterial.SetFloatArray("_HolesRadius", new float[MAX_HOLES]);
    holesMaterial.SetVectorArray("_HolesPositions", new Vector4[MAX_HOLES]);

        // make sure we hide the spheres
        foreach (var item in holes)
        {
            item.GetComponent<Renderer>().enabled = false;
        }
    }

  void Update()
  {
    for (int i = 0; i < holes.Length; i++)
    {
      holesPositionBuffer[i]  = (Vector4)holes[i].transform.position;
      holesRadiusBuffer[i]    = holes[i].radius * holes[i].transform.localScale.x; // on suppose que c'est une sphère
    }

    holesMaterial.SetInt("_HolesCount", holes.Length);
    holesMaterial.SetFloatArray("_HolesRadius", holesRadiusBuffer);
    holesMaterial.SetVectorArray("_HolesPositions", holesPositionBuffer);
  }
}
