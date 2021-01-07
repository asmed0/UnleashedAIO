using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UnleashedAIO.JSON
{
    public class FootlockerJSON
    {
        // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
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
