using HuaweiApiClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HuaweiApiClientTests {
    [TestClass]
    public class SMSTest
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
        public void TestSend2SMS()
        {
            string username = "admin";
            string password = "validpass";

            Client client = new Client(ctx);

            var res = client.Login(username, password);

            Assert.IsTrue(ctx.LoggedIn);

            res = client.SendSMS("+12223334444", "This is first message from C# API");
            res = client.SendSMS("+12223334444", "This is second message from C# API");


        }

    }
}
