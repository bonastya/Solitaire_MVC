using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    private Image cardImage;    // ����������� �����
    private void Awake()
    {
        cardImage= GetComponent<Image>();
    }
    public void SetSprite(Sprite sprite)
    {
        cardImage.sprite = sprite; 
    }
}
