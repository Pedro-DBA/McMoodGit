using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Guarda o nome do usuário e troca "[Usuário]" por ele em TODOS os textos TMP
// de TODAS as cenas. Não precisa colocar em nenhum objeto: roda sozinho.
public static class UsuarioSessao
{
    public const string Marcador = "[Usuário]"; // "[Usuário]"

    public static string Nome { get; private set; } = "";

    // Chamado pelo MenuManager quando o login do funcionário é aceito
    public static void Definir(string nome)
    {
        Nome = nome.Trim();
        AplicarNaCenaAtual(); // atualiza também a cena que já está aberta
    }

    // Registra o gancho que roda a cada cena carregada
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Registrar()
    {
        Nome = ""; // evita sobrar valor antigo se o Domain Reload estiver desligado
        SceneManager.sceneLoaded -= AoCarregarCena;
        SceneManager.sceneLoaded += AoCarregarCena;
    }

    private static void AoCarregarCena(Scene cena, LoadSceneMode modo)
    {
        AplicarNaCenaAtual();
    }

    private static void AplicarNaCenaAtual()
    {
        if (string.IsNullOrEmpty(Nome))
            return;

        // Include = pega também os textos de painéis que estão desativados
        TMP_Text[] textos = Object.FindObjectsByType<TMP_Text>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (TMP_Text t in textos)
        {
            if (t.text.Contains(Marcador))
                t.text = t.text.Replace(Marcador, Nome);
        }
    }
}