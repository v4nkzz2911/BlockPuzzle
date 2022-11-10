using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BestScore : MonoBehaviour
{
    public Text bestScoreText;

    private void OnEnable()
    {
        GameEvent.UpdateBestScore += UpdateBestScore;
    }

    private void OnDisable()
    {
        GameEvent.UpdateBestScore -= UpdateBestScore;
    }

    private void UpdateBestScore(int current, int best)
    {
        bestScoreText.text = best.ToString();
    }

}
