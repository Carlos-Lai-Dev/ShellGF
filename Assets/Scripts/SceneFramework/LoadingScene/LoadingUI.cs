using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    private float fillSpeed = 0.1f;
    [SerializeField] private Slider progressSlider;

    private void Start()
    {
        SceneController.LoaderCallback();
        StartCoroutine(nameof(SliderBuffer));
    }
    IEnumerator SliderBuffer()
    {
        float t = 0;
        while (progressSlider.value < progressSlider.maxValue)
        {
            t += Time.deltaTime * fillSpeed;
            progressSlider.value = Mathf.Lerp(progressSlider.value, progressSlider.maxValue, t * SceneController.GetLoadingProgress());
            yield return null;
        }
    }
}
