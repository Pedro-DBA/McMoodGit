using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Botão da lista lateral. Se for a base (pão), chama SelectBase; senão, AddIngredient.
/// Troca entre o sprite normal e o escurecido conforme o estado do lanche.
/// </summary>
[RequireComponent(typeof(Button), typeof(Image))]
public class IngredientButton : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private BurgerBuilder builder;
    [SerializeField] private BurgerPlateView plateView;   // o mesmo objeto do BurgerBuilder

    [Header("Dados")]
    [SerializeField] private IngredientData ingredient;
    [Tooltip("Marque só no botão do Pão Comum (fica em Lista_Base).")]
    [SerializeField] private bool isBase;

    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite usedSprite;           // versão escurecida

    private Button button;
    private Image image;
    private bool used;   // este ingrediente já foi colocado no lanche atual

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        if (builder != null)
        {
            builder.OnStateChanged    += HandleState;
            builder.OnIngredientAdded += HandleIngredientAdded;
        }
        used = false;
        Refresh();
    }

    private void OnDisable()
    {
        if (builder != null)
        {
            builder.OnStateChanged    -= HandleState;
            builder.OnIngredientAdded -= HandleIngredientAdded;
        }
    }

    private void Update() => Refresh();   // barato; mantém o visual sempre certo (ex: durante a queda)

    private void HandleState(BurgerBuilder.State s)
    {
        if (s == BurgerBuilder.State.SelectingBase) used = false;   // novo lanche
    }

    private void HandleIngredientAdded(IngredientData i)
    {
        if (i == ingredient) used = true;
    }

    private void OnClick()
    {
        if (builder == null || ingredient == null) return;

        if (isBase)
        {
            builder.SelectBase(ingredient);
        }
        else
        {
            if (plateView != null && plateView.IsBusy) return;   // espera a queda terminar
            builder.AddIngredient(ingredient);
        }
    }

    private void Refresh()
    {
        if (builder == null || image == null || button == null) return;

        bool dark;
        bool clickable;

        if (isBase)
        {
            clickable = builder.CurrentState == BurgerBuilder.State.SelectingBase;
            dark = !clickable;
        }
        else
        {
            bool selecting = builder.CurrentState == BurgerBuilder.State.SelectingIngredients;
            bool busy = plateView != null && plateView.IsBusy;

            clickable = selecting && !used && !busy;
            // escurece se já usado, durante a queda, ou quando o lanche acabou
            dark = used
                   || builder.CurrentState == BurgerBuilder.State.Finished
                   || (selecting && busy);
        }

        button.interactable = clickable;

        Sprite target = dark && usedSprite != null ? usedSprite : normalSprite;
        if (target != null && image.sprite != target) image.sprite = target;
    }
}
