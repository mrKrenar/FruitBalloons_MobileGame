using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToGameOverScreen : MonoBehaviour {
    PlayerAndTrampolines player;
    UiController ui;
    Animator gameOverAnimator;

    private void Start()
    {
        ui = GameObject.Find("GameController").GetComponent<UiController>();
        gameOverAnimator = GameObject.Find("Canvas-GameOver").GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAndTrampolines>();
    }

    public void GameOver()
    {
        gameOverAnimator.ResetTrigger("tr_EnterSecondChanceAnim");
        gameOverAnimator.SetTrigger("tr_ExitSecondChanceAnim");
        ui.hasContinuedPlaying = true;
        ui.UpdateScore(2);
    }

    public void ContinuePlaying()
    {
        gameOverAnimator.ResetTrigger("tr_EnterSecondChanceAnim");
        gameOverAnimator.SetTrigger("tr_ExitSecondChanceAnim");
        player.SecondChance();
    }

}
