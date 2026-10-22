using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Paineis")]
    public GameObject panelClickToBegin;
    public GameObject panelMenu;
    public GameObject panelFuncionario;
    public GameObject panelProprietario;

    [Header("Cena do jogo")]
    public string nomeCenaJogo = "McMoodGame";

<<<<<<< Updated upstream
    [Header("Login do funcionário")]
    public TMP_InputField inputUsuarioFuncionario; // InputField_Usuario do Panel_Funcionario
    public string mensagemUsuarioVazio = "Digite um nome de usuário.";

    [Header("Pop-up (topo da tela)")]
    public GameObject popupAcessoNegado;   // painel no topo da tela, com o texto dentro
    public TMP_Text textoPopup;            // o texto TMP dentro do pop-up
    public string mensagemAcessoNegado = "Acesso negado.";
    public float tempoVisivel = 1.0f;      // quanto tempo fica totalmente visível
    public float tempoFade = 1.0f;         // duração do desaparecimento
=======
    [Header("Pop-up 'Acesso negado.'")]
    public GameObject popupAcessoNegado;
    public float tempoVisivel = 1.0f;
    public float tempoFade = 1.0f;
>>>>>>> Stashed changes

    private GameObject panelAtual;
    private Coroutine popupRoutine;

    void Start()
    {
        
        panelClickToBegin.SetActive(true);
        panelMenu.SetActive(false);
        panelFuncionario.SetActive(false);
        panelProprietario.SetActive(false);

        if (popupAcessoNegado != null)
            popupAcessoNegado.SetActive(false);
    }


    public void ComecarClick()
    {
        panelClickToBegin.SetActive(false);
        panelMenu.SetActive(true);
    }

    
    public void AbrirFuncionario()
    {
        AbrirPainel(panelFuncionario);
    }

    
    public void AbrirProprietario()
    {
        AbrirPainel(panelProprietario);
    }

    
    public void Voltar()
    {
        if (panelAtual != null)
        {
            panelAtual.SetActive(false);
            panelAtual = null;
        }
        panelMenu.SetActive(true);
    }

<<<<<<< Updated upstream
    // OnClick() do Button_Entrar do PANEL_FUNCIONARIO: exige algo escrito no usuário
=======
    
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
        if (textoPopup != null)
            textoPopup.text = mensagem;

        // Se clicar de novo enquanto o pop-up ainda aparece, reinicia a animação
=======
        
>>>>>>> Stashed changes
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

        
        yield return new WaitForSeconds(tempoVisivel);

        
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