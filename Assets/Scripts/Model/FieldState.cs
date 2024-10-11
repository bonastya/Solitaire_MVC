using System.Collections.Generic;

/// <summary>
/// Основные значения состояния поля
/// </summary>
public class FieldState
{
    /// <summary>
    /// Группы карт, начальная раскладка
    /// </summary>
    public List<List<Card>> CardGroups { get; private set; }   

    /// <summary>
    /// Верхние карты каждой группы
    /// </summary>
    public List<Card?> TopCardsInGroups { get; private set; }   

    /// <summary>
    /// Комбинации текущего сгенерированного уровня
    /// </summary>
    public List<List<(CardValue, CardSuit)>> CardCombinations { get; private set; }   

    /// <summary>
    /// Колличество каррт на поле в начале (без учёта карт банка)
    /// </summary>
    public int CardsNumber { get;  set; }

    /// <summary>
    /// Текущее колличество карт на поле (без учёта карт банка)
    /// </summary>
    public int CurrentFieldCardsNumber { get;  set; }

    /// <summary>
    /// Карты банка текущего уровня (все)
    /// </summary>
    public List<Card> Bank { get; private set; }

    /// <summary>
    /// Текущее колличество карт в банке
    /// </summary>
    public int CurrentBankCardsNumber { get;  set; }

    /// <summary>
    /// Флаг правильного расположения карт с условием что у каждой не больше 1 потомка и 1 предка
    /// </summary>
    public bool IFcardsPositionedCorrectly { get;  set; }

    /// <summary>
    /// Текущая карта последовательности
    /// </summary>
    public Card currentCard;

    public FieldState() {
        CardGroups = new List<List<Card>>();
        Bank = new List<Card>();
        TopCardsInGroups = new List<Card>();
        CardCombinations = new List<List<(CardValue, CardSuit)>>();
    }


}
