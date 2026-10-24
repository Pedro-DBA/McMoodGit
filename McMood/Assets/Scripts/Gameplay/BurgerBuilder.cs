using System;
using System.Collections.Generic;
using UnityEngine;

public class BurgerBuilder : MonoBehaviour
{
    public enum State { SelectingBase, SelectingIngredients, Finished }

    [SerializeField] private List<ComboData> combos = new();
    [SerializeField] private List<RecipeData> recipes = new();
    [SerializeField] private EnergySystem energy;

    [Tooltip("Se marcado, o mesmo ingrediente pode ser usado mais de uma vez no mesmo lanche.")]
    [SerializeField] private bool allowRepeatedIngredients = false;

    public event Action<State> OnStateChanged;
    public event Action<IngredientData> OnBaseSelected;
    public event Action<IngredientData> OnIngredientAdded;
    public event Action<ComboData> OnComboTriggered;
    public event Action<RecipeData, bool> OnRecipeDiscovered;
    /// <summary>Lanche terminou sem bater com nenhuma receita. Envia a base (para fechar com o pão de cima).</summary>
    public event Action<IngredientData> OnNoRecipe;
    public event Action<int> OnScoreChanged;
    public event Action<int, int> OnFinished;
    public event Action OnNotEnoughEnergy;

    public State CurrentState { get; private set; } = State.SelectingBase;
    public int Score { get; private set; }
    public int Hearts { get; private set; }
    public int Stars { get; private set; }
    public IngredientData Base { get; private set; }
    public bool AllowRepeatedIngredients => allowRepeatedIngredients;
    public IReadOnlyList<IngredientData> Placed => placed;
    public int IngredientsRemaining => GameRules.IngredientsPerBurger - placed.Count;

    private readonly List<IngredientData> placed = new();
    private readonly HashSet<ComboData> triggered = new();
    private readonly Dictionary<string, RecipeData> recipeByKey = new();

    private void Awake()
    {
        foreach (var r in recipes)
            if (r != null && !recipeByKey.TryAdd(r.Key, r))
                Debug.LogWarning($"Receita duplicada (mesma combinação): {r.name}");
    }

    private void OnEnable() => ResetBuild();

    public void ResetBuild()
    {
        placed.Clear();
        triggered.Clear();
        Base = null;
        Score = 0; Hearts = 0; Stars = 0;
        OnScoreChanged?.Invoke(Score);
        SetState(State.SelectingBase);
    }

    public void SelectBase(IngredientData baseItem)
    {
        if (CurrentState != State.SelectingBase || baseItem == null) return;

        if (energy != null && !energy.TryConsume(1))
        {
            OnNotEnoughEnergy?.Invoke();
            return;
        }

        Base = baseItem;
        OnBaseSelected?.Invoke(baseItem);
        SetState(State.SelectingIngredients);
    }

    public void AddIngredient(IngredientData ing)
    {
        if (CurrentState != State.SelectingIngredients || ing == null) return;
        if (!allowRepeatedIngredients && placed.Contains(ing)) return;

        placed.Add(ing);
        AddScore(GameRules.PointsPerIngredient);
        OnIngredientAdded?.Invoke(ing);

        CheckCombos(ing);

        if (placed.Count >= GameRules.IngredientsPerBurger) Finish();
    }

    private void CheckCombos(IngredientData justPlaced)
    {
        foreach (var combo in combos)
        {
            if (combo == null || triggered.Contains(combo)) continue;
            if (!combo.Contains(justPlaced) || !combo.IsSatisfiedBy(placed)) continue;

            triggered.Add(combo);
            Hearts += combo.hearts;
            AddScore(combo.hearts * GameRules.PointsPerHeart);
            OnComboTriggered?.Invoke(combo);
        }
    }

    private void Finish()
    {
        if (recipeByKey.TryGetValue(RecipeData.MakeKey(placed), out var recipe))
        {
            bool firstTime = !IsDiscovered(recipe);

            Hearts += recipe.hearts;
            AddScore(recipe.hearts * GameRules.PointsPerHeart);

            if (firstTime)
            {
                Stars += recipe.stars;
                AddScore(recipe.stars * GameRules.PointsPerStar);
                MarkDiscovered(recipe);
            }
            OnRecipeDiscovered?.Invoke(recipe, firstTime);
        }
        else
        {
            OnNoRecipe?.Invoke(Base);
        }

        SetState(State.Finished);
        OnFinished?.Invoke(Score, GameRules.CalculatePrice(Score));
    }

    private void AddScore(int amount) { Score += amount; OnScoreChanged?.Invoke(Score); }
    private void SetState(State s) { CurrentState = s; OnStateChanged?.Invoke(s); }

    private static bool IsDiscovered(RecipeData r) => PlayerPrefs.GetInt("recipe_" + r.Id, 0) == 1;
    private static void MarkDiscovered(RecipeData r) { PlayerPrefs.SetInt("recipe_" + r.Id, 1); PlayerPrefs.Save(); }
}