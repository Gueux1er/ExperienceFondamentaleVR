using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythmePingPong
{
    public class PongSpawner : MonoBehaviour
    {
        int score;
        int scoreMiss;
        UIMain uiMain;

        [SerializeField] GameObject pingPongPrefab;
        [SerializeField] GameObject grenadePrefab;

        //[SerializeField] float BPM;

        float SpawnMaxAngle;
        float spawnTime = 1.2f;

        float nowHp = 100;
        float totalHp = 100;

        bool gameOver;

        // Start is called before the first frame update
        void Start()
        {
            uiMain = FindObjectOfType<UIMain>();

            SpawnMaxAngle = Mathf.Abs(Mathf.Atan2(5.5f - 1.4f, 0 - 0.9f) * (180 / Mathf.PI));
            if (SpawnMaxAngle > 90) SpawnMaxAngle -= 90;

            StartCoroutine(SpawnPongCoroutine());
        }

        IEnumerator SpawnPongCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnTime);
                transform.eulerAngles = new Vector3(0, Random.Range(-SpawnMaxAngle, SpawnMaxAngle));

                GameObject prefab = pingPongPrefab;
                if (score > 25)
                {
                    var rand = Random.Range(0, 6);
                    prefab = rand > 0 ? pingPongPrefab : grenadePrefab;
                }

                var o = Instantiate(prefab, transform.position, transform.rotation);
                o.GetComponent<AIPingPong>().SetUpZSpeed(-7f);
            }
        }

        public void AddScore()
        {
            if (gameOver) return;
            score += 1;
            uiMain.SetScoreText(score);

            if (spawnTime > 0.05f && score % 7 == 0 && score > 0)
                spawnTime -= 0.05f;

            if (score >= 168)
            {
                gameOver = true;
                uiMain.Complete();
            }
        }

        public void AddMiss()
        {
            scoreMiss += 1;
            uiMain.SetScoreMissText(scoreMiss);
        }

        public void Hurt()
        {
            if (gameOver) return;
            nowHp -= 15;
            var value = nowHp <= 0 ? 0 : nowHp / totalHp;
            uiMain.SetHpValue(value);

            //game over
            if (nowHp <= 0)
            {
                gameOver = true;
                spawnTime = 0.05f;
                uiMain.GameOver();
            }
        }
    }
}