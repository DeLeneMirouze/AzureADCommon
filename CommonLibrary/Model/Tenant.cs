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
        /// <summary>
        /// Tenant's id
        /// </summary>
        public string TenantId { get; set; }
        /// <summary>
        /// Tenant's name
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Domains in the tenant
        /// </summary>
        public string[] DomainNames { get; set; }
        /// <summary>
        /// This is the current tenant
        /// </summary>
        public bool IsCurrentTenant { get; set; }

        #region DeserializeTenantList (static)
        public static List<Tenant> DeserializeTenantList(string json)
        {
            JArray tenantsJson = (JArray)(JObject.Parse(json))["value"];

            List<Tenant> tenants = new List<Tenant>();

            foreach (JToken tenantString in tenantsJson)
            {
                Tenant tenant = new Tenant();

                tenant.TenantId = tenantString["tenantId"].Value<string>();

                tenants.Add(tenant);
            }

            return tenants;
        }
        #endregion

        #region DeserializeTenantInfo (static)
        public static void DeserializeTenantInfo(string json, Tenant tenant)
        {
            JArray tenantsJson = (JArray)(JObject.Parse(json))["value"];
            tenant.DisplayName = tenantsJson.First["displayName"].Value<string>();

            JToken vds = tenantsJson.First["verifiedDomains"];
            JArray verifiedDomains = (JArray)vds;

            tenant.DomainNames = new string[verifiedDomains.Count];
            for (int i = 0; i < verifiedDomains.Count; i++)
            {
                JToken verifiedDomain = verifiedDomains[i];
                tenant.DomainNames[i] = verifiedDomain["name"].Value<string>();
            }
        }
        #endregion
    }
}
