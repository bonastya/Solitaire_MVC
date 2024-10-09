using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(CardSpriteManager))]
public class FieldController : MonoBehaviour
{
    public List<List<RectTransform>> cardsGroups;          // ������ ����� ����
    public RectTransform[] cards;                          // ������ ���� �� ����
    public FieldState fieldState;

    private CardSpriteManager cardSpriteManager;

    public GameObject cardPrefab;
    public Transform bankPanel;
    public Transform combinationPanel;
    public float cardOffset = 50f;

    void Start()
    {
        cardSpriteManager = GetComponent<CardSpriteManager>();

        cardsGroups = new List<List<RectTransform>>();

        fieldState = new FieldState();
        fieldState.CardNumber = cards.Length;

        GroupCards();
        SetGroupsHierarchy();
        MakeGameCombinations();
        DistributeCombinationsToCardsGroups();

        SpavnBankCards();


        /*        foreach (List<Card> cardsGroupModel in fieldState.CardGroups)
                {
                    print("Num: " + cardsGroupModel.Count);
                    foreach (Card card in cardsGroupModel)
                    {
                        print("Card " + card.CardView.gameObject.name);
                        print("Parent " + (card.Parent != null ? card.Parent.CardView.gameObject.name : "null"));
                        print("Child " + (card.Child != null ? card.Child.CardView.gameObject.name : "null"));
                        print("-----");
                    }
                    print("---------------------------");
                }*/


        foreach (List<(CardValue, CardSuit)> cardsCombination in fieldState.CardCombinations)
        {
            print("Num: " + cardsCombination.Count);
            foreach ((CardValue, CardSuit) card in cardsCombination)
            {
                print("card "+card.Item1 + card.Item2);
            }
            print("---------------------------");
        }


    }

    private void SpavnBankCards()
    {
        float position = cardPrefab.GetComponent<RectTransform>().rect.width/2;
        foreach (Card bankCard in fieldState.Bank) 
        {
            GameObject card = Instantiate(cardPrefab, bankPanel);
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(position, 0f);
            position += cardOffset;

            // �������� ������ �� View, ���� ��� - �������� ���������
            if (card.TryGetComponent<CardView>(out CardView cardView))
            {
                cardView = card.AddComponent<CardView>();
            }
            bankCard.CardView = cardView;
            cardSpriteManager.UpdateView(bankCard, bankCard.CardView);

            bankCard.CardView.cardButton.onClick.AddListener(()=> ValidateCardInput(bankCard));
        }


        fieldState.currentCard = fieldState.Bank.Last();
        fieldState.currentCard.CardView.GoToCombinationPlace(combinationPanel, () => SendCardToCombinationPlace(fieldState.currentCard));

    }

    private void ValidateCardInput(Card card)
    {
        if (GameDesignData.GetNextCardValue(card.CardValue) == fieldState.currentCard.CardValue ||
            GameDesignData.GetPreviousCardValue(card.CardValue) == fieldState.currentCard.CardValue)
        {
            card.CardView.GoToCombinationPlace(combinationPanel, () => SendCardToCombinationPlace(card));
        }
        
    }

    private void SendCardToCombinationPlace(Card card)
    {
        card.CardView.gameObject.transform.SetParent(combinationPanel);
        card.CardView.cardButton.onClick.RemoveAllListeners();

        fieldState.currentCard = card;
    }





    private void DistributeCombinationsToCardsGroups()
    {
        // ��������� ����� ���� � �������
        int[] availableGroupsSlots = new int[fieldState.CardGroups.Count];
        int[] groupsFullness = new int[fieldState.CardGroups.Count];

        for (int i = 0; i < availableGroupsSlots.Length; i++)
        {
            availableGroupsSlots[i]= fieldState.CardGroups[i].Count;
            groupsFullness[i] = 0;
        }

        int groupNum; // ����� ������ ���� ������������ ������� �����
        Card currentCard;

        // ��� ������ ����������
        foreach (List<(CardValue, CardSuit)> cardsCombination in fieldState.CardCombinations) 
        {
            print("����������");
            // ������ ����� � ����
            fieldState.Bank.Add(new Card(cardsCombination[0].Item1, cardsCombination[0].Item2));
            print("� ����" + cardsCombination[0].Item1+ cardsCombination[0].Item2);
            // ��������� ����������� �� ������� � �����
            for (int i = cardsCombination.Count-1; i>0; i--)
            {
                groupNum = UnityEngine.Random.Range(0, fieldState.CardGroups.Count);
                while (groupsFullness[groupNum] == availableGroupsSlots[groupNum]) // ����� ������������� ������
                {
                    groupNum = UnityEngine.Random.Range(0, fieldState.CardGroups.Count);
                }

                currentCard = fieldState.CardGroups[groupNum][groupsFullness[groupNum]];
                currentCard.CardValue = cardsCombination[i].Item1;
                currentCard.CardSuit = cardsCombination[i].Item2;

                cardSpriteManager.UpdateView(currentCard, currentCard.CardView); // �������� ������� � ��������� ������
                print("����� ���������� " + i + cardsCombination[i].Item1 + cardsCombination[i].Item2);
                print("����������� � ������ " + groupNum+"� ������ "+ currentCard.CardView.gameObject);
                groupsFullness[groupNum]++;

            }


        }



    }


    private void MakeGameCombinations()
    {

        int distributedCards = 0;

        int combinationLength = 0;        // ������ ���������� �� 2 �� 7
        int combinationDirection = 1;     // ����������� ���������� (65%)����� (1), (35%) ���� (-1)
        bool directionChange = false;     // ����� �� ��������� �����������
        int directionChangeCard = 0;      // �� ����� ����� ���������� �����������

        CardValue cardValue = 0;
        CardSuit cardSuit = 0;

        fieldState.CardCombinations.Clear();

        while (distributedCards < fieldState.CardNumber)
        {
            // ����������� ���������� ����������
            // ������ �� 2� �� 7 ��� ������� ��������
            combinationLength = UnityEngine.Random.Range(2, Math.Min(8, fieldState.CardNumber-distributedCards+1));

            combinationDirection = (UnityEngine.Random.Range(0f, 1f) <= 0.65) ? 1 : -1;
            directionChange = UnityEngine.Random.Range(0f, 1f) <= 0.15;
            // ���������� �� ����� ����� �������� ����������� - ���� ��� ������ �������� � ��� �������� (���� ������ 2�)
            if (directionChange && combinationLength>2) 
            {
                directionChangeCard = UnityEngine.Random.Range(3, combinationLength+1); 
            }

            // �������� ������ ����� ����������
            cardValue = GameDesignData.RandomValue();
            cardSuit = GameDesignData.RandomSuit();

            // ������ �������� ����������
            List<(CardValue, CardSuit)> cardValues = new List<(CardValue, CardSuit)>(); 

            cardValues.Add((cardValue, cardSuit));


            // ������� �������� � ����� �����
            (CardValue, CardSuit) cardValueAndSuit = new();

            for (int i = 1; i < combinationLength; i++)
            {
                // ���� �� ���� ����� �������� ����������� - ������ �� ���������������
                if (directionChange && i == directionChangeCard) 
                {
                    combinationDirection = -combinationDirection;
                }
                cardValueAndSuit = MakeNextCardValueAndSuit(cardValue, combinationDirection);
                cardValue = cardValueAndSuit.Item1;
                cardValues.Add(cardValueAndSuit);
            }

            // ���� ������� 1 ��������� �� ��� ����� ��� ������������ � ���������� ���������� (���� ��� �� 7)
            // ��� �������� �� ��������� 1 � ������� ����
            if(fieldState.CardNumber - ( distributedCards + combinationLength-1 ) == 1) // -1 ��� ��� ���� ����� ���� � ����
            {
                if (combinationLength < 7)
                {
                    cardValueAndSuit = MakeNextCardValueAndSuit(cardValue, combinationDirection);
                    cardValue = cardValueAndSuit.Item1;
                    cardValues.Add(cardValueAndSuit);
                    combinationLength++;
                }
                else
                {
                    cardValues.Remove(cardValues.Last());
                    combinationLength--;
                }
            }
            print(combinationDirection);
            fieldState.CardCombinations.Add(cardValues);
            distributedCards += combinationLength-1;// -1 ��� ��� ���� ����� ���� � ����

        }

    }


