using System.Collections;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image loadingScreen;
    [SerializeField] Sprite[] allLoadingScreens;
    [SerializeField] float fadeInDuration = 2f;
    [SerializeField] float holdDuration = 2f;
    [SerializeField] float fadeOutDuration = 4f;

    void OnEnable()
    {
        EventManager.Instance.OnStartLoadingScreen += StartAnimation;
    }

    void OnDisable()
    {
        EventManager.Instance.OnStartLoadingScreen -= StartAnimation;
    }

    void Start()
    {
        loadingScreen = transform.Find("Image").GetComponent<UnityEngine.UI.Image>();
        Color color = loadingScreen.color;
        color.a = 0f;
        loadingScreen.color = color;
    }

    private void StartAnimation(int dayNumber)
    {
        if (dayNumber == 0) return;
        loadingScreen.sprite = allLoadingScreens[dayNumber - 1];
        EventManager.Instance.RaisePause();
        StartCoroutine(PlayLoadingScreenAnimation());
    }

    IEnumerator PlayLoadingScreenAnimation()
    {
        Color color = loadingScreen.color;
        color.a = 0f;
        loadingScreen.color = color;
        loadingScreen.gameObject.SetActive(true);

        // lerp alpha for fadeInDuration seconds
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeInDuration;
            color.a = Mathf.Lerp(0f, 1f, t);
            loadingScreen.color = color;
            yield return null;
        }

        // wait for holdDuration seconds
        yield return new WaitForSeconds(holdDuration);

        EventManager.Instance.RaisePause();

        // lerp alpha for fadeOutDuration seconds
        timer = 0f;
        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeOutDuration;
            color.a = Mathf.Lerp(1f, 0f, t);
            loadingScreen.color = color;
            yield return null;
        }

        loadingScreen.gameObject.SetActive(false);

        EventManager.Instance.RaiseFinishLoadingScreen();
    }
}