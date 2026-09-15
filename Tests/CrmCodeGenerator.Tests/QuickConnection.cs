using System;
using System.Net;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;

namespace D365VsTools.CodeGenerator
{
    // Reconstructed for CrmCodeGenerator.Tests: the original QuickConnection class was removed from
    // the product code at some point after these connected tests were written. Only the Connect
    // overload actually used by ConnectedTestsBase is implemented here.
    public static class QuickConnection
    {
        public static OrganizationServiceProxy Connect(string url, string domain, string userName, string password, string organization)
        {
            var serviceUri = new Uri($"{url.TrimEnd('/')}/{organization}/XRMServices/2011/Organization.svc");

            var credentials = new ClientCredentials();
            if (!string.IsNullOrEmpty(domain))
                credentials.Windows.ClientCredential = new NetworkCredential(userName, password, domain);
            else
            {
                credentials.UserName.UserName = userName;
                credentials.UserName.Password = password;
            }

            var proxy = new OrganizationServiceProxy(serviceUri, null, credentials, null);
            proxy.EnableProxyTypes();
            return proxy;
        }
    }
}
