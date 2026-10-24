using System;
using UnityEngine;

public class EnergySystem : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxEnergy = 1;

    [Tooltip("Hora do dia em que o MP recarrega (0 = meia-noite, 4 = 4h da manhã...).")]
    [SerializeField, Range(0, 23)] private int resetHour = 0;

    [Tooltip("Se marcado, o MP volta ao máximo toda vez que o jogo inicia (útil para demo/testes). " +
             "Desmarque para valer só o reset diário.")]
    [SerializeField] private bool resetOnPlay = true;

    public event Action<int, int> OnEnergyChanged;

    public int Current { get; private set; }
    public int Max => maxEnergy;

    private string lastRefillDay;
    private float nextCheck;

    private const string KeyEnergy = "energy_current";
    private const string KeyDay = "energy_last_day";

    private void Awake()
    {
        if (resetOnPlay)
        {
            Current = maxEnergy;
            lastRefillDay = TodayKey();
            Save();
            return;
        }

        Current = Mathf.Clamp(PlayerPrefs.GetInt(KeyEnergy, maxEnergy), 0, maxEnergy);
        lastRefillDay = PlayerPrefs.GetString(KeyDay, TodayKey());
        CheckDailyRefill();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextCheck) return;
        nextCheck = Time.unscaledTime + 1f;
        CheckDailyRefill();
    }

    private void OnApplicationFocus(bool focus) { if (focus) CheckDailyRefill(); }

    public bool HasEnergy(int amount = 1) => Current >= amount;

    public bool TryConsume(int amount = 1)
    {
        CheckDailyRefill();
        if (Current < amount) return false;

        Current -= amount;
        Save();
        OnEnergyChanged?.Invoke(Current, maxEnergy);
        return true;
    }

    public TimeSpan TimeUntilRefill()
    {
        DateTime now = DateTime.Now;
        DateTime next = now.Date.AddHours(resetHour);
        if (next <= now) next = next.AddDays(1);
        return next - now;
    }

    [ContextMenu("Encher MP (teste)")]
    public void RefillAll()
    {
        Current = maxEnergy;
        Save();
        OnEnergyChanged?.Invoke(Current, maxEnergy);
    }

    [ContextMenu("Simular novo dia (teste)")]
    public void SimulateNewDay()
    {
        lastRefillDay = "00000000";
        CheckDailyRefill();
    }

    private void CheckDailyRefill()
    {
        string today = TodayKey();
        if (today == lastRefillDay) return;

        lastRefillDay = today;
        Current = maxEnergy;
        Save();
        OnEnergyChanged?.Invoke(Current, maxEnergy);
    }

    private string TodayKey() => DateTime.Now.AddHours(-resetHour).ToString("yyyyMMdd");

    private void Save()
    {
        PlayerPrefs.SetString(KeyDay, lastRefillDay);
        PlayerPrefs.SetInt(KeyEnergy, Current);
        PlayerPrefs.Save();
    }
}