using UnityEngine;

/**
 * Attach this script to the CanvasGroup containing the flashbang panel
 */

public class FlashBang : MonoBehaviour
{
    private static FlashBang instance;
    private static bool flashing = false;

    [Range(0.1f, 10f)]
    public float flashDuration = 1.0f;

    [Range(0.1f, 10f)]
    public float fadeDuration = 1.0f;

    private static CanvasGroup CG;
    private float flashTime;
    private float fadeTime;

    public static void Flash()
    {
        instance.StartFlashBang();
    }

    public static void Flash(float flashDuration, float fadeDuration)
    {
        instance.StartFlashBang(flashDuration, fadeDuration);
    }

    private void StartFlashBang()
    {
        CG.alpha = 1;
        flashTime = flashDuration;
        fadeTime = fadeDuration;
        flashing = true;
    }

    private void StartFlashBang(float flashDuration, float fadeDuration)
    {
        CG.alpha = 1;
        flashTime = flashDuration;
        fadeTime = fadeDuration;
        flashing = true;
    }

    private void Start()
    {
        instance = this;
        CG = gameObject.GetComponent<CanvasGroup>();
        flashTime = flashDuration;
    }

    private void Update()
    {
        if (flashing)
        {
            if (flashTime > 0)
            {
                flashTime -= Time.deltaTime;
                return;
            }

            float delta = Time.deltaTime / fadeTime;
            CG.alpha = CG.alpha - delta;
            if (CG.alpha <= 0)
            {
                CG.alpha = 0;
                flashing = false;
            }
        }
    }
}