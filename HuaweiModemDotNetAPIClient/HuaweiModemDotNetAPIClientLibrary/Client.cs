using System.Security.Cryptography;
using System.Text;

namespace HuaweiApiClient {
    public class Client
        : API {

        public Client(APIContext ctx)
            : base(ctx) {

        }

        public APIResponse SesTokInfo() {
            var response = Ctx.Session.HttpGet($"{Ctx.Config.BaseURL}/api/webserver/SesTokInfo");

            if (response.Response.ContainsKey("SesInfo") && response.Response.ContainsKey("TokInfo")) {
                // setup context values
                Ctx.AddRequestVerificationToken(response.Response["TokInfo"] as string);
                Ctx.SessionId = (response.Response["SesInfo"] as string).Substring("SessionID=".Length);
                return response;
            }
            else {
                throw new System.Exception("SesInfo and/or TokInfo missing in response");
            }
        }
        private string b64_hex_sha256(string value) {
            using (SHA256 sha256 = SHA256.Create()) {
                var sha256bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                string beforeb64 = BitConverter.ToString(sha256bytes).Replace("-", "").ToLower();
                return System.Convert.ToBase64String(Encoding.UTF8.GetBytes(beforeb64));
            }
        }

        private string getPasswordValue(string loginToken, string username, string password) {

            // password calculation is:
            // b64(sha256(username + b64(sha256(password)) + loginToken))

            return b64_hex_sha256(username + b64_hex_sha256(password) + loginToken);
        }

        public APIResponse Login(string username, string password) {
            if (Ctx.SessionId == null) {
                // first, setup a session
                var webServerAPI = new Client(Ctx);
                var innerReponse = webServerAPI.SesTokInfo();
                if (Ctx.SessionId == null) {
                    throw new System.Exception("Can't get SessionID. Impossible to continue");
                }
            }

            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(password);
            var password4 = getPasswordValue(Ctx.CurrentRequestVerificationToken(), username, password);

            string xml = $@"<?xml version:""1.0"" encoding=""UTF - 8""?>
<request>
<Username>{username}</Username>
<Password>{password4}</Password>
<password_type>4</password_type>
</request>";

            var response = Ctx.Session.HttpPostXML($"{Ctx.Config.BaseURL}/api/user/login", xml);

            if (response.Response.Count == 0) {
                Ctx.LoggedIn = true;
            }

            return response;

        }
    }


}