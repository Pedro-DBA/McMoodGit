
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Anim_Botoes : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sprites do botão")]
    public Sprite spriteNormal;
    public Sprite spritePressionado;

    private Image imagem;

    private void Awake()
    {
        imagem = GetComponent<Image>();

        imagem.sprite = spriteNormal;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        imagem.sprite = spritePressionado;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        imagem.sprite = spriteNormal;
    }
}
