using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fireRandom : MonoBehaviour
{
    public GameObject[] droppingObjects;
    private List<int> availableIndices = new List<int>();
    private bool isFirstDrop = true;



    [Header("UI")]
    public float currentTime;
    public float gameDuration = 90f;
    public Text timeText;

    public fireDrone _fireDrone;

    void OnEnable()
    {
        if (GameObject.Find("gameManager").GetComponent<gameManager>()._allData != null)
        {
            currentTime = gameDuration;
            ResetAvailableIndices();
            DeactivateAll();

            StartCoroutine(waitRandomFirst());
        }
    }

    public IEnumerator waitRandomFirst()
    {
        yield return new WaitForSeconds(1f);
        randomDropping();
    }


    private void Update()
    {
        if (_fireDrone.play)
        {
            if (currentTime > 0f)
            {
                currentTime -= Time.deltaTime;
                currentTime = Mathf.Max(currentTime, 0f);

                int minutes = Mathf.FloorToInt(currentTime / 60f);
                int seconds = Mathf.FloorToInt(currentTime % 60f);

                if (timeText != null)
                    timeText.text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                _fireDrone.play = false;
                Debug.Log("game over");
                _fireDrone.statusEndGame = "over";
                _fireDrone.checkEndGame();
            }
        }
    }

    void ResetAvailableIndices()
    {
        availableIndices.Clear();
        for (int i = 0; i < droppingObjects.Length; i++)
        {
            availableIndices.Add(i);
        }
    }

    void DeactivateAll()
    {
        foreach (GameObject obj in droppingObjects)
        {
            obj.SetActive(false);
        }
    }

    public void randomDropping()
    {
        if (availableIndices.Count == 0)
        {
            Debug.Log("ครบทุกตำแหน่งแล้ว รีเซ็ตใหม่");
            ResetAvailableIndices();
            DeactivateAll();
            isFirstDrop = true;
        }

        int selectedIndex;

        if (isFirstDrop)
        {
            // สร้างลิสต์ที่ไม่รวม index 0 และ 1
            List<int> filtered = new List<int>();
            foreach (int i in availableIndices)
            {
                if (i != 0 && i != 1)
                    filtered.Add(i);
            }

            int randIndex = Random.Range(0, filtered.Count);
            selectedIndex = filtered[randIndex];
            availableIndices.Remove(selectedIndex);
            isFirstDrop = false;
        }
        else
        {
            int randIndex = Random.Range(0, availableIndices.Count);
            selectedIndex = availableIndices[randIndex];
            availableIndices.RemoveAt(randIndex);
        }

        droppingObjects[selectedIndex].SetActive(true);
    }
}