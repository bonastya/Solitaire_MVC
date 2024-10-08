using System;

public static class GameDesignData 
{
    public const int  NUM_OF_CARDS = 40;                   // Количество карт которое должно быть на поле

    public const int MIN_COMB = 2;                         // Минимальное количество карт в комбинации
    public const int MAX_COMB = 7;                         // Максимальное количество карт в комбинации
    public const float increasing_combination = 0.65f ;    // Вероятность возрастающей комбинации
    public const float change_direction = 0.15f;           // Вероятность изменения направлениия комбинации


    // статичные массивы формируются из enum CardValue и enum CardSuit
    public static CardValue[] CardValues = Enum.GetValues(typeof(CardValue)) as CardValue[];
    public static int CardValuesNumber = CardValues.Length;

    public static CardSuit[] CardSuits = Enum.GetValues(typeof(CardSuit)) as CardSuit[];
    public static int CardSuitsNumber = CardSuits.Length;


    public static CardValue GetNextCardValue(CardValue cardValue)
    {
        if ((int)cardValue != (CardValuesNumber-1))
        {
            return (CardValue)((int)cardValue + 1);//todo убрать приведения
        }
        else
        {
            return (CardValue)0; //todo убрать приведения
        }
        
    }

    public static CardValue GetPreviousCardValue(CardValue cardValue)
    {
        if ((int)cardValue != 0)
        {
            return (CardValue)((int)cardValue - 1);
        }
        else
        {
            return (CardValue)(CardValuesNumber - 1);
        }

    }

    public static CardSuit RandomSuit()
    {
        return (CardSuit)UnityEngine.Random.Range(0, CardSuitsNumber);
    }
    public static CardValue RandomValue()
    {
        return (CardValue)UnityEngine.Random.Range(0, CardValuesNumber);
    }

}

public enum CardSuit
{
    Spades,  // Пики
    Hearts,  // Черви
    Clubs,   // Трефы
    Diamonds // Бубны
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