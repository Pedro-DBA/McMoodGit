using UnityEngine;

[CreateAssetMenu(menuName = "McMood/Ingredient", fileName = "Ingredient_")]
public class IngredientData : ScriptableObject
{
    [SerializeField] private string id;            // ex: "hamburguer" (único, minúsculo)
    public string displayName;                     // ex: "Hambúrguer"
    public Sprite listIcon;                        // ícone da lista lateral
    public Sprite plateSprite;                     // camada que aparece empilhada no prato

    public string Id => string.IsNullOrEmpty(id) ? name : id;
}
