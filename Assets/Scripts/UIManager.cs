using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image[] coracoes;
    public Text coinText;
    public GameObject gameOverPanel;
    public Text scoreText;

    public void UpdateLives(int vidas)
    {
        for (int i = 0; i < coracoes.Length; i++)
        {
            if (vidas > i)
            {
                coracoes[i].color = Color.white;
            }
            else
            {
                coracoes[i].color = Color.black;
            }
        }
    }

    public void UpdateCoins(int coins)
    {
        coinText.text = coins.ToString();
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score + "m";
    }
}
