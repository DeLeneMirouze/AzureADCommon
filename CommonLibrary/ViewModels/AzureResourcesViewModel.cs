using CommonLibrary.Model;
using System.Collections.Generic;

namespace CommonLibrary.ViewModels
{
    public class AzureResourcesViewModel
    {
        #region Constructor
        public AzureResourcesViewModel()
        {
            Tenants = new List<Tenant>();
            Subscriptions = new List<Subscription>();
        }
        #endregion

        public List<Tenant> Tenants { get; set; }

        public List<Subscription> Subscriptions { get; set; }
    }
}
