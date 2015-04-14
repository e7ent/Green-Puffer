using UnityEngine;
using System.Collections;

public class UM_PurchaseResult  {
	
	public bool isSuccess =  false;
	public UM_InAppProduct product =  new UM_InAppProduct();


	
	public GooglePurchaseTemplate Google_PurchaseInfo = null;
	public IOSStoreKitResponse IOS_PurchaseInfo = null;
	public WP8PurchseResponce WP8_PurchaseInfo = null;
}
