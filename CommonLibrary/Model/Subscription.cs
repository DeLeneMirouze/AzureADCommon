using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace CommonLibrary.Model
{
    [DebuggerDisplay("{DisplayName}")]
    public class Subscription
    {
        public string SubscriptionId { get; set; }
        public string DisplayName { get; set; }

        #region Deserialize
        public static List<Subscription> Deserialize(string json)
        {
            JArray tenantsJson = (JArray)(JObject.Parse(json))["value"];

            List<Subscription> subscriptions = new List<Subscription>();
            foreach (JToken tenantString in tenantsJson)
            {
                Subscription subscription = new Subscription();

                subscription.SubscriptionId = tenantString["subscriptionId"].Value<string>();
                subscription.DisplayName = tenantString["displayName"].Value<string>();

                subscriptions.Add(subscription);
            }

            return subscriptions;
        } 
        #endregion
    }
}
