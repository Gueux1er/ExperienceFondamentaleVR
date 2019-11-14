using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythmePingPong
{
    public class AIPingPongRacket : MonoBehaviour
    {
        AudioSource myAudio;
        private void Start()
        {
            myAudio = GetComponent<AudioSource>();
            //Physics.IgnoreLayerCollision(8, 10);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<AIPingPong>() != null && !myAudio.isPlaying)
            {
                myAudio.Play();
            }
        }

        public void OnAttached()
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
        }
    }
}