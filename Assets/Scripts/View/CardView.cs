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
    /// ����������� ��������� ����� �� ����
    /// </summary>
    private Vector3 startPosition;

    private void Awake()
    {
        cardImage= GetComponent<Image>();
        startPosition = transform.position;

        // ���� ��� button - �������
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
        DOTween.Init();
        gameObject.transform.DOMove(combinationPlace.position,1).OnComplete(()=> Complete());
        gameObject.transform.SetAsLastSibling();

    }

    public void GoToCombinationPlaceBank(Transform combinationPlace, Action Complete)
    {
        float time = GameDesignData.ANIM_OPEN_BANK_DURATION;
        gameObject.transform.SetAsLastSibling();

        DOTween.Init();
        // �����������
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(gameObject.transform.DOMove(combinationPlace.position, time));
        // � ���������
        OpenCard(() => Complete());
    }

    public void OpenCard( Action Complete)
    {
        float time = GameDesignData.ANIM_OPEN_CARD_DURATION;
        DOTween.Init();
        Sequence mySequence = DOTween.Sequence();

        // �������� ����������� � �������� ��������
        mySequence.Insert(0.0f, gameObject.transform.DORotate(new Vector3(0, -90, 0), time / 2).SetEase(Ease.Linear)); // �� ���
        mySequence.Insert(time / 2, gameObject.transform.DORotate(new Vector3(0, 0, 0), time / 2).SetEase(Ease.Linear)); // � �������
        mySequence.InsertCallback(time / 2, () => Complete());

    }

    public void GoToStartPosition(Transform fromPosition, Action Complete) 
    {
        float time = GameDesignData.ANIM_SPAWN_CARDS_DURATION;

        DOTween.Init();
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(gameObject.transform.DOMove(fromPosition.position, 0)); // �� ��������� �����
        mySequence.Append(gameObject.transform.DOMove(startPosition, time)); // �� ���� �������
        
        mySequence.InsertCallback(time, () => Complete());

    }

    #endregion animations

}
