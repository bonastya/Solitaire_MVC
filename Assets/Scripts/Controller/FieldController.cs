using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

[RequireComponent(typeof(CardSpriteManager))]
[RequireComponent(typeof(UIController))]
public class FieldController : MonoBehaviour
{
    [Header("Field sections")]

    [Tooltip("The parent object of the field cards")]
    public Transform gameField;
    [Tooltip("Bank card panel")]
    public Transform bankPanel;
    [Tooltip("Current combination panel")]
    public Transform combinationPanel;

    [Header("Cards description")]

    [Tooltip("List of field cards (all must be in the same parent object)")]
    public RectTransform[] cardsTransforms; 
    public GameObject cardPrefab;
    [Tooltip("The position from where cards move at start")]
    public Transform cardsSpavnPosition;

    // Объекты для хранения данных
    private List<List<RectTransform>> cardsGroups;         
    private FieldState fieldState;

    // Ссылки на вспомогательные контроллеры
    private CardSpriteManager cardSpriteManager;
    private UIController uiController;
    private CardController cardController;

    private void Awake()
    {
        // Инициализация вспомогательных контроллеров
        cardSpriteManager = GetComponent<CardSpriteManager>();
        uiController = GetComponent<UIController>();
        cardController = new CardController();
        cardController.cardSpriteManager = cardSpriteManager;
        // Инициализация объектов данных
        cardsGroups = new List<List<RectTransform>>();
        fieldState = new FieldState();

        fieldState.CardsNumber = cardsTransforms.Length;
        if (fieldState.CardsNumber!=GameDesignData.NUM_OF_CARDS) Debug.LogWarning("The number of cards does not match the required number");

        // Определение групп и установление иерархии перед запуском уровня
        GroupCards();
        SetGroupsHierarchy();

    }
    void Start()
    {
        StartLevel();
    }

    #region level lifecycle
    public void RestartLevel()
    {
        // Очистка банка карт
        foreach (var card in fieldState.Bank) 
        {
            cardController.Remove(card);
        }
        fieldState.Bank.Clear();
        // Возвращение карт на начальные позиции
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
        SpawnBankCards();
        AnimateCards();
        uiController.StartGame();

        fieldState.CurrentFieldCardsNumber = fieldState.CardsNumber;
    }
    #endregion level lifecycle

    #region end of game checking
    private IEnumerator CheckIfGameEnd(float waitTime)
    {
        // Проверка проходит через время, когда карточки разлетятся на свои места
        yield return new WaitForSeconds(waitTime);
        if(!CheckIfGameWin())
            CheckIfGameFail();
    }
    private bool CheckIfGameWin()
    {
        // Если не осталось карт на поле
        if (fieldState.CurrentFieldCardsNumber==0)
        {
            uiController.WinGame();
            return true;
        }
        return false;
    }
    private void CheckIfGameFail()
    {
        bool isGameFail = true;
        // Если в банке ещё есть карты то нет
        if (fieldState.CurrentBankCardsNumber != 0)
        {
            return;
        }
        // Если есть карты подходящие для продолжения комбинации то нет
        foreach (Card? topCard in fieldState.TopCardsInGroups)
        {
            if(topCard != null)
            {
                if(GameDesignData.IfContinueSequence(topCard.CardValue, fieldState.currentCard.CardValue))
                {
                    isGameFail = false;
                    break; 
                }
            }
        }
        // Иначе конец
        if (isGameFail) 
        {
            uiController.EndMoves();
        }
    }

    #endregion end of game checking

    #region user card inputs
    private void ValidateCardInput(Card card)
    {
        // Если карта продолжает текущую последовательность
        if (GameDesignData.IfContinueSequence(card.CardValue, fieldState.currentCard.CardValue))
        {
            fieldState.currentCard = card;
            cardController.UnlockParentCardWithAnim(card, fieldState.TopCardsInGroups);
            cardController.AnimateToCombinationPlace(card, combinationPanel);
            fieldState.CurrentFieldCardsNumber--;

            StartCoroutine(CheckIfGameEnd(GameDesignData.ANIM_MOVE_CARDS_DURATION));
        }
    }

