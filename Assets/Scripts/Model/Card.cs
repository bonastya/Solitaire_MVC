
public class Card 
{
    public CardValue CardValue { get; set; }  // ������� �����
    public CardSuit CardSuit { get; set; }    // ����� �����

    #nullable enable
    public Card? Parent { get; set; }      // ������ �����
    public Card? Child { get; set; }        // ������� �����
    #nullable disable

    public bool FacedUp  { get; set; } = false;        // ��������� ��

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
