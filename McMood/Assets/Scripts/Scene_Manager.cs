using UnityEngine;
using TMPro;

public class Scene_Manager : MonoBehaviour
{
    [Header("Tela inicial")]
    public TMP_InputField inputLogin;
    public TMP_InputField inputSenha;
    public GameObject panelLogin; // o painel da tela inicial

    [Header("Próximo painel")]
    public GameObject panelMainMenu;
    public TMP_Text textoSaudacao; // o objeto de texto que tem "Olá, [Usuário]!"

    private string textoOriginalSaudacao;

    public static string usuarioAtual;

    void Start()
    {
        // Guarda o texto original ("Olá, [Usuário]!") antes de qualquer alteração
        textoOriginalSaudacao = textoSaudacao.text;
    }

    // Conecte esse método no OnClick() do botão de Login
    public void OnClickLogin()
    {
        string usuarioDigitado = inputLogin.text;

        usuarioAtual = usuarioDigitado; // guarda pra usar em outros painéis

        textoSaudacao.text = textoOriginalSaudacao.Replace("[Usuário]", usuarioDigitado);

        AbrirMainMenu();
    }

    void AbrirMainMenu()
    {
        panelLogin.SetActive(false);
        panelMainMenu.SetActive(true);
    }
}