using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FieldController : MonoBehaviour
{
    public List<List<RectTransform>> cardsGroups;          // ������ ����� ����
    public RectTransform[] cards;                          // ������ ���� �� ����
    public FieldState fieldState;

    void Start()
    {
        cardsGroups = new List<List<RectTransform>>();

        fieldState = new FieldState();
        fieldState.CardNumber = cards.Length;

        GroupCards();
        SetGroupsHierarchy();
        MakeGameCombinations();


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


        /*foreach (List < (CardValue, CardSuit)> cardsCombination in fieldState.CardCombinations)
        {
            print("Num: " + cardsCombination.Count);
            foreach ((CardValue, CardSuit) card in cardsCombination)
            {
                print("Value " + card.Item1);
                print("Suit " + card.Item2);
                print("-----");
            }
            print("---------------------------");
        }*/


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
            if(fieldState.CardNumber - ( distributedCards + combinationLength ) == 1)
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

            fieldState.CardCombinations.Add(cardValues);
            distributedCards += combinationLength;

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

                // �������� ������ �� View
                cardModel.CardView = cardsGroup[i].GetComponent<CardView>();

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
