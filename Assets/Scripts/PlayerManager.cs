using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static bool gameOver;
    public GameObject gameOverPanel;

    public static bool isGameStarted;
    public GameObject StartText;

    public static int numberOfCoins;
    public TMP_Text skorText;

    void Start()
    {
        gameOver = false;
        Time.timeScale = 1;
        isGameStarted = false;
        numberOfCoins = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Game Over
        if (gameOver)
        {
            Time.timeScale = 0;
            gameOverPanel.SetActive(true);

        }
        skorText.text = "Skor: " + numberOfCoins * 5;
        if (Input.GetKeyDown(KeyCode.Mouse0) || SweepManager.tap)
        {
            isGameStarted = true;
            Destroy(StartText);
            Destroy(skorText);
        }
    }
}
