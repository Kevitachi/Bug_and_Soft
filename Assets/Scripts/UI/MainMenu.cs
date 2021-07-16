using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private GameObject pnlSettings, pnlButtons;
    private CanvasGroup dimmerCG;
    private Button btnPlay, btnSettings, btnApply, btnDiscard, btnQuit;
    private Dropdown dpwResolutions, dpwScreenMode, dpwLanguages;
    private Slider sldFPS;
    private Toggle togVSYNC;

    private List<Dropdown.OptionData> resolutionListDpw;

    bool needToUpdateResolutions,
         needToUpdateScreenMode,
         needToUpdateVSYNC,
         needToUpdateFPS = false;

    public int sliderFPSamount;
    public ScreenOpts screenOpts;

    void OnEnable()
    {
        // Registra los componentes del Canvas (UI) necesarios.
        pnlButtons = GameObject.Find("PanelButtons");
        
        pnlButtons = GameObject.Find("PanelButtons");
        pnlSettings = GameObject.Find("PanelSettings");
        dimmerCG = GameObject.Find("Dimmer").GetComponent<CanvasGroup>();
        btnPlay = GameObject.Find("ButtonPlay").GetComponent<Button>();
        btnSettings = GameObject.Find("ButtonSettings").GetComponent<Button>();
        btnApply = GameObject.Find("ButtonApply").GetComponent<Button>();
        btnDiscard = GameObject.Find("ButtonDiscard").GetComponent<Button>();
        btnQuit = GameObject.Find("ButtonQuit").GetComponent<Button>();
        dpwResolutions = GameObject.Find("DropdownResolutions").GetComponent<Dropdown>();
        dpwScreenMode = GameObject.Find("DropdownScreenMode").GetComponent<Dropdown>();
        dpwLanguages = GameObject.Find("DropdownLanguages").GetComponent<Dropdown>();
        sldFPS = GameObject.Find("SliderFPS").GetComponent<Slider>();
        togVSYNC = GameObject.Find("ToggleVSYNC").GetComponent<Toggle>();

        // DEFAULT 

        // Setea la lista del dpwResolutions con las resoluciones soportadas por
        // el monitor (vease el metodo Start).
        int selectedValue = PopulateResolutions();
        dpwResolutions.options = resolutionListDpw;
        dpwResolutions.value = selectedValue;


        // setea los datos por defecto de FPS
        screenOpts = GameObject.Find("TextFPSLimit").GetComponent<ScreenOpts>();
        SetLimitFPS();
        QualitySettings.vSyncCount = 1;
        sldFPS.interactable = false;
        sliderFPSamount = Screen.currentResolution.refreshRate;
        sldFPS.value = sliderFPSamount; // Para mejor performance setea los fps al refresh rate del monitor
        screenOpts.SetFPSStr(sliderFPSamount);

        // EVENTS
        // Agrega events & callbacks
        btnPlay.onClick.AddListener(() => BtnPlayCallBack());
        btnSettings.onClick.AddListener(() => BtnSettingsCallBack());
        btnApply.onClick.AddListener(() => BtnApplyCallBack());
        btnDiscard.onClick.AddListener(() => CloseSettingsPanel());
        btnQuit.onClick.AddListener(() => BtnQuitCallback());

        // Dropdown resolutions : event
        dpwResolutions.onValueChanged.AddListener(delegate
        {
            needToUpdateResolutions = true;
        });
        // Dropdown screenMode : event
        dpwScreenMode.onValueChanged.AddListener(delegate
        {
            needToUpdateScreenMode = true;
        });

        // Dropdown idiomas : event
        dpwLanguages.onValueChanged.AddListener(delegate
        {
            StartCoroutine(OnLocaleSelected(dpwLanguages.value));
        });

        sldFPS.onValueChanged.AddListener(delegate
        {
            needToUpdateFPS = true;
            sliderFPSamount = (int)sldFPS.value;
            screenOpts.SetFPSStr(sliderFPSamount);

        });

        togVSYNC.onValueChanged.AddListener(delegate
        {
            sldFPS.interactable = !togVSYNC.isOn;
            needToUpdateVSYNC = true;
        });
    }

    /*
     * Setea la lista resolutionListDpw que sera usada por el Dropdown de resoluciones
     * Retorna el indice en el que se encuentra la resolucion que se esta usando en el momento.
     */
    private int PopulateResolutions()
    {
        // Llena la lista de resoluciuones a usar por el Dropdown de res.
        resolutionListDpw = new List<Dropdown.OptionData>();
        Resolution[] resolutions = Screen.resolutions;
        Resolution current = Screen.currentResolution;

        int selectedIndex = 0;
        bool setted = false;
        foreach (var res in resolutions)
        {
            Dropdown.OptionData resData = new Dropdown.OptionData($"{res.width}x{res.height} : {res.refreshRate}");
            resolutionListDpw.Add(resData);

            if (!setted &&
                res.width != current.width ||
                res.height != current.height ||
                res.refreshRate != current.refreshRate)
                selectedIndex++;
            else
                setted = true;
        }

        return selectedIndex;
    }

    private FullScreenMode GetSelectedScreenMode()
    {
        int selected = dpwScreenMode.value;

        return selected == 0 ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
    }
    private void BtnPlayCallBack()
    {
        GameManager.Instance.SetInputEnabled(true);
        GameManager.Instance.SetCameraOffsetX(0f);

        var panelButtonsGroup = pnlButtons.GetComponent<CanvasGroup>();
        var panelSettingsGroup = pnlSettings.GetComponent<CanvasGroup>();
        panelButtonsGroup.interactable = false;
        panelSettingsGroup.interactable = false;

        StartCoroutine(DoFade(panelButtonsGroup, 1, 0, .3f));
    }

    private void BtnSettingsCallBack()
    {
        var panelButtonsGroup = pnlButtons.GetComponent<CanvasGroup>();
        var panelSettingsGroup = pnlSettings.GetComponent<CanvasGroup>();

        panelButtonsGroup.interactable = false;
        panelSettingsGroup.interactable = true;

        StartCoroutine(DoFade(panelButtonsGroup, 1, 0, .3f));
        StartCoroutine(DoFade(panelSettingsGroup, 0, 1, .3f, 1f));
        StartCoroutine(DoFade(dimmerCG, 0, 1, .3f, 1f));
    }

    private void BtnApplyCallBack()
    {
        Dropdown.OptionData resData = resolutionListDpw[dpwResolutions.value];
        string[] subsWidth = resData.text.Split('x');
        string[] subsHeight = subsWidth[1].Split(' ');

        int width = int.Parse(subsWidth[0]);
        int height = int.Parse(subsHeight[0]);
        int refreshRate = int.Parse(subsHeight[2]);

        if (needToUpdateScreenMode)
        {
            Screen.fullScreenMode = GetSelectedScreenMode();
        }

        if (needToUpdateResolutions)
        {
            Debug.Log($"Applying resolution + refresh rate + screenmode ({width}x{height}:{refreshRate} - {GetSelectedScreenMode()})");
            Screen.SetResolution(width, height, GetSelectedScreenMode(), refreshRate);
        }

        if (needToUpdateVSYNC)
        {
            if (togVSYNC.isOn)
            {
                sldFPS.interactable = false;
                SetLimitFPS();
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                sldFPS.interactable = true;
                SetLimitFPS((int)sldFPS.value);
            }
        }

        if (needToUpdateFPS)
        {
            if (!togVSYNC.isOn)
                SetLimitFPS(sliderFPSamount);
        }

        CloseSettingsPanel();
    }

    private void CloseSettingsPanel()
    {
        var panelButtonsGroup = pnlButtons.GetComponent<CanvasGroup>();
        var panelSettingsGroup = pnlSettings.GetComponent<CanvasGroup>();

        panelButtonsGroup.interactable = true;
        panelSettingsGroup.interactable = false;

        StartCoroutine(DoFade(panelButtonsGroup, 0, 1, .3f, 1f));
        StartCoroutine(DoFade(dimmerCG, 1, 0, .3f, 1f));
        StartCoroutine(DoFade(panelSettingsGroup, 1, 0, .3f));
    }

    private void BtnQuitCallback()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }

    /// <summary>
    /// Cambia el locale actual del juego.
    /// </summary>
    /// <param name="index">0 = en | 1 = es | 2 = it</param>
    /// <returns></returns>
    public IEnumerator OnLocaleSelected(int index)
    {
        Debug.Log("Locale seleccionado = " + index);
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    private void SetLimitFPS(int newFPS = 60)
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = newFPS;
    }

    public IEnumerator DoFade(CanvasGroup cg, float start, float end, float duration, float delay = 0.0f)
    {
        float counter = 0f;

        if (delay > 0.0f)
            yield return new WaitForSeconds(delay);

        while (counter < duration)
        {
            counter += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, counter / duration);

            yield return null;
        }
    }
}
