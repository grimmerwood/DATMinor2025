using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public TextMeshProUGUI waveText; // UI text to show wave announcements
    public float announcementDuration = 2f; // Time the text stays visible
    private int waveNumber = 1;

    public void AnnounceWave()
    {
        StartCoroutine(ShowWaveAnnouncement());
    }

    private IEnumerator ShowWaveAnnouncement()
    {
        if (waveText != null)
        {
            waveText.text = "Round " + waveNumber;
            yield return StartCoroutine(FadeText(waveText, 0f, 1f, 0.5f)); // Fade in
            yield return new WaitForSeconds(announcementDuration); // Stay visible
            yield return StartCoroutine(FadeText(waveText, 1f, 0f, 0.5f)); // Fade out
        }
    }

    private IEnumerator FadeText(TextMeshProUGUI text, float startAlpha, float endAlpha, float duration)
    {
        float time = 0;
        Color color = text.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            text.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        text.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    public int GetCurrentWave()
    {
        return waveNumber;
    }

    public void IncreaseWave()
    {
        waveNumber++;
    }
}

