using System;
using UnityEngine;
using UnityEngine.UI;


public class LobbyNavigation : MonoBehaviour
{
    [Serializable]
    private class Tab
    {
        public string name;
        public Button button;
        public GameObject panel;
    }

    [Tooltip("Objeto que agrupa a tela do lobby (fundo, botões...). NÃO pode ser pai dos Panels.")]
    [SerializeField] private GameObject lobbyRoot;

    [Tooltip("Marque se quiser que a barra de botões do lobby continue visível com a aba aberta.")]
    [SerializeField] private bool keepLobbyVisible = false;

    [SerializeField] private Tab[] tabs;

    private void Awake()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;
            if (tabs[i].button != null)
                tabs[i].button.onClick.AddListener(() => OpenTab(index));
        }
    }

    private void Start() => ShowLobby();

    public void OpenTab(int index)
    {
        for (int i = 0; i < tabs.Length; i++)
            if (tabs[i].panel != null)
                tabs[i].panel.SetActive(i == index);

        if (lobbyRoot != null && !keepLobbyVisible)
            lobbyRoot.SetActive(false);
    }

    public void ShowLobby()
    {
        foreach (var t in tabs)
            if (t.panel != null) t.panel.SetActive(false);

        if (lobbyRoot != null) lobbyRoot.SetActive(true);
    }
}
