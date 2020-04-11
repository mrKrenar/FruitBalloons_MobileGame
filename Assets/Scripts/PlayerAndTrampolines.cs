using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EZCameraShake;
using UnityEngine.UI;
using GameAnalyticsSDK;
using System;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;

public class PlayerAndTrampolines : MonoBehaviour
{
    ResetPlayerRotation ResetPlayer;
    private UiController ui;
    private AnimationController anim;
    //private CameraScript camera;


    GameObject[] allTrampolines;
    public GameObject[] levelTrampolinePrefabs;
    public GameObject balloonPopPrefab, GameOverCanvas, tutorialCanvas, requirePremissionsForNotifications, collectiblesGroup, collectiblesHuge;
    public Button GamePauser, gameStarter;

    public Text gravityValue, playerPos, camSize;

    public GameObject trampolinePrefab, finishLine;
    private GameObject playerRotatingImage;
    private Rigidbody2D rb;

    private Transform target; //trampoline
    public float timeToDestroy = 2f, playerRotateSpeed,swordAngularDrag = 3f, maxGravityValue, torque;

    private float nextTrampolineDistance = 10f, firingAngle = 45.0f, rotation, lerpTime = 0f, tmpGravity, gravity/*touchDelay = 0,*/;
    int perfectAngle1, perfectAngle2;
    private Transform player; //player

    private bool waitBeforeJump, gameOver, readyToUpdate, onFire, stopIdleAnimAndPlay;
    public ParticleSystem fire, smokeLow, smokeHigh;
    ParticleSystem balloonParticles;

    //public Color trampolineNormalColor, trampolineFireColor; 
    private Color lastBgColor, newBgColor, lastTrampolineColor, newTrampolineColor;

    public Sprite fireSword, normalSword;
    private SpriteRenderer bgSprite;
    private bool changeColors, paused;

    public Animator idleTrampolineAnimator;

    [HideInInspector]
    public bool isResetingPlayer, spawningHugeCollectible;

    private void Awake()
    {
        gravity = PlayerPrefs.GetFloat("gravity", 4);
        tmpGravity = gravity;
        print("loaded gravity: " + gravity);
        ResetPlayer = GameObject.Find("GameController").GetComponent<ResetPlayerRotation>();
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        //PlayerPrefs.DeleteAll();
        //gravityValue.text = "gravity: " + tmpGravity;
        //if(SceneManager.GetActiveScene().buildIndex == 0)
        //{
        //    gravityValue.enabled = true;
        //    playerPos.enabled = true;
        //    camSize.enabled = true;
        //}
        //else
        //{
            gravityValue.enabled = false;
            playerPos.enabled = false;
            camSize.enabled = false;
        //}
        
        GameAnalytics.Initialize();
        Time.timeScale = 1;
        gameOver = false; changeColors = false;
        //lastTrampolineColor = trampolineNormalColor;
        target = Instantiate(trampolinePrefab, new Vector3(0,1.5f,0), Quaternion.Euler(0,0,0)).transform;
        target.GetChild(0).gameObject.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerRotatingImage = GameObject.FindGameObjectWithTag("RotatingPlayerImg");
        waitBeforeJump = true;
        rb = playerRotatingImage.GetComponent<Rigidbody2D>();
        //camera = Camera.main.GetComponent<CameraScript>();
        ui = GameObject.Find("GameController").GetComponent<UiController>();
        ui.hasContinuedPlaying = false;
        anim = GameObject.Find("UI").GetComponent<AnimationController>();
        anim.UpdateBalloonAnimator(target.GetComponentInChildren<Animator>());
        //anim.ballonAnimator = target.GetComponentInChildren<Animator>();
        //anim.UpdateBalloonAnimator();
        //targetColor = target.GetComponentInChildren<SpriteRenderer>().color;
        //target.GetComponentInChildren<SpriteRenderer>().color = trampolineNormalColor;
        bgSprite = GameObject.FindGameObjectWithTag("Background").GetComponentInChildren<SpriteRenderer>();
        GamePauser.enabled = true;
        //StartCoroutine(SimulateProjectile());
        perfectAngle1 = PlayerPrefs.GetInt("perfectAngle1", 15);
        perfectAngle2 = PlayerPrefs.GetInt("perfectAngle2", 345);
#if UNITY_IOS
        int askedTimes = PlayerPrefs.GetInt("timesAskedForPremission", 1);
        if (askedTimes != -1)
        {
            if (askedTimes % 5 == 0)
            {
                gameStarter.enabled = false;
                EnableCanvasNotificationPremissions();
            }
            PlayerPrefs.SetInt("timesAskedForPremission", ++askedTimes);
            PlayerPrefs.Save();
        }
        else ScheduleNotifications();
#endif
    }
    public void DontAskForNotifications()
    {
        PlayerPrefs.SetInt("timesAskedForPremission", -1);
    }

