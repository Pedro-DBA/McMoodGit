using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Coloque no Panel_Conta. Cuida de:
/// - abrir/fechar o sub-painel "Estabelecimento";
/// - o botão de Logout, que volta para a cena do menu/login.
/// </summary>
public class AccountPanel : MonoBehaviour
{
    [Header("Estabelecimento")]
    [SerializeField] private Button establishmentButton;       // Button_Estabelecimento
    [SerializeField] private GameObject establishmentPanel;    // Panel_Estabelecimento (filho do Panel_Conta)

    [Header("Logout")]
    [SerializeField] private Button logoutButton;              // Button_Logout
    [SerializeField] private string menuSceneName = "McMoodMenu";
    [Tooltip("Ligue aqui o que limpa a sessão do login (ex: apagar o usuário salvo). Roda ANTES de trocar de cena.")]
    [SerializeField] private UnityEvent onLogout;

    private void Awake()
    {
        if (establishmentButton != null) establishmentButton.onClick.AddListener(OpenEstablishment);
        if (logoutButton != null) logoutButton.onClick.AddListener(Logout);
    }

    // Sempre que a aba Conta abre, o sub-painel começa fechado.
    private void OnEnable() => CloseEstablishment();

    public void OpenEstablishment()
    {
        if (establishmentPanel != null) establishmentPanel.SetActive(true);
    }

    /// <summary>Ligue ao OnClick do botão de voltar DENTRO do Panel_Estabelecimento.</summary>
    public void CloseEstablishment()
    {
        if (establishmentPanel != null) establishmentPanel.SetActive(false);
    }

    public void Logout()
    {
        onLogout?.Invoke();
        // Nome completo de propósito: o projeto tem uma classe própria chamada SceneManager.
        UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
    }
}