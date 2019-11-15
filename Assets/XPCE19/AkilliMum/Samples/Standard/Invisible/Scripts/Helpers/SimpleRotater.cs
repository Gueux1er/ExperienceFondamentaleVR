
using UnityEngine;

namespace AkilliMum.Standard.Invisible
{
    public class SimpleRotater : MonoBehaviour
    {
        public float XSpeed = 0;
        public float YSpeed = 0;
        public float ZSpeed = 0;

        void FixedUpdate()
        {
            gameObject.transform.Rotate(new Vector3(XSpeed, YSpeed, ZSpeed),
                Space.Self);
        }
    }
}
