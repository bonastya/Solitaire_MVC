using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    private Image cardImage;    // Изображение карты
    public Button cardButton;    // Изображение карты
    private void Awake()
    {
        cardImage= GetComponent<Image>();
        // Если нет button - создать
        if (!gameObject.TryGetComponent<Button>(out Button button))
        {
            button = gameObject.AddComponent<Button>();
        }
        cardButton = button;

        //cardButton.onClick.AddListener(GoToCombinationPlace);
    }
    public void SetSprite(Sprite sprite)
    {
        cardImage.sprite = sprite; 
    }


    public void GoToCombinationPlace(Transform combinationPlace, Action Complete)
    {
        DOTween.Init();
        gameObject.transform.DOMove(combinationPlace.position,1).OnComplete(()=> Complete());
        gameObject.transform.SetAsLastSibling();

    }

}
