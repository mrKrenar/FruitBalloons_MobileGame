using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour {
    AnimationController PlayAnimation;
    PlayerAndTrampolines GameElements;
    GameColorsController Colors;
    ResetPlayerRotation ResetPlayer;

    private void Awake()//Assign PlayAnimation and TMPro components
    {
        ResetPlayer = GameObject.Find("GameController").GetComponent<ResetPlayerRotation>();
        PlayAnimation = GameObject.Find("UI").GetComponent<AnimationController>();
        GameElements = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAndTrampolines>();
        Colors = GameObject.FindGameObjectWithTag("Background").GetComponent<GameColorsController>();

        scoreTxt = GameObject.FindGameObjectWithTag("ScoreTxt").GetComponent<TextMeshProUGUI>();
        tmpScoreTxt = GameObject.FindGameObjectWithTag("TmpScoreTxt").GetComponent<TextMeshProUGUI>();
        updateTmpScoreTxt = GameObject.FindGameObjectWithTag("UpdateTmpScoreTxt").GetComponent<TextMeshProUGUI>();
        perfectLandTxt = GameObject.FindGameObjectWithTag("PerfectLandTxt").GetComponent<TextMeshProUGUI>();
        currLevelTxt = GameObject.FindGameObjectWithTag("CurrentLevelNo").GetComponent<TextMeshProUGUI>();
        nxtLevelTxt = GameObject.FindGameObjectWithTag("NextLevelNo").GetComponent<TextMeshProUGUI>();
        scorePlus1 = GameObject.FindGameObjectWithTag("ScoreUpdateTxt").GetComponent<TextMeshProUGUI>();
        tmpScorePlus1 = GameObject.FindGameObjectWithTag("TmpScoreUpdateTxt").GetComponent<TextMeshProUGUI>();
        currentLevelWorldSpace = GameObject.FindGameObjectWithTag("InWorldStats").GetComponentInChildren<TextMeshPro>();
        highScoreTxt = GameObject.FindGameObjectWithTag("HighScoreTxt").GetComponent<TextMeshProUGUI>();
        //highScoreTxtGO = GameObject.FindGameObjectWithTag("HighScoreTxtGO").GetComponent<TextMeshProUGUI>();
        //lastScoreTxt = GameObject.FindGameObjectWithTag("LastScoreTxt").GetComponent<TextMeshProUGUI>();
        //lastScoreTxtGO = GameObject.FindGameObjectWithTag("LastScoreTxtGO").GetComponent<TextMeshProUGUI>();        
    }

    private TextMeshProUGUI scorePlus1, tmpScorePlus1, scoreTxt, tmpScoreTxt, updateTmpScoreTxt, perfectLandTxt, currLevelTxt, nxtLevelTxt, highScoreTxt;
    public TextMeshProUGUI highScoreTxtGO, lastScoreTxtGO, lastScoreTxt, diamondTxt, goDiamondsNo;
    private TextMeshPro currentLevelWorldSpace;
    int score, highScore, perfectInRow, tmpScore, currentLevel, nextLevel, reqProgressForNextLevel, diamondsNo;
    float progressIncreasingValue, currentProgress;
    public Color txtNormal, txtFire;

    public Slider progressBar;
    [HideInInspector]
    public bool hasContinuedPlaying;

    public void ResetTmpScore()
    {
        tmpScore = 0;
    }
    public void IncreaseDiamondsNo(int value)
    {
        diamondsNo += value;
        diamondTxt.text = diamondsNo + "<sprite index=0>";
    }

    // Use this for initialization
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAndTrampolines>();
        diamondsNo = PlayerPrefs.GetInt("diamonds", 400);
        diamondTxt.text = diamondsNo + "<sprite index=0>";
        perfectInRow = 1;
        score = 0;
        tmpScore = 0;
        lastScoreTxt.text = "Last:\n" + PlayerPrefs.GetInt("LastScore", 0);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreTxt.text = "Best:\n" + highScore;
        scoreTxt.text = score.ToString();
        UpdateBarMaxValue();
        currentLevel = PlayerPrefs.GetInt("MaxLevel", 1); nextLevel = currentLevel+1; UpdateLevelNoText();
        tmpScorePlus1.text = scorePlus1.text = "+" + currentLevel;
        currentLevelWorldSpace.text = "LEVEL " + currentLevel;
        reqProgressForNextLevel = currentLevel > 2 ? 20 : 10;
        UpdateBarMaxValue();
    }

    void UpdateBarMaxValue()
    {
        progressBar.maxValue = reqProgressForNextLevel;
    }

    void UpdateLevelNoText()
    {
        currLevelTxt.text = currentLevel.ToString();
        nxtLevelTxt.text = nextLevel.ToString();
    }

    public void UpdateScore(int state)
    {
        switch (state)
        {
            case 0:
                PerfectLand();
                IncreaseProgress();
                break;
            case 1:
                NormalLand();
                IncreaseProgress();
                break;
            case 2:
                GameOver();
                break;
            case 3:
                CompleteRotation();
                break;
        }
    }

    void CompleteRotation()//animation controller set trigger per animacion, mandej reset
    {
        if (tmpScore > 0)
        {
            tmpScore += currentLevel == 0 ? 1 : currentLevel;
            tmpScoreTxt.text = perfectInRow + "X" + tmpScore;
            PlayAnimation.PerfectLandAnim(true);
        }
        else
        {
            score += currentLevel == 0 ? 1 : currentLevel;
            scoreTxt.text = score.ToString();
            PlayAnimation.UpdateScoreAnim();
        }
    }

    void PerfectLand()// 1. txtPerfectLand active animation; 2. if already active updateTmpScore anim
    {
        PlayAnimation.BalloonCollide();
        tmpScore += ++perfectInRow - 1;
        tmpScoreTxt.text = perfectInRow + "X" + tmpScore;
        bool first = perfectInRow <= 2;
        bool dispTxt = (perfectInRow - 2) % 3 == 0;

        if (perfectInRow == 2)
            perfectLandTxt.text = "Perfect!";
        else if (perfectInRow > 8)
            perfectLandTxt.text = "UNBELIVALBE!!!";
        else if (perfectInRow > 5)
            perfectLandTxt.text = "Exellent!";
        else if (perfectInRow > 2)
            perfectLandTxt.text = "Awesome!";

        if (perfectInRow == 2)
        {
            GameElements.EnableOrDisableSmoke(0);
            progressIncreasingValue = 2;
        }
        else if (perfectInRow == 3)
            GameElements.EnableOrDisableSmoke(1);
        else if (perfectInRow > 3)
        {
            //GameElements.EnableOrDisableSmoke(2);
            //TxtColor(txtFire);
            GameElements.GameOnFire(perfectInRow == 4); //game on fire
            progressIncreasingValue = 5;
        }
        if (perfectInRow == 4)
        {
            PlayAnimation.FlashScreen();
            GameElements.EnableOrDisableSmoke(2);
            TxtColor(txtFire);
        }
        updateTmpScoreTxt.text = perfectInRow + "X" + tmpScore;
        
        PlayAnimation.PerfectLandAnim(first, dispTxt);
    }


    void NormalLand()
    {
        progressIncreasingValue = 1;
        PlayAnimation.BallonCollideLow();

        TxtColor(txtNormal);
        GameElements.GameNotOnFire();
        GameElements.EnableOrDisableSmoke(2);


        if (tmpScore > 0)
        {
            tmpScore *= perfectInRow;
            score += tmpScore;
            tmpScoreTxt.text = "+" + tmpScore;
            scoreTxt.text = score.ToString();
            
            PlayAnimation.NormalLandAnim();
            perfectInRow = 1;
        }
    }
    
    void GameOver()
    {
        if (currentLevel <= 2)
        {
            PlayAnimation.WarnForBadLand();
            GameElements.NormalLand();
            GameElements.isResetingPlayer = true;
            ResetPlayer.StartReseting();
            return;
        }
        else if(hasContinuedPlaying == false && diamondsNo >= 200)
        {
            PlayAnimation.GameOverAnim(false, hasContinuedPlaying);
            hasContinuedPlaying = true;
            GameElements.HoldGame(true);
            PlayerPrefs.SetInt("diamonds", diamondsNo);
            goDiamondsNo.text = diamondsNo + "<sprite index=0>";
            return;
        }

        if (currentLevel > PlayerPrefs.GetInt("MaxLevel", 1))
            PlayerPrefs.SetInt("MaxLevel", currentLevel);

        score += (tmpScore * perfectInRow);
        GameElements.SubmitScoreForAnalytics(score);
        PlayerPrefs.SetInt("LastScore", score);
        lastScoreTxt.text = lastScoreTxtGO.text = "Last:\n" + score;
        perfectInRow = 1;
        tmpScore = 0;

        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScoreTxtGO.text = "New Best:\n" + PlayerPrefs.GetInt("HighScore", 0);
            PlayAnimation.GameOverAnim(true, true);
        }
        else
        {
            highScoreTxtGO.text = "Best:\n" + PlayerPrefs.GetInt("HighScore", 0);
            PlayAnimation.GameOverAnim(false, true);
        }

        //if (PlayerPrefs.GetInt("diamonds", 0) < diamondsNo)
            PlayerPrefs.SetInt("diamonds", diamondsNo);

        PlayerPrefs.Save();
        GameElements.GameOver();
    }


    void TxtColor(Color color)
    {
        TextMeshProUGUI [] txts = FindObjectsOfType<TextMeshProUGUI>();
        for (int i = 0; i < txts.Length; i++)
        {
            if (txts[i].tag == "CurrentLevelNo" || txts[i].tag == "NextLevelNo" || txts[i].tag == "DiamondsNo")
                continue;
            txts[i].faceColor = color;
        }
    }

    
    void IncreaseProgress()
    {
        currentProgress += progressIncreasingValue;
        if (currentProgress >= reqProgressForNextLevel)
            NextLevel();
        else 
            progressBar.value = currentProgress;
        //print("bar value:" + progressBar.value + "\ncurrent progres: " + currentProgress);
        //print("Curr Progress: " + currentProgress + "\nreq prog for n level += " + reqProgressForNextLevel);
    }

    private void NextLevel()
    {
        currentProgress -= reqProgressForNextLevel; // qka ka teprue prej progresit te levelit kalum
        progressBar.value = currentProgress;
        currentLevel++; nextLevel++; UpdateLevelNoText();
        if (currentLevel % 3 == 0 && currentLevel <= 60)
        {
            PlayerPrefs.SetInt("perfectAngle1", PlayerPrefs.GetInt("perfectAngle1",15) + 1);
            PlayerPrefs.SetInt("perfectAngle2", PlayerPrefs.GetInt("perfectAngle2", 345) - 1);
            //no need to save, since saving is instantiated in GameOver()
        }
        Colors.ChangeColors();
        tmpScorePlus1.text = scorePlus1.text = "+" + currentLevel;
        PlayAnimation.DisplayLevelUpText();
        GameElements.isResetingPlayer = true;
        ResetPlayer.StartReseting();
        GameElements.spawningHugeCollectible = true;
        GameElements.NextLevelGravity();
        if(reqProgressForNextLevel < 30)
        {
            reqProgressForNextLevel += 5;
            UpdateBarMaxValue();
        }
    }
}
