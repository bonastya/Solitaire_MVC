using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    private Image cardImage;    // Изображение карты
    public Button cardButton;    // Изображение карты

    public Vector3 startPosition;
    private void Awake()
    {
        cardImage= GetComponent<Image>();
        startPosition = transform.position;

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

    public void DestroyCard()
    {
        Destroy(gameObject); 
    }
    public void ReturnToStartPos()
    {
        transform.position = startPosition;
    }





    public void GoToCombinationPlace(Transform combinationPlace, Action Complete)
    {
        DOTween.Init();
        gameObject.transform.DOMove(combinationPlace.position,1).OnComplete(()=> Complete());
        gameObject.transform.SetAsLastSibling();

    }

    public void GoToCombinationPlaceBank(Transform combinationPlace, Action Complete)
    {
        float time = GameDesignData.animation_open_bank_duration;
        gameObject.transform.SetAsLastSibling();

        DOTween.Init();

        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(gameObject.transform.DOMove(combinationPlace.position, time));

        // Карточка открывается в середине анимации
        mySequence.Insert(0.0f, gameObject.transform.DORotate(new Vector3(0, -90, 0), time / 2).SetEase(Ease.Linear));
        mySequence.Insert(time/ 2, gameObject.transform.DORotate(new Vector3(0, 0, 0), time / 2).SetEase(Ease.Linear));
        mySequence.InsertCallback(time / 2, () => Complete());

    }

    public void OpenCard( Action Complete)
    {
        float time = GameDesignData.animation_open_card_duration;
        DOTween.Init();
        Sequence mySequence = DOTween.Sequence();
        // Карточка открывается в середине анимации
        mySequence.Insert(0.0f, gameObject.transform.DORotate(new Vector3(0, -90, 0), time / 2).SetEase(Ease.Linear));
        mySequence.Insert(time / 2, gameObject.transform.DORotate(new Vector3(0, 0, 0), time / 2).SetEase(Ease.Linear));
        mySequence.InsertCallback(time / 2, () => Complete());

    }

    public void GoToStartPosition(Transform fromPosition, Action Complete) 
    {
        float time = GameDesignData.animation_spavn_cards_duration;

        DOTween.Init();
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(gameObject.transform.DOMove(fromPosition.position, 0));
        mySequence.Append(gameObject.transform.DOMove(startPosition, time));
        
        mySequence.InsertCallback(time, () => Complete());

    }


}