    private void BankInput(Card card)
    {
        fieldState.currentCard = card;
        cardController.UnlockParentBankCard(card);
        cardController.AnimateBankToCombinationPlace(card, combinationPanel);
        fieldState.CurrentBankCardsNumber--;

        StartCoroutine(CheckIfGameEnd(GameDesignData.ANIM_MOVE_CARDS_DURATION));
    }
    #endregion user inputs

    #region cards views preparing 
    private void SpawnBankCards()
    {
        fieldState.CurrentBankCardsNumber = fieldState.Bank.Count();

        float position = cardPrefab.GetComponent<RectTransform>().rect.width/2;
        foreach (Card bankCard in fieldState.Bank) 
        {
            GameObject card = Instantiate(cardPrefab, bankPanel);
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(position, 0f);
            position += GameDesignData.BANK_CARD_OFFSET;

            cardController.InitCardView(card, bankCard);
            cardController.UpdateView(bankCard);
            cardController.AddOnClickListener(bankCard, () => BankInput(bankCard));
        }

        fieldState.currentCard = fieldState.Bank.Last();

    }

    private void AnimateCards()
    {
        // Карты разлетаются на поле из одной точки 
        foreach (List<Card> group in fieldState.CardGroups)
        {
            foreach (Card card in group)
            {
                card.CardView.GoToStartPosition(cardsSpavnPosition, () => { });
            }
        }
        StartCoroutine(UnlockTopCards()); // как закончится анимация - переворачиваются
    }

    private IEnumerator UnlockTopCards()
    {
        yield return new WaitForSeconds(GameDesignData.ANIM_SPAWN_CARDS_DURATION);
        fieldState.TopCardsInGroups.Clear();
        // Верхние карты каждой группы переворачиваются и записываются
        foreach (List<Card> group in fieldState.CardGroups)
        {
            cardController.UnlockCardWithAnim(group.Last()); // перевернуть карту
            fieldState.TopCardsInGroups.Add(group.Last()); // записать в список верхних карт
        }
        BankInput(fieldState.currentCard); // открывается первая карта банка
    }

    #endregion cards views preparing 

    #region make level
    private void MakeGameCombinations()
    {
        int distributedCards = 0;         // распределенные карты (прибавляется после распределения каждой последовательности)

        int combinationLength = 0;        // длина комбинации от 2 до 7 (от MIN_COMB до MAX_COMB)
        int combinationDirection = 1;     // направление комбинации (65%)вверх(INC_COMB)(1), (35%) вниз (-1)
        bool directionChange = false;     // будет ли изменение направления
        int directionChangeCard = 0;      // на какой карте изменяется направление

        CardValue cardValue = 0;
        CardSuit cardSuit = 0;

        fieldState.CardCombinations.Clear();

        while (distributedCards < fieldState.CardsNumber)
        {
            // Определение параметров комбинации
            // Длина от MIN_COMB до MAX_COMB или сколько осталось
            combinationLength = UnityEngine.Random.Range(GameDesignData.MIN_COMB, Math.Min(GameDesignData.MAX_COMB+1, fieldState.CardsNumber-distributedCards+1));

            // Направление вверх с вероятностью INC_COMB
            combinationDirection = (UnityEngine.Random.Range(0f, 1f) <= GameDesignData.INC_COMB) ? 1 : -1;

            // Изменение направления с вероятностью 
            directionChange = UnityEngine.Random.Range(0f, 1f) <= GameDesignData.CHANGE_COMB_DIR;

            // Определяем на какой карте меняется направление - если оно должно меняться и это возможно (карт больше 2х)
            if (directionChange && combinationLength>2) 
            {
                directionChangeCard = UnityEngine.Random.Range(3, combinationLength+1); 
            }

            // Значения первой карты комбинации
            cardValue = GameDesignData.RandomValue(); // в последующем текущее значение карты
            cardSuit = GameDesignData.RandomSuit();

            // Кортеж значений комбинации
            List<(CardValue, CardSuit)> cardValues = new List<(CardValue, CardSuit)>(); 

            cardValues.Add((cardValue, cardSuit));

            // Текущее значение и масть карты
            (CardValue, CardSuit) cardValueAndSuit = new();

            // Заполняем комбинацию в соответствии с определёнными параметрами
            for (int i = 1; i < combinationLength; i++)
            {
                // Если на этой карте меняется направление - меняем на противоположное
                if (directionChange && i == directionChangeCard) 
                {
                    combinationDirection = -combinationDirection;
                }
                cardValueAndSuit = MakeNextCardValueAndSuit(cardValue, combinationDirection);
                cardValue = cardValueAndSuit.Item1; // используется в следующей итерации
                cardValues.Add(cardValueAndSuit);
            }

            // Если остаётся 1 последний 
            if(fieldState.CardsNumber - ( distributedCards + combinationLength-1 ) == 1) // -1 так как одна карта уйдёт в банк
            {
                // Если последняя последовательность не заполнена до MAX_COMB - присоединим к ней
                if (combinationLength < GameDesignData.MAX_COMB)
                {
                    cardValueAndSuit = MakeNextCardValueAndSuit(cardValue, combinationDirection);
                    cardValue = cardValueAndSuit.Item1;
                    cardValues.Add(cardValueAndSuit);
                    combinationLength++;
                }
                // Или откусим от последней 1 и чтобы осталась пара
                else
                {
                    cardValues.Remove(cardValues.Last());
                    combinationLength--;
                }
            }

            fieldState.CardCombinations.Add(cardValues);
            distributedCards += combinationLength-1;// -1 так как одна карта уйдёт в банк

        }

    }


