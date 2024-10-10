using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(CardSpriteManager))]
public class FieldController : MonoBehaviour
{
    public List<List<RectTransform>> cardsGroups;          // Список групп карт
    public RectTransform[] cards;                          // Список карт на поле
    public FieldState fieldState;

    private CardSpriteManager cardSpriteManager;
    private CardController cardController;

    public GameObject cardPrefab;
    public Transform gameField;
    public Transform bankPanel;
    public Transform combinationPanel;
    public float cardOffset = 50f;

    public Transform cardsSpavnPosition;
    public Button restartButton;

    private void Awake()
    {
        cardSpriteManager = GetComponent<CardSpriteManager>();
        cardController = new CardController();
        cardController.cardSpriteManager = cardSpriteManager;

        cardsGroups = new List<List<RectTransform>>();

        fieldState = new FieldState();
        fieldState.CardNumber = cards.Length;

        GroupCards();
        SetGroupsHierarchy();
        

        restartButton.onClick.AddListener(RestartLevel);
    }
    void Start()
    {
        StartLevel();


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


        /*foreach (List<(CardValue, CardSuit)> cardsCombination in fieldState.CardCombinations)
        {
            print("Num: " + cardsCombination.Count);
            foreach ((CardValue, CardSuit) card in cardsCombination)
            {
                print("card "+card.Item1 + card.Item2);
            }
            print("---------------------------");
        }*/


    }

    private void RestartLevel()
    {
        foreach (var card in fieldState.Bank) 
        {
            cardController.Remove(card);
        }
        fieldState.Bank.Clear();

        foreach (List<Card> group in fieldState.CardGroups)
        {
            foreach (Card card in group)
            {
                cardController.SetToStartPos(card, gameField);
                card.FacedUp = false;
            }
        }


        StartLevel();
    }

    private void StartLevel()
    {
        MakeGameCombinations();
        DistributeCombinationsToCardsGroups();
        SpavnBankCards();
        AnimateCards();
    }

    private void AnimateCards()
    {
        foreach (List<Card> group in fieldState.CardGroups)
        {
            foreach (Card card in group)
            {
                card.CardView.GoToStartPosition(cardsSpavnPosition, () => { });
            }
        }
        StartCoroutine(UnlockTopCards());
    }

    private void SpavnBankCards()
    {
        float position = cardPrefab.GetComponent<RectTransform>().rect.width/2;
        foreach (Card bankCard in fieldState.Bank) 
        {
            GameObject card = Instantiate(cardPrefab, bankPanel);
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(position, 0f);
            position += cardOffset;

            cardController.InitCardView(card, bankCard);
            cardController.UpdateView(bankCard);
            cardController.AddOnClickListener(bankCard, () => BankInput(bankCard));
        }

        fieldState.currentCard = fieldState.Bank.Last();

    }

    private IEnumerator UnlockTopCards()
    {
        yield return new WaitForSeconds(GameDesignData.animation_spavn_cards_duration);
        foreach(List<Card> group in fieldState.CardGroups)
        {
            cardController.UnlockCardWithAnim(group.Last());
        }
        BankInput(fieldState.currentCard);
    }



    #region user inputs
    private void ValidateCardInput(Card card)
    {
        if (GameDesignData.GetNextCardValue(card.CardValue) == fieldState.currentCard.CardValue ||
            GameDesignData.GetPreviousCardValue(card.CardValue) == fieldState.currentCard.CardValue)
        {
            fieldState.currentCard = card;
            cardController.UnlockParentCardWithAnim(card);
            cardController.AnimateToCombinationPlace(card, combinationPanel);
        }
    }

    private void BankInput(Card card)
    {
        fieldState.currentCard = card;
        cardController.UnlockParentBankCard(card);
        cardController.AnimateBankToCombinationPlace(card, combinationPanel);
    }
    #endregion user inputs

    private void DistributeCombinationsToCardsGroups()
    {
        // доступные слоты карт в группах
        int[] availableGroupsSlots = new int[fieldState.CardGroups.Count];
        int[] groupsFullness = new int[fieldState.CardGroups.Count];

        for (int i = 0; i < availableGroupsSlots.Length; i++)
        {
            availableGroupsSlots[i]= fieldState.CardGroups[i].Count;
            groupsFullness[i] = 0;
        }

        int groupNum; // номер группы куда распределяем текущую карту
        Card currentCard;

        // для каждой комбинации
        foreach (List<(CardValue, CardSuit)> cardsCombination in fieldState.CardCombinations) 
        {
            print("комбинация");
            // первую карту в банк
            fieldState.Bank.Add(new Card(cardsCombination[0].Item1, cardsCombination[0].Item2, fieldState.Bank.LastOrDefault()));

            print("в банк" + cardsCombination[0].Item1+ cardsCombination[0].Item2);
            // остальные раскидываем по группам с конца
            for (int i = cardsCombination.Count-1; i>0; i--)
            {
                groupNum = UnityEngine.Random.Range(0, fieldState.CardGroups.Count);
                while (groupsFullness[groupNum] == availableGroupsSlots[groupNum]) // найти незаполненную группу
                {
                    groupNum = UnityEngine.Random.Range(0, fieldState.CardGroups.Count);
                }

                currentCard = fieldState.CardGroups[groupNum][groupsFullness[groupNum]];
                currentCard.CardValue = cardsCombination[i].Item1;
                currentCard.CardSuit = cardsCombination[i].Item2;

                cardController.UpdateView(currentCard);
                print("карта комбинации " + i + cardsCombination[i].Item1 + cardsCombination[i].Item2);
                print("заполняется в группу " + groupNum+"в объект "+ currentCard.CardView.gameObject);
                groupsFullness[groupNum]++;

            }


        }



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
            if(fieldState.CardNumber - ( distributedCards + combinationLength-1 ) == 1) // -1 так как одна карта уйдёт в банк
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
            distributedCards += combinationLength-1;// -1 так как одна карта уйдёт в банк

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
            cardController.InitCardView(cardsGroup[0].gameObject, cardModel0);
            // Добавляем Listener нажатия для проверки
            cardController.AddOnClickListener(cardModel0, () => ValidateCardInput(cardModel0));


            for (int i = 1; i < cardsGroup.Count; i++)
            {
                // Создать карточку
                // Присвоить ей предка 
                // Присвоить её как потомка предку

                Card cardModel = new Card();
                cardModel.Parent = groupModel[i - 1];
                groupModel[i - 1].Child = cardModel;

                cardController.InitCardView(cardsGroup[i].gameObject, cardModel);

                // Добавляем Listener нажатия для проверки
                cardController.AddOnClickListener(cardModel, () => ValidateCardInput(cardModel));

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
