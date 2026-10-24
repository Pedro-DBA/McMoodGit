using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mostra o rank (D, C, B, A, S...) conforme a pontuação do lanche sobe.
/// Cada rank tem uma pontuação mínima e um sprite. Tudo editável no Inspector.
/// </summary>
public class RankDisplay : MonoBehaviour
{
    [Serializable]
    public class RankTier
    {
        public string name = "D";
        [Min(0)] public int minPoints;   // a partir de quantos pontos este rank é atingido
        public Sprite sprite;            // sprite da letra
    }

    [SerializeField] private ScoreCounter counter;         // o rank sobe junto com o número na tela
    [SerializeField] private Image rankImage;              // a imagem da letra do rank

    // VALORES DE EXEMPLO: ajuste no Inspector para o balanceamento do jogo.
    [SerializeField]
    private List<RankTier> tiers = new List<RankTier>
    {
        new RankTier { name = "D", minPoints = 0   },
        new RankTier { name = "C", minPoints = 500  },
        new RankTier { name = "B", minPoints = 1000 },
        new RankTier { name = "A", minPoints = 1500 },
    };

    [Header("Animação ao subir de rank")]
    [SerializeField] private float popScale = 1.3f;
    [SerializeField] private float popTime = 0.25f;

    public event Action<RankTier> OnRankChanged;
    public RankTier Current { get; private set; }

    private int currentMinPoints = -1;

    private void OnEnable()
    {
        counter.OnDisplayedChanged += Refresh;
        currentMinPoints = -1;
        Refresh(counter.Displayed);
    }

    private void OnDisable() => counter.OnDisplayedChanged -= Refresh;

    private void Refresh(int score)
    {
        // Pega o rank de maior pontuação mínima que o jogador já alcançou (a ordem da lista não importa).
        RankTier best = null;
        foreach (var tier in tiers)
            if (tier != null && score >= tier.minPoints && (best == null || tier.minPoints > best.minPoints))
                best = tier;

        if (best == null || best == Current) return;

        bool rankedUp = currentMinPoints >= 0 && best.minPoints > currentMinPoints;
        Current = best;
        currentMinPoints = best.minPoints;

        if (rankImage != null && best.sprite != null) rankImage.sprite = best.sprite;
        if (rankedUp && rankImage != null && isActiveAndEnabled)
        {
            StopAllCoroutines();
            StartCoroutine(Pop(rankImage.rectTransform));
        }
        OnRankChanged?.Invoke(best);
    }

    private IEnumerator Pop(RectTransform rt)
    {
        float t = 0f;
        while (t < popTime && rt != null)
        {
            t += Time.unscaledDeltaTime;
            float s = Mathf.Lerp(1f, popScale, Mathf.Sin(Mathf.Clamp01(t / popTime) * Mathf.PI));
            rt.localScale = Vector3.one * s;
            yield return null;
        }
        if (rt != null) rt.localScale = Vector3.one;
    }
}