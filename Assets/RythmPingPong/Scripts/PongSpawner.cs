using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythmePingPong
{
    public class PongSpawner : MonoBehaviour
    {
        [SerializeField] GameObject pingPongPrefab;
        [SerializeField] GameObject grenadePrefab;

        //[SerializeField] float BPM;

        Coroutine spawnCoroutine;
        float SpawnMaxAngle;
        [SerializeField] float defaultSpawntTime = 1.2f;
        int nowLevel;

        // Start is called before the first frame update
        void Start()
        {
            SpawnMaxAngle = Mathf.Abs(Mathf.Atan2(5.5f - 1.4f, 0 - 0.9f) * (180 / Mathf.PI));
            if (SpawnMaxAngle > 90) SpawnMaxAngle -= 90;
        }

        public void StartSpawnPong()
        {
            if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
            spawnCoroutine = StartCoroutine(SpawnCorou());
        }

        IEnumerator SpawnCorou()
        {
            while (true)
            {
                yield return new WaitForSeconds(defaultSpawntTime - Mathf.Clamp(0.05f * nowLevel, 0, defaultSpawntTime));
                transform.eulerAngles = new Vector3(0, Random.Range(-SpawnMaxAngle, SpawnMaxAngle));

                GameObject prefab = pingPongPrefab;
                if (nowLevel >= 5)
                {
                    var rand = Random.Range(0, 6);
                    prefab = rand > 0 ? pingPongPrefab : grenadePrefab;
                }

                var o = Instantiate(prefab, transform.position, transform.rotation);
                o.GetComponent<AIPingPong>().SetUpZSpeed(-7f);
            }
        }

        public void LevelUp()
        {
            nowLevel += 1;
        }
    }
}