using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FieldController : MonoBehaviour
{
    public List<List<RectTransform>> cardsGroups;          // Список групп карт
    public RectTransform[] cards;                          // Список карт на поле
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

        int combinationLength = 0;        // длинна комбинации от 2 до 7
        int combinationDirection = 1;     // направление комбинации (65%)вверх (1), (35%) вниз (-1)
        bool directionChange = false;     // будет ли изменение направления
        int directionChangeCard = 0;      // на какой карте изменяется направление

        CardValue cardValue = 0;
        CardSuit cardSuit = 0;

        fieldState.CardCombinations.Clear();

        while (distributedCards < fieldState.CardNumber)
        {
            // определение параметров комбинации
            // длинна от 2х до 7 или сколько осталось
            combinationLength = UnityEngine.Random.Range(2, Math.Min(8, fieldState.CardNumber-distributedCards+1));

            combinationDirection = (UnityEngine.Random.Range(0f, 1f) <= 0.65) ? 1 : -1;
            directionChange = UnityEngine.Random.Range(0f, 1f) <= 0.15;
            // определяем на какой карте меняется направление - если оно должно меняться и это возможно (карт больше 2х)
            if (directionChange && combinationLength>2) 
            {
                directionChangeCard = UnityEngine.Random.Range(3, combinationLength+1); 
            }

            // значения первой карты комбинации
            cardValue = GameDesignData.RandomValue();
            cardSuit = GameDesignData.RandomSuit();

            // Кортеж значений комбинации
            List<(CardValue, CardSuit)> cardValues = new List<(CardValue, CardSuit)>(); 

            cardValues.Add((cardValue, cardSuit));


            // Текущее значение и масть карты
            (CardValue, CardSuit) cardValueAndSuit = new();

            for (int i = 1; i < combinationLength; i++)
            {
                // если на этой карте меняется направление - меняем на противоположное
                if (directionChange && i == directionChangeCard) 
                {
                    combinationDirection = -combinationDirection;
                }
                cardValueAndSuit = MakeNextCardValueAndSuit(cardValue, combinationDirection);
                cardValue = cardValueAndSuit.Item1;
                cardValues.Add(cardValueAndSuit);
            }

            // если остаётся 1 последний то его нужно или присоединить к предыдущей комбинации (если она не 7)
            // или откусить от последней 1 и сделать пару
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
        // в зависимости от направления берём предыдущую или следующую карту
        if (combinationDirection == 1)
        {
            nextCardValue = GameDesignData.GetNextCardValue(cardValue);
        }
        else
        {
            nextCardValue = GameDesignData.GetPreviousCardValue(cardValue);
        }

        // масть - случайная
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

        // С 0 до конца по кучке карт, 0 нижняя (предок) -> и вверх (потомки)
        foreach (List<RectTransform> cardsGroup in cardsGroups)
        {
            List<Card> groupModel = new List<Card>();

            // Добавляем первую карту
            Card cardModel0 = new Card();
            groupModel.Add(cardModel0);
            cardModel0.CardView = cardsGroup[0].GetComponent<CardView>();

            for (int i = 1; i < cardsGroup.Count; i++)
            {
                // Создать карточку
                // Присвоить ей предка 
                // Присвоить её как потомка предку

                Card cardModel = new Card();
                cardModel.Parent = groupModel[i - 1];
                groupModel[i - 1].Child = cardModel;

                // Оставить ссылку на View
                cardModel.CardView = cardsGroup[i].GetComponent<CardView>();

                groupModel.Add(cardModel);

                //Проверяем что карта касается предыдущей
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
