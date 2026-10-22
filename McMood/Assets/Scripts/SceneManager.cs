using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Paineis")]
    public GameObject panelClickToBegin;   // "Clique para começar"
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
    public float tempoVisivel = 1.0f;      // quanto tempo fica totalmente visível
    public float tempoFade = 1.0f;         // duração do desaparecimento

    private GameObject panelAtual; // painel de login aberto (Funcionario ou Proprietario)
    private Coroutine popupRoutine;

    void Start()
    {
        // Estado inicial: só o "Clique para começar" aparece
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
        panelClickToBegin.SetActive(false); // use Destroy(panelClickToBegin) se quiser excluir de vez
        panelMenu.SetActive(true);          // mostra os botões Funcionário/Proprietário
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

    // OnClick() do Button_Voltar (nos dois painéis): fecha o painel atual
    public void Voltar()
    {
        if (panelAtual != null)
        {
            panelAtual.SetActive(false);
            panelAtual = null;
        }
        panelMenu.SetActive(true);
    }

    // OnClick() do Button_Entrar do PANEL_FUNCIONARIO: exige algo escrito no usuário
    public void Entrar()
    {
        string nome = inputUsuarioFuncionario.text;

        // Vazio (ou só espaços) não passa
        if (string.IsNullOrWhiteSpace(nome))
        {
            MostrarPopup(mensagemUsuarioVazio);
            return;
        }

        UsuarioSessao.Definir(nome); // vale para todas as cenas
        SceneManager.LoadScene(nomeCenaJogo);
    }

    // OnClick() do Button_Entrar do PANEL_PROPRIETARIO: sempre nega, não troca de cena
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

        // Se clicar de novo enquanto o pop-up ainda aparece, reinicia a animação
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

        // Fica visível por um instante
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