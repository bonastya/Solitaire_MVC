
public class Card 
{
    public CardValue CardValue { get; set; }  // Номинал карты
    public CardSuit CardSuit { get; set; }    // Масть карты
    public Card Parent { get; set; }          // Предок карты
    public Card Child { get; set; }           // Потомок карты
    public bool FacedUp { get; set; }         // Повернута ли



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
