using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class DisplayNewestItems : MonoBehaviour
{
    public WebserviceCalls webservicecalls;
    public NGUIStoreLoader storeLoader;

    public string appID;
    public FilterNewestItems.SortTimeType timeFilterType = FilterNewestItems.SortTimeType.hours;
    public int itemDisplayCount = 0;
    public int timeDifference = 5;

    FilterNewestItems newestItemFilter = new FilterNewestItems();
    List<StoreItemInfo> items = new List<StoreItemInfo>();

    void Start()
    {
        if (!webservicecalls)
            webservicecalls = GameObject.Find("WebserviceCalls").GetComponent<WebserviceCalls>();
        if (!storeLoader)
            storeLoader = this.gameObject.GetComponent<NGUIStoreLoader>();

        webservicecalls.GetStoreItems(appID, OnReceivedStoreItems);
    }

    void OnReceivedStoreItems(List<StoreItemInfo> storeItems)
    {
            for (int i = 0; i < storeItems.Count; i++)
            {
                items.Add(storeItems[i]);
            }
      

        FilterItemsByDateTime();
    }

    void FilterItemsByDateTime()
    {
        List<StoreItemInfo> newestItems = newestItemFilter.FilterItems(items, timeFilterType, itemDisplayCount, timeDifference);

        storeLoader.LoadStoreWithPaging(newestItems, 0);
    }
}
