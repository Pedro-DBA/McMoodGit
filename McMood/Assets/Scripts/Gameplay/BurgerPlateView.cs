using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Parte VISUAL do prato: empilha as camadas, fecha com o pão de cima,
/// troca pelo lanche evoluído (com piscada) e solta corações e estrelas.
/// Só escuta os eventos do BurgerBuilder.
/// </summary>
public class BurgerPlateView : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private BurgerBuilder builder;
    [SerializeField] private RectTransform stackRoot;   // onde as camadas nascem (centro do prato)
    [SerializeField] private RectTransform fxRoot;      // onde corações/estrelas nascem

    [Header("Prato")]
    [Tooltip("Altura (em pixels) onde a base (pão) fica sobre o prato.")]
    [SerializeField] private float firstLayerY = 0f;
    [Tooltip("Ajuste fino da altura do pão de cima (positivo sobe, negativo desce).")]
    [SerializeField] private float topBunOffsetY = 0f;

    [Header("Efeitos")]
    [SerializeField] private GameObject heartPrefab;    // UI Image de coração
    [SerializeField] private GameObject starPrefab;     // UI Image de estrela
    [SerializeField] private Vector2 heartArea = new Vector2(140f, 25f); // meia-largura/altura da área dos corações
    [SerializeField] private Vector2 starArea = new Vector2(60f, 20f);
    [SerializeField] private float starHeight = 130f;  // estrelas nascem acima do lanche
    [SerializeField] private float fxRise = 70f;   // quanto sobem antes de sumir
    [SerializeField] private float fxDuration = 1.1f;

    [Header("Tempos")]
    [SerializeField] private float dropHeight = 250f;  // de que altura a camada cai
    [SerializeField] private float dropTime = 0.22f;
    [SerializeField] private float evolveDelay = 0.7f;  // pausa antes de virar o lanche bonito
    [SerializeField] private int flashCount = 2;     // quantas vezes pisca do branco para o normal
    [SerializeField] private float flashTime = 0.1f;  // duração de cada fase da piscada

    private readonly List<GameObject> layers = new();
    private float currentY;
    private int dropsInProgress;

    /// <summary>True enquanto alguma camada ainda está caindo (os botões usam isso para escurecer).</summary>
    public bool IsBusy => dropsInProgress > 0;

    // ---------- Ligação com o BurgerBuilder ----------
    private void OnEnable()
    {
        if (builder == null)
        {
            Debug.LogError("BurgerPlateView: o campo Builder está vazio.", this);
            return;
        }

        builder.OnStateChanged += HandleState;
        builder.OnBaseSelected += HandleBase;
        builder.OnIngredientAdded += HandleIngredient;
        builder.OnComboTriggered += HandleCombo;
        builder.OnRecipeDiscovered += HandleRecipe;
        builder.OnNoRecipe += HandleNoRecipe;
        ClearAll();
    }

    private void OnDisable()
    {
        if (builder == null) return;

        builder.OnStateChanged -= HandleState;
        builder.OnBaseSelected -= HandleBase;
        builder.OnIngredientAdded -= HandleIngredient;
        builder.OnComboTriggered -= HandleCombo;
        builder.OnRecipeDiscovered -= HandleRecipe;
        builder.OnNoRecipe -= HandleNoRecipe;
    }

    private void HandleState(BurgerBuilder.State s)
    {
        if (s == BurgerBuilder.State.SelectingBase) ClearAll();   // novo lanche = prato limpo
    }

    private void HandleBase(IngredientData b)
    {
        currentY = firstLayerY;
        AddLayer(b);
    }

    private void HandleIngredient(IngredientData i) => AddLayer(i);

    private void HandleCombo(ComboData c) =>
        StartCoroutine(SpawnFx(heartPrefab, c.hearts, 0.2f, heartArea, 0f));

    private void HandleRecipe(RecipeData r, bool firstTime) =>
        StartCoroutine(EvolveRoutine(r, firstTime));

    private void HandleNoRecipe(IngredientData baseItem) =>
        StartCoroutine(CloseBurgerRoutine(baseItem));

    // ---------- Camadas ----------
    private void AddLayer(IngredientData ing)
    {
        if (ing == null || ing.plateSprite == null)
        {
            Debug.LogWarning($"Ingrediente sem Plate Sprite: {(ing != null ? ing.name : "null")}");
            return;
        }

        RectTransform rt = CreateImage(ing.displayName, ing.plateSprite);
        layers.Add(rt.gameObject);

        float y = currentY;
        currentY += ing.layerHeight;          // a próxima camada começa mais acima
        StartCoroutine(DropIn(rt, y));
    }

    private RectTransform CreateImage(string objName, Sprite sprite)
    {
        var go = new GameObject(objName, typeof(RectTransform), typeof(Image));
        var rt = (RectTransform)go.transform;
        rt.SetParent(stackRoot, false);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0f);      // pivô embaixo: a camada "senta" na altura Y

        var img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        img.raycastTarget = false;
        img.SetNativeSize();
        return rt;
    }

    private IEnumerator DropIn(RectTransform rt, float targetY)
    {
        dropsInProgress++;

        Vector2 start = new Vector2(0f, targetY + dropHeight);
        Vector2 end = new Vector2(0f, targetY);
        float t = 0f;
        while (t < dropTime && rt != null)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / dropTime);
            float eased = 1f - (1f - k) * (1f - k);      // desacelera ao chegar
            rt.anchoredPosition = Vector2.LerpUnclamped(start, end, eased);
            yield return null;
        }

        dropsInProgress--;
        if (rt == null) yield break;
        rt.anchoredPosition = end;
        yield return Pop(rt, 1.08f, 0.12f);
    }

    // ---------- Lanche sem receita: o pão de cima desce e fecha ----------
    private IEnumerator CloseBurgerRoutine(IngredientData baseItem)
    {
        if (baseItem == null || baseItem.topSprite == null)
        {
            Debug.LogWarning($"A base não tem Top Sprite, o lanche não fecha: {(baseItem != null ? baseItem.name : "null")}");
            yield break;
        }

        while (IsBusy) yield return null;                 // espera o último ingrediente assentar
        yield return new WaitForSecondsRealtime(0.1f);

        RectTransform rt = CreateImage("Pão de cima", baseItem.topSprite);
        layers.Add(rt.gameObject);
        yield return DropIn(rt, currentY + topBunOffsetY);
    }

    // ---------- Receita descoberta ----------
    private IEnumerator EvolveRoutine(RecipeData r, bool firstTime)
    {
        yield return new WaitForSecondsRealtime(evolveDelay);
        while (IsBusy) yield return null;

        if (r.evolvedSprite != null)
        {
            ClearLayers();                                   // some as camadas soltas...
            RectTransform rt = CreateImage(r.recipeName, r.evolvedSprite);
            rt.anchoredPosition = new Vector2(0f, firstLayerY);
            layers.Add(rt.gameObject);                       // ...e entra o lanche bonito

            // pisca do branco (silhueta) para o normal
            if (r.silhouetteSprite != null)
            {
                var img = rt.GetComponent<Image>();
                for (int i = 0; i < flashCount; i++)
                {
                    img.sprite = r.silhouetteSprite;
                    yield return new WaitForSecondsRealtime(flashTime);
                    img.sprite = r.evolvedSprite;
                    yield return new WaitForSecondsRealtime(flashTime);
                }
            }

            StartCoroutine(Pop(rt, 1.15f, 0.2f));
        }

        StartCoroutine(SpawnFx(heartPrefab, r.hearts, 0f, heartArea, 0f));
        if (firstTime)
            StartCoroutine(SpawnFx(starPrefab, r.stars, 0.25f, starArea, starHeight));
    }

    // ---------- Efeitos ----------
    private IEnumerator SpawnFx(GameObject prefab, int count, float startDelay, Vector2 area, float baseY)
    {
        if (prefab == null || count <= 0) yield break;
        if (startDelay > 0f) yield return new WaitForSecondsRealtime(startDelay);

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(prefab, fxRoot);
            var rt = (RectTransform)go.transform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(Random.Range(-area.x, area.x),
                                              baseY + Random.Range(-area.y, area.y));
            StartCoroutine(FloatAndFade(rt));
            yield return new WaitForSecondsRealtime(0.07f);  // um de cada vez
        }
    }

    private IEnumerator FloatAndFade(RectTransform rt)
    {
        if (!rt.TryGetComponent(out CanvasGroup cg)) cg = rt.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        Vector2 start = rt.anchoredPosition;
        float t = 0f;
        while (t < fxDuration && rt != null)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fxDuration);
            rt.localScale = Vector3.one * Mathf.Min(1f, t / 0.15f);          // nasce crescendo
            rt.anchoredPosition = start + Vector2.up * (fxRise * k);          // sobe
            cg.alpha = k < 0.6f ? 1f : 1f - (k - 0.6f) / 0.4f;                // some no final
            yield return null;
        }
        if (rt != null) Destroy(rt.gameObject);
    }

    private IEnumerator Pop(RectTransform rt, float peak, float time)
    {
        float t = 0f;
        while (t < time && rt != null)
        {
            t += Time.unscaledDeltaTime;
            float s = Mathf.Lerp(1f, peak, Mathf.Sin(Mathf.Clamp01(t / time) * Mathf.PI));
            rt.localScale = Vector3.one * s;
            yield return null;
        }
        if (rt != null) rt.localScale = Vector3.one;
    }

    // ---------- Limpeza ----------
    private void ClearLayers()
    {
        foreach (var go in layers) if (go != null) Destroy(go);
        layers.Clear();
    }

    private void ClearAll()
    {
        StopAllCoroutines();
        dropsInProgress = 0;
        ClearLayers();
        if (fxRoot != null)
            for (int i = fxRoot.childCount - 1; i >= 0; i--)
                Destroy(fxRoot.GetChild(i).gameObject);
        currentY = firstLayerY;
    }
}