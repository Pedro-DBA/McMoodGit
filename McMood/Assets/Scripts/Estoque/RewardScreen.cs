using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Tela "Você ganha...": aparece quando o questionário termina, mostra quantas
/// perguntas foram respondidas e faz os ingredientes caírem um por vez.
/// Coloque este script no Panel_Estoque (o mesmo do QuestionnairePanel).
/// </summary>
public class RewardScreen : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private QuestionnairePanel questionnaire;
    [Tooltip("Objeto da tela inteira (fundo roxo + textos + cesta + ingredientes). Começa desligado.")]
    [SerializeField] private GameObject screenRoot;

    [Header("Textos")]
    [Tooltip("\"Você respondeu 3 de 3 perguntas\" (os números são preenchidos pelo script).")]
    [SerializeField] private TMP_Text summaryText;
    [Tooltip("\"3 novos ingredientes!\" (opcional; o número vem da lista abaixo).")]
    [SerializeField] private TMP_Text rewardCountText;

    [Header("Ingredientes que caem (na posição final desejada)")]
    [SerializeField] private RectTransform[] rewardItems;

    [Header("Saída")]
    [Tooltip("Botão para fechar a tela. Pode ser um Button no fundo inteiro (toque em qualquer lugar).")]
    [SerializeField] private Button closeButton;
    [SerializeField] private UnityEvent onClosed;

    [Header("Tempos")]
    [SerializeField] private float startDelay = 0.4f;
    [SerializeField] private float stagger = 0.25f;     // intervalo entre um ingrediente e outro
    [SerializeField] private float dropHeight = 450f;
    [SerializeField] private float dropTime = 0.35f;

    private Vector2[] finalPositions;

    private void Awake()
    {
        finalPositions = new Vector2[rewardItems != null ? rewardItems.Length : 0];
        for (int i = 0; i < finalPositions.Length; i++)
            if (rewardItems[i] != null) finalPositions[i] = rewardItems[i].anchoredPosition;

        if (closeButton != null) closeButton.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        if (screenRoot != null) screenRoot.SetActive(false);   // painel reaberto = tela fechada
        if (questionnaire != null) questionnaire.Finished += Show;
        else Debug.LogWarning("RewardScreen: o campo Questionnaire está vazio.", this);
    }

    private void OnDisable()
    {
        if (questionnaire != null) questionnaire.Finished -= Show;
    }

    public void Show()
    {
        if (screenRoot == null) return;

        StopAllCoroutines();
        screenRoot.SetActive(true);

        int total = questionnaire != null ? questionnaire.QuestionCount : 0;
        int answered = questionnaire != null ? questionnaire.Answers.Count(a => a >= 0) : 0;
        if (summaryText != null)
            summaryText.text = $"Você respondeu <b>{answered}</b> de <b>{total}</b> perguntas";

        int items = rewardItems != null ? rewardItems.Count(r => r != null) : 0;
        if (rewardCountText != null)
            rewardCountText.text = items == 1 ? "1 novo ingrediente!" : $"{items} novos ingredientes!";

        // esconde todos e deixa cair um por vez
        for (int i = 0; i < finalPositions.Length; i++)
            if (rewardItems[i] != null) rewardItems[i].gameObject.SetActive(false);

        StartCoroutine(PlayRoutine());
    }

    public void Close()
    {
        StopAllCoroutines();
        if (screenRoot != null) screenRoot.SetActive(false);
        onClosed?.Invoke();
    }

    private IEnumerator PlayRoutine()
    {
        yield return new WaitForSecondsRealtime(startDelay);

        for (int i = 0; i < finalPositions.Length; i++)
        {
            if (rewardItems[i] == null) continue;
            StartCoroutine(DropItem(rewardItems[i], finalPositions[i]));
            yield return new WaitForSecondsRealtime(stagger);
        }
    }

    private IEnumerator DropItem(RectTransform rt, Vector2 end)
    {
        rt.gameObject.SetActive(true);
        rt.localScale = Vector3.one;

        Vector2 start = end + Vector2.up * dropHeight;
        float t = 0f;
        while (t < dropTime && rt != null)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / dropTime);
            float eased = 1f - (1f - k) * (1f - k);          // desacelera ao chegar
            rt.anchoredPosition = Vector2.LerpUnclamped(start, end, eased);
            yield return null;
        }
        if (rt == null) yield break;
        rt.anchoredPosition = end;

        // pequeno "pop" ao chegar
        t = 0f;
        const float popTime = 0.15f;
        while (t < popTime && rt != null)
        {
            t += Time.unscaledDeltaTime;
            float s = Mathf.Lerp(1f, 1.15f, Mathf.Sin(Mathf.Clamp01(t / popTime) * Mathf.PI));
            rt.localScale = Vector3.one * s;
            yield return null;
        }
        if (rt != null) rt.localScale = Vector3.one;
    }
}
