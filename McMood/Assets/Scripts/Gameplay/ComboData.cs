using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "McMood/Combo", fileName = "Combo_")]
public class ComboData : ScriptableObject
{
    public string comboName;
    public Sprite bannerIcon;
    public List<IngredientData> required = new();
    [Min(1)] public int hearts = 3;

    public bool Contains(IngredientData ing) => required.Contains(ing);
    public bool IsSatisfiedBy(IReadOnlyList<IngredientData> placed) => required.All(placed.Contains);
}
