namespace GameBench
{
    using System;
    using UnityEngine;
#if ENABLE_IAP
    using UnityEngine.Purchasing;
#endif

    // Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    public class IAPManager : MonoBehaviour
#if ENABLE_IAP
    , IStoreListener
#endif
    {
#if ENABLE_IAP
        public static IStoreController m_StoreController;
        // Reference to the Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider;
        IAPItem[] coinItems = new IAPItem[4];
        void Start()
        {
#if !UNITY_WEBGL
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
#endif
            coinItems = GetComponentsInChildren<IAPItem>();
            //coinItems[0].SetValues(GameSetup.Instance.freeCoinCount.ToString(), "Free", freeSp);
            //coinItems[0].quantityText.text = Configs.Instance.freeCoinCount.ToString();
            for (int i = 0; i < coinItems.Length; i++)
            {
                coinItems[i].SetValues(GameManager.GetValueFormated(Configs.Instance.iapItems[i].coinsCount),
                    Configs.Instance.iapItems[i].iapLocalPrice);
            }
        }
        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            for (int i = 0; i < Configs.Instance.iapItems.Length; i++)
            {
                builder.AddProduct(Configs.Instance.iapItems[i].iapItemId, ProductType.Consumable, new IDs() { {
                Configs.Instance.iapItems[i].iapAppleID,
                AppleAppStore.Name
            }, {
                Configs.Instance.iapItems[i].iapAndroidID,
                GooglePlay.Name
                }
        });
            }
            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }


        void BuyProductID(string productId)
        {
            // If the stores throw an unexpected exception, use try..catch to protect our logic here.
            try
            {
                // If Purchasing has been initialized ...
                if (IsInitialized())
                {
                    // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
                    Product product = m_StoreController.products.WithID(productId);

                    // If the look up found a product for this device's store and that product is ready to be sold ... 
                    if (product != null && product.availableToPurchase)
                    {
                        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                        // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                        m_StoreController.InitiatePurchase(product);
                    }
                    // Otherwise ...
                    else
                    {
                        // ... report the product look-up failure situation  
                        Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    }
                }
                // Otherwise ...
                else
                {
                    // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
                    Debug.Log("BuyProductID FAIL. Not initialized.");
                }
            }
            // Complete the unexpected exception handling ...
            catch (Exception e)
            {
                // ... by reporting any unexpected exception for later diagnosis.
                Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
            }
        }

        // Restore purchases previously made by this customer. Some platforms automatically restore purchases. Apple currently requires explicit purchase restoration for IAP.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                         Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        //
        // --- IStoreListener
        //
       
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");
            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
            try
            {
                if (IsInitialized())
                    for (int i = 0; i < 5; i++)
                    {
                        coinItems[i + 1].SetLocalPrice
                            (m_StoreController.products.WithID(Configs.Instance.iapItems[i].iapItemId).metadata.localizedPriceString);
                    }
            }
            catch (Exception e)
            {
                print("Editor Only Bug " + e.StackTrace);
            }
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            bool isError = true;
            for (int i = 0; i < Configs.Instance.iapItems.Length; i++)
            {
                if (string.Equals(args.purchasedProduct.definition.id, Configs.Instance.iapItems[i].iapItemId, StringComparison.Ordinal))
                {
                    Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                    GameManager.Instance.AddIAPCoins(i);
                    isError = false;
                    break;
                }
            }
            if (isError)
            {
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }
            return PurchaseProcessingResult.Complete;
        }
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
#endif
        public void BuyCoins(int id)
        {
            GameManager.Instance.PlayClick();
            // Buy the consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
#if !UNITY_WEBGL && ENABLE_IAP
            BuyProductID(Configs.Instance.iapItems[id].iapItemId);
#endif
        }

    }
    [Serializable]
    public class IAPInfoItem
    {
        //public Sprite iapSprite;
        public string iapItemId;
        public string iapAppleID;
        public string iapAndroidID;
        public string iapLocalPrice;
        public int coinsCount;
    }
}