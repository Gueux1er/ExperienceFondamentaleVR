using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RythmePingPong
{
    public class UIMain : MonoBehaviour
    {
        [SerializeField] Text textScore;
        [SerializeField] Text textMiss;

        [SerializeField] Image hpValue;
        [SerializeField] GameObject gameCompleteText;
        [SerializeField] GameObject gameOverText;
        private void Awake()
        {
            gameCompleteText.SetActive(false);
            gameOverText.SetActive(false);
        }

        public void SetScoreText(int score)
        {
            textScore.text = $"Score: {score}";
        }

        public void SetScoreMissText(int score)
        {
            textMiss.text = $"Miss: {score}";
        }

        public void SetHpValue(float value)
        {
            hpValue.fillAmount = value;
        }

        public void Complete()
        {
            gameCompleteText.SetActive(true);
        }

        internal void GameOver()
        {
            gameOverText.SetActive(true);
        }
    }
}