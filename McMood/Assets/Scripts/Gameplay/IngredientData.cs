using UnityEngine;

[CreateAssetMenu(menuName = "McMood/Ingredient", fileName = "Ingredient_")]
public class IngredientData : ScriptableObject
{
    [SerializeField] private string id;            // ex: "hamburguer" (único, minúsculo)
    public string displayName;                     // ex: "Hambúrguer"
    public Sprite listIcon;                        // ícone da lista lateral
    public Sprite plateSprite;                     // camada que aparece empilhada no prato

    [Tooltip("SÓ PARA BASES: pão de cima que fecha o lanche quando nenhuma receita bate.")]
    public Sprite topSprite;

    [Tooltip("Quantos pixels a PRÓXIMA camada sobe depois desta (altura visual do ingrediente).")]
    public float layerHeight = 25f;

    public string Id => string.IsNullOrEmpty(id) ? name : id;
}