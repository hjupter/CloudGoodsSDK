using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using LitJson;
using SocialPlay.Data;
using SocialPlay.Generic;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

public class WebserviceCalls : MonoBehaviour, IServiceCalls
{
    public IServiceObjectConverter ServiceConverter;
    public Action<string> OnErrorEvent;

    public string AppSecret;

    public static IServiceCalls webservice = null;

    string cloudGoodsURL = "https://SocialPlayWebService.azurewebsites.net/cloudgoods/cloudgoodsservice.svc/";

    void Awake()
    {
        if (webservice == null)
        {
            webservice = this;
        }

        ServiceConverter = new LitJsonFxJsonObjectConverter();

        if (this.GetComponent<WebServiceUrlSwitcher>())
        {
            WebServiceUrlSwitcher switcher = this.GetComponent<WebServiceUrlSwitcher>();
            if (switcher.isUsed)
            {
                cloudGoodsURL = switcher.preferredURL;
                Debug.Log("Changed Preffered URL to " + cloudGoodsURL);
            }
        }
    }

    public class UserInfo
    {
        public string userGuid = "";
        public bool isNewUserToWorld = false;
        public string userName = "";
        public string userEmail = "";

        public UserInfo(string newUserGuid, string newUserName, string newUserEmail)
        {
            userGuid = newUserGuid;
            userName = newUserName;
            userEmail = newUserEmail;
        }
    }

    #region ItemContainerManagementCalls

    public void GenerateItemsAtLocation(string OwnerID, string OwnerType, int Location, Guid AppID, int MinimumEnergyOfItem, int TotalEnergyToGenerate, Action<List<ItemData>> callback, string ANDTags = "", string ORTags = "")
    {
        string url = string.Format("{0}GenerateItemsAtLocation?OwnerID={1}&OwnerType={2}&Location={3}&AppID={4}&MinimumEnergyOfItem={5}&TotalEnergyToGenerate={6}&ANDTags={7}&ORTags={8}", cloudGoodsURL, OwnerID, OwnerType, Location, AppID, MinimumEnergyOfItem, TotalEnergyToGenerate, ANDTags, ORTags);
        WWW www = new WWW(url);

        StartCoroutine(ServiceCallGetListItemDatas(www, callback));
    }

    public void GetOwnerItems(string ownerID, string ownerType, int location, Guid AppID, Action<List<ItemData>> callback)
    {
        string url = string.Format("{0}GetOwnerItems?ownerID={1}&ownerType={2}&location={3}&AppID={4}", cloudGoodsURL, ownerID, ownerType, location, AppID.ToString());
        WWW www = new WWW(url);

        StartCoroutine(ServiceCallGetListItemDatas(www, callback));
    }

    public void MoveItemStack(Guid StackToMove, int MoveAmount, string DestinationOwnerID, string DestinationOwnerType, Guid AppID, int DestinationLocation, Action<Guid> callback)
    {
        Debug.Log(StackToMove.ToString());

        string url = string.Format("{0}MoveItemStack?StackToMove={1}&MoveAmount={2}&DestinationOwnerID={3}&DestinationOwnerType={4}&AppID={5}&DestinationLocation={6}", cloudGoodsURL, StackToMove, MoveAmount, DestinationOwnerID, DestinationOwnerType, AppID.ToString(), DestinationLocation);
        WWW www = new WWW(url);

        StartCoroutine(ServiceGetGuid(www, callback));
    }

    public void MoveItemStacks(string stacks, string DestinationOwnerID, string DestinationOwnerType, Guid AppID, int DestinationLocation, Action<MoveMultipleItemsResponse> callback)
    {
        string url = string.Format("{0}MoveItemStacks?stacks={1}&DestinationOwnerID={2}&DestinationOwnerType={3}&AppID={4}&DestinationLocation={5}", cloudGoodsURL, stacks, DestinationOwnerID, DestinationOwnerType, AppID.ToString(), DestinationLocation);

        WWW www = new WWW(url);

        StartCoroutine(ServiceMoveItemsResponse(www, callback));
    }

    public void RemoveItemStack(Guid StackRemove, Action<string> callback)
    {
        string url = string.Format("{0}RemoveStackItem?stackID={1}", cloudGoodsURL, StackRemove);

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    public void DeductStackAmount(Guid StackRemove, int amount, Action<string> callback)
    {
        string url = string.Format("{0}DeductStackAmount?stackID={1}&amount={2}", cloudGoodsURL, StackRemove, amount);
        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    public void RemoveItemStacks(List<Guid> StacksToRemove, Action<string> callback)
    {
        RemoveMultipleItems infos = new RemoveMultipleItems();
        infos.stacks = StacksToRemove;
        string stacksInfo = JsonConvert.SerializeObject(infos);
        string url = string.Format("{0}RemoveStackItems?stacks={1}", cloudGoodsURL, stacksInfo);
        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    public void GiveOwnerItems(WebModels.OwnerTypes OwnerType, List<WebModels.ItemsInfo> listOfItems, Action<string> callback)
    {
        string jsonList = JsonConvert.SerializeObject(listOfItems);

        string url = string.Format("{0}GiveOwnerItems?AppID={1}&OwnerID={2}&OwnerType={3}&listOfItems={4}", cloudGoodsURL, ItemSystemGameData.AppID, ItemSystemGameData.UserID.ToString(), OwnerType.ToString(), jsonList);
        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    #endregion

    #region UserManagement

    public void GetUserFromWorld(Guid appID, int platformID, string platformUserID, string userName, string userEmail, Action<WebserviceCalls.UserInfo> callback)
    {
        string url = cloudGoodsURL + "GetUserFromWorld?appID=" + appID + "&platformID=" + platformID + "&platformUserID=" + platformUserID + "&userName=" + WWW.EscapeURL(userName) + "&loginUserEmail=" + userEmail;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetUserInfo(www, callback));
    }

    public void RegisterGameSession(Guid userID, string AppID, int instanceID, Action<Guid> callback)
    {
        string url = cloudGoodsURL + "RegisterSession?UserId=" + userID + "&AppID=" + AppID + "&InstanceId=" + instanceID;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetGuid(www, callback));
    }

    public void GetAccessPinFromGuid(string userPin, Action<string> callback)
    {
        string url = cloudGoodsURL + "GetUserInfoFromPin?pin=" + userPin;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    public void GetAccessPinForUser(string UserId, Action<string> callback)
    {
        string url = cloudGoodsURL + "GetUserPin?UserId=" + UserId;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    public void SPLogin_UserLogin(Guid gameID, string userEmail, string password, Action<SPLogin.SPLogin_Responce> callback)
    {
        string url = string.Format("{0}SPLoginUserLogin?gameID={1}&userEMail={2}&userPassword={3}", cloudGoodsURL, gameID, WWW.EscapeURL(userEmail), WWW.EscapeURL(password));

        WWW www = new WWW(url);

        StartCoroutine(ServiceSpLoginResponse(www, callback));
    }

    public void SPLogin_UserRegister(Guid gameID, string userEmail, string password, string userName, Action<SPLogin.SPLogin_Responce> callback)
    {
        string url = string.Format("{0}SPLoginUserRegister?gameID={1}&userEMail={2}&userPassword={3}&userName={4}", cloudGoodsURL, gameID, WWW.EscapeURL(userEmail), WWW.EscapeURL(password), WWW.EscapeURL(userName));

        WWW www = new WWW(url);

        StartCoroutine(ServiceSpLoginResponse(www, callback));
    }

    public void SPLoginForgotPassword(Guid gameID, string userEmail, Action<SPLogin.SPLogin_Responce> callback)
    {
        string url = string.Format("{0}SPLoginForgotPassword?gameID={1}&userEMail={2}", cloudGoodsURL, gameID, WWW.EscapeURL(userEmail));
        WWW www = new WWW(url);

        StartCoroutine(ServiceSpLoginResponse(www, callback));
    }


    public void SPLoginResendVerificationEmail(Guid gameID, string userEmail, Action<SPLogin.SPLogin_Responce> callback)
    {
        string url = string.Format("{0}SPLoginResendVerificationEmail?gameID={1}&userEMail={2}", cloudGoodsURL, gameID, WWW.EscapeURL(userEmail));
        WWW www = new WWW(url);

        StartCoroutine(ServiceSpLoginResponse(www, callback));
    }

    #endregion

    #region StoreCalls

    public void GetFreeCurrencyBalance(string userID, int accessLocation, string appID, Action<string> callback)
    {
        string url = cloudGoodsURL + "GetFreeCurrencyBalance?userID=" + userID + "&accessLocation=" + accessLocation + "&appID=" + appID;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    public void GetPaidCurrencyBalance(string userID, string appID, Action<string> callback)
    {
        string url = cloudGoodsURL + "GetPaidCurrencyBalance?userID=" + userID + "&appID=" + appID;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    public void GetStoreItems(string appID, Action<List<StoreItemInfo>> callback)
    {
        string url = cloudGoodsURL + "LoadStoreItems?appID=" + appID;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetStoreItems(www, callback));
    }

    public void StoreItemPurchase(string URL, Guid userID, int itemID, int amount, string paymentType, Guid appID, int saveLocation, Action<string> callback)
    {
        string url = URL + "StoreItemPurchase?UserID=" + userID + "&ItemID=" + itemID + "&Amount=" + amount + "&PaymentType=" + paymentType + "&AppID=" + appID + "&saveLocation=" + saveLocation;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));

    }

    public void GetItemBundles(string appID, Action<List<ItemBundle>> callback)
    {
        string url = cloudGoodsURL + "GetItemBundles?Appid=" + appID;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetItemBundles(www, callback));
    }

    public void PurchaseItemBundles(Guid appID, Guid UserID, int bundleID, string paymentType, int location, Action<string> callback)
    {
        string url = cloudGoodsURL + "PurchaseItemBundle?AppID=" + appID + "&UserID=" + UserID + "&BundleID=" + bundleID + "&PaymentType=" + paymentType + "&Location=" + location;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    public void GetCreditBundles(string appID, int platformID, Action<List<CreditBundleItem>> callback)
    {
        string url = cloudGoodsURL + "GetCreditBundles?Appid=" + appID + "&Platform=" + platformID;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetCreditBundles(www, callback));
    }

    public void PurchaseCreditBundles(Guid appId, string payload, Action<string> callback)
    {
        string url = cloudGoodsURL + "PurchaseCreditBundle?AppID=" + appId + "&payload=" + WWW.EscapeURL(EncryptStringUnity(payload));

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }


    public void GetToken(string appID, string securePayload, Action<string> callback)
    {
        string url = cloudGoodsURL + "GetToken?appID=" + appID + "&payload=" + WWW.EscapeURL(EncryptStringUnity(securePayload));

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, (x) => {
            SecureCall(DecryptString(AppSecret, x.Replace("\\", "")), "", callback);
        }));
    }

    public void SecureCall(string token, string securePayload, Action<string> callback)
    {
        List<WebModels.ItemsInfo> listOfItems = new List<WebModels.ItemsInfo>();

        WebModels.ItemsInfo item = new WebModels.ItemsInfo();
        item.amount = 1;
        item.ItemID = 106465;
        item.location = 0;
        listOfItems.Add(item);

        GiveOwnerItemWebserviceRequest request = new GiveOwnerItemWebserviceRequest();
        request.listOfItems = listOfItems;
        request.ownerID = "ef595214-369f-4313-9ac7-b0036e5ac25c";
        request.appID = GameAuthentication.GetAppID();
        request.OwnerType = WebModels.OwnerTypes.User;

        string newStringRequest = JsonConvert.SerializeObject(request);

        SecurePayload payload = new SecurePayload();
        payload.token = token;
        payload.data = newStringRequest;

        string securePayloadString = JsonConvert.SerializeObject(payload);

        Debug.Log(securePayloadString);

        string url = cloudGoodsURL + "SecureAction?appID=" + GameAuthentication.GetAppID() + "&payload=" + WWW.EscapeURL(EncryptStringUnity(securePayloadString));

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    public class SecurePayload
    {
        public string token;
        public string data;
    }

    public class GiveOwnerItemWebserviceRequest
    {
        public List<WebModels.ItemsInfo> listOfItems;
        public WebModels.OwnerTypes OwnerType;
        public string ownerID;
        public string appID;
    }

    #endregion

    #region RecipeCalls
    public void GetGameRecipes(string appID, Action<List<RecipeInfo>> callback)
    {
        string url = cloudGoodsURL + "GetRecipes?appID=" + appID;

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetRecipeInfos(www, callback));
    }

    public void CompleteQueueItem(Guid gameID, int QueueID, int percentScore, int location, Action<string> callback)
    {
        string url = string.Format("{0}CompleteQueueItem?gameID={1}&QueueID={2}&percentScore={3}&location={4}", cloudGoodsURL, gameID, QueueID, percentScore, location);
        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    public void AddInstantCraftToQueue(Guid gameID, Guid UserID, int ItemID, int Amount, List<KeyValuePair<string, int>> ItemIngredients, Action<string> callback)
    {
        string url = string.Format("{0}AddInstantCraftToQueue?gameID={1}&UserID={2}&ItemID={3}&Amount={4}&ItemIngredients={5}", cloudGoodsURL, gameID, UserID, ItemID, Amount, WWW.EscapeURL(JsonConvert.SerializeObject(ItemIngredients)));

        WWW www = new WWW(url);

        StartCoroutine(ServiceGetString(www, callback));
    }

    #endregion

    #region IEnumeratorCalls

    IEnumerator ServiceGetString(WWW www, Action<string> callback)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            callback(ServiceConverter.ConvertToString(www.text));
        }
        else
        {
            callback("WWW Error: " + www.error);
        }
    }

    IEnumerator ServiceGetUserInfo(WWW www, Action<UserInfo> callback)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log(www.text);
            callback(ServiceConverter.ConvertToUserInfo(www.text));
        }
        else
        {
            OnErrorEvent("Error:" + www.error);
        }
    }


    IEnumerator ServiceCallGetListItemDatas(WWW www, Action<List<ItemData>> callback)
    {
        yield return www;

        if (www.error == null)
            callback(ServiceConverter.ConvertToItemDataList(www.text));
        else
            OnErrorEvent(www.error);
    }

    IEnumerator ServiceGetStoreItems(WWW www, Action<List<StoreItemInfo>> callback)
    {
        yield return www;

        if (www.error == null)
            callback(ServiceConverter.ConvertToStoreItems(www.text));
        else
            OnErrorEvent(www.error);
    }

    IEnumerator ServiceGetGuid(WWW www, Action<Guid> callback)
    {
        yield return www;

        if (www.error == null)
            callback(ServiceConverter.ConvertToGuid(www.text));
        else
            OnErrorEvent(www.error);
    }

    IEnumerator ServiceGetRecipeInfos(WWW www, Action<List<RecipeInfo>> callback)
    {
        yield return www;

        if (www.error == null)
            callback(ServiceConverter.ConvertToListRecipeInfo(www.text));
        else
            OnErrorEvent(www.error);
    }

    IEnumerator ServiceGetItemBundles(WWW www, Action<List<ItemBundle>> callback)
    {
        yield return www;

        Debug.Log(www.text);

        if (www.error == null)
            callback(ServiceConverter.ConvertToListItemBundle(www.text));
        else
            OnErrorEvent(www.error);
    }

    IEnumerator ServiceGetCreditBundles(WWW www, Action<List<CreditBundleItem>> callback)
    {
        yield return www;
        Debug.Log(www.text);
        if (www.error == null)
            callback(ServiceConverter.ConvertToListCreditBundleItem(www.text));
        else
            OnErrorEvent(www.error);
    }

    IEnumerator ServiceMoveItemsResponse(WWW www, Action<MoveMultipleItemsResponse> callback)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            callback(ServiceConverter.ConvertToMoveMultipleItemsResponse(www.text));
        }
        else
        {
            OnErrorEvent("Error: " + www.error);
        }
    }

    IEnumerator ServiceSpLoginResponse(WWW www, Action<SPLogin.SPLogin_Responce> callback)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            callback(ServiceConverter.ConvertToSPLoginResponse(www.text));
        }
        else
        {
            OnErrorEvent("Error: " + www.error);
        }
    }

    #endregion

    public string EncryptStringUnity(string Message)
    {
        byte[] Results;

        UTF8Encoding UTF8 = new UTF8Encoding();


        MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();

        //TODO put in pass phrase
        byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(AppSecret));

        TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

        TDESAlgorithm.Key = TDESKey;
        TDESAlgorithm.Mode = CipherMode.ECB;
        TDESAlgorithm.Padding = PaddingMode.PKCS7;

        byte[] DataToEncrypt = UTF8.GetBytes(Message);

        try
        {
            ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
            Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
        }
        finally
        {
            TDESAlgorithm.Clear();
            HashProvider.Clear();
        }
        return Convert.ToBase64String(Results);
    }

    public static string DecryptString(string passphrase, string Message)
    {
        byte[] Results;
        UTF8Encoding UTF8 = new UTF8Encoding();

        MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
        byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));

        TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

        TDESAlgorithm.Key = TDESKey;
        TDESAlgorithm.Mode = CipherMode.ECB;
        TDESAlgorithm.Padding = PaddingMode.PKCS7;

        byte[] DataToDecrypt = Convert.FromBase64String(Message);

        try
        {
            ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
            Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
        }
        catch (Exception ex)
        {
            TDESAlgorithm.Clear();
            HashProvider.Clear();
            return ex.ToString();
        }
        finally
        {
            TDESAlgorithm.Clear();
            HashProvider.Clear();
        }
        return UTF8.GetString(Results);
    }
}
