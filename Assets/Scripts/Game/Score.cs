using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text scoreText;

    private int currentScore_;
    // Start is called before the first frame update
    void Start()
    {
        currentScore_ = 0;
    }

    private void OnEnable()
    {
        GameEvent.AddScore += AddScore;
    }

    private void OnDisable()
    {
        GameEvent.AddScore -= AddScore;
    }

    private void AddScore(int score)
    {
        currentScore_ += score;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = currentScore_.ToString();
    }

    // Update is called once per frame

}
