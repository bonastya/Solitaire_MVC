using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public TMP_Text valueText; // ����� �������� �����
    public Image cardImage;    // ����������� �����

    public void UpdateView(Card cardModel)
    {
        valueText.text = cardModel.CardValue.ToString();
        cardImage.enabled = cardModel.FacedUp;
    }

}
