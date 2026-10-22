using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public static class UsuarioSessao
{
    public const string Marcador = "[Usuário]";

    public static string Nome { get; private set; } = "";

    public static void Definir(string nome)
    {
        Nome = nome.Trim();
        AplicarNaCenaAtual();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Registrar()
    {
        Nome = "";
        UnitySceneManager.sceneLoaded -= AoCarregarCena;
        UnitySceneManager.sceneLoaded += AoCarregarCena;
    }

    private static void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        AplicarNaCenaAtual();
    }

    private static void AplicarNaCenaAtual()
    {
        if (string.IsNullOrEmpty(Nome))
            return;

        TMP_Text[] textos = Object.FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (TMP_Text t in textos)
        {
            if (t.text.Contains(Marcador))
                t.text = t.text.Replace(Marcador, Nome);
        }
    }
}