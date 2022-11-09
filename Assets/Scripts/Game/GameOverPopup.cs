using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopup : MonoBehaviour
{
    public GameObject gameOverPopup;
    //public GameObject loosePopup;
    public GameObject newBestScorePopup;

    
    // Start is called before the first frame update
    void Start()
    {
        gameOverPopup.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvent.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvent.GameOver -= OnGameOver;
    }

    private void OnGameOver(bool newBestScore)
    {
        gameOverPopup.SetActive(true);
        //loosePopup.SetActive(false);
        newBestScorePopup.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
