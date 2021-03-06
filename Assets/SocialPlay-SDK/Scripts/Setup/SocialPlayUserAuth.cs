using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CloudGoodsSDK.Models;
using System.Collections.Generic;

public interface ISocialPlayerUserGetter
{
    void GetSocialPlayUser(PlatformUser socialPlayUser, Action<WebserviceCalls.UserInfo> callback);
}

public class SocialPlayUserWebServiceGetter : ISocialPlayerUserGetter
{
    public void GetSocialPlayUser(PlatformUser socialPlayUser, Action<WebserviceCalls.UserInfo> callback)
    {
        WebserviceCalls webserviceCalls = GameObject.FindObjectOfType(typeof(WebserviceCalls)) as WebserviceCalls;
        //todo remove email
        webserviceCalls.GetUserFromWorld(socialPlayUser.appID, socialPlayUser.platformID, socialPlayUser.platformUserID, socialPlayUser.userName, null, (WebserviceCalls.UserInfo x) =>
        {
            WebserviceCalls.UserInfo userGuid = x;
            callback(userGuid);
        });
    }
}
