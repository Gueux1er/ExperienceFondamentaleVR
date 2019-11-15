using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace RythmePingPong
{
    public enum PingPongTypes
    {
        PingPong,
        Grenade
    }

    public class AIPingPong : MonoBehaviour
    {
        PingPongMain main;

        [SerializeField] GameObject fxExplosion;
        [SerializeField] PingPongTypes pingPongType;
        AudioSource myAudio;
        bool touchedRacket;

        Rigidbody rb;
        Vector3 moveVelocity;

        //float touchTime;

        void Awake()
        {
            main = FindObjectOfType<PingPongMain>();
            myAudio = GetComponent<AudioSource>();

            rb = GetComponent<Rigidbody>();
            Physics.IgnoreLayerCollision(8, 9);
        }

        //private void Update()
        //{
        //    touchTime += Time.deltaTime;
        //}

        public void SetUpZSpeed(float speed)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(transform.forward * speed, ForceMode.Impulse);
            //rb.AddForce(force, ForceMode.Impulse);
        }

        private void FixedUpdate()
        {
            if (rb.position.y < -1)
            {
                if (!touchedRacket) main.AddMiss();
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<AIPingPongRacket>() == null && rb.velocity.magnitude > 0.5f && !myAudio.isPlaying)
            {
                myAudio.volume = rb.velocity.magnitude / 10;
                myAudio.Play();
            }

            if (gameObject.layer != 9)
            {
                //Debug.Log(touchTime);
                gameObject.layer = 9;
            }

            if (touchedRacket) return;
            if (collision.gameObject.GetComponent<AIPingPongRacket>() != null && collision.gameObject.GetComponent<Throwable>().Attached)
            {
                switch (pingPongType)
                {
                    case PingPongTypes.PingPong:
                        GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                        main.AddScore();
                        break;
                    case PingPongTypes.Grenade:
                        fxExplosion.transform.SetParent(null, true);
                        fxExplosion.SetActive(true);
                        Destroy(fxExplosion, 1.5f);

                        main.Hurt();
                        Destroy(gameObject, 0.1f);
                        break;
                }
                touchedRacket = true;
            }
        }
    }
}