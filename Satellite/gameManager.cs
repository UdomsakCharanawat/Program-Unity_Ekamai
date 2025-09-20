using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public bool test;
    public bool gameOver;
    public bool blockPress;
    public string language;

    private float temp_roundTimeInSeconds;
    public float roundTimeInSeconds = 0f;
    public int stage;
    public int getCountMission;

    private string filePath;
    public Text text_roundTime;
    public GameObject obj_roundTime;
    public GameObject obj_groupSatelliteStage;
    public GameObject[] obj_pointStageActive;

    public string stayPage;

    public Color[] colorBt;

    public RawImage[] raw_colorBtZ;
    public RawImage[] raw_colorBtX;
    public RawImage[] raw_colorBtC;


    public GameObject[] canvas;

    public GameObject[] obj_show;
    public GameObject[] obj_hide;

    public void OnEnable()
    {
        filePath = Path.Combine(Application.dataPath, "../setting/setting.txt");

        if (!File.Exists(filePath))
        {
            CreateDefaultSettingsFile();
        }

        LoadSettings();


        if (!test) setFirst();
    }



    public void updateStage()
    {
        stage++;
        obj_pointStageActive[stage - 1].SetActive(true);
    }
    public void resetState()
    {

        obj_roundTime.SetActive(false);
        obj_groupSatelliteStage.SetActive(false);

        roundTimeInSeconds = temp_roundTimeInSeconds;
        UpdateRoundTimeText(roundTimeInSeconds);

        gameOver = false;
        getCountMission = 0;
        stage = 0;

        for(int i = 0; i < obj_pointStageActive.Length; i++)
        {
            obj_pointStageActive[stage].SetActive(false);
        }


        setFirst();
    }

    public void playAgain()
    {
        resetState();
        stayPage = "intro";


        for(int i = 0;i < canvas.Length; i++)
            canvas[i].SetActive(false);

        canvas[5].SetActive(true);
    }







    void CreateDefaultSettingsFile()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        string defaultContent = "round time:3:00min.";
        File.WriteAllText(filePath, defaultContent);
        Debug.Log("สร้างไฟล์ setting.txt พร้อมค่าเริ่มต้นแล้ว");
    }

    void LoadSettings()
    {
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (line.StartsWith("round time:"))
            {
                string timeText = line.Replace("round time:", "").Trim();
                temp_roundTimeInSeconds = ParseTimeToSeconds(timeText);
                roundTimeInSeconds = temp_roundTimeInSeconds;
                Debug.Log($"เวลาในรอบ: {roundTimeInSeconds} วินาที");
            }
        }
    }

    float ParseTimeToSeconds(string timeText)
    {
        timeText = timeText.ToLower().Replace("min.", "").Replace("min", "").Replace("sec", "").Trim();

        if (timeText.Contains(":"))
        {
            string[] parts = timeText.Split(':');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int minutes) &&
                int.TryParse(parts[1], out int seconds))
            {
                return minutes * 60f + seconds;
            }
        }
        else if (float.TryParse(timeText, out float value))
        {
            return value * 60f;
        }

        Debug.LogWarning("ไม่สามารถแปลงเวลาได้จาก: " + timeText);
        return 0f;
    }

    public void UpdateRoundTimeText(float time)
    {

        roundTimeInSeconds = time;
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        text_roundTime.text = $"{minutes:00}:{seconds:00}";


        if (roundTimeInSeconds <= 0f)
        {
            stayPage = "time up";

            gameOver = true;

            canvas[10].SetActive(true);
        }
    }






    public void setFirst()
    {
        for (int i = 0; i < obj_hide.Length; i++)
        {
            obj_hide[i].SetActive(false);
        }
        for (int i = 0; i < obj_show.Length; i++)
        {
            obj_show[i].SetActive(true);
        }



        for (int i = 0; i < raw_colorBtZ.Length; i++)
        {
            raw_colorBtZ[i].color = colorBt[0];
        }
        for (int i = 0; i < raw_colorBtX.Length; i++)
        {
            raw_colorBtX[i].color = colorBt[1];
        }
        for (int i = 0; i < raw_colorBtC.Length; i++)
        {
            raw_colorBtC[i].color = colorBt[2];
        }
    }

    public IEnumerator waitChangePanel(int open, int close)
    {
        blockPress = true;

        yield return new WaitForSeconds(0.5f);

        blockPress = false;

        canvas[open].SetActive(true);
        canvas[close].SetActive(false);

    }
}
