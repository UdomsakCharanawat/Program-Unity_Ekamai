using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mission : MonoBehaviour
{
    public int stayPointFix = -1;
    public int countPointFix;
    public GameObject[] obj_progressActive;
    public bool inArea = false;

    private Coroutine areaCheckCoroutine;

    public gameManager _gameManager;
    public player _player;

    public float currentTime;
    private bool isCounting;

    public GameObject panel;

    public Animator anim_btHome;

    public void OnEnable()
    {
        panel.SetActive(true);

        stayPointFix = -1;
        countPointFix = 0;

        for (int i = 0; i < obj_progressActive.Length; i++) obj_progressActive[i].SetActive(false);
        inArea = false;


        currentTime = _gameManager.roundTimeInSeconds;
        _gameManager.UpdateRoundTimeText(currentTime);
        PauseTimer();
    }


    public void startPlay()
    {
        _gameManager.obj_roundTime.SetActive(true);
        _gameManager.obj_groupSatelliteStage.SetActive(true);
        ResumeTimer();
    }

    private void FixedUpdate()
    {
        if (!_gameManager.gameOver)
        {
            if (!isCounting || currentTime <= 0f) return;

            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(currentTime, 0f);

            _gameManager.UpdateRoundTimeText(currentTime);

            if (currentTime <= 0f)
            {
                isCounting = false;
                Debug.Log("หมดเวลาแล้ว!");
            }

        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            btHome();
            anim_btHome.SetTrigger("Pressed");
        }
    }



    public void addPointFiexed()
    {
        countPointFix++;
        _gameManager.getCountMission++;

        for (int i = 0; i <= countPointFix - 1; i++)
        {
            obj_progressActive[i].SetActive(true);
        }

        if (countPointFix == obj_progressActive.Length)
        {
            if (!_gameManager.gameOver)
            {
                _gameManager.updateStage();
                nextMission();
            }
        }
    }

    public void nextMission()
    {
        if (_gameManager != null && !_gameManager.blockPress)
        {
            if (int.Parse(this.gameObject.name.Split(' ')[1]) != 5)
            {
                StartCoroutine(_gameManager.waitChangePanel(int.Parse(this.gameObject.name.Split(' ')[1]) + 5, int.Parse(this.gameObject.name.Split(' ')[1]) + 4));
            }
            else
            {
                Debug.Log("done");
                _gameManager.stayPage = "done";
                _gameManager.canvas[11].SetActive(true);
            }
        }
    }



    public void PauseTimer() => isCounting = false;
    public void ResumeTimer() => isCounting = true;
    public void ResetTimer()
    {
        currentTime = _gameManager.roundTimeInSeconds;
        isCounting = true;
        _gameManager.UpdateRoundTimeText(currentTime);
    }



    public void OnPlayerEnterPoint(int index, GameObject player)
    {
        stayPointFix = index;
        
        if (areaCheckCoroutine != null)
            StopCoroutine(areaCheckCoroutine);

        areaCheckCoroutine = StartCoroutine(CheckStayInArea());
    }

    public void OnPlayerExitPoint()
    {
        if (areaCheckCoroutine != null)
            StopCoroutine(areaCheckCoroutine);

        inArea = false;
        stayPointFix = -1;

        _player.useObject(_player.obj_lockPosition, false);
        _player.playerFrontGlow.SetActive(false);
    }

    private IEnumerator CheckStayInArea()
    {
        yield return new WaitForSeconds(1f);
        inArea = true;
        _player.useObject(_player.obj_lockPosition, true);
        _player.playerFrontGlow.SetActive(true);
    }







    public void btHome()
    {
        StartCoroutine(_gameManager.waitChangePanel(0, 5));
    }

}
