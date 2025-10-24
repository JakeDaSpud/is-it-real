using System.Collections;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image loadingScreen;
    [SerializeField] Sprite[] allLoadingScreens;
    [SerializeField] float fadeInDuration = 2f;
    [SerializeField] float holdDuration = 2f;
    [SerializeField] float fadeOutDuration = 4f;
    private Coroutine activeLoadingScreenCoroutine;
    private bool coroutineCancelled = false;

    void OnEnable()
    {
        EventManager.Instance.OnStartLoadingScreen += StartAnimation;
        EventManager.Instance.OnGameOver += ResetLoadingScreen;
    }

    void OnDisable()
    {
        EventManager.Instance.OnStartLoadingScreen -= StartAnimation;
        EventManager.Instance.OnGameOver -= ResetLoadingScreen;
    }

    void Start()
    {
        loadingScreen = transform.Find("Image").GetComponent<UnityEngine.UI.Image>();
        ResetLoadingScreen();
    }

    private void ResetLoadingScreen()
    {
        // Try to stop the coroutine before it can raise FinishLoadingScreen event
        coroutineCancelled = true;

        if (activeLoadingScreenCoroutine != null)
        {
            StopCoroutine(activeLoadingScreenCoroutine);
            activeLoadingScreenCoroutine = null;
        }
        
        loadingScreen.gameObject.SetActive(false);
        Color color = loadingScreen.color;
        color.a = 0f;
        loadingScreen.color = color;
    }

    private void StartAnimation(int dayNumber)
    {
        if (dayNumber == 0)
        {
            EventManager.Instance.RaiseFinishLoadingScreen();
            return;
        }

        ResetLoadingScreen();
        coroutineCancelled = false;

        loadingScreen.sprite = allLoadingScreens[dayNumber - 1];
        EventManager.Instance.RaisePause();
        activeLoadingScreenCoroutine = StartCoroutine(PlayLoadingScreenAnimation());
    }

    [Header("DEBUG - Time Variables")]
    [SerializeField] private float timer;

    IEnumerator PlayLoadingScreenAnimation()
    {
        Color color = loadingScreen.color;
        color.a = 0f;
        loadingScreen.color = color;
        loadingScreen.gameObject.SetActive(true);

        // lerp alpha for fadeInDuration seconds
        timer = 0f;
        while (timer < fadeInDuration)
        {
            if (coroutineCancelled) yield break;
            timer += Time.deltaTime;
            float t = timer / fadeInDuration;
            color.a = Mathf.Lerp(0f, 1f, t);
            loadingScreen.color = color;
            yield return null;
        }

        // wait for holdDuration seconds
        timer = 0f;
        while (timer < holdDuration)
        {
            if (coroutineCancelled) yield break;
            timer += Time.deltaTime;
            yield return null;
        }

        // lerp alpha for fadeOutDuration seconds
        timer = 0f;
        while (timer < fadeOutDuration)
        {
            if (coroutineCancelled) yield break;
            timer += Time.deltaTime;
            float t = timer / fadeOutDuration;
            color.a = Mathf.Lerp(1f, 0f, t);
            loadingScreen.color = color;
            yield return null;
        }

        loadingScreen.gameObject.SetActive(false);

        if (!coroutineCancelled) EventManager.Instance.RaiseFinishLoadingScreen();

        activeLoadingScreenCoroutine = null;
    }
}