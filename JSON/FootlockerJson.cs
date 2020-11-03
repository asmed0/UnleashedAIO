using System.Collections.Generic;

namespace UnleashedAIO.JSON
{
    //SubmitShipping Classes
    public class Country
    {
        public string isocode { get; set; }
        public string name { get; set; }
    }

    public class SubmitShipping
    {
        public bool setAsDefaultBilling { get; set; }
        public bool setAsDefaultShipping { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool email { get; set; }
        public string phone { get; set; }
        public Country country { get; set; }
        public object id { get; set; }
        public bool setAsBilling { get; set; }
        public string type { get; set; }
        public string line1 { get; set; }
        public string postalCode { get; set; }
        public string town { get; set; }
        public bool shippingAddress { get; set; }
    }


    //ATC Classes
    public class ATC
    {
        public int productQuantity { get; set; }
        public string productId { get; set; }
    }

    //StockCheck Classes
    public class Category
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Variation
    {
        public string altText { get; set; }
        public string format { get; set; }
        public string url { get; set; }
    }

    public class Image
    {
        public string code { get; set; }
        public List<Variation> variations { get; set; }
    }

    public class Attribute
    {
        public string id { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Price
    {
        public string currencyIso { get; set; }
        public string formattedOriginalPrice { get; set; }
        public string formattedValue { get; set; }
        public double originalPrice { get; set; }
        public double value { get; set; }
    }

    public class SellableUnit
    {
        public List<Attribute> attributes { get; set; }
        public string barCode { get; set; }
        public string code { get; set; }
        public bool isBackOrderable { get; set; }
        public bool isPreOrder { get; set; }
        public bool isRecaptchaOn { get; set; }
        public Price price { get; set; }
        public bool singleStoreInventory { get; set; }
        public bool sizeAvailableInStores { get; set; }
        public string sku { get; set; }
        public string stockLevelStatus { get; set; }
    }

    public class SizeChartGridMap
    {
        public string label { get; set; }
        public List<string> sizes { get; set; }
    }

    public class Price2
    {
        public string currencyIso { get; set; }
        public string formattedOriginalPrice { get; set; }
        public string formattedValue { get; set; }
        public double originalPrice { get; set; }
        public double value { get; set; }
    }

    public class VariantAttribute
    {
        public string code { get; set; }
        public bool displayCountDownTimer { get; set; }
        public bool freeShipping { get; set; }
        public string imageSku { get; set; }
        public bool isSelected { get; set; }
        public bool launchProduct { get; set; }
        public bool mapEnable { get; set; }
        public Price2 price { get; set; }
        public bool recaptchaOn { get; set; }
        public bool riskified { get; set; }
        public bool shipToAndFromStore { get; set; }
        public bool shippingRestrictionExists { get; set; }
        public string sku { get; set; }
        public bool skuExclusions { get; set; }
        public string stockLevelStatus { get; set; }
        public bool webOnlyLaunch { get; set; }
    }

    public class FootlockerJson
    {
        public string brand { get; set; }
        public List<Category> categories { get; set; }
        public string description { get; set; }
        public bool dropShip { get; set; }
        public bool freeShipping { get; set; }
        public List<Image> images { get; set; }
        public bool isNewProduct { get; set; }
        public bool isSaleProduct { get; set; }
        public string modelNumber { get; set; }
        public string name { get; set; }
        public List<SellableUnit> sellableUnits { get; set; }
        public List<SizeChartGridMap> sizeChartGridMap { get; set; }
        public string sizeChartTipTx { get; set; }
        public List<VariantAttribute> variantAttributes { get; set; }
    }
}
