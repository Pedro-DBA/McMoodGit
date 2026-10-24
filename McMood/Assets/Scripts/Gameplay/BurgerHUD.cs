using TMPro;
using UnityEngine;

public class BurgerHUD : MonoBehaviour
{
    [SerializeField] private BurgerBuilder builder;
    [SerializeField] private EnergySystem energy;       // o mesmo usado no BurgerBuilder
    [SerializeField] private TMP_Text energyText;       // "1/1MP"
    [SerializeField] private TMP_Text remainingText;    // "1 Ingrediente restante"
    [SerializeField] private TMP_Text instructionText;  // barra de baixo
    [SerializeField] private TMP_Text comboBannerText;  // "Verde no Ponto!"
    [SerializeField] private TMP_Text priceText;        // preço final

    private void OnEnable()
    {
        builder.OnStateChanged += HandleState;
        builder.OnIngredientAdded += HandleIngredient;
        builder.OnComboTriggered += HandleCombo;
        builder.OnFinished += HandleFinished;
        builder.OnNotEnoughEnergy += HandleNoEnergy;
        if (energy != null) energy.OnEnergyChanged += HandleEnergy;

        HandleState(builder.CurrentState);
        if (energy != null) HandleEnergy(energy.Current, energy.Max);
        if (comboBannerText != null) comboBannerText.text = "";
        if (priceText != null) priceText.text = "";
    }

    private void OnDisable()
    {
        builder.OnStateChanged -= HandleState;
        builder.OnIngredientAdded -= HandleIngredient;
        builder.OnComboTriggered -= HandleCombo;
        builder.OnFinished -= HandleFinished;
        builder.OnNotEnoughEnergy -= HandleNoEnergy;
        if (energy != null) energy.OnEnergyChanged -= HandleEnergy;
    }

    private void HandleEnergy(int current, int max)
    {
        if (energyText != null) energyText.text = $"{current}/{max}MP";
        // Se voltou MP enquanto o aviso "Sem MP" estava na tela, restaura a instrução.
        HandleState(builder.CurrentState);
    }

    private void HandleNoEnergy() => instructionText.text = "Sem MP! Aguarde recarregar";

    private void HandleIngredient(IngredientData _) => RefreshRemaining();

    private void RefreshRemaining()
    {
        int n = builder.IngredientsRemaining;
        remainingText.text = n == 1 ? "1 Ingrediente\nrestante" : $"{n} Ingredientes\nrestantes";
    }

    private void HandleState(BurgerBuilder.State s)
    {
        instructionText.text = s switch
        {
            BurgerBuilder.State.SelectingBase => "Selecione uma base",
            BurgerBuilder.State.SelectingIngredients => "Selecione três ingredientes",
            _ => "Lanche pronto!"
        };
        RefreshRemaining();
        if (s == BurgerBuilder.State.SelectingBase)
        {
            if (comboBannerText != null) comboBannerText.text = "";
            if (priceText != null) priceText.text = "";
        }
    }

    private void HandleCombo(ComboData c) => comboBannerText.text = c.comboName;

    private void HandleFinished(int points, int price) => priceText.text = $"R$ {price}";
}