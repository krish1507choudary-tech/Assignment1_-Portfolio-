using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Image buttonImage;

    public Sprite normalSprite;
    public Sprite hoverSprite;

    [Header("Hover Sound")]
    public AudioSource hoverAudio;
    public AudioClip hoverSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.sprite = hoverSprite;

        if (hoverAudio != null && hoverSound != null)
        {
            hoverAudio.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.sprite = normalSprite;
    }
}