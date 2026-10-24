using TMPro;
using UnityEngine;

public class BurgerHUD : MonoBehaviour
{
    [SerializeField] private BurgerBuilder builder;
    [SerializeField] private EnergySystem energy;       // o mesmo usado no BurgerBuilder
    [SerializeField] private TMP_Text energyText;       // "1/1MP"
    [SerializeField] private TMP_Text remainingText;    // "1 Ingrediente restante"

    private void OnEnable()
    {
        if (builder == null)
        {
            Debug.LogError("BurgerHUD: o campo Builder está vazio.", this);
            return;
        }

        builder.OnStateChanged += HandleState;
        builder.OnIngredientAdded += HandleIngredient;
        if (energy != null) energy.OnEnergyChanged += HandleEnergy;

        RefreshRemaining();
        if (energy != null) HandleEnergy(energy.Current, energy.Max);
    }

    private void OnDisable()
    {
        if (builder == null) return;

        builder.OnStateChanged -= HandleState;
        builder.OnIngredientAdded -= HandleIngredient;
        if (energy != null) energy.OnEnergyChanged -= HandleEnergy;
    }

    private void HandleEnergy(int current, int max)
    {
        if (energyText != null) energyText.text = $"{current}/{max}MP";
    }

    private void HandleIngredient(IngredientData _) => RefreshRemaining();

    private void HandleState(BurgerBuilder.State _) => RefreshRemaining();

    private void RefreshRemaining()
    {
        if (remainingText == null || builder == null) return;

        int n = builder.IngredientsRemaining;
        remainingText.text = n == 1 ? "1 Ingrediente\nrestante" : $"{n} Ingredientes\nrestantes";
    }
}