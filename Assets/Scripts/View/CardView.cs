using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public TMP_Text valueText; // Текст номинала карты
    public Image cardImage;    // Изображение карты

    public void UpdateView(Card cardModel)
    {
        valueText.text = cardModel.CardValue.ToString();
        cardImage.enabled = cardModel.FacedUp;
    }

}
