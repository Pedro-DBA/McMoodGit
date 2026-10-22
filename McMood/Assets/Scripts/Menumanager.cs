using System.Collections;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Paineis")]
    public GameObject panelClickToBegin;   // "Clique para comecar"
    public GameObject panelMenu;           // Panel_Menu (com Button_funcionario e Button_proprietario)
    public GameObject panelFuncionario;
    public GameObject panelProprietario;

    [Header("Cena do jogo")]
    public string nomeCenaJogo = "McMoodGame";

    [Header("Login do funcionário")]
    public TMP_InputField inputUsuarioFuncionario; // InputField_Usuario do Panel_Funcionario
    public string mensagemUsuarioVazio = "Digite um nome de usuário.";

    [Header("Pop-up (topo da tela)")]
    public GameObject popupAcessoNegado;   // painel no topo da tela, com o texto dentro
    public TMP_Text textoPopup;            // o texto TMP dentro do pop-up
    public string mensagemAcessoNegado = "Acesso negado.";
    public float tempoVisivel = 1.0f;      // quanto tempo fica totalmente visivel
    public float tempoFade = 1.0f;         // duracao do desaparecimento

    private GameObject panelAtual; // painel de login aberto (Funcionario ou Proprietario)
    private Coroutine popupRoutine;

    void Start()
    {
        // Estado inicial: so o "Clique para comecar" aparece
        panelClickToBegin.SetActive(true);
        panelMenu.SetActive(false);
        panelFuncionario.SetActive(false);
        panelProprietario.SetActive(false);

        if (popupAcessoNegado != null)
            popupAcessoNegado.SetActive(false);
    }

    // OnClick() do Panel_ClickToBegin (o painel precisa ter um componente Button)
    public void ComecarClick()
    {
        panelClickToBegin.SetActive(false);
        panelMenu.SetActive(true);
    }

    // OnClick() do Button_funcionario
    public void AbrirFuncionario()
    {
        AbrirPainel(panelFuncionario);
    }

    // OnClick() do Button_proprietario
    public void AbrirProprietario()
    {
        AbrirPainel(panelProprietario);
    }

    // OnClick() do Button_Voltar (nos dois paineis): fecha o painel atual
    public void Voltar()
    {
        if (panelAtual != null)
        {
            panelAtual.SetActive(false);
            panelAtual = null;
        }
        panelMenu.SetActive(true);
    }

    // OnClick() do Button_Entrar do PANEL_FUNCIONARIO: exige algo escrito no usuario
    public void Entrar()
    {
        string nome = inputUsuarioFuncionario.text;

        // Vazio (ou so espacos) nao passa
        if (string.IsNullOrWhiteSpace(nome))
        {
            MostrarPopup(mensagemUsuarioVazio);
            return;
        }

        UsuarioSessao.Definir(nome); // vale para todas as cenas

        // Nome completo da classe do Unity, para nao confundir com a classe deste arquivo
        UnityEngine.SceneManagement.SceneManager.LoadScene(nomeCenaJogo);
    }

    // OnClick() do Button_Entrar do PANEL_PROPRIETARIO: sempre nega, nao troca de cena
    public void EntrarProprietario()
    {
        MostrarPopup(mensagemAcessoNegado);
    }

    private void MostrarPopup(string mensagem)
    {
        if (popupAcessoNegado == null)
        {
            Debug.LogWarning("popupAcessoNegado não foi atribuído no Inspector.");
            return;
        }

        if (textoPopup != null)
            textoPopup.text = mensagem;

        // Se clicar de novo enquanto o pop-up ainda aparece, reinicia a animacao
        if (popupRoutine != null)
            StopCoroutine(popupRoutine);

        popupRoutine = StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        CanvasGroup grupo = popupAcessoNegado.GetComponent<CanvasGroup>();
        if (grupo == null)
            grupo = popupAcessoNegado.AddComponent<CanvasGroup>();

        popupAcessoNegado.SetActive(true);
        grupo.alpha = 1f;

        // Fica visivel por um instante
        yield return new WaitForSeconds(tempoVisivel);

        // Some aos poucos
        float t = 0f;
        while (t < tempoFade)
        {
            t += Time.deltaTime;
            grupo.alpha = 1f - Mathf.Clamp01(t / tempoFade);
            yield return null;
        }

        popupAcessoNegado.SetActive(false);
        popupRoutine = null;
    }

    private void AbrirPainel(GameObject painel)
    {
        panelMenu.SetActive(false);
        painel.SetActive(true);
        panelAtual = painel;
    }
}