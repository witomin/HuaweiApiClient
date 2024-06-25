using HuaweiApiClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HuaweiApiClientTests {
    [TestClass]
    public class LoginTest
    {

        private APIConfig config;
        private APIContext ctx;

        [TestInitialize]
        public void init()
        {
            config = new APIConfig();
            config.BaseURL = "http://192.168.8.1";
            ctx = new APIContext(config);
        }

        [TestMethod]
        public void TestSesTokInfo()
        {
            Client client = new Client(ctx);
            var res = client.SesTokInfo();
            Assert.IsNotNull(res.Response);
            Assert.Equals(res.Type, APIResponse.ApiResponseType.XML);
            Assert.IsTrue(ctx.VerificationTokensCount > 0);
            Assert.IsNotNull(ctx.SessionId);
        }

        [TestMethod]
        public void TestValidLogin()
        {
            string username = "admin";
            string password = "validpass";


            huaweisms.api.User user = new huaweisms.api.User(ctx);

            var res = user.Login(username, password);

            Assert.IsTrue(ctx.LoggedIn);

        }

        [TestMethod]
        public void TestInValidLogin()
        {
            string username = "admin";
            string password = "invalidpass";


            huaweisms.api.User user = new huaweisms.api.User(ctx);

            var res = user.Login(username, password);

            Assert.IsFalse(ctx.LoggedIn);

        }
    }
}
