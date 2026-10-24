using TMPro;
using UnityEngine;

/// <summary>
/// Troca a lista lateral: começa mostrando a BASE (título "Base");
/// depois que o pão é escolhido, mostra os INGREDIENTES (título "Ingredientes").
/// </summary>
public class IngredientListSwitcher : MonoBehaviour
{
    [SerializeField] private BurgerBuilder builder;
    [SerializeField] private GameObject baseList;          // agrupa os botões da Base
    [SerializeField] private GameObject ingredientList;    // agrupa os botões dos Ingredientes
    [SerializeField] private TMP_Text titleText;           // o título da caixa ("Base" / "Ingredientes")
    [SerializeField] private string baseTitle = "Base";
    [SerializeField] private string ingredientsTitle = "Ingredientes";

    private void OnEnable()
    {
        builder.OnStateChanged += Refresh;
        Refresh(builder.CurrentState);
    }

    private void OnDisable() => builder.OnStateChanged -= Refresh;

    private void Refresh(BurgerBuilder.State state)
    {
        bool choosingBase = state == BurgerBuilder.State.SelectingBase;

        if (baseList != null)       baseList.SetActive(choosingBase);
        if (ingredientList != null) ingredientList.SetActive(!choosingBase);
        if (titleText != null)      titleText.text = choosingBase ? baseTitle : ingredientsTitle;
    }
}
