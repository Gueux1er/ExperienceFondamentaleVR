using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPCE_FollowThePlayer_Rotate : MonoBehaviour

{

    public float maxSpeedOfRotation;
    public float maxSize;
    public float maxSpeedOfGrowth;
    GameObject ballToGrow;

    public GameObject headOfPlayer;

    // Start is called before the first frame update
    void Start()
    {
        ballToGrow = this.transform.GetChild(0).transform.gameObject;
        maxSpeedOfRotation = Random.Range(1f, maxSpeedOfRotation);
        this.transform.LookAt (headOfPlayer.transform);
        this.transform.Rotate(0, 0, Random.Range(0f, 360f));
        maxSpeedOfGrowth = Random.Range(1, maxSpeedOfGrowth);
        maxSize = Random.Range(0.1f, maxSize);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3 (headOfPlayer.transform.position.x, headOfPlayer.transform.position.y, headOfPlayer.transform.position.z);

        float sizeOfTheObject = Mathf.PingPong(Time.time / maxSpeedOfGrowth, maxSize) + 0.1f;
        ballToGrow.transform.localScale = new Vector3(sizeOfTheObject, sizeOfTheObject, sizeOfTheObject);
        this.transform.Rotate(0, maxSpeedOfRotation, 0);
    }
}