    private (CardValue, CardSuit) MakeNextCardValueAndSuit(CardValue cardValue, int combinationDirection)
    {
        CardValue nextCardValue;
        // В зависимости от направления берём предыдущую или следующую карту
        if (combinationDirection == 1)
        {
            nextCardValue = GameDesignData.GetNextCardValue(cardValue);
        }
        else
        {
            nextCardValue = GameDesignData.GetPreviousCardValue(cardValue);
        }

        // Масть - случайная
        return (nextCardValue, GameDesignData.RandomSuit());
    }

    private void DistributeCombinationsToCardsGroups()
    {
        // Доступные слоты карт в группах (всего, не изменяется)
        int[] availableGroupsSlots = new int[fieldState.CardGroups.Count];
        // Текущая наполненность групп (изначально 0, изменяется)
        int[] groupsFullness = new int[fieldState.CardGroups.Count];

        for (int i = 0; i < availableGroupsSlots.Length; i++)
        {
            availableGroupsSlots[i] = fieldState.CardGroups[i].Count;
            groupsFullness[i] = 0;
        }

        int groupNum; // номер группы куда распределяем текущую карту
        Card currentCard;

        // Для каждой комбинации
        foreach (List<(CardValue, CardSuit)> cardsCombination in fieldState.CardCombinations)
        {
            Debug.Log("------- комбинация -------");

            // Первую карту в банк
            fieldState.Bank.Add(new Card(cardsCombination[0].Item1, cardsCombination[0].Item2, fieldState.Bank.LastOrDefault()));
            Debug.Log("карта банка " + cardsCombination[0].Item1 + " " + cardsCombination[0].Item2);

            // Остальные раскидываем по группам с конца
            for (int i = cardsCombination.Count - 1; i > 0; i--)
            {
                groupNum = UnityEngine.Random.Range(0, fieldState.CardGroups.Count);
                while (groupsFullness[groupNum] == availableGroupsSlots[groupNum]) // найти незаполненную группу
                {
                    groupNum = UnityEngine.Random.Range(0, fieldState.CardGroups.Count);
                }

                currentCard = fieldState.CardGroups[groupNum][groupsFullness[groupNum]]; // берём ещё незаполненную карту этой группы
                currentCard.CardValue = cardsCombination[i].Item1;
                currentCard.CardSuit = cardsCombination[i].Item2;

                cardController.UpdateView(currentCard);
                Debug.Log("карта комбинации " + i + " [" + cardsCombination[i].Item1 + " " + cardsCombination[i].Item2 + "] , в группе " + groupNum);

                groupsFullness[groupNum]++;

            }

        }

    }

    #endregion make level

    #region field cards preparing
    private void GroupCards()
    {
        // Флаги были ли проверены карты на пересечения с остальными
        bool[] cardIsChecked = new bool[cardsTransforms.Length];

        // Обходим "графы"
        // Каждая карта в теории может лежать отдельно
        for (int i = 0; i < cardsTransforms.Length; i++)
        {
            // Проходим по непроверенным картам
            // Для каждой первой карты группы будет создана группа
            if (!cardIsChecked[i])
            {
                List<RectTransform> group = new List<RectTransform>();
                group.Add(cardsTransforms[i]);
                SearchIntersections(i, cardIsChecked, group); // и найдены все пересечения 
                cardsGroups.Add(group);
            }
        }
    }

    private void SearchIntersections(int i, bool[] cardIsChecked, List<RectTransform> group)
    {
        // Для карты началась проверка
        cardIsChecked[i] = true;

        for (int j = 0; j < cardsTransforms.Length; j++)
        {
            // Находим все карты с которыми пересекается карта i
            if (!cardIsChecked[j] && IfCardsIntersect(cardsTransforms[i], cardsTransforms[j]))
            {
                group.Add(cardsTransforms[j]); // добавляем в группу
                SearchIntersections(j, cardIsChecked, group); // проверяем их на пересечения тоже
            }
        }

    }

    private bool IfCardsIntersect(RectTransform i, RectTransform j)
    {
        Rect rectI = new Rect(i.anchoredPosition, i.sizeDelta);
        Rect rectJ = new Rect(j.anchoredPosition, j.sizeDelta);

        return rectI.Overlaps(rectJ);
    }

    private void SetGroupsHierarchy()
    {
        // Сортируем карты в группах по их иерархии в родительском объекте
        foreach (List<RectTransform> cardsGroup in cardsGroups)
        {
            cardsGroup.OrderBy(p => p.transform.GetSiblingIndex());
        }
        int groupNumber = 0;

        // Проходим с 0 до конца по кучке карт, 0 нижняя (предок) -> и вверх (потомки)
        foreach (List<RectTransform> cardsGroup in cardsGroups)
        {
            List<Card> groupModel = new List<Card>();

            // Добавляем первую карту
            Card cardModel0 = new Card(groupNumber);
            groupModel.Add(cardModel0);
            cardController.InitCardView(cardsGroup[0].gameObject, cardModel0);

            // Добавляем Listener нажатия 
            cardController.AddOnClickListener(cardModel0, () => ValidateCardInput(cardModel0));


            for (int i = 1; i < cardsGroup.Count; i++)
            {
                // Создаём карточку
                Card cardModel = new Card(groupNumber);
                cardModel.Parent = groupModel[i - 1]; // присваиваем ей предка
                groupModel[i - 1].Child = cardModel; // присваиваем её как потомка предку

                cardController.InitCardView(cardsGroup[i].gameObject, cardModel);

                // Добавляем Listener нажатия для проверки
                cardController.AddOnClickListener(cardModel, () => ValidateCardInput(cardModel));

                groupModel.Add(cardModel);

                //Проверяем что карта касается предыдущей (условие что у каждой карты 1 предок и 1 потомок)
                if(!IfCardsIntersect(cardModel.CardView.GetComponentInParent<RectTransform>(), groupModel[i - 1].CardView.GetComponentInParent<RectTransform>()))
                {
                    fieldState.IFcardsPositionedCorrectly = false;
                    // Выводим сообщение если это не так
                    Debug.LogWarning("Cards relationships is not correct: " + cardModel.CardView.gameObject.name);

                }
            }

            fieldState.CardGroups.Add(groupModel);

            // У первой нет предка, у последней потомка
            groupModel[0].Parent = null;
            groupModel[groupModel.Count-1].Child = null;

            groupNumber++;

        }

    }

    #endregion field cards preparing


}
