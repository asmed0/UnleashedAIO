using System;
using System.Collections.Generic;
using System.Text;

namespace UnleashedAIO.JSON
{
    class ZalandoJson
    {
        public class NewCustomerData
        {
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string email { get; set; }
            public string password { get; set; }
            public List<string> fashion_preference { get; set; }
            public bool subscribe_to_news_letter { get; set; }
            public bool accepts_terms_and_conditions { get; set; }
        }

        public class Register
        {
            public NewCustomerData newCustomerData { get; set; }
            public string wnaMode { get; set; }
        }

        public class ATC
        {
            public class AddToCartInput
            {
                public string productId { get; set; }
                public string clientMutationId { get; set; }
            }

            public class Variables
            {
                public AddToCartInput addToCartInput { get; set; }
            }

            public class Array
            {
                public string id { get; set; }
                public Variables variables { get; set; }
            }

            public class Head
            {
                public List<Array> Array { get; set; }
            }
        }

        public class Shipping
        {
            public string type { get; set; }
            public string city { get; set; }
            public string countryCode { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string street { get; set; }
            public string additional { get; set; }
            public string gender { get; set; }
            public bool defaultBilling { get; set; }
            public bool defaultShipping { get; set; }
            public string zip { get; set; }
        }

        public class ShippingResponse
        {
            public string city { get; set; }
            public string countryCode { get; set; }
            public bool defaultBilling { get; set; }
            public bool defaultShipping { get; set; }
            public string firstname { get; set; }
            public string gender { get; set; }
            public int id { get; set; }
            public string lastname { get; set; }
            public string street { get; set; }
            public string type { get; set; }
            public string zip { get; set; }
        }

        public class SetDefaultShip
        {
            public bool isDefaultShipping { get; set; }
        }
        public class CheckoutInfo
        {
            public string checkoutId { get; set; }
            public string eTag { get; set; }
        }
        public class CreditCardJson
        {
            public string id { get; set; }
        }
    }
}
