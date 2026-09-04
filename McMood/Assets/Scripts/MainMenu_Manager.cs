using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu_Manager : MonoBehaviour
{
    [Header("Painel inicial (Main Menu)")]
    public GameObject panelMainMenu;

    // Pilha que guarda o histórico de painéis abertos
    private Stack<GameObject> historico = new Stack<GameObject>();
    private GameObject panelAtual;

    public GameObject panelLogin;

    void Start()
    {
        // O Main Menu começa como o painel atual
        panelAtual = panelMainMenu;
    }

    // Conecte esse método no OnClick() de CADA botão (Profile, Pesquisa, Rankings, Reportar)
    // No Inspector, ao selecionar o método na lista, vai aparecer um campo de GameObject:
    // arraste ali o Panel correspondente (ex: no Button_Profile, arraste o Panel_Profile)
    public void AbrirPanel(GameObject novoPanel)
    {
        historico.Push(panelAtual);   // guarda o painel atual no histórico
        panelAtual.SetActive(false);  // esconde o painel atual
        novoPanel.SetActive(true);    // mostra o novo painel
        panelAtual = novoPanel;       // atualiza qual é o painel atual
    }

    // Conecte esse método no OnClick() do Button_Back
    public void Voltar()
    {
        if (historico.Count == 0)
        {
            Debug.LogWarning("Não há painel anterior no histórico.");
            return;
        }

        panelAtual.SetActive(false);           // esconde o painel atual
        GameObject anterior = historico.Pop(); // pega o último painel do histórico
        anterior.SetActive(true);              // mostra ele
        panelAtual = anterior;                 // atualiza o painel atual
    }

    // Conecte esse método no OnClick() do botão de logout (dentro do Panel_Profile)
    public void Logout()
    {
        panelAtual.SetActive(false);     // fecha o painel atual, seja qual for
        historico.Clear();               // limpa o histórico de navegação
        panelAtual = panelMainMenu;       // reseta pro estado inicial do menu
        panelMainMenu.SetActive(false);   // garante que o MainMenu também está fechado
        panelLogin.SetActive(true);       // volta pra tela de login
    }
}