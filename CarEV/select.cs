using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class select : MonoBehaviour
{
    public Text tex_presen;
    public bool block_selectPage;

    public GameObject iconCharging;
    public Image barBattery;
    [SerializeField] private Material gradientMaterial;
    [SerializeField] private Color[] startColors;
    [SerializeField] private Color[] endColors;
    [SerializeField] private float transitionSpeed = 2f;
    private Color currentColor1;
    private Color currentColor2;
    private Color targetColor1;
    private Color targetColor2;



    public GameObject panelMap;

    public GameObject panelSiamSquare;
    public GameObject panelMaskMapSiamSquare;

    public GameObject panelPattaya;
    public GameObject panelMaskMapPattaya;

    public GameObject panelChiangMai;
    public GameObject panelMaskMapChiangMai;


    public GameObject panelDone;
    public GameObject panelWarning;
    public GameObject panelWarningExtra;


    public GameObject batteryDone_en;
    public GameObject batteryDone_th;

    public gameManager _gameManager;
    private Coroutine _updateCharging;
    private Coroutine _updateBattery;

    public void OnEnable()
    {
        block_selectPage = false;

        resetPageSelect();

        int index = GetColorIndex(_gameManager.persenBattery);
        SetTargetColors(index);
        currentColor1 = targetColor1;
        currentColor2 = targetColor2;
        gradientMaterial.SetColor("_Color1", currentColor1);
        gradientMaterial.SetColor("_Color2", currentColor2);



        _updateCharging = StartCoroutine(updateCharging());
        _updateBattery = StartCoroutine(updateBattery());
    }
    public void OnDisable()
    {
        if (_updateBattery != null)
        {
            StopCoroutine(_updateCharging);
            StopCoroutine(_updateBattery);
            _updateBattery = null;
            _updateBattery = null;
        }
    }

    private void Update()
    {
        currentColor1 = Color.Lerp(currentColor1, targetColor1, Time.deltaTime * transitionSpeed);
        currentColor2 = Color.Lerp(currentColor2, targetColor2, Time.deltaTime * transitionSpeed);
        gradientMaterial.SetColor("_Color1", currentColor1);
        gradientMaterial.SetColor("_Color2", currentColor2);

    }

    IEnumerator updateCharging()
    {
        float waitCharging = 0;

        while (true)
        {
            barBattery.fillAmount = Mathf.Clamp01(_gameManager.persenBattery / 100f);

            if (_gameManager.ac_charging)
                waitCharging = _gameManager.ac_speedCharging;

            if (_gameManager.dc_charging)
                waitCharging = _gameManager.dc_speedCharging;


            int index = GetColorIndex(_gameManager.persenBattery);
            SetTargetColors(index);




            yield return new WaitForSeconds(waitCharging);

            if (_gameManager.ac_charging || _gameManager.dc_charging)
            {
                iconCharging.SetActive(true);
                _gameManager.persenBattery++;
            }
            else
            {
                iconCharging.SetActive(false);
            }
        }
    }
    private int GetColorIndex(float val)
    {
        if (val < 21f) return 2;
        else if (val < 50f) return 1;
        else return 0;
    }
    private void SetTargetColors(int index)
    {
        index = Mathf.Clamp(index, 0, Mathf.Min(startColors.Length, endColors.Length) - 1);
        targetColor1 = startColors[index];
        targetColor2 = endColors[index];
    }




    IEnumerator updateBattery()
    {
        while (true)
        {
            if (_gameManager.persenBattery > 100)
            {
                _gameManager.persenBattery = 100;
            }
            else if (_gameManager.persenBattery <= 20)
            {
                if (_gameManager.run_ontheway)
                {
                    _gameManager.running = false;

                    if (!_gameManager.ac_charging && !_gameManager.dc_charging)
                    {
                        panelWarning.SetActive(true);
                        panelWarningExtra.SetActive(true);
                    }
                }
            }
            else if (_gameManager.persenBattery >= 21)
            {
                panelWarning.SetActive(false);
                panelWarningExtra.SetActive(false);
            }

            tex_presen.text = _gameManager.persenBattery.ToString();

            yield return new WaitForSeconds(0.1f);
        }
    }



    public void btRouteSelect(int route)
    {
        if (!block_selectPage)
        {
            _gameManager.orderSelectedRoute = route;

            if (route == 0)
            {
                _gameManager.routeSelected = "siamsquare";
                StartCoroutine(waitBlockpressSelect(panelSiamSquare, panelMap));
            }
            else if (route == 1)
            {
                _gameManager.routeSelected = "pattaya";
                StartCoroutine(waitBlockpressSelect(panelPattaya, panelMap));
            }
            else
            {
                _gameManager.routeSelected = "chiangmai";
                StartCoroutine(waitBlockpressSelect(panelChiangMai, panelMap));
            }
        }
    }



    public void btHome()
    {
        if (!block_selectPage)
        {
            if (_gameManager != null && !_gameManager.blockPress)
            {
                StartCoroutine(_gameManager.waitBlockPress(_gameManager.canvas[0], _gameManager.canvas[1]));
            }
        }
    }

    public void btBack()
    {
        if (_gameManager != null && _gameManager.orderSelectedRoute == -1)
        {
            if (!_gameManager.blockPress)
            {
                StartCoroutine(_gameManager.waitBlockPress(_gameManager.canvas[0], _gameManager.canvas[1]));
            }
        }
        else
        {
            if (!block_selectPage)
            {
                StartCoroutine(waitForBtResetPageSelect());
                //resetPageSelect();
            }
        }
    }

    IEnumerator waitForBtResetPageSelect()
    {
        block_selectPage = true;
        yield return new WaitForSeconds(0.5f);
        block_selectPage = false;

        resetPageSelect();
    }



    //IEnumerator waitBlockpressSelect(GameObject open, GameObject close, GameObject mask)
    IEnumerator waitBlockpressSelect(GameObject open, GameObject close)
    {
        block_selectPage = true;

        yield return new WaitForSeconds(0.5f);

        open.SetActive(true);
        close.SetActive(false);
        //mask.GetComponent<RectMask2D>().enabled = false;

        block_selectPage = false;
    }



    public void btAgain()
    {
        resetPageSelect();
    }


    public void resetPageSelect()
    {
        if (!block_selectPage)
        {
            panelMap.SetActive(true);
            panelSiamSquare.SetActive(false);
            panelPattaya.SetActive(false);
            panelChiangMai.SetActive(false);
            panelDone.SetActive(false);
            panelWarning.SetActive(false);
            panelWarningExtra.SetActive(false);

            panelMaskMapSiamSquare.GetComponent<RectMask2D>().enabled = true;
            panelMaskMapPattaya.GetComponent<RectMask2D>().enabled = true;
            panelMaskMapChiangMai.GetComponent<RectMask2D>().enabled = true;

            if (_gameManager.orderSelectedRoute != -1) _gameManager.orderSelectedRoute = -1;

            _gameManager.run_ontheway = false;
            _gameManager.running = false;
        }
    }
}
