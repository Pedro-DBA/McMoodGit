using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Questionário em cartão: o botão inicial abre a 1ª pergunta; clicar em QUALQUER
/// emoji (ou em "Passar") vai para a próxima. Ao acabar, fecha o cartão e dispara onFinished.
/// Coloque este script no Panel_Estoque (que não é o objeto que liga/desliga).
/// </summary>
public class QuestionnairePanel : MonoBehaviour
{
    [Serializable]
    public class Question
    {
        public string title;                  // ex: "Relação com a equipe:"
        [TextArea(2, 4)] public string text;  // ex: "Como é a colaboração entre seus colegas?"
    }

    [Header("Botão que abre o questionário")]
    [SerializeField] private Button startButton;          // ButtonVamosComecar

    [Header("Cartão de perguntas")]
    [Tooltip("Objeto que contém o fundo escuro + cartão. Começa desligado.")]
    [SerializeField] private GameObject cardRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_Text progressText;       // opcional: "1/3"
    [Tooltip("Os 3 botões de emoji (Ruim, Médio, Ótimo). Qualquer um avança.")]
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private Button skipButton;           // opcional: "Passar"

    [Header("Perguntas (edite à vontade)")]
    [SerializeField]
    private List<Question> questions = new()
    {
        new Question { title = "Relação com a equipe:", text = "Como é a colaboração entre seus colegas de trabalho?" },
        new Question { title = "Carga de trabalho:",    text = "Como você sente a quantidade de tarefas que recebe?" },
        new Question { title = "Ambiente:",             text = "Como você avalia o ambiente do seu local de trabalho?" },
    };

    [Header("Ao terminar a última pergunta")]
    [SerializeField] private UnityEvent onFinished;

    /// <summary>Respostas dadas: 0 = primeiro emoji, 1 = segundo, 2 = terceiro, -1 = passou.</summary>
    public IReadOnlyList<int> Answers => answers;
    public int QuestionCount => questions.Count;

    /// <summary>Disparado quando a última pergunta é respondida (usado pela tela de recompensa).</summary>
    public event Action Finished;

    private readonly List<int> answers = new();
    private int index;

    private void Awake()
    {
        if (startButton != null) startButton.onClick.AddListener(Open);

        if (answerButtons != null)
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i] == null) continue;
                int value = i;
                answerButtons[i].onClick.AddListener(() => Answer(value));
            }
        }

        if (skipButton != null) skipButton.onClick.AddListener(() => Answer(-1));
    }

    private void OnEnable()
    {
        if (cardRoot != null) cardRoot.SetActive(false);   // painel reaberto = cartão fechado
    }

    public void Open()
    {
        if (cardRoot == null || questions.Count == 0)
        {
            Debug.LogWarning("QuestionnairePanel: faltam o Card Root ou as perguntas.", this);
            return;
        }

        answers.Clear();
        index = 0;
        cardRoot.SetActive(true);
        ShowCurrent();
    }

    private void Answer(int value)
    {
        if (cardRoot == null || !cardRoot.activeSelf) return;

        answers.Add(value);
        index++;

        if (index >= questions.Count) Finish();
        else ShowCurrent();
    }

    private void ShowCurrent()
    {
        Question q = questions[index];
        if (titleText != null) titleText.text = q.title;
        if (questionText != null) questionText.text = q.text;
        if (progressText != null) progressText.text = $"{index + 1}/{questions.Count}";
    }

    private void Finish()
    {
        cardRoot.SetActive(false);
        Finished?.Invoke();
        onFinished?.Invoke();
    }
}