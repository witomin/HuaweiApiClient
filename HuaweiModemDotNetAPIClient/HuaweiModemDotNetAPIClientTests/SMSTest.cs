using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HuaweiApiClientTests {
    [TestClass]
    public class SMSTest
    {

        private ApiConfig config;
        private ApiCtx ctx;

        [TestInitialize]
        public void init()
        {
            config = new ApiConfig();
            config.BaseURL = "http://192.168.8.1";
            ctx = new ApiCtx(config);
        }


        [TestMethod]
        public void TestSend2SMS()
        {
            string username = "admin";
            string password = "validpass";

            User user = new User(ctx);
            api.SMS sms = new SMS(ctx);

            var res = user.Login(username, password);

            Assert.IsTrue(ctx.LoggedIn);

            res = sms.SendSMS("+12223334444", "This is first message from C# API");
            res = sms.SendSMS("+12223334444", "This is second message from C# API");


        }

    }
}
