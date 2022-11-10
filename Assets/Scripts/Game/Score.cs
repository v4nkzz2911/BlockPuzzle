using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BestScoreData
{
    public int score = 0;
}


public class Score : MonoBehaviour
{
    public Text scoreText;
    public Text endScoreText;

    public static bool newBestScore = false;
    private BestScoreData bestScore_ = new BestScoreData();
    private int currentScore_;
    private string bestScorekey_ = "best";
    // Start is called before the first frame update
    void Start()
    {
        currentScore_ = 0;
        newBestScore = false;
        UpdateScoreText();
    }

    private void Awake()
    {
        if (BinaryDataStream.Exist(bestScorekey_))
        {
            StartCoroutine(ReadDataFile());
        }
    }

    private IEnumerator ReadDataFile()
    {
        bestScore_ = BinaryDataStream.Read<BestScoreData>(bestScorekey_);
        yield return new WaitForEndOfFrame();
        Debug.Log("Best Score: " + bestScore_.score);
        GameEvent.UpdateBestScore(currentScore_, bestScore_.score);
    }

    private void OnEnable()
    {
        GameEvent.AddScore += AddScore;
        GameEvent.GameOver += SaveBestScore;
    }

    private void OnDisable()
    {
        GameEvent.AddScore -= AddScore;
        GameEvent.GameOver -= SaveBestScore;
    }

    private void AddScore(int score)
    {
        currentScore_ += score;
        if (currentScore_> bestScore_.score)
        {
            newBestScore = true;
            bestScore_.score = currentScore_;
        }
        GameEvent.UpdateBestScore(currentScore_, bestScore_.score);
        UpdateScoreText();
    }



    private void SaveBestScore(bool newBestScore)
    {
        BinaryDataStream.Save<BestScoreData>(bestScore_,bestScorekey_);
    }

    private void UpdateScoreText()
    {
        scoreText.text = currentScore_.ToString();
        endScoreText.text = scoreText.text;
    }

    // Update is called once per frame

}
