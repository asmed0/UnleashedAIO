using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UnleashedAIO.JSON
{
    public class FootlockerJSON
    {
        // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);

        public class ShippingResponse
        {
            public class Country
            {
                [JsonProperty("isocode")]
                public string Isocode { get; set; }
            }

            public class Root
            {
                [JsonProperty("billingAddress")]
                public bool BillingAddress { get; set; }

                [JsonProperty("country")]
                public Country Country { get; set; }

                [JsonProperty("defaultAddress")]
                public bool DefaultAddress { get; set; }

                [JsonProperty("firstName")]
                public string FirstName { get; set; }

                [JsonProperty("id")]
                public string Id { get; set; }

                [JsonProperty("lastName")]
                public string LastName { get; set; }

                [JsonProperty("line1")]
                public string Line1 { get; set; }

                [JsonProperty("phone")]
                public string Phone { get; set; }

                [JsonProperty("postalCode")]
                public string PostalCode { get; set; }

                [JsonProperty("setAsBilling")]
                public bool SetAsBilling { get; set; }

                [JsonProperty("shippingAddress")]
                public bool ShippingAddress { get; set; }

                [JsonProperty("town")]
                public string Town { get; set; }

                [JsonProperty("visibleInAddressBook")]
                public bool VisibleInAddressBook { get; set; }
            }


        }
        public class Shipping
        {
            public class Country
            {
                [JsonProperty("isocode")]
                public string Isocode { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }
            }

            public class ShippingAddress
            {
                [JsonProperty("setAsDefaultBilling")]
                public bool SetAsDefaultBilling { get; set; }

                [JsonProperty("setAsDefaultShipping")]
                public bool SetAsDefaultShipping { get; set; }

                [JsonProperty("firstName")]
                public string FirstName { get; set; }

                [JsonProperty("lastName")]
                public string LastName { get; set; }

                [JsonProperty("email")]
                public bool Email { get; set; }

                [JsonProperty("phone")]
                public string Phone { get; set; }

                [JsonProperty("country")]
                public Country Country { get; set; }

                [JsonProperty("id")]
                public string Id { get; set; }

                [JsonProperty("setAsBilling")]
                public bool SetAsBilling { get; set; }

                [JsonProperty("type")]
                public string Type { get; set; }

                [JsonProperty("line1")]
                public string Line1 { get; set; }

                [JsonProperty("postalCode")]
                public string PostalCode { get; set; }

                [JsonProperty("town")]
                public string Town { get; set; }

                [JsonProperty("shippingAddress")]
                public bool ShippingAddressInside { get; set; }
            }

            public class Root
            {
                [JsonProperty("shippingAddress")]
                public ShippingAddress ShippingAddress { get; set; }
            }
        }
        public class Cart
        {
            public class Image
            {
                [JsonProperty("altText")]
                public string AltText { get; set; }

                [JsonProperty("format")]
                public string Format { get; set; }

                [JsonProperty("url")]
                public string Url { get; set; }
            }

            public class PriceData
            {
                [JsonProperty("currencyIso")]
                public string CurrencyIso { get; set; }

                [JsonProperty("formattedOriginalPrice")]
                public string FormattedOriginalPrice { get; set; }

                [JsonProperty("formattedValue")]
                public string FormattedValue { get; set; }

                [JsonProperty("originalPrice")]
                public double OriginalPrice { get; set; }

                [JsonProperty("priceType")]
                public string PriceType { get; set; }

                [JsonProperty("value")]
                public double Value { get; set; }
            }

            public class Stock
            {
                [JsonProperty("stockLevel")]
                public int StockLevel { get; set; }

                [JsonProperty("stockLevelStatus")]
                public string StockLevelStatus { get; set; }
            }

            public class VariantOption
            {
                [JsonProperty("ageBucket")]
                public List<object> AgeBucket { get; set; }

                [JsonProperty("backOrderable")]
                public bool BackOrderable { get; set; }

                [JsonProperty("code")]
                public string Code { get; set; }

                [JsonProperty("images")]
                public List<object> Images { get; set; }

                [JsonProperty("mapEnable")]
                public bool MapEnable { get; set; }

                [JsonProperty("mobileBarCode")]
                public string MobileBarCode { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("potentialPromotions")]
                public List<object> PotentialPromotions { get; set; }

                [JsonProperty("preOrder")]
                public bool PreOrder { get; set; }

                [JsonProperty("priceData")]
                public PriceData PriceData { get; set; }

                [JsonProperty("shippingRestrictionExists")]
                public bool ShippingRestrictionExists { get; set; }

                [JsonProperty("size")]
                public string Size { get; set; }

                [JsonProperty("sizeAvailableInStores")]
                public bool SizeAvailableInStores { get; set; }

                [JsonProperty("stock")]
                public Stock Stock { get; set; }

                [JsonProperty("variantOptionQualifiers")]
                public List<object> VariantOptionQualifiers { get; set; }

                [JsonProperty("variantOptions")]
                public List<object> VariantOptions { get; set; }
            }

            public class Selected
            {
                [JsonProperty("ageBucket")]
                public List<object> AgeBucket { get; set; }

                [JsonProperty("backOrderable")]
                public bool BackOrderable { get; set; }

                [JsonProperty("code")]
                public string Code { get; set; }

                [JsonProperty("displayCountDownTimer")]
                public bool DisplayCountDownTimer { get; set; }

                [JsonProperty("images")]
                public List<Image> Images { get; set; }

                [JsonProperty("launchProduct")]
                public bool LaunchProduct { get; set; }

                [JsonProperty("mapEnable")]
                public bool MapEnable { get; set; }

                [JsonProperty("mobileBarCode")]
                public string MobileBarCode { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("potentialPromotions")]
                public List<object> PotentialPromotions { get; set; }

                [JsonProperty("preOrder")]
                public bool PreOrder { get; set; }

                [JsonProperty("priceData")]
                public PriceData PriceData { get; set; }

                [JsonProperty("recaptchaOn")]
                public bool RecaptchaOn { get; set; }

                [JsonProperty("shippingRestrictionExists")]
                public bool ShippingRestrictionExists { get; set; }

                [JsonProperty("size")]
                public string Size { get; set; }

                [JsonProperty("sizeAvailableInStores")]
                public bool SizeAvailableInStores { get; set; }

                [JsonProperty("sku")]
                public string Sku { get; set; }

                [JsonProperty("stock")]
                public Stock Stock { get; set; }

                [JsonProperty("style")]
                public string Style { get; set; }

                [JsonProperty("variantOptionQualifiers")]
                public List<object> VariantOptionQualifiers { get; set; }

                [JsonProperty("variantOptions")]
                public List<VariantOption> VariantOptions { get; set; }

                [JsonProperty("imageSku")]
                public string ImageSku { get; set; }
            }

            public class BaseOption
            {
                [JsonProperty("options")]
                public List<object> Options { get; set; }

                [JsonProperty("selected")]
                public Selected Selected { get; set; }

                [JsonProperty("variantType")]
                public string VariantType { get; set; }
            }

            public class Price
            {
                [JsonProperty("currencyIso")]
                public string CurrencyIso { get; set; }

                [JsonProperty("formattedValue")]
                public string FormattedValue { get; set; }

                [JsonProperty("priceType")]
                public string PriceType { get; set; }

                [JsonProperty("value")]
                public double Value { get; set; }
            }

            public class Product
            {
                [JsonProperty("baseOptions")]
                public List<BaseOption> BaseOptions { get; set; }

                [JsonProperty("baseProduct")]
                public string BaseProduct { get; set; }

                [JsonProperty("categories")]
                public List<object> Categories { get; set; }

                [JsonProperty("classifications")]
                public List<object> Classifications { get; set; }

                [JsonProperty("code")]
                public string Code { get; set; }

                [JsonProperty("description")]
                public string Description { get; set; }

                [JsonProperty("displayCountDownTimer")]
                public bool DisplayCountDownTimer { get; set; }

                [JsonProperty("freeShipping")]
                public bool FreeShipping { get; set; }

                [JsonProperty("futureStocks")]
                public List<object> FutureStocks { get; set; }

                [JsonProperty("giftCosts")]
                public List<object> GiftCosts { get; set; }

                [JsonProperty("images")]
                public List<object> Images { get; set; }

                [JsonProperty("launchProduct")]
                public bool LaunchProduct { get; set; }

                [JsonProperty("potentialPromotions")]
                public List<object> PotentialPromotions { get; set; }

                [JsonProperty("price")]
                public Price Price { get; set; }

                [JsonProperty("productReferences")]
                public List<object> ProductReferences { get; set; }

                [JsonProperty("reviews")]
                public List<object> Reviews { get; set; }

                [JsonProperty("sizeChartGridMap")]
                public List<object> SizeChartGridMap { get; set; }

                [JsonProperty("skuExclusions")]
                public bool SkuExclusions { get; set; }

                [JsonProperty("stock")]
                public Stock Stock { get; set; }

                [JsonProperty("styleVariantCode")]
                public List<object> StyleVariantCode { get; set; }

                [JsonProperty("variantMatrix")]
                public List<object> VariantMatrix { get; set; }

                [JsonProperty("variantOptions")]
                public List<object> VariantOptions { get; set; }

                [JsonProperty("volumePrices")]
                public List<object> VolumePrices { get; set; }
            }

            public class TotalPrice
            {
                [JsonProperty("currencyIso")]
                public string CurrencyIso { get; set; }

                [JsonProperty("formattedValue")]
                public string FormattedValue { get; set; }

                [JsonProperty("priceType")]
                public string PriceType { get; set; }

                [JsonProperty("value")]
                public double Value { get; set; }
            }

            public class Entry
            {
                [JsonProperty("applicableDeliveryModes")]
                public List<object> ApplicableDeliveryModes { get; set; }

                [JsonProperty("entryNumber")]
                public int EntryNumber { get; set; }

                [JsonProperty("product")]
                public Product Product { get; set; }

                [JsonProperty("productPriceVariation")]
                public bool ProductPriceVariation { get; set; }

                [JsonProperty("quantity")]
                public int Quantity { get; set; }

                [JsonProperty("shippingRestricted")]
                public bool ShippingRestricted { get; set; }

                [JsonProperty("totalPrice")]
                public TotalPrice TotalPrice { get; set; }
            }

            public class OrderDiscounts
            {
                [JsonProperty("currencyIso")]
                public string CurrencyIso { get; set; }

                [JsonProperty("formattedValue")]
                public string FormattedValue { get; set; }

                [JsonProperty("priceType")]
                public string PriceType { get; set; }

                [JsonProperty("value")]
                public double Value { get; set; }
            }

            public class ProductDiscounts
            {
                [JsonProperty("currencyIso")]
                public string CurrencyIso { get; set; }

                [JsonProperty("formattedValue")]
                public string FormattedValue { get; set; }

                [JsonProperty("priceType")]
                public string PriceType { get; set; }

                [JsonProperty("value")]
                public double Value { get; set; }
            }

            public class SubTotal
            {
                [JsonProperty("currencyIso")]
                public string CurrencyIso { get; set; }

                [JsonProperty("formattedValue")]
                public string FormattedValue { get; set; }

                [JsonProperty("priceType")]
                public string PriceType { get; set; }

                [JsonProperty("value")]
                public double Value { get; set; }
            }

            public class TotalDiscounts
            {
                [JsonProperty("currencyIso")]
                public string CurrencyIso { get; set; }

                [JsonProperty("formattedValue")]
                public string FormattedValue { get; set; }

                [JsonProperty("priceType")]
                public string PriceType { get; set; }

                [JsonProperty("value")]
                public double Value { get; set; }
            }

            public class TotalPriceWithTax
            {
                [JsonProperty("currencyIso")]
                public string CurrencyIso { get; set; }

                [JsonProperty("formattedValue")]
                public string FormattedValue { get; set; }

                [JsonProperty("priceType")]
                public string PriceType { get; set; }

                [JsonProperty("value")]
                public double Value { get; set; }
            }

            public class TotalTax
            {
                [JsonProperty("currencyIso")]
                public string CurrencyIso { get; set; }

                [JsonProperty("formattedValue")]
                public string FormattedValue { get; set; }

                [JsonProperty("priceType")]
                public string PriceType { get; set; }

                [JsonProperty("value")]
                public double Value { get; set; }
            }

            public class Root
            {
                [JsonProperty("appliedCoupons")]
                public List<object> AppliedCoupons { get; set; }

                [JsonProperty("appliedOrderPromotions")]
                public List<object> AppliedOrderPromotions { get; set; }

                [JsonProperty("appliedProductPromotions")]
                public List<object> AppliedProductPromotions { get; set; }

                [JsonProperty("appliedVouchers")]
                public List<object> AppliedVouchers { get; set; }

                [JsonProperty("cartMerged")]
                public bool CartMerged { get; set; }

                [JsonProperty("code")]
                public string Code { get; set; }

                [JsonProperty("deliveryOrderGroups")]
                public List<object> DeliveryOrderGroups { get; set; }

                [JsonProperty("eligiblePaymentTypesForCart")]
                public List<object> EligiblePaymentTypesForCart { get; set; }

                [JsonProperty("entries")]
                public List<Entry> Entries { get; set; }

                [JsonProperty("gfPaymentInfo")]
                public List<object> GfPaymentInfo { get; set; }

                [JsonProperty("giftBoxAdded")]
                public bool GiftBoxAdded { get; set; }

                [JsonProperty("giftOrder")]
                public bool GiftOrder { get; set; }

                [JsonProperty("guid")]
                public string Guid { get; set; }

                [JsonProperty("isCartContainGiftCard")]
                public bool IsCartContainGiftCard { get; set; }

                [JsonProperty("isCarthasOnlyEMailGiftCard")]
                public bool IsCarthasOnlyEMailGiftCard { get; set; }

                [JsonProperty("orderDiscounts")]
                public OrderDiscounts OrderDiscounts { get; set; }

                [JsonProperty("outOfStockProducts")]
                public List<object> OutOfStockProducts { get; set; }

                [JsonProperty("pickupOrderGroups")]
                public List<object> PickupOrderGroups { get; set; }

                [JsonProperty("potentialOrderPromotions")]
                public List<object> PotentialOrderPromotions { get; set; }

                [JsonProperty("potentialProductPromotions")]
                public List<object> PotentialProductPromotions { get; set; }

                [JsonProperty("productDiscounts")]
                public ProductDiscounts ProductDiscounts { get; set; }

                [JsonProperty("subTotal")]
                public SubTotal SubTotal { get; set; }

                [JsonProperty("totalDiscounts")]
                public TotalDiscounts TotalDiscounts { get; set; }

                [JsonProperty("totalItems")]
                public int TotalItems { get; set; }

                [JsonProperty("totalPrice")]
                public TotalPrice TotalPrice { get; set; }

                [JsonProperty("totalPriceWithTax")]
                public TotalPriceWithTax TotalPriceWithTax { get; set; }

                [JsonProperty("totalTax")]
                public TotalTax TotalTax { get; set; }

                [JsonProperty("totalUnitCount")]
                public int TotalUnitCount { get; set; }

                [JsonProperty("type")]
                public string Type { get; set; }
            }

        }
        public class Session
        {
            public class User
            {
                [JsonProperty("firstName")]
                public string FirstName { get; set; }

                [JsonProperty("serverUTC")]
                public DateTime ServerUTC { get; set; }

                [JsonProperty("optIn")]
                public bool OptIn { get; set; }

                [JsonProperty("militaryVerified")]
                public bool MilitaryVerified { get; set; }

                [JsonProperty("loyaltyStatus")]
                public bool LoyaltyStatus { get; set; }

                [JsonProperty("ssoComplete")]
                public bool SsoComplete { get; set; }

                [JsonProperty("vipUser")]
                public bool VipUser { get; set; }

                [JsonProperty("authenticated")]
                public bool Authenticated { get; set; }

                [JsonProperty("loyalty")]
                public bool Loyalty { get; set; }

                [JsonProperty("recognized")]
                public bool Recognized { get; set; }

                [JsonProperty("vip")]
                public bool Vip { get; set; }
            }

            public class Cart
            {
            }

            public class Data
            {
                [JsonProperty("csrfToken")]
                public string CsrfToken { get; set; }

                [JsonProperty("user")]
                public User User { get; set; }

                [JsonProperty("cart")]
                public Cart Cart { get; set; }
            }

            public class Root
            {
                [JsonProperty("data")]
                public Data Data { get; set; }

                [JsonProperty("success")]
                public bool Success { get; set; }

                [JsonProperty("errors")]
                public List<object> Errors { get; set; }
            }


        }

        public class Product
        {
            public class Category
            {
                [JsonPropertyName("code")]
                public string Code { get; set; }

                [JsonPropertyName("name")]
                public string Name { get; set; }
            }

            public class Variation
            {
                [JsonPropertyName("altText")]
                public string AltText { get; set; }

                [JsonPropertyName("format")]
                public string Format { get; set; }

                [JsonPropertyName("url")]
                public string Url { get; set; }
            }

            public class Image
            {
                [JsonPropertyName("code")]
                public string Code { get; set; }

                [JsonPropertyName("variations")]
                public List<Variation> Variations { get; set; }
            }

            public class Attribute
            {
                [JsonPropertyName("id")]
                public string Id { get; set; }

                [JsonPropertyName("type")]
                public string Type { get; set; }

                [JsonPropertyName("value")]
                public string Value { get; set; }
            }

            public class Price
            {
                [JsonPropertyName("currencyIso")]
                public string CurrencyIso { get; set; }

                [JsonPropertyName("formattedOriginalPrice")]
                public string FormattedOriginalPrice { get; set; }

                [JsonPropertyName("formattedValue")]
                public string FormattedValue { get; set; }

                [JsonPropertyName("originalPrice")]
                public double OriginalPrice { get; set; }

                [JsonPropertyName("value")]
                public double Value { get; set; }
            }

            public class SellableUnit
            {
                [JsonPropertyName("attributes")]
                public List<Attribute> Attributes { get; set; }

                [JsonPropertyName("barCode")]
                public string BarCode { get; set; }

                [JsonPropertyName("code")]
                public string Code { get; set; }

                [JsonPropertyName("isBackOrderable")]
                public bool IsBackOrderable { get; set; }

                [JsonPropertyName("isPreOrder")]
                public bool IsPreOrder { get; set; }

                [JsonPropertyName("isRecaptchaOn")]
                public bool IsRecaptchaOn { get; set; }

                [JsonPropertyName("price")]
                public Price Price { get; set; }

                [JsonPropertyName("singleStoreInventory")]
                public bool SingleStoreInventory { get; set; }

                [JsonPropertyName("sizeAvailableInStores")]
                public bool SizeAvailableInStores { get; set; }

                [JsonPropertyName("sku")]
                public string Sku { get; set; }

                [JsonPropertyName("stockLevelStatus")]
                public string StockLevelStatus { get; set; }
            }

            public class SizeChartGridMap
            {
                [JsonPropertyName("label")]
                public string Label { get; set; }

                [JsonPropertyName("sizes")]
                public List<string> Sizes { get; set; }
            }

            public class VariantAttribute
            {
                [JsonPropertyName("code")]
                public string Code { get; set; }

                [JsonPropertyName("cstSkuLaunchDate")]
                public string CstSkuLaunchDate { get; set; }

                [JsonPropertyName("definedTimeForCountDown")]
                public string DefinedTimeForCountDown { get; set; }

                [JsonPropertyName("displayCountDownTimer")]
                public bool DisplayCountDownTimer { get; set; }

                [JsonPropertyName("freeShipping")]
                public bool FreeShipping { get; set; }

                [JsonPropertyName("imageSku")]
                public string ImageSku { get; set; }

                [JsonPropertyName("isSelected")]
                public bool IsSelected { get; set; }

                [JsonPropertyName("launchProduct")]
                public bool LaunchProduct { get; set; }

                [JsonPropertyName("mapEnable")]
                public bool MapEnable { get; set; }

                [JsonPropertyName("pdpActivationDate")]
                public string PdpActivationDate { get; set; }

                [JsonPropertyName("price")]
                public Price Price { get; set; }

                [JsonPropertyName("recaptchaOn")]
                public bool RecaptchaOn { get; set; }

                [JsonPropertyName("riskified")]
                public bool Riskified { get; set; }

                [JsonPropertyName("shipToAndFromStore")]
                public bool ShipToAndFromStore { get; set; }

                [JsonPropertyName("shippingRestrictionExists")]
                public bool ShippingRestrictionExists { get; set; }

                [JsonPropertyName("sku")]
                public string Sku { get; set; }

                [JsonPropertyName("skuExclusions")]
                public bool SkuExclusions { get; set; }

                [JsonPropertyName("skuLaunchDate")]
                public string SkuLaunchDate { get; set; }

                [JsonPropertyName("stockLevelStatus")]
                public string StockLevelStatus { get; set; }

                [JsonPropertyName("webOnlyLaunch")]
                public bool WebOnlyLaunch { get; set; }
            }

            public class Root
            {
                [JsonPropertyName("brand")]
                public string Brand { get; set; }

                [JsonPropertyName("categories")]
                public List<Category> Categories { get; set; }

                [JsonPropertyName("description")]
                public string Description { get; set; }

                [JsonPropertyName("dropShip")]
                public bool DropShip { get; set; }

                [JsonPropertyName("freeShipping")]
                public bool FreeShipping { get; set; }

                [JsonPropertyName("images")]
                public List<Image> Images { get; set; }

                [JsonPropertyName("isNewProduct")]
                public bool IsNewProduct { get; set; }

                [JsonPropertyName("isSaleProduct")]
                public bool IsSaleProduct { get; set; }

                [JsonPropertyName("modelNumber")]
                public string ModelNumber { get; set; }

                [JsonPropertyName("name")]
                public string Name { get; set; }

                [JsonPropertyName("sellableUnits")]
                public List<SellableUnit> SellableUnits { get; set; }

                [JsonPropertyName("sizeChartGridMap")]
                public List<SizeChartGridMap> SizeChartGridMap { get; set; }

                [JsonPropertyName("sizeChartTipTx")]
                public string SizeChartTipTx { get; set; }

                [JsonPropertyName("variantAttributes")]
                public List<VariantAttribute> VariantAttributes { get; set; }
            }
        }
    }
}