    private (CardValue, CardSuit) MakeNextCardValueAndSuit(CardValue cardValue, int combinationDirection)
    {
        CardValue nextCardValue;
        // � ����������� �� ����������� ���� ���������� ��� ��������� �����
        if (combinationDirection == 1)
        {
            nextCardValue = GameDesignData.GetNextCardValue(cardValue);
        }
        else
        {
            nextCardValue = GameDesignData.GetPreviousCardValue(cardValue);
        }

        // ����� - ���������
        return (nextCardValue, GameDesignData.RandomSuit());
    }


    private void GroupCards()
    {
        bool[] cardIsChecked = new bool[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            if (!cardIsChecked[i])
            {
                List<RectTransform> group = new List<RectTransform>();
                group.Add(cards[i]);
                SearchIntersections(i, cardIsChecked, group);
                cardsGroups.Add(group);
            }
        }
    }

    private void SearchIntersections(int i, bool[] cardIsChecked, List<RectTransform> group)
    {
        cardIsChecked[i] = true;

        for (int j = 0; j < cards.Length; j++)
        {
            if (!cardIsChecked[j] && IfCardsIntersect(i, j))
            {
                group.Add(cards[j]);
                SearchIntersections(j, cardIsChecked, group);
            }
        }

    }

    private bool IfCardsIntersect(int i, int j)
    {
        Rect rectI = new Rect(cards[i].anchoredPosition, cards[i].sizeDelta);
        Rect rectJ = new Rect(cards[j].anchoredPosition, cards[j].sizeDelta);

        return rectI.Overlaps(rectJ);
    }
    private bool IfCardsIntersect1(RectTransform i, RectTransform j)
    {
        Rect rectI = new Rect(i.anchoredPosition, i.sizeDelta);
        Rect rectJ = new Rect(j.anchoredPosition, j.sizeDelta);

        return rectI.Overlaps(rectJ);
    }


    private void SetGroupsHierarchy()
    {

        foreach (List<RectTransform> cardsGroup in cardsGroups)
        {
            cardsGroup.OrderBy(p => p.transform.GetSiblingIndex());
        }

        // � 0 �� ����� �� ����� ����, 0 ������ (������) -> � ����� (�������)
        foreach (List<RectTransform> cardsGroup in cardsGroups)
        {
            List<Card> groupModel = new List<Card>();

            // ��������� ������ �����
            Card cardModel0 = new Card();
            groupModel.Add(cardModel0);
            cardModel0.CardView = cardsGroup[0].GetComponent<CardView>();

            for (int i = 1; i < cardsGroup.Count; i++)
            {
                // ������� ��������
                // ��������� �� ������ 
                // ��������� � ��� ������� ������

                Card cardModel = new Card();
                cardModel.Parent = groupModel[i - 1];
                groupModel[i - 1].Child = cardModel;

                // �������� ������ �� View, ���� ��� - �������� ���������
                if (!cardsGroup[i].gameObject.TryGetComponent<CardView>(out CardView cardView))
                {
                    cardView = cardsGroup[i].gameObject.AddComponent<CardView>();
                }
                cardModel.CardView = cardView;

                groupModel.Add(cardModel);

                //��������� ��� ����� �������� ����������
                if(!IfCardsIntersect1(cardModel.CardView.GetComponentInParent<RectTransform>(), groupModel[i - 1].CardView.GetComponentInParent<RectTransform>()))
                {
                    fieldState.IFcardsPositionedCorrectly = false;
                    print("Cards relationships is not correct: "+ cardModel.CardView.gameObject.name);
                }
            }

            fieldState.CardGroups.Add(groupModel);

            groupModel[0].Parent = null;
            groupModel[groupModel.Count-1].Child = null;

        }

    }



}
