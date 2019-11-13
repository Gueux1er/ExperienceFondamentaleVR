using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class XPCE17_GameManager : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            DOTween.KillAll();

            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
    }
}