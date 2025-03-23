using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public Text gameOverText;
    public Text buttonText;

    public void Setup(string gameOverTxt, string buttonTxt)
    {
        gameObject.SetActive(true);
        gameOverText.text = gameOverTxt;
        buttonText.text = buttonTxt;
    }

    public void RetryButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
}