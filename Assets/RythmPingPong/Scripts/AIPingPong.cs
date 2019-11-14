using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythmePingPong
{
    public enum PingPongTypes
    {
        PingPong,
        Grenade
    }

    public class AIPingPong : MonoBehaviour
    {
        [SerializeField] GameObject fxExplosion;
        [SerializeField] PingPongTypes pingPongType;
        AudioSource myAudio;
        PongSpawner pongSpawner;
        bool touchedRacket;

        Rigidbody rb;
        Vector3 moveVelocity;

        //float touchTime;

        void Awake()
        {
            pongSpawner = FindObjectOfType<PongSpawner>();
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
            if (rb.position.y < -10)
            {
                if (!touchedRacket) pongSpawner.AddMiss();
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

            if (collision.gameObject.GetComponent<AIPingPongRacket>() != null && collision.gameObject.GetComponent<Rigidbody>().isKinematic)
            {
                if (!touchedRacket)
                {
                    switch (pingPongType)
                    {
                        case PingPongTypes.PingPong:
                            GetComponent<MeshRenderer>().material.color = Color.green;
                            pongSpawner.AddScore();
                            break;
                        case PingPongTypes.Grenade:
                            var fx = Instantiate(fxExplosion, transform.position, Quaternion.identity);
                            Destroy(fx, 1.5f);
                            pongSpawner.Hurt();
                            Destroy(gameObject,0.1f);
                            break;
                    }

                    touchedRacket = true;
                }
            }
        }
    }
}