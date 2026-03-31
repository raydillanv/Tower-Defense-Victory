using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;

    public void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
    }

    public void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }

}
