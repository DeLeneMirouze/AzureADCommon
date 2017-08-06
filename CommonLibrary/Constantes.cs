namespace CommonLibrary
{
    /// <summary>
    /// Miscelaneous constants used overall the application
    /// </summary>
    public static class Constantes
    {
        public static class Endpoints
        {
            /// <summary>
            /// Azure Management Service
            /// </summary>
            public const string ArmEndpoint = "https://management.azure.com/";
            public const string GraphEndpoint = "https://graph.windows.net/";
        }

        public static class ServiceVersion
        {
            public const string ArmVersion = "2016-09-01";
            public const string GraphVersion = "1.6";
        }

        public static class ClaimsType
        {
            public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";
            public const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        }
    }
}
