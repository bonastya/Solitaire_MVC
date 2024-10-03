
public class Card 
{
    public CardValue CardValue { get; set; }  // Номинал карты
    public CardSuit CardSuit { get; set; }    // Масть карты

    #nullable enable
    public Card? Parent { get; set; }      // Предок карты
    public Card? Child { get; set; }        // Потомок карты
    #nullable disable

    public bool FacedUp  { get; set; } = false;        // Повернута ли

    public CardView CardView { get; set; }         // View



/*    Card(Card? parent, Card? child)
    {
        Parent = parent;
        Child = child;
        FacedUp = false;
    }*/



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
