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

    // ������� ��� �������� ������
    private List<List<RectTransform>> cardsGroups;         
    private FieldState fieldState;

    // ������ �� ��������������� �����������
    private CardSpriteManager cardSpriteManager;
    private UIController uiController;
    private CardController cardController;

    private void Awake()
    {
        // ������������� ��������������� ������������
        cardSpriteManager = GetComponent<CardSpriteManager>();
        uiController = GetComponent<UIController>();
        cardController = new CardController();
        cardController.cardSpriteManager = cardSpriteManager;
        // ������������� �������� ������
        cardsGroups = new List<List<RectTransform>>();
        fieldState = new FieldState();

        fieldState.CardsNumber = cardsTransforms.Length;
        if (fieldState.CardsNumber!=GameDesignData.NUM_OF_CARDS) Debug.LogWarning("The number of cards does not match the required number");

        // ����������� ����� � ������������ �������� ����� �������� ������
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
        // ������� ����� ����
        foreach (var card in fieldState.Bank) 
        {
            cardController.Remove(card);
        }
        fieldState.Bank.Clear();
        // ����������� ���� �� ��������� �������
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
        // �������� �������� ����� �����, ����� �������� ���������� �� ���� �����
        yield return new WaitForSeconds(waitTime);
        if(!CheckIfGameWin())
            CheckIfGameFail();
    }
    private bool CheckIfGameWin()
    {
        // ���� �� �������� ���� �� ����
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
        // ���� � ����� ��� ���� ����� �� ���
        if (fieldState.CurrentBankCardsNumber != 0)
        {
            return;
        }
        // ���� ���� ����� ���������� ��� ����������� ���������� �� ���
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
        // ����� �����
        if (isGameFail) 
        {
            uiController.EndMoves();
        }
    }

    #endregion end of game checking

    #region user card inputs
    private void ValidateCardInput(Card card)
    {
        // ���� ����� ���������� ������� ������������������
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
        // ����� ����������� �� ���� �� ����� ����� 
        foreach (List<Card> group in fieldState.CardGroups)
        {
            foreach (Card card in group)
            {
                card.CardView.GoToStartPosition(cardsSpavnPosition, () => { });
            }
        }
        StartCoroutine(UnlockTopCards()); // ��� ���������� �������� - ����������������
    }

    private IEnumerator UnlockTopCards()
    {
        yield return new WaitForSeconds(GameDesignData.ANIM_SPAWN_CARDS_DURATION);
        fieldState.TopCardsInGroups.Clear();
        // ������� ����� ������ ������ ���������������� � ������������
        foreach (List<Card> group in fieldState.CardGroups)
        {
            cardController.UnlockCardWithAnim(group.Last()); // ����������� �����
            fieldState.TopCardsInGroups.Add(group.Last()); // �������� � ������ ������� ����
        }
        BankInput(fieldState.currentCard); // ����������� ������ ����� �����
    }

    #endregion cards views preparing 

    #region make level
    private void MakeGameCombinations()
    {
        int distributedCards = 0;         // �������������� ����� (������������ ����� ������������� ������ ������������������)

        int combinationLength = 0;        // ����� ���������� �� 2 �� 7 (�� MIN_COMB �� MAX_COMB)
        int combinationDirection = 1;     // ����������� ���������� (65%)�����(INC_COMB)(1), (35%) ���� (-1)
        bool directionChange = false;     // ����� �� ��������� �����������
        int directionChangeCard = 0;      // �� ����� ����� ���������� �����������

        CardValue cardValue = 0;
        CardSuit cardSuit = 0;

        fieldState.CardCombinations.Clear();

        while (distributedCards < fieldState.CardsNumber)
        {
            // ����������� ���������� ����������
            // ����� �� MIN_COMB �� MAX_COMB ��� ������� ��������
            combinationLength = UnityEngine.Random.Range(GameDesignData.MIN_COMB, Math.Min(GameDesignData.MAX_COMB+1, fieldState.CardsNumber-distributedCards+1));

            // ����������� ����� � ������������ INC_COMB
            combinationDirection = (UnityEngine.Random.Range(0f, 1f) <= GameDesignData.INC_COMB) ? 1 : -1;

            // ��������� ����������� � ������������ 
            directionChange = UnityEngine.Random.Range(0f, 1f) <= GameDesignData.CHANGE_COMB_DIR;

            // ���������� �� ����� ����� �������� ����������� - ���� ��� ������ �������� � ��� �������� (���� ������ 2�)
            if (directionChange && combinationLength>2) 
            {
                directionChangeCard = UnityEngine.Random.Range(3, combinationLength+1); 
            }

            // �������� ������ ����� ����������
            cardValue = GameDesignData.RandomValue(); // � ����������� ������� �������� �����
            cardSuit = GameDesignData.RandomSuit();

            // ������ �������� ����������
            List<(CardValue, CardSuit)> cardValues = new List<(CardValue, CardSuit)>(); 

            cardValues.Add((cardValue, cardSuit));

            // ������� �������� � ����� �����
            (CardValue, CardSuit) cardValueAndSuit = new();

            // ��������� ���������� � ������������ � ������������ �����������
            for (int i = 1; i < combinationLength; i++)
            {
                // ���� �� ���� ����� �������� ����������� - ������ �� ���������������
                if (directionChange && i == directionChangeCard) 
                {
                    combinationDirection = -combinationDirection;
                }
                cardValueAndSuit = MakeNextCardValueAndSuit(cardValue, combinationDirection);
                cardValue = cardValueAndSuit.Item1; // ������������ � ��������� ��������
                cardValues.Add(cardValueAndSuit);
            }

            // ���� ������� 1 ��������� 
            if(fieldState.CardsNumber - ( distributedCards + combinationLength-1 ) == 1) // -1 ��� ��� ���� ����� ���� � ����
            {
                // ���� ��������� ������������������ �� ��������� �� MAX_COMB - ����������� � ���
                if (combinationLength < GameDesignData.MAX_COMB)
                {
                    cardValueAndSuit = MakeNextCardValueAndSuit(cardValue, combinationDirection);
                    cardValue = cardValueAndSuit.Item1;
                    cardValues.Add(cardValueAndSuit);
                    combinationLength++;
                }
                // ��� ������� �� ��������� 1 � ����� �������� ����
                else
                {
                    cardValues.Remove(cardValues.Last());
                    combinationLength--;
                }
            }

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

    private void DistributeCombinationsToCardsGroups()
    {
        // ��������� ����� ���� � ������� (�����, �� ����������)
        int[] availableGroupsSlots = new int[fieldState.CardGroups.Count];
        // ������� ������������� ����� (���������� 0, ����������)
        int[] groupsFullness = new int[fieldState.CardGroups.Count];

        for (int i = 0; i < availableGroupsSlots.Length; i++)
        {
            availableGroupsSlots[i] = fieldState.CardGroups[i].Count;
            groupsFullness[i] = 0;
        }

        int groupNum; // ����� ������ ���� ������������ ������� �����
        Card currentCard;

        // ��� ������ ����������
        foreach (List<(CardValue, CardSuit)> cardsCombination in fieldState.CardCombinations)
        {
            Debug.Log("------- ���������� -------");

            // ������ ����� � ����
            fieldState.Bank.Add(new Card(cardsCombination[0].Item1, cardsCombination[0].Item2, fieldState.Bank.LastOrDefault()));
            Debug.Log("����� ����� " + cardsCombination[0].Item1 + " " + cardsCombination[0].Item2);

            // ��������� ����������� �� ������� � �����
            for (int i = cardsCombination.Count - 1; i > 0; i--)
            {
                groupNum = UnityEngine.Random.Range(0, fieldState.CardGroups.Count);
                while (groupsFullness[groupNum] == availableGroupsSlots[groupNum]) // ����� ������������� ������
                {
                    groupNum = UnityEngine.Random.Range(0, fieldState.CardGroups.Count);
                }

                currentCard = fieldState.CardGroups[groupNum][groupsFullness[groupNum]]; // ���� ��� ������������� ����� ���� ������
                currentCard.CardValue = cardsCombination[i].Item1;
                currentCard.CardSuit = cardsCombination[i].Item2;

                cardController.UpdateView(currentCard);
                Debug.Log("����� ���������� " + i + " [" + cardsCombination[i].Item1 + " " + cardsCombination[i].Item2 + "] , � ������ " + groupNum);

                groupsFullness[groupNum]++;

            }

        }

    }

    #endregion make level

    #region field cards preparing
    private void GroupCards()
    {
        // ����� ���� �� ��������� ����� �� ����������� � ����������
        bool[] cardIsChecked = new bool[cardsTransforms.Length];

        // ������� "�����"
        // ������ ����� � ������ ����� ������ ��������
        for (int i = 0; i < cardsTransforms.Length; i++)
        {
            // �������� �� ������������� ������
            // ��� ������ ������ ����� ������ ����� ������� ������
            if (!cardIsChecked[i])
            {
                List<RectTransform> group = new List<RectTransform>();
                group.Add(cardsTransforms[i]);
                SearchIntersections(i, cardIsChecked, group); // � ������� ��� ����������� 
                cardsGroups.Add(group);
            }
        }
    }

    private void SearchIntersections(int i, bool[] cardIsChecked, List<RectTransform> group)
    {
        // ��� ����� �������� ��������
        cardIsChecked[i] = true;

        for (int j = 0; j < cardsTransforms.Length; j++)
        {
            // ������� ��� ����� � �������� ������������ ����� i
            if (!cardIsChecked[j] && IfCardsIntersect(cardsTransforms[i], cardsTransforms[j]))
            {
                group.Add(cardsTransforms[j]); // ��������� � ������
                SearchIntersections(j, cardIsChecked, group); // ��������� �� �� ����������� ����
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
        // ��������� ����� � ������� �� �� �������� � ������������ �������
        foreach (List<RectTransform> cardsGroup in cardsGroups)
        {
            cardsGroup.OrderBy(p => p.transform.GetSiblingIndex());
        }
        int groupNumber = 0;

        // �������� � 0 �� ����� �� ����� ����, 0 ������ (������) -> � ����� (�������)
        foreach (List<RectTransform> cardsGroup in cardsGroups)
        {
            List<Card> groupModel = new List<Card>();

            // ��������� ������ �����
            Card cardModel0 = new Card(groupNumber);
            groupModel.Add(cardModel0);
            cardController.InitCardView(cardsGroup[0].gameObject, cardModel0);

            // ��������� Listener ������� 
            cardController.AddOnClickListener(cardModel0, () => ValidateCardInput(cardModel0));


            for (int i = 1; i < cardsGroup.Count; i++)
            {
                // ������ ��������
                Card cardModel = new Card(groupNumber);
                cardModel.Parent = groupModel[i - 1]; // ����������� �� ������
                groupModel[i - 1].Child = cardModel; // ����������� � ��� ������� ������

                cardController.InitCardView(cardsGroup[i].gameObject, cardModel);

                // ��������� Listener ������� ��� ��������
                cardController.AddOnClickListener(cardModel, () => ValidateCardInput(cardModel));

                groupModel.Add(cardModel);

                //��������� ��� ����� �������� ���������� (������� ��� � ������ ����� 1 ������ � 1 �������)
                if(!IfCardsIntersect(cardModel.CardView.GetComponentInParent<RectTransform>(), groupModel[i - 1].CardView.GetComponentInParent<RectTransform>()))
                {
                    fieldState.IFcardsPositionedCorrectly = false;
                    // ������� ��������� ���� ��� �� ���
                    Debug.LogWarning("Cards relationships is not correct: " + cardModel.CardView.gameObject.name);

                }
            }

            fieldState.CardGroups.Add(groupModel);

            // � ������ ��� ������, � ��������� �������
            groupModel[0].Parent = null;
            groupModel[groupModel.Count-1].Child = null;

            groupNumber++;

        }

    }

    #endregion field cards preparing


}
