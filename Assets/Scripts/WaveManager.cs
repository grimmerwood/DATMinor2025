using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public TextMeshProUGUI waveText; // UI text for wave announcement
    public float announcementDuration = 2f;
    private int waveNumber = 1;

    public void AnnounceWave()
    {
        StartCoroutine(ShowWaveAnnouncement());
    }

    private IEnumerator ShowWaveAnnouncement()
    {
        Debug.Log("Showing wave announcement: Round " + waveNumber);

        if (waveText != null)
        {
            waveText.text = "Round " + waveNumber;
            waveText.color = new Color(waveText.color.r, waveText.color.g, waveText.color.b, 1f); // Ensure text is visible
            waveText.enabled = true;

            yield return new WaitForSeconds(announcementDuration);

            waveText.enabled = false; // Hide text after delay
        }
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
