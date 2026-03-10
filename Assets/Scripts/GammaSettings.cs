using UnityEngine;
using UnityEngine.UI;

public class GammaSettings : MonoBehaviour
{
    [SerializeField] Slider gammaSlider;
    public GameObject gammaObject;
    public CanvasGroup brightness;

    private void Start()
    {
        SetGamma(PlayerPrefs.GetFloat("gamma", 0.5f));
        gammaObject = GameObject.Find("Brightness Panel");
        brightness = gammaObject.GetComponent<CanvasGroup>();
    }

    public void SetGamma(float _value)
    {
        RefreshSlider(_value);
        brightness.alpha = _value;
        PlayerPrefs.SetFloat("gamma", _value);
    }

    public void SetGammaFromSlider()
    {
        SetGamma(gammaSlider.value);
    }

    public void RefreshSlider(float _value)
    {
        gammaSlider.value = _value;
    }
}