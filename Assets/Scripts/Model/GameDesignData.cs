using System;
using Unity.IO.LowLevel.Unsafe;

public static class GameDesignData 
{
    // �������������� ������

    /// <summary>
    /// ���������� ���� ������� ������ ���� �� ����
    /// </summary>
    public const int  NUM_OF_CARDS = 40;                   
    
    /// <summary>
    /// ����������� ���������� ���� � ����������
    /// </summary>
    public const int MIN_COMB = 2;                         
    
    /// <summary>
    /// ������������ ���������� ���� � ����������
    /// </summary>
    public const int MAX_COMB = 7;                         
    
    /// <summary>
    /// ����������� ������������ ����������
    /// </summary>
    public const float INC_COMB = 0.65f ;

    /// <summary>
    /// ����������� ��������� ������������ ����������
    /// </summary>
    public const float CHANGE_COMB_DIR = 0.15f;

    // ������������

    /// <summary>
    /// ����������������� �������� ��������� ����
    /// </summary>
    public const float ANIM_SPAWN_CARDS_DURATION = 3f;

    /// <summary>
    /// ����������������� �������� �������� �����
    /// </summary>
    public const float ANIM_OPEN_CARD_DURATION = 0.2f;

    /// <summary>
    /// ����������������� �������� �������� ���� �����
    /// </summary>
    public const float ANIM_OPEN_BANK_DURATION = 0.5f;

    /// <summary>
    /// �������� ����� �����
    /// </summary>
    public const float BANK_CARD_OFFSET = 20f;


    // C�������� ������� ����������� �� enum CardValue � enum CardSuit
    public static CardValue[] CardValues = Enum.GetValues(typeof(CardValue)) as CardValue[];
    public static int CardValuesNumber = CardValues.Length;

    public static CardSuit[] CardSuits = Enum.GetValues(typeof(CardSuit)) as CardSuit[];
    public static int CardSuitsNumber = CardSuits.Length;

    #region service functions

    public static CardValue GetNextCardValue(CardValue cardValue)
    {
        // ��������� ��������, ���� ������� �� ������� �� ���������� - 0, �� ���� ������ �������
        return (CardValue)(((int)cardValue + 1) % CardValuesNumber);

    }

    public static CardValue GetPreviousCardValue(CardValue cardValue)
    {
        // ���������� ��������, � ������ 0 ����� ����� CardValuesNumber - 1, � � ���������
        return (CardValue)(((int)cardValue - 1 + CardValuesNumber) % CardValuesNumber);

    }

    public static bool IfContinueSequence(CardValue checkCardValue, CardValue currentCardValue)
    {
        return (GameDesignData.GetNextCardValue(checkCardValue) == currentCardValue ||
            GameDesignData.GetPreviousCardValue(checkCardValue) == currentCardValue);
    }

    public static CardSuit RandomSuit()
    {
        return (CardSuit)UnityEngine.Random.Range(0, CardSuitsNumber);
    }

    public static CardValue RandomValue()
    {
        return (CardValue)UnityEngine.Random.Range(0, CardValuesNumber);
    }

    #endregion service functions

}

#region enums
public enum CardSuit
{
    Spades,  // ����
    Hearts,  // �����
    Clubs,   // �����
    Diamonds // �����
}

public enum CardValue
{
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}

#endregion enums