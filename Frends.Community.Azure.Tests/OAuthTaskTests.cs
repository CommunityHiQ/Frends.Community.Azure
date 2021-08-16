using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.Community.Azure.Tests
{
    [TestFixture]
    [Ignore("Unit tests requires valid Azure login info")]
    public class OAuthTaskTests
    {
        private OAuthProperties props;

        [SetUp]
        public void Setup()
        {
            string tenantId = Environment.GetEnvironmentVariable("AZURE_OAUTH_TENANTID");

            props = new OAuthProperties()
            {
                AuthContextURL = Environment.GetEnvironmentVariable("AZURE_OAUTH_AUTHCONTEXTURLPREFIX") + tenantId,
                Resource = Environment.GetEnvironmentVariable("AZURE_OAUTH_RESOURCE"),
                ClientId = Environment.GetEnvironmentVariable("AZURE_OAUTH_CLIENTID"),
                ClientSecret = Environment.GetEnvironmentVariable("AZURE_OAUTH_CLIENTSECRET")
            };
        }

        /// <summary>
        /// Get token from Azure
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestAccessToken()
        {
            var token = await AzureOAuthTasks.GetAccessToken(props, new CancellationToken());

            Assert.IsTrue(token != null);
        }
    }
}