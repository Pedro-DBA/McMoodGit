using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "McMood/Recipe", fileName = "Recipe_")]
public class RecipeData : ScriptableObject
{
    [SerializeField] private string id;
    public string recipeName;
    public List<IngredientData> ingredients = new();
    public Sprite normalSprite;
    public Sprite evolvedSprite;
    [Min(0)] public int hearts = 4;
    [Min(0)] public int stars  = 2;

    public string Id  => string.IsNullOrEmpty(id) ? name : id;
    public string Key => MakeKey(ingredients);

    public static string MakeKey(IEnumerable<IngredientData> items) =>
        string.Join("|", items.Select(i => i.Id).OrderBy(x => x, StringComparer.Ordinal));
}
