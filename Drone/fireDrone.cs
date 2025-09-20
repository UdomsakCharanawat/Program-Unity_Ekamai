using NUnit.Framework.Internal.Execution;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class fireDrone : MonoBehaviour
{
    public bool play;
    public bool blockPlay;
    public int getCapture;
    public string statusEndGame;

    public GameObject[] progress;

    [Header("how to play")]
    public float[] pos_slide;
    public int current_slide;
    public Transform groupHowToPlay_slide;
    public Transform ts_pointSlideActive;
    public Transform[] ts_pointSlide;
    private Coroutine slideCoroutine;

    public GameObject obj_btNext;
    public GameObject obj_btStart;

    public GameObject gp_howtoplay;
    public GameObject gp_play;
    public GameObject gp_stop;

    public GameObject popupMissionComplete;
    public GameObject popupTimeup;
    public GameObject popupEndMission;


    public GameObject panel_tutorial;
    public GameObject panel_play;
    public GameObject obj_dronePlayer;
    public GameObject gp_dropping;


    public gameManager _gameManager;


    public Animator anim_btHome;
    public Animator anim_btNext;
    public Animator anim_btStart;
    public Animator anim_btGo;

    public void OnEnable()
    {
        if (_gameManager != null)
        {
            _gameManager.stayPage = "fire drone";
        }
        resetPanel();
        resetSlide();
    }

    private void resetPanel()
    {
        play = false;
        blockPlay = true;
        getCapture = 0;
        statusEndGame = "";

        gp_howtoplay.SetActive(true);
        gp_play.SetActive(false);
        gp_stop.SetActive(false);

        popupMissionComplete.SetActive(false);
        popupTimeup.SetActive(false);
        popupEndMission.SetActive(false);

        panel_tutorial.SetActive(true);
        panel_play.SetActive(false);

        obj_dronePlayer.SetActive(false);

        gp_dropping.SetActive(false);


        for(int i  = 0; i < progress.Length; i++)
            progress[i].SetActive(false);
    }

    IEnumerator waitUnlockBlockPlay()
    
        yield return new WaitForSeconds(2f);

        blockPlay = false;
    }


    private void Update()
    {
        if (!_gameManager.blockPress)
        {
            if (_gameManager.stayPage == "fire drone")
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    btSlide("prev");
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    btSlide("next");
                }
                else if (Input.GetKeyDown(KeyCode.V))
                {
                    if (current_slide < 2)
                    {
                        btSlide("next");
                        anim_btNext.SetTrigger("Pressed");
                        //Debug.Log("next : " + current_slide);
                    }
                    else
                    {
                        //Debug.Log("start");
                        StartCoroutine(waitPlayGame());

                        anim_btStart.SetTrigger("Pressed");

                        StartCoroutine(waitUnlockBlockPlay());
                    }

                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    _gameManager.changePage(0, 5);
                    anim_btHome.SetTrigger("Pressed");
                }
            }
            else if (_gameManager.stayPage == "fire drone play")
            {
                //if (Input.GetKeyDown(KeyCode.A))
                //{
                //    Debug.Log("move left");
                //}
                //else if (Input.GetKeyDown(KeyCode.D))
                //{
                //    Debug.Log("move right");
                //}
                //else if (Input.GetKeyDown(KeyCode.W))
                //{
                //    Debug.Log("move front");
                //}
                //else if (Input.GetKeyDown(KeyCode.S))
                //{
                //    Debug.Log("move back");
                //}
                //else if (Input.GetKeyDown(KeyCode.Q))
                //{
                //    Debug.Log("press front");
                //}
                //else if (Input.GetKeyDown(KeyCode.E))
                //{
                //    Debug.Log("press top");
                //}
                if (Input.GetKeyDown(KeyCode.V))
                {
                    if (!blockPlay)
                    {
                        Debug.Log("game start");
                        gameStart();
                        anim_btGo.SetTrigger("Pressed");
                    }
                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    _gameManager.changePage(0, 5);
                    anim_btHome.SetTrigger("Pressed");
                }
            }
        }

    }


    private void gameStart()
    {
        panel_tutorial.SetActive(false);

        obj_dronePlayer.SetActive(true);
        gp_dropping.SetActive(true);

        //StartCoroutine(gp_enemy.GetComponent<enemyMilitarySpawner>().SpawnRoutine());

        play = true;
    }



    public void checkEndGame()
    {
        gp_stop.SetActive(true);

        _gameManager._allData.score = getCapture;
        _gameManager._allData.lastPage = 5;

        if (statusEndGame == "win")
            popupMissionComplete.SetActive(true);
        else if(statusEndGame == "over")
            popupTimeup.SetActive(true);

        StartCoroutine(waitpopupEndMission());
    }

    IEnumerator waitpopupEndMission()
    {
        yield return new WaitForSeconds(5f);

        popupMissionComplete.SetActive(false);
        popupTimeup.SetActive(false);

        if (getCapture <= 0)
        {
            _gameManager.changePage(6, 5);

            StopCoroutine(waitpopupEndMission());
        }
        else
        {
            popupEndMission.SetActive(true);
        }

        yield return new WaitForSeconds(5f);

        popupEndMission.SetActive(false);

        _gameManager.changePage(6, 5);
    }





    IEnumerator waitPlayGame()
    {
        _gameManager.blockPress = true;

        _gameManager.stayPage = "fire drone play";
        yield return new WaitForSeconds(0.5f);


        _gameManager.blockPress = false;

        gp_howtoplay.SetActive(false);
        gp_play.SetActive(true);


        panel_tutorial.SetActive(true);
        panel_play.SetActive(true);
    }







    #region control slide
    public void resetSlide()
    {
        current_slide = 0;

        obj_btNext.SetActive(true);
        obj_btStart.SetActive(false);
        //Debug.Log(obj_btStart.activeInHierarchy);

        UpdatePositionHowToPlay_Slide();
    }
    public void btSlide(string direction)
    {
        if (!_gameManager.blockPress)
        {
            if (direction == "next")
            {
                if (current_slide < pos_slide.Length - 1)
                {
                    current_slide++;
                    UpdatePositionHowToPlay_Slide();
                }

                if (current_slide >= pos_slide.Length - 1)
                {
                    obj_btNext.SetActive(false);
                    obj_btStart.SetActive(true);
                }
            }
            else
            {
                if (current_slide > 0)
                {
                    current_slide--;
                    UpdatePositionHowToPlay_Slide();

                    obj_btNext.SetActive(true);
                    obj_btStart.SetActive(false);
                }

            }
        }
    }
    void UpdatePositionHowToPlay_Slide()
    {
        //_gameManager.blockPress = true;

        ts_pointSlideActive.transform.localPosition = ts_pointSlide[current_slide].transform.localPosition;

        if (slideCoroutine != null)
            StopCoroutine(slideCoroutine);

        slideCoroutine = StartCoroutine(SmoothSlide(pos_slide[current_slide]));
    }
    IEnumerator SmoothSlide(float targetX)
    {
        float duration = 0.2f; // ระยะเวลาในการเลื่อน
        float elapsed = 0f;

        Vector3 startPos = groupHowToPlay_slide.localPosition;
        Vector3 endPos = new Vector3(targetX, startPos.y, startPos.z);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            groupHowToPlay_slide.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        groupHowToPlay_slide.localPosition = endPos;

        _gameManager.blockPress = false;
    }
    #endregion control slide

}
