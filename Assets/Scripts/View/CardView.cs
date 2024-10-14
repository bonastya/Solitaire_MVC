using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    private Image cardImage;    
    
    [HideInInspector]
    public Button cardButton;

    /// <summary>
    /// Изначальное положение карты на поле
    /// </summary>
    private Vector3 startPosition;

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

    #region animations
    public void GoToCombinationPlace(Transform combinationPlace, Action Complete)
    {
        float time = GameDesignData.ANIM_MOVE_CARDS_DURATION;
        DOTween.Init();
        gameObject.transform.DOMove(combinationPlace.position, time).OnComplete(()=> Complete());
        gameObject.transform.SetAsLastSibling();

    }

    public void GoToCombinationPlaceBank(Transform combinationPlace, Action Complete, Action Open)
    {
        float time = GameDesignData.ANIM_MOVE_CARDS_DURATION;

        DOTween.Init();
        // Перемещение
        gameObject.transform.DOMove(combinationPlace.position, time).OnComplete(() => Complete());
        // С поворотом
        OpenCard(() => Open());
    }

    public void OpenCard(Action Open)
    {
        float time = GameDesignData.ANIM_OPEN_CARD_DURATION;
        DOTween.Init();
        Sequence sequence = DOTween.Sequence();

        // Карточка открывается в середине анимации
        sequence.Insert(0.0f, gameObject.transform.DORotate(new Vector3(0, -90, 0), time / 2f).SetEase(Ease.Linear)); // на бок
        sequence.Insert(time / 2f, gameObject.transform.DORotate(new Vector3(0, 0, 0), time / 2f).SetEase(Ease.Linear)); // и обратно
        sequence.InsertCallback(time / 2f, () => Open());
    }

    public void GoToStartPosition(Transform fromPosition, Action Complete) 
    {
        float time = GameDesignData.ANIM_SPAWN_CARDS_DURATION;

        DOTween.Init();
        Sequence sequence = DOTween.Sequence();

        sequence.Append(gameObject.transform.DOMove(fromPosition.position, 0)); // из начальной точки
        sequence.Append(gameObject.transform.DOMove(startPosition, time)); // на свою позицию

        sequence.InsertCallback(time, () => Complete());

    }

    #endregion animations

}
