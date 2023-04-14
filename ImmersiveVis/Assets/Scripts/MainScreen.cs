using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainScreen : MonoBehaviour
{
    public ExperienceManager manager;
    public GameObject buttons;
    public GameObject Table;
    public List<Button> WeatherButtons;
    public List<Button> LocationButtons;
    public Button ClearButton;
    public Button ToggleData;
    public TextMeshProUGUI ScaleText;

    public GameObject[] quantities;
    public GameObject[] sizes;

    public bool isShowingDashboard = false;
    public bool shouldShowDashboard = false;
    public bool shouldHideDashboard = false;

    float beginPosition = 0.0f;
    float endPosition = 0.0f;

    public Sprite playSprite;
    public Sprite pauseSprite;
    public Button playPauseButton;
    private bool isPlaying = true;
    // Start is called before the first frame update
    void Start()
    {
        var positionX = buttons.transform.position.x;
        var positionY = buttons.transform.position.y;
        buttons.transform.position = new Vector3(-Screen.width, (Screen.height * 0.9f), buttons.transform.position.z);
        WeatherButtons[0].interactable = false;
        LocationButtons[0].interactable = false;
        beginPosition = buttons.transform.position.x;
        endPosition = Screen.width/6;

        Table.transform.position = new Vector3(-Screen.width - 30, (Screen.height * 0.38f), Table.transform.position.z);
        Table.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(shouldShowDashboard) {
            buttons.transform.Translate(Vector3.right * Time.deltaTime * 1000);
            if(buttons.transform.position.x >= endPosition) {
                isShowingDashboard = true;
                shouldShowDashboard = false;
            }
        }

        if(shouldHideDashboard) {
            buttons.transform.Translate(Vector3.left * Time.deltaTime * 1000);
            if(buttons.transform.position.x <= beginPosition) {
                isShowingDashboard = false;
                shouldHideDashboard = false;
            }
        }
    }

    public void ShowDashboard() {

        if(buttons.transform.position.x >= beginPosition) {
            shouldHideDashboard = true;
        } else {
            FillQuantities();
            FillSize();
            shouldShowDashboard = true;
        }
    }

    public void DidTapOnWeatherButton(int index) {
        isPlaying = true;
        playPauseButton.GetComponent<Image>().sprite = pauseSprite;
        foreach(Button button in WeatherButtons) {
            button.interactable = true;
        }
        WeatherButtons[index].interactable = false;
        manager.DidTapOnWeather(index);

        
        FillQuantities();
        FillSize();
    }

    public void DidTapOnLocationButton(int index) {
        isPlaying = true;
        playPauseButton.GetComponent<Image>().sprite = pauseSprite;
        foreach(Button button in LocationButtons) {
            button.interactable = true;
        }
        LocationButtons[index].interactable = false;
        manager.DidTapOnLocationButton(index);

        FillQuantities();
        FillSize();
    }

    public void DidTapOnClearButton() {

    }

    public void FillQuantities() {
        for(int i = 0; i < this.quantities.Length; i++) {
            var tmp = quantities[i].GetComponent<TextMeshProUGUI>();
            int value = (int)(manager.GetQuantityFor(i) * 100.0f);
            tmp.text = string.Format("{0}%", value);
        }
    }

    public void FillSize() {
        for(int i = 0; i < this.quantities.Length; i++) {
            var tmp = sizes[i].GetComponent<TextMeshProUGUI>();
            float[] minMax = manager.GetSizeFor(i);
            var text = string.Format("[{0}, {1})", minMax[0], minMax[1]);
            tmp.text = text;
        }
    }

    public void SetScale(float value) {
        ScaleText.text = "Rate Multiplier: 15\nScale: " + (int)value + ":1";
    }

    public void PlayPauseAction() {
        isPlaying = !isPlaying;
        if(isPlaying) {
            manager.PlayParticles();
            playPauseButton.GetComponent<Image>().sprite = pauseSprite;
        } else {
            manager.StopParticles();
            playPauseButton.GetComponent<Image>().sprite = playSprite;
        }
    }
}
