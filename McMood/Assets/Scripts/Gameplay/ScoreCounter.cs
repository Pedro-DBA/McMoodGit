using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Texto de pontos que vai SUBINDO até o valor novo (000000 -> 000520 -> 000721...).
/// Ao começar um lanche novo, volta a zero na hora.
/// O rank escuta este contador, então ele sobe junto com o número na tela.
/// </summary>
public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private BurgerBuilder builder;
    [SerializeField] private TMP_Text scoreText;

    [Header("Velocidade da contagem")]
    [SerializeField] private float pointsPerSecond = 400f;   // velocidade mínima
    [SerializeField] private float maxDuration     = 1.5f;   // contagens grandes aceleram para não passar disso

    public event Action<int> OnDisplayedChanged;              // dispara a cada número novo na tela
    public int Displayed { get; private set; }

    private float shown;     // valor "suave" (decimal)
    private int target;      // pontuação real

    private void OnEnable()
    {
        builder.OnScoreChanged += HandleScore;
        target = builder.Score;
        shown = target;
        SetDisplayed(target, force: true);
    }

    private void OnDisable() => builder.OnScoreChanged -= HandleScore;

    private void HandleScore(int score)
    {
        target = score;
        if (score < Displayed)               // lanche novo / reset: zera na hora, sem contar para trás
        {
            shown = score;
            SetDisplayed(score);
        }
    }

    private void Update()
    {
        if (Displayed == target) return;

        float remaining = target - shown;
        float speed = Mathf.Max(pointsPerSecond, remaining / maxDuration);
        shown = Mathf.MoveTowards(shown, target, speed * Time.unscaledDeltaTime);
        SetDisplayed(Mathf.RoundToInt(shown));
    }

    private void SetDisplayed(int value, bool force = false)
    {
        if (!force && value == Displayed) return;
        Displayed = value;
        if (scoreText != null) scoreText.text = value.ToString("D6");   // 6 dígitos: 000520
        OnDisplayedChanged?.Invoke(value);
    }
}
