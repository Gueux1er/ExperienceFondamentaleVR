using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythmePingPong
{
    public class PingPongMain : MonoBehaviour
    {
        [SerializeField] GameObject startMenu;
        PongSpawner spawner;

        int score;
        int scoreMiss;
        UIMain uiMain;

        float nowHp = 100;
        float totalHp = 100;

        public bool GameOver { get; private set; }

        List<AIPingPongRacket> pickedRackets = new List<AIPingPongRacket>();

        // Start is called before the first frame update
        void Start()
        {
            GameOver = true;
            uiMain = FindObjectOfType<UIMain>();
            uiMain.gameObject.SetActive(false);

            spawner = FindObjectOfType<PongSpawner>();

            startMenu.SetActive(true);
        }

        internal void StartGame()
        {
            foreach(var r in pickedRackets)
                r.OnSelectFinished();

            GameOver = false;
            spawner.StartSpawnPong();
            uiMain.gameObject.SetActive(true);

            startMenu.SetActive(false);
        }

        public void AddScore()
        {
            if (GameOver) return;
            score += 1;
            uiMain.SetScoreText(score);

            if(score != 0 && score % 7 == 0)            
                spawner.LevelUp();            

            if (score >= 168)
            {
                GameOver = true;
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
            if (GameOver) return;
            nowHp -= 15;
            var value = nowHp <= 0 ? 0 : nowHp / totalHp;
            uiMain.SetHpValue(value);

            //game over
            if (nowHp <= 0)
            {
                GameOver = true;
                uiMain.GameOver();
            }
        }

        public void PickRacket(AIPingPongRacket racket)
        {
            if (!pickedRackets.Contains(racket)) pickedRackets.Add(racket);
        }

        public void DropRacket(AIPingPongRacket racket)
        {
            if (pickedRackets.Contains(racket)) pickedRackets.Remove(racket);
        }
    }
}