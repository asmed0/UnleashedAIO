using Discord;
using Discord.Webhook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnleashedAIO.Modules
{
    class WebhookSend
    {
        public static readonly string publicWebhook = "/api/webhooks/752926808307007510/7fTzMV1nkxVFOHjvyPK4AOfykQtNw2zNw77WFidSu7D2FQckrMc_U0LhfzTJqCFA_HVr";
        public async void Send(DiscordWebhookClient webhookClient, string site, string url, string size, string description, TimeSpan checkoutTimeSpan = default, string orderNumber = default, string sku = default)
        {
            var successEmbed = new EmbedBuilder
            {
                Title = "⛓️ UnleashedAIO Success",
                Description = description,
                Footer = new EmbedFooterBuilder
                {
                    IconUrl = "https://cdn.discordapp.com/attachments/740309778785632406/764527828183547904/image0.jpg",
                    Text = $"UnleashedAIO | Version {Program.version}"
                },
                Color = Discord.Color.Green
            };
            successEmbed.AddField("Store", site, false);
            successEmbed.AddField("Product", url, false);
            successEmbed.AddField("Size", size, false);
            if (sku != default)
            {
                successEmbed.ThumbnailUrl = $"https://images.footlocker.com/is/image/FLEU/{sku}?wid=763&hei=538&fmt=png-alpha";
            }
            if (orderNumber != default)
            {
                successEmbed.AddField("Order number", $"||{orderNumber}||", true);
            }
            if (checkoutTimeSpan != default)
            {
                successEmbed.AddField("Checkout Time", $"{checkoutTimeSpan.Minutes}minute(s) {checkoutTimeSpan.Seconds}second(s)", true);
            }
            var publicEmbed = new List<Embed> { successEmbed.Build() }.AsEnumerable();
            if (webhookClient != null) await webhookClient.SendMessageAsync("", false, publicEmbed);
        }
    }
}
