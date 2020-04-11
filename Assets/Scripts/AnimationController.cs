using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;

public class AnimationController : MonoBehaviour {
    UiController ui;
    Animator ballonAnimator, flashAnimator, scoreAnimator, tmpScoreAnimator, perfectLandAnimator, gameOverAnimator,
        startGameAnimator, inGameStatsAnimator, warnForBadLandAnimator;
    bool resetTmp;
    GameObject GameUi;


    void Awake()//assign animator and texts
    {
        ui = GameObject.Find("GameController").GetComponent<UiController>();
        scoreAnimator = GameObject.Find("ScoreAnimations").GetComponent<Animator>();
        tmpScoreAnimator = GameObject.Find("TmpScoreAnimations").GetComponent<Animator>();
        perfectLandAnimator = GameObject.Find("PerfectLandTextAnimation").GetComponent<Animator>();
        gameOverAnimator = GameObject.Find("Canvas-GameOver").GetComponent<Animator>();
        flashAnimator = GameObject.Find("Flash").GetComponent<Animator>();
        startGameAnimator = GameObject.Find("Canvas-StartUI").GetComponent<Animator>();
        inGameStatsAnimator = GameObject.FindGameObjectWithTag("InWorldStats").GetComponent<Animator>();
        warnForBadLandAnimator = GameObject.Find("BadLand").GetComponent<Animator>();

        GameUi = GameObject.Find("Canvas-GamePlayUI");
    }

    public void WarnForBadLand()
    {
        warnForBadLandAnimator.SetBool("badLandWarn", true);
    }

    public void PerfectLandAnim(bool incJustTmpOne)
    {
        tmpScoreAnimator.SetBool("bo_IncreaseTmpOnly", true);
    }
    public void PerfectLandAnim(bool firstLand, bool displayPerfectTxt)
    {
        if (firstLand)
        {
            perfectLandAnimator.SetBool("bo_PerfectLand", true);
            tmpScoreAnimator.SetBool("bo_ActivateTmpScore", true);
        }
        else if(displayPerfectTxt)
        {
            tmpScoreAnimator.SetBool("bo_UpdateTmpScore", true);
            perfectLandAnimator.SetBool("bo_PerfectLand", true);
        }
        else
        {
            tmpScoreAnimator.SetBool("bo_UpdateTmpScore", true);
        }
    }
    public void NormalLandAnim()
    {
        ui.ResetTmpScore();
        tmpScoreAnimator.SetBool("bo_CloseTmpScore", true);
    }
    public void UpdateScoreAnim()
    {
        scoreAnimator.SetBool("bo_UpdateScore", true);
    }
    public void ContinuePlayingAfterGameover()
    {
        gameOverAnimator.SetTrigger("tr_ExitSecondChanceAnim");
        gameOverAnimator.ResetTrigger("tr_EnterSecondChanceAnim");
    }
    public void GameOverAnim(bool newHighScore, bool secondChance)
    {
        if (!secondChance)
            gameOverAnimator.SetTrigger("tr_EnterSecondChanceAnim");
        else
        {
            if (newHighScore)
                gameOverAnimator.SetTrigger("tr_GameOverHighScore");
            else gameOverAnimator.SetTrigger("tr_GameOver");
        }
    }
    public void FlashScreen()
    {
        flashAnimator.SetBool("Flash", true);
    }
    public void BalloonCollide()
    {
        ballonAnimator.SetBool("bo_BalloonCollide", true);
    }
    public void BallonCollideLow()
    {
        ballonAnimator.SetBool("bo_BalloonCollideLow", true);
        
    }
    public void UpdateBalloonAnimator(Animator anim)
    {
        ballonAnimator = anim;
    }
    public void DisplayLevelUpText()
    {
        perfectLandAnimator.SetBool("bo_LevelUp", true);
    }
    public void StartGameAnim()
    {
        startGameAnimator.SetTrigger("StartGame");
        inGameStatsAnimator.SetTrigger("EnableStats");
        GameUi.SetActive(false);
        GameUi.GetComponent<Canvas>().enabled = true;
        GameUi.SetActive(true);
    }
}
