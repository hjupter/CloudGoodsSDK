﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SocialPlay.Data;

public interface IServiceCalls
{
    void GenerateItemsAtLocation(string OwnerID, string OwnerType, int Location, Guid AppID, int MinimumEnergyOfItem, int TotalEnergyToGenerate, Action<List<ItemData>> callback, string ANDTags = "", string ORTags = "");

    void GetOwnerItems(string ownerID, string ownerType, int location, Guid AppID, Action<List<ItemData>> callback);

    void MoveItemStack(Guid StackToMove, int MoveAmount, string DestinationOwnerID, string DestinationOwnerType, Guid AppID, int DestinationLocation, Action<Guid> callback);

    void GetUserFromWorld(Guid appID, int platformID, string platformUserID, string userName, string userEmail, Action<WebserviceCalls.UserInfo> callback);

    void GetStoreItems(string appID, Action<List<StoreItemInfo>> callback);

    void GetFreeCurrencyBalance(string userID, int accessLocation, string appID, Action<string> callback);

    void GetPaidCurrencyBalance(string userID, string appID, Action<string> callback);

    void RegisterGameSession(Guid userID, string AppID, int instanceID, Action<Guid> callback);

    void GetGameRecipes(string appID, Action<List<RecipeInfo>> callback);

    void StoreItemPurchase(string URL, Guid userID, int itemID, int amount, string paymentType, Guid appID, int saveLocation, Action<string> callback);

    void GetItemBundles(string appID, Action<List<ItemBundle>> callback);

    void PurchaseItemBundles(Guid appID, Guid UserID, int bundleID, string paymentType, int location, Action<string> callback);

    void GetCreditBundles(string appID, int platform, Action<List<CreditBundleItem>> callback);

    void PurchaseCreditBundles(Guid appId, string payload, Action<string> callback);

    void GetAccessPinFromGuid(string userPin, Action<string> callback);

    void GetAccessPinForUser(string UserId, Action<string> callback);

    void MoveItemStacks(string stacks, string DestinationOwnerID, string DestinationOwnerType, Guid AppID, int DestinationLocation, Action<MoveMultipleItemsResponse> callback);

    void RemoveItemStack(Guid StackRemove, Action<string> callback);

    void DeductStackAmount(Guid StackRemove, int amount, Action<string> callback);

    void RemoveItemStacks(List<Guid> StacksToRemove, Action<string> callback);

    void CompleteQueueItem(Guid gameID, int QueueID, int percentScore, int location, Action<string> callback);

    void AddInstantCraftToQueue(Guid gameID, Guid UserID, int ItemID, int Amount, List<KeyValuePair<string, int>> ItemIngredients, Action<string> callback);

    void SPLogin_UserLogin(Guid gameID, string userEmail, string password, Action<SPLogin.SPLogin_Responce> callback);

    void SPLogin_UserRegister(Guid gameID, string userEmail, string password, string userName, Action<SPLogin.SPLogin_Responce> callback);

    void SPLoginForgotPassword(Guid gameID, string userEmail, Action<SPLogin.SPLogin_Responce> callback);

    void SPLoginResendVerificationEmail(Guid gameID, string userEmail, Action<SPLogin.SPLogin_Responce> callback);



    void GiveOwnerItems(WebModels.OwnerTypes OwnerType, List<WebModels.ItemsInfo> listOfItems, Action<string> callback);
}

