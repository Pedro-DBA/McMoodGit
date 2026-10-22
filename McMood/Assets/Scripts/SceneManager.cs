using System.Collections;
using TMPro;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [Header("Paineis")]
    public GameObject panelClickToBegin;
    public GameObject panelMenu;
    public GameObject panelFuncionario;
    public GameObject panelProprietario;

    [Header("Cena do jogo")]
    public string nomeCenaJogo = "McMoodGame";

    [Header("Login do funcion\u00e1rio")]
    public TMP_InputField inputUsuarioFuncionario;
    public string mensagemUsuarioVazio = "Digite um nome de usu\u00e1rio.";

    [Header("Pop-up (topo da tela)")]
    public GameObject popupAcessoNegado;
    public TMP_Text textoPopup;
    public string mensagemAcessoNegado = "Acesso negado.";
    public float tempoVisivel = 1.0f;
    public float tempoFade = 1.0f;

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

    public void Entrar()
    {
        string nome = inputUsuarioFuncionario.text;

        if (string.IsNullOrWhiteSpace(nome))
        {
            MostrarPopup(mensagemUsuarioVazio);
            return;
        }

        UsuarioSessao.Definir(nome);
        UnityEngine.SceneManagement.SceneManager.LoadScene(nomeCenaJogo);
    }


    public void EntrarProprietario()
    {
        MostrarPopup(mensagemAcessoNegado);
    }

    private void MostrarPopup(string mensagem)
    {
        if (popupAcessoNegado == null)
        {
            Debug.LogWarning("popupAcessoNegado n\u00e3o foi atribu\u00eddo no Inspector.");
            return;
        }

        if (textoPopup != null)
            textoPopup.text = mensagem;

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