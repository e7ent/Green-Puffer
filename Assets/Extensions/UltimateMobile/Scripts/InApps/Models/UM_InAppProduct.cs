using UnityEngine;
using System.Collections;


[System.Serializable]
public class UM_InAppProduct  {

	public bool IsConsumable;
	public bool IsOpen;
	public string id = "new_product";


	public string IOSId = string.Empty;
	public string AndroidId  = string.Empty;
	public string WP8Id  = string.Empty;



	private WP8ProductTemplate	   	_WP8Template		= new WP8ProductTemplate();
	private IOSProductTemplate		_IOSTemplate 		= new IOSProductTemplate();
	private GoogleProductTemplate  	_AndroidTemplate 	= new GoogleProductTemplate();
	private UM_InAppProductTemplate _template 			= new UM_InAppProductTemplate();



	public WP8ProductTemplate WP8Template {
		get {
			return _WP8Template;
		}
	}

	public IOSProductTemplate IOSTemplate {
		get {
			return _IOSTemplate;
		}
	}

	public GoogleProductTemplate AndroidTemplate {
		get {
			return _AndroidTemplate;
		}
	}

	public UM_InAppProductTemplate template {
		get {
			return _template;
		}
	}


	public void SetTemplate(WP8ProductTemplate tpl) {
		_WP8Template = tpl;
		_template = new UM_InAppProductTemplate();
		_template.id = tpl.ProductId;
		_template.title = tpl.Name;
		_template.description = tpl.Description;
		_template.price = tpl.Price;

	}

	public void SetTemplate(IOSProductTemplate tpl) {
		_IOSTemplate = tpl;
		_template = new UM_InAppProductTemplate();
		_template.id = tpl.id;
		_template.title = tpl.title;
		_template.description = tpl.description;
		_template.price = tpl.price;
	}

	public void SetTemplate(GoogleProductTemplate tpl) {
		_AndroidTemplate = tpl;
		_template = new UM_InAppProductTemplate();
		_template.id = tpl.SKU;
		_template.title = tpl.title;
		_template.description = tpl.description;
		_template.price = tpl.price;
	}

	public string Title  {
		get {
			switch(Application.platform) {
				
			case RuntimePlatform.Android:
				return _AndroidTemplate.title;
			case RuntimePlatform.IPhonePlayer:
				return _IOSTemplate.title;
			case RuntimePlatform.WP8Player:
				return _WP8Template.Name;
			}

			return string.Empty;
		}

	}

	public string Description  {
		get {
			switch(Application.platform) {
				
			case RuntimePlatform.Android:
				return _AndroidTemplate.description;
			case RuntimePlatform.IPhonePlayer:
				return _IOSTemplate.description;
			case RuntimePlatform.WP8Player:
				return _WP8Template.Description;
			}

			return string.Empty;
		}
	}

	public string Price  {
		get {
			switch(Application.platform) {
				
			case RuntimePlatform.Android:
				return _AndroidTemplate.price;
			case RuntimePlatform.IPhonePlayer:
				return _IOSTemplate.localizedPrice;
			case RuntimePlatform.WP8Player:
				return _WP8Template.Price;
			}

			return string.Empty;
		}
	}


}
