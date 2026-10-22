using TMPro;
using UnityEngine;

public class BurgerHUD : MonoBehaviour
{
    [SerializeField] private BurgerBuilder builder;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text remainingText;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private TMP_Text comboBannerText;
    [SerializeField] private TMP_Text priceText;

    private void OnEnable()
    {
        builder.OnStateChanged   += HandleState;
        builder.OnScoreChanged   += HandleScore;
        builder.OnIngredientAdded += _ => RefreshRemaining();
        builder.OnComboTriggered += HandleCombo;
        builder.OnFinished       += HandleFinished;
        HandleState(builder.CurrentState); HandleScore(0); RefreshRemaining();
    }

    private void OnDisable()
    {
        builder.OnStateChanged   -= HandleState;
        builder.OnScoreChanged   -= HandleScore;
        builder.OnComboTriggered -= HandleCombo;
        builder.OnFinished -= HandleFinished;
    }

    private void HandleScore(int s) => scoreText.text = s.ToString("D6");

    private void RefreshRemaining()
    {
        int n = builder.IngredientsRemaining;
        remainingText.text = n == 1 ? "1 Ingrediente\nrestante" : $"{n} Ingredientes\nrestantes";
    }

    private void HandleState(BurgerBuilder.State s) => instructionText.text = s switch
    {
        BurgerBuilder.State.SelectingBase        => "Selecione uma base",
        BurgerBuilder.State.SelectingIngredients => "Selecione três ingredientes",
        _                                        => "Lanche pronto!"
    };

    private void HandleCombo(ComboData c) => comboBannerText.text = c.comboName;

    private void HandleFinished(int points, int price) => priceText.text = $"R$ {price}";
}
