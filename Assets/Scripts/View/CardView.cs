using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    private Image cardImage;    // Изображение карты
    private Button cardButton;    // Изображение карты
    private void Awake()
    {
        cardImage= GetComponent<Image>();
        var cardButton = GetComponent<Button>();

        //cardButton.onClick.AddListener(GoToCombinationPlace);
    }
    public void SetSprite(Sprite sprite)
    {
        cardImage.sprite = sprite; 
    }


    private void GoToCombinationPlace(Transform combinationPlace)
    {
        DOTween.Init();
        gameObject.transform.DOMove(combinationPlace.position,1);
    }

}
