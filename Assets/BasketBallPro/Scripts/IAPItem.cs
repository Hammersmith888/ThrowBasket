namespace GameBench
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class IAPItem : MonoBehaviour
    {
        public Text quantityText;
        public Text priceText;

        public void SetValues(string quantity, string price)
        {
            quantityText.text = quantity;
            priceText.text = price;
        }
        public void SetValues(int quantity, int price)
        {
            quantityText.text = quantity.ToString();
            priceText.text = price.ToString();
        }
        internal void SetLocalPrice(string localizedPriceString)
        {
            priceText.text = localizedPriceString;
        }
    }
}