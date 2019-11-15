using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


// Build a room that is the size of the SteamVR_PlayArea
public class XPCE14_RoomBuilder : MonoBehaviour
{
  public GameObject wallPrefab;
  public float ceilingHeight = 3f;
  public bool showCeiling = false;

  float wallThickness = 0.2f;
  SteamVR_PlayArea playArea;

  GameObject roomRoot;

  void Awake()
  {
    playArea = FindObjectOfType<SteamVR_PlayArea>();
  }

  /*void Start()
  {
    BuildRoom();
  }*/

  [ContextMenu("Build Room")]
  void BuildRoom()
  {
    playArea = FindObjectOfType<SteamVR_PlayArea>();

    roomRoot = new GameObject("PlayRoom");
    roomRoot.transform.position = Vector3.zero;

    var rect = new HmdQuad_t();
    if (!SteamVR_PlayArea.GetBounds(playArea.size, ref rect))
        return;

    Vector3[] corners = {
      new Vector3(rect.vCorners0.v0, rect.vCorners0.v1, rect.vCorners0.v2),
      new Vector3(rect.vCorners1.v0, rect.vCorners1.v1, rect.vCorners1.v2),
      new Vector3(rect.vCorners2.v0, rect.vCorners2.v1, rect.vCorners2.v2),
      new Vector3(rect.vCorners3.v0, rect.vCorners3.v1, rect.vCorners3.v2)
    };

    float width = Vector3.Distance(  corners[0], corners[1]  );
    float depth = Vector3.Distance(  corners[0], corners[3]  );

    GameObject wall = GameObject.Instantiate(wallPrefab);
    wall.transform.localScale = new Vector3(width, ceilingHeight*2, wallThickness);
    wall.transform.position = corners[0] - Vector3.right * width * 0.5f;
    wall.transform.parent = roomRoot.transform;

    wall = GameObject.Instantiate(wallPrefab);
    wall.transform.localScale = new Vector3(width, ceilingHeight*2, wallThickness);
    wall.transform.position = corners[3] - Vector3.right * width * 0.5f;
    wall.transform.parent = roomRoot.transform;

    wall = GameObject.Instantiate(wallPrefab);
    wall.transform.localScale = new Vector3(wallThickness, ceilingHeight*2, depth);
    wall.transform.position = corners[0] + Vector3.forward * depth * 0.5f;
    wall.transform.parent = roomRoot.transform;

    wall = GameObject.Instantiate(wallPrefab);
    wall.transform.localScale = new Vector3(wallThickness, ceilingHeight*2, depth);
    wall.transform.position = corners[1] + Vector3.forward * depth * 0.5f;
    wall.transform.parent = roomRoot.transform;

    if(showCeiling)
    {
      wall = GameObject.Instantiate(wallPrefab);
      wall.transform.localScale = new Vector3(width, wallThickness, depth);
      wall.transform.position = new Vector3(0, ceilingHeight, 0);
      wall.transform.parent = roomRoot.transform;
    }
  } 
}
