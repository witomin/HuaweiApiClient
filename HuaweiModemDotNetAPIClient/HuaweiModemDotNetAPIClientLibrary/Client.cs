using HuaweiApiClient.Responses;
using System.Security.Cryptography;
using System.Text;

namespace HuaweiApiClient {
    public class Client
        : API {

        public Client(APIContext ctx)
            : base(ctx) {

        }

        public APIResponse SesTokInfo() {
            var response = Context.Session.HttpGet($"{Context.Config.BaseURL}/api/webserver/SesTokInfo");

            if (response.Response.ContainsKey("SesInfo") && response.Response.ContainsKey("TokInfo")) {
                // setup context values
                Context.AddRequestVerificationToken(response.Response["TokInfo"] as string);
                Context.SessionId = (response.Response["SesInfo"] as string).Substring("SessionID=".Length);
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
            return b64_hex_sha256(username + b64_hex_sha256(password) + loginToken);
        }

        public APIResponse Login(string username, string password) {
            if (Context.SessionId == null) {
                // first, setup a session
                var webServerAPI = new Client(Context);
                var innerReponse = webServerAPI.SesTokInfo();
                if (Context.SessionId == null) {
                    throw new Exception("Can't get SessionID. Impossible to continue");
                }
            }

            var plainTextBytes = Encoding.UTF8.GetBytes(password);
            var password4 = getPasswordValue(Context.CurrentRequestVerificationToken(), username, password);

            string xml = $@"<?xml version:""1.0"" encoding=""UTF - 8""?>
<request>
<Username>{username}</Username>
<Password>{password4}</Password>
<password_type>4</password_type>
</request>";

            var response = Context.Session.HttpPostXML($"{Context.Config.BaseURL}/api/user/login", xml);

            if (response.Response.Count == 0) {
                Context.LoggedIn = true;
            }

            return response;

        }
        public APIResponse SendSMS(string phone, string message) {
            return SendSMS(new string[] { phone }, message);

        }

        public APIResponse SendSMS(string[] phones, string message) {
            if (!Context.LoggedIn) {
                throw new Exception("You need to call User.Login(user,pass) first");
            }

            if (phones == null || phones.Length == 0) {
                throw new Exception("You need to call providing at least ONE phone number for SMS submission");
            }

            if (message == null || message.Length == 0) {
                throw new Exception("You need to call providing a non-null non-empty message for SMS submission");
            }

            string phoneTags = "";
            foreach (string phone in phones) {
                phoneTags += $"<Phone>{phone}</Phone>";
            }

            string xml = $@"<?xml version:""1.0"" encoding=""UTF - 8""?>
<request>
    <Index>-1</Index>
    <Phones>{phoneTags}</Phones>
    <Sca></Sca>
    <Content>{message}</Content>
    <Length>{message.Length}</Length>
    <Reserved>1</Reserved>
    <Date>{String.Format("{0:u}", DateTime.Now)}</Date>
</request>";

            var response = Context.Session.HttpPostXML($"{Context.Config.BaseURL}/api/sms/send-sms", xml);

            return response;

        }
        /// <summary>
        /// Получить список подключенных клиентов WiFi
        /// </summary>
        /// <returns></returns>
        public APIResponse GetWifiClients() {
            var response = Context.Session.HttpGet<HostListResponse>($"{Context.Config.BaseURL}/api/wlan/host-list");

            return response;
        }

    }


}