using UnityEngine;
using TMPro;

public class Profile_Manager : MonoBehaviour
{
    public TMP_Text textoUsuario;

    private string textoOriginal;

    void Awake() // trocado de Start() para Awake()
    {
        textoOriginal = textoUsuario.text;
    }

    void OnEnable()
    {
        textoUsuario.text = textoOriginal.Replace("[Usuário]", Scene_Manager.usuarioAtual);
    }
}