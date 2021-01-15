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
        public static readonly DiscordWebhookClient publicWebhook = new DiscordWebhookClient("https://discord.com/api/webhooks/799089710218870826/X5jJpLxnvdhljDaHTDB6xEQsZhCjkZ4lfQqEpDJV0N3gzjaBtreKsQz-DZeXHgmDukQb");
        public async void Send(DiscordWebhookClient webhookClient, string site, string url, string size, string description, TimeSpan checkoutTimeSpan = default, string orderNumber = default)
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
            successEmbed.AddField("Store", site, true);
            successEmbed.AddField("Product", url, true);
            successEmbed.AddField("Size", size, true);
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
