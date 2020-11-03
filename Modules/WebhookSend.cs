using Discord;
using Discord.Webhook;
using System;
using System.Collections.Generic;
using System.Linq;
namespace UnleashedAIO.Modules
{
    class WebhookSend
    {
        public static readonly DiscordWebhookClient publicWebhook = new DiscordWebhookClient("https://discordapp.com/api/webhooks/752926808307007510/7fTzMV1nkxVFOHjvyPK4AOfykQtNw2zNw77WFidSu7D2FQckrMc_U0LhfzTJqCFA_HVr");
        public async void Send(DiscordWebhookClient webhookClient, string site, string url, string size, string description, TimeSpan checkoutTimeSpan = default)
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
            successEmbed.AddField("Site", site, true);
            successEmbed.AddField("Url", url, true);
            successEmbed.AddField("Size", size, true);
            if (checkoutTimeSpan != default)
            {
                successEmbed.AddField("Checkout Time", $"{checkoutTimeSpan.Minutes}minute(s) {checkoutTimeSpan.Seconds}second(s)", true);
            }
            var publicEmbed = new List<Embed> { successEmbed.Build() }.AsEnumerable();
            if (webhookClient != null) await webhookClient.SendMessageAsync("", false, publicEmbed);
        }
    }
}
