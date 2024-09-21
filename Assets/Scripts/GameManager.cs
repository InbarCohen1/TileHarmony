using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("TileBoard")]
    [SerializeField] private TileBoard board;


    private void Start()
    {
       NewGame();
    }

    public void NewGame()
    {
        // reset score
        //SetScore(0);
        //hiscoreText.text = LoadHiscore().ToString();

        // hide game over screen
        // gameOver.alpha = 0f;
        // gameOver.interactable = false;

        // update board state
        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        //gameOver.interactable = true;

        //StartCoroutine(Fade(gameOver, 1f, 1f));
    }

}