    public void RequireNotificationsPremissions()
    {
        DontAskForNotifications();
        NotificationServices.RegisterForNotifications(
        NotificationType.Alert |
        NotificationType.Badge |
        NotificationType.Sound);
    }

    void EnableCanvasNotificationPremissions()
    {
        //if (UnityEngine.iOS.NotificationServices.enabledNotificationTypes == UnityEngine.iOS.NotificationType.None)
        requirePremissionsForNotifications.SetActive(true);
        //else ScheduleNotifications();
    }

    void ScheduleNotifications()
    {
#if UNITY_IOS
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (PlayerPrefs.GetInt("notificationsAreScheduled",0)==0)//UnityEngine.iOS.NotificationServices.localNotificationCount == 0)
            {
                PlayerPrefs.SetInt("notificationsAreScheduled",1);
                PlayerPrefs.Save();
                //UnityEngine.iOS.LocalNotification firstNotif = new UnityEngine.iOS.LocalNotification();
                //if (System.DateTime.Now.Hour < 8)
                //    firstNotif.fireDate = System.DateTime.Now.AddMinutes((8 - System.DateTime.Now.Hour)*60 - System.DateTime.Now.Minute);
                //else if (System.DateTime.Now.Hour == 8)
                //    firstNotif.fireDate = System.DateTime.Now.AddMinutes(24 * 60);
                //else firstNotif.fireDate = System.DateTime.Now.AddMinutes((8 + 24 - System.DateTime.Now.Hour)*60 - System.DateTime.Now.Minute);
                
                ////firstNotif.alertAction = String.Format("Morning smasher! Good or bad day?");
                //firstNotif.alertBody = String.Format("Morning smasher! Good or bad day? Smash some peaches to make your day go right.");
                //firstNotif.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
                //UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(firstNotif);

                //UnityEngine.iOS.LocalNotification secNotif = new UnityEngine.iOS.LocalNotification();
                //if (System.DateTime.Now.Hour < 12)
                //    secNotif.fireDate = System.DateTime.Now.AddMinutes((12 - System.DateTime.Now.Hour)*60 - System.DateTime.Now.Minute);
                //else if (System.DateTime.Now.Hour == 12)
                //    secNotif.fireDate = System.DateTime.Now.AddMinutes(24*60);
                //else secNotif.fireDate = System.DateTime.Now.AddMinutes((12 + 24 - System.DateTime.Now.Hour)*60 - System.DateTime.Now.Minute);
                ////secNotif.alertAction = String.Format("You definitively need a brake smasher");
                //secNotif.alertBody = String.Format("You definitively need a brake smasher. Smashing peaches makes you healthy. Let's smash some now!");
                //secNotif.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
                //UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(secNotif);

                UnityEngine.iOS.LocalNotification thirdNotif = new UnityEngine.iOS.LocalNotification();
                if (System.DateTime.Now.Hour < 17)
                    thirdNotif.fireDate = System.DateTime.Now.AddMinutes((17 - System.DateTime.Now.Hour)* 60 - System.DateTime.Now.Minute);
                else if (System.DateTime.Now.Hour == 17)
                    thirdNotif.fireDate = System.DateTime.Now.AddMinutes(24 * 60);
                else thirdNotif.fireDate = System.DateTime.Now.AddMinutes((17 + 24 - System.DateTime.Now.Hour) * 60 - System.DateTime.Now.Minute);
                //thirdNotif.alertAction = String.Format("You're tired, aren't you?");
                thirdNotif.alertBody = String.Format("You're tired, aren't you? Relax now by smashing some peaches");
                thirdNotif.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
                UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(thirdNotif);

                //UnityEngine.iOS.LocalNotification fourthNotif = new UnityEngine.iOS.LocalNotification();
                //if (System.DateTime.Now.Hour < 22)
                //    fourthNotif.fireDate = System.DateTime.Now.AddMinutes((22 - System.DateTime.Now.Hour)* 60 - System.DateTime.Now.Minute);
                //else if (System.DateTime.Now.Hour == 22)
                //    fourthNotif.fireDate = System.DateTime.Now.AddMinutes(24 * 60);
                //else fourthNotif.fireDate = System.DateTime.Now.AddMinutes((22 + 24 - System.DateTime.Now.Hour)* 60 - System.DateTime.Now.Minute);
                ////fourthNotif.alertAction = String.Format("We're missing you, the coolest peaches smasher");
                //fourthNotif.alertBody = String.Format("We're missing you, the coolest peaches smasher.Wanna make a new record? Try it now!");
                //fourthNotif.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
                //UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(fourthNotif);
            }
        }
#endif

    }

    public void StartGame()
    {
        if(PlayerPrefs.GetInt("tutorialShown",0) == 0)
        {
            tutorialCanvas.SetActive(true);
            PlayerPrefs.SetInt("tutorialShown", 1);
            //print("showing tutorial");
            anim.StartGameAnim();
            return;
        }
        //print("tutorial already shown. starting game");
        if (tutorialCanvas.activeInHierarchy)
            tutorialCanvas.SetActive(false);
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "game");

        //StartCoroutine(SimulateProjectile());
        stopIdleAnimAndPlay = true;
        anim.StartGameAnim();
        Destroy(GameObject.FindGameObjectWithTag("DestroyOnStart"), 10f);
        Destroy(GameObject.FindGameObjectWithTag("InWorldStats"), 10f);
    }

    public void PopBalloons()
    {
        allTrampolines = GameObject.FindGameObjectsWithTag("Trampoline");
        StartCoroutine(PlayPopAnim());
        rb.gravityScale = 1;
        rb.AddForce(new Vector2(0.0002f, 0.0004f), ForceMode2D.Impulse);
    }

    IEnumerator PlayPopAnim()
    {
        foreach (var item in allTrampolines)
        {
            Instantiate(balloonPopPrefab, item.transform.position, Quaternion.Euler(0,0, UnityEngine.Random.Range(0f, 360f)));
            Destroy(item, 0.25f);
            yield return null;
        }
        StopCoroutine(PlayPopAnim());
    }




    private void Disorientate()
    {
        if (rb.angularVelocity > -80f && !gameOver)
        rb.AddTorque(-120f, ForceMode2D.Force);         
    }

    private void UpdateTrampoline() // Instantiate new trampoline and set as target
    {
        if (onFire)
        {
            CameraShaker.Instance.ShakeOnce(4f, 4f, 0.1f, 0.5f);
            target.GetComponentInChildren<ParticleSystem>().Play();
            //if (firstTimeOnFire)
            //{
            //    target.GetComponentInChildren<SpriteRenderer>().color = trampolineFireColor;
            //}
            Destroy(target.gameObject, timeToDestroy);

            target = Instantiate(trampolinePrefab, new Vector3(target.transform.position.x + nextTrampolineDistance, target.transform.position.y, 0), Quaternion.Euler(0, 0, 0)).transform;
            //target.GetComponentInChildren<SpriteRenderer>().color = trampolineFireColor;
            target.GetComponentInChildren<ParticleSystem>().Play();
            anim.UpdateBalloonAnimator(target.GetComponentInChildren<Animator>());
            GetComponentInChildren<SpriteRenderer>().sprite = fireSword;
        }
        else if (!onFire)
        {
            try{target.GetComponentInChildren<ParticleSystem>().Stop();}
            catch (Exception e) { }
            //target.GetComponentInChildren<SpriteRenderer>().color = trampolineNormalColor;
            //anim.BalloonCollide();
            Destroy(target.gameObject, timeToDestroy);
            target = Instantiate(trampolinePrefab, new Vector3(target.transform.position.x + nextTrampolineDistance, target.transform.position.y, 0), Quaternion.Euler(0, 0, 0)).transform;
            //target.GetComponentInChildren<SpriteRenderer>().color = trampolineNormalColor;
            anim.UpdateBalloonAnimator(target.GetComponentInChildren<Animator>());

            GetComponentInChildren<SpriteRenderer>().sprite = normalSword;
        }
    }
    private void UpdateScore()
    {
        int perfectAngle1 = 0, perfectAngle2 = 0;
        if (onFire) {
            if(perfectAngle1 != this.perfectAngle1 + 10)
                perfectAngle1 = this.perfectAngle1 + 10;
            if(perfectAngle2 != this.perfectAngle2 - 10)
                perfectAngle2 = this.perfectAngle2 - 10;
            print("perfectAngle1 " + perfectAngle1 + "\nperfectangle2 " + perfectAngle2+" CurrentAngle"+rotation);
        }
        else { perfectAngle1 = this.perfectAngle1; perfectAngle2 = this.perfectAngle2; }

        if (perfectAngle1 == 0 || perfectAngle2 == 0) Debug.LogError("Perfect Angles are ZERO");
        
        if (rotation <= perfectAngle1 || rotation >= perfectAngle2)
        {
            if (firingAngle < 60)
                firingAngle += 3f;
            if (nextTrampolineDistance < 15)
                nextTrampolineDistance += 2f;
            ui.UpdateScore(0);
            if (gravity > tmpGravity - 3)
                gravity--;
            //print("perfect Land, gravity(change): "+gravity+" tmpgravity "+tmpGravity );
        }
        else if (rotation < 55 || rotation > 305)
        {
            NormalLand();
            //print("normal Land, gravity(change): " + gravity + " tmpgravity " + tmpGravity);
            //print("land");
        }
        else
        {
            ui.UpdateScore(2); 
            //GameOver();
            //print("game over");
        }
    }

    public void NormalLand()
    {
        if (firingAngle != 45f || nextTrampolineDistance != 10)
        {
            firingAngle = 45f;
            nextTrampolineDistance = 10;
        }
        ui.UpdateScore(1);
        gravity = tmpGravity;
    }

    public void NextLevelGravity()
    {
        if (tmpGravity < maxGravityValue)
            tmpGravity += 2;
        if (gravity < tmpGravity - 3)
            gravity = tmpGravity - 3;
        gravityValue.text = "gravity: " + tmpGravity;

        Destroy(Instantiate(finishLine, new Vector2(target.transform.position.x + (nextTrampolineDistance / 2), 7f), Quaternion.identity),5f);
        Destroy(Instantiate(collectiblesHuge, new Vector3(target.transform.position.x + nextTrampolineDistance / 2, 9f), Quaternion.identity),10f);
        
        //print("nextLevelGravity, gravity(change): " + gravity + " tmpgravity " + tmpGravity);

    }

    public void HoldGame(bool hold)
    {
        if (hold)
        {
            rb.angularVelocity = 0;
            gameOver = true;
        }
        else
        {
            gameOver = false;
        }
    }

    public void PauseGame()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            SceneManager.LoadScene(1);
        else SceneManager.LoadScene(0);


        //if (gravityValue.enabled != true)
        //{
        //    gravityValue.enabled = true;
        //    playerPos.enabled = true;
        //    camSize.enabled = true;
        //}
        //else
        //{
        //    gravityValue.enabled = false;
        //    playerPos.enabled = false;
        //    camSize.enabled = false;

        //}


        //paused = true;
        //GamePauser.enabled = false;
        //Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        paused = false;
        GamePauser.enabled = true;
        Time.timeScale = 1;
    }

    public void Restart()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SubmitScoreForAnalytics(int score)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "game", score);
    }

    public void SecondChance()
    {
        ui.hasContinuedPlaying = true;
        waitBeforeJump = true;
        HoldGame(false);//set gameover = false
        //rotation = 50;
        ResetPlayer.StartReseting();
        //UpdateTrampoline();
        StartCoroutine(SimulateProjectile());
        anim.ContinuePlayingAfterGameover();
        ui.IncreaseDiamondsNo(-200);
    }

    public void GameOver()
    {
        //float savedGrav = PlayerPrefs.GetFloat("gravity", 5);
        //if (tmpGravity < savedGrav + 6 && savedGrav >= 8)//e lehteson gravitetin nese nuk e arrin gravitetin e fundit
        //    PlayerPrefs.SetFloat("gravity", savedGrav - 2);
        //else if(tmpGravity > savedGrav + 6 && tmpGravity > 8 && savedGrav > 4)
        //{
        //    if(tmpGravity > 12)//sigurohet qe ne cdo loj graviteti maximal mu load osht 20
        //        PlayerPrefs.SetFloat("gravity", 12);
        //    else PlayerPrefs.SetFloat("gravity", tmpGravity - 4);
        //}
        if (tmpGravity > 8)
            PlayerPrefs.SetFloat("gravity", 8);
        else PlayerPrefs.SetFloat("gravity", 6);

        rb.angularVelocity = 0;
        gameOver = true;

        PopBalloons();
        GamePauser.enabled = false;
        GameOverCanvas.SetActive(true);
        GameObject.FindGameObjectWithTag("CamParent").GetComponent<CameraScript>().PauseCloudsMovement();
        EnableOrDisableSmoke(2);
        PlayerPrefs.Save();
    }
    
    private void FixedUpdate()
    {

        if (Input.GetKey(KeyCode.Mouse0) && !waitBeforeJump && !gameOver && !isResetingPlayer)
        {
            if(rb.angularVelocity > -playerRotateSpeed)
            {
                rb.angularDrag = swordAngularDrag;
                rb.AddTorque(-torque, ForceMode2D.Force);
            }
            else
            {
                rb.angularVelocity = -playerRotateSpeed;
                rb.angularDrag = 0;
            }
            ////rb.angularDrag = 2f;
            ////rb.AddTorque(-torque, ForceMode2D.Force);
            //touchDelay += Time.deltaTime;
            //if (touchDelay < 8 * Time.deltaTime)
            //    rb.angularVelocity = -200;
            //else rb.angularVelocity = -playerRotateSpeed;
        }
        else if (isResetingPlayer)
        {
            rb.angularVelocity = 0;
            //swordAngularDrag = 0;
        }
        //else if (rb.angularDrag != swordAngularDrag)
        //{
        //    rb.angularDrag = swordAngularDrag;
        //}
        else rb.angularDrag = swordAngularDrag;

        rotation = playerRotatingImage.transform.rotation.eulerAngles.z;

        //playerPos.text = "touchtime" + touchDelay;
        //camSize.text = "Cam ortho size = "+ Camera.main.orthographicSize;

        if(waitBeforeJump && stopIdleAnimAndPlay && player.position.y <= 3.4f)
        {
            player.position = new Vector2(player.position.x, 3.3f);
            player.gameObject.GetComponent<Animator>().enabled = false;
            idleTrampolineAnimator.enabled = false;
            stopIdleAnimAndPlay = false;
            StartCoroutine(SimulateProjectile());
        }

    }
    private void Update()//detects 360deg rotation
    {
        if (rotation < 10 || rotation > 350)
        {
            if (readyToUpdate)
            {
                ui.UpdateScore(3);
                readyToUpdate = false;
            }
        }
        else readyToUpdate = true;

        if (changeColors)
        {
            lerpTime += Time.deltaTime;
            bgSprite.color = Color.Lerp(lastBgColor, newBgColor, lerpTime);
            
            if (lerpTime >= 1)
            {
                bgSprite.color = newBgColor;
                changeColors = false;
                lerpTime = 0f;
            }
        }
    }

    public void ChangeColors(int nextTrampoline, Color newBgColor, Color lastBgColor)
    {
        //trampolineNormalColor = newTrampolineColor;
        trampolinePrefab = levelTrampolinePrefabs[nextTrampoline];
        this.newBgColor = newBgColor;
        this.lastBgColor = lastBgColor;
        changeColors = true;
    }

    public void EnableOrDisableSmoke(int state)
    {
        switch (state)
        {
            case 0:
                smokeLow.Play();
                break;
            case 1:
                smokeLow.Stop();
                smokeHigh.Play();
                break;
            case 2:
                smokeHigh.Stop();
                smokeLow.Stop();
                break;
            case 3:
                if (smokeHigh.isEmitting) smokeHigh.Pause();
                if (smokeLow.isEmitting) smokeLow.Pause();
                if (fire.isEmitting) fire.Pause();
                if (target.GetComponentInChildren<ParticleSystem>().isEmitting) target.GetComponentInChildren<ParticleSystem>().Pause();
                break;
        }
    }

    public void GameOnFire(bool firstTime)
    {
        fire.Play();
        onFire = true;

        //firstTimeOnFire = firstTime ? true : false;
        
    }

    public void GameNotOnFire()
    {
        
        if (onFire)
        {
            fire.Stop();
            onFire = false;
        }
    }

    IEnumerator SimulateProjectile()
    {
        if (!gameOver)
        {
            // Short delay added before Projectile is thrown
            if (waitBeforeJump)
            {
                yield return new WaitForSeconds(0.5f);
                UpdateTrampoline();
                //target = Instantiate(trampolinePrefab, new Vector3(target.transform.position.x + 10, target.transform.position.y, 0), Quaternion.Euler(0, 0, 0)).transform;
                waitBeforeJump = false;
            }

            // Move projectile to the position of throwing object + add some offset if needed.
            //Projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);

            // Calculate distance to target
            float target_Distance = Vector3.Distance(player.position, target.position);

            // Calculate the velocity needed to throw the object to the target at specified angle.
            float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

            // Extract the X  Y componenent of the velocity
            float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
            float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
            //print("Vx: "+Vx+"\nVy: "+Vy);
            // Calculate flight time.
            //float flightDuration = target_Distance / Vx;

            // Rotate projectile to face the target.
            //Projectile.rotation = Quaternion.LookRotation(Target.position - Projectile.position);

            float elapse_time = 0;

            while (player.position.y >= 3.3f)
            {
                if (!paused)
                {
                    //if (Time.timeScale != 1) Time.timeScale = 1;
                    playerPos.text = "currTime: " + elapse_time;
                    player.Translate(Vx * Time.fixedDeltaTime, (Vy - (gravity * elapse_time)) * Time.fixedDeltaTime, 0);
                    elapse_time += Time.fixedDeltaTime;    
                }
                //else Time.timeScale = 0;
                yield return null;
            }
            if (isResetingPlayer) isResetingPlayer = false;
            player.position = new Vector2(player.position.x, 3.3f);
            UpdateScore();
            Disorientate();
            if (!spawningHugeCollectible)
                Destroy(Instantiate(collectiblesGroup, new Vector3(target.transform.position.x + nextTrampolineDistance / 2, 10f), Quaternion.identity), 20f);
            else spawningHugeCollectible = false;
            camSize.text = "t2tTime: " + elapse_time;


            StopCoroutine(SimulateProjectile());
            if (!gameOver)
            {
                UpdateTrampoline();
                StartCoroutine(SimulateProjectile());
            }
            //anim.UpdateBalloonAnimator();

        }
        //else StopCoroutine(SimulateProjectile());
    }

}