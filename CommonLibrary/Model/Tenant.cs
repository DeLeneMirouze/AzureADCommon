#region using
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics; 
#endregion

namespace CommonLibrary.Model
{
    [DebuggerDisplay("{DisplayName}")]
    public sealed class Tenant
    {
        public string TenantId { get; set; }
        public string DisplayName { get; set; }
        public string DomainName { get; set; }
        public bool IsCurrentTenant { get; set; }

        #region Deserialize (static)
        public static List<Tenant> Deserialize(string json)
        {
            JArray tenantsJson = (JArray)(JObject.Parse(json))["value"];

            List<Tenant> tenants = new List<Tenant>();

            foreach (JToken tenantString in tenantsJson)
            {
                Tenant tenant = new Tenant();

                tenant.DisplayName = tenantString["tenantId"].Value<string>();
                tenant.DomainName = tenantString["tenantId"].Value<string>();
                tenant.TenantId = tenantString["tenantId"].Value<string>();

                tenants.Add(tenant);
            }

            return tenants;
        }
        #endregion
    }
}
