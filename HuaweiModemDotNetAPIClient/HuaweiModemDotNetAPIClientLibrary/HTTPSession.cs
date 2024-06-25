using System.Text;

namespace HuaweiApiClient {

    public class HTTPSession {

        const string REQUEST_VERIFICATION_TOKEN_HEADER = "__RequestVerificationToken";
        const string SET_COOKIE_HEADER = "Set-Cookie";
        const string COOKIE_HEADER = "Cookie";
        const string SESSION_ID_COOKIE = "SessionID";


        public HTTPSession(APIContext apiContext) {
            config = apiContext.Config;
            context = apiContext;
            // setup http client
            HttpClient = new HttpClient(new HttpClientHandler() { UseCookies = false });
            HttpClient.Timeout = TimeSpan.FromMilliseconds(config.HttpTimeout);

        }

        private APIConfig config;
        private APIContext context;


        public APIConfig ApiConfig {
            get; private set;
        }

        public HttpClient HttpClient {
            get; private set;
        }


        private void updateSecurityHeaders(HttpResponseMessage response) {

            // security headers
            IEnumerable<string> headerValues;
            if (response.Headers.TryGetValues(REQUEST_VERIFICATION_TOKEN_HEADER, out headerValues)) {
                foreach (string header in headerValues) {
                    string[] securityHeaders = header.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    if (securityHeaders != null && securityHeaders.Length > 0) {
                        // only add the first one
                        context.AddRequestVerificationToken(securityHeaders[0]);
                    }
                }
            }

            if (response.Headers.TryGetValues(SET_COOKIE_HEADER, out headerValues)) {
                foreach (string header in headerValues) {
                    if (header.StartsWith(SESSION_ID_COOKIE + "=")) {
                        context.SessionId = header.Substring((SESSION_ID_COOKIE + "=").Length);

                    }
                }
            }

        }

        private void setupRequestHeaders(HttpRequestMessage requestMessage) {
            if (context.VerificationTokensCount > 0) {
                requestMessage.Headers.Add(REQUEST_VERIFICATION_TOKEN_HEADER, context.NextRequestVerificationToken());
            }
            if (context.SessionId != null) {
                requestMessage.Headers.Remove(COOKIE_HEADER);
                requestMessage.Headers.Add(COOKIE_HEADER, $"{SESSION_ID_COOKIE}={context.SessionId}");
            }
        }
        public APIResponse HttpGet(string url) {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)) {
                setupRequestHeaders(requestMessage);
                var response = HttpClient.SendAsync(requestMessage).Result;
                if (response.IsSuccessStatusCode) {
                    updateSecurityHeaders(response);
                    return APIResponse.ReadAndCreateApiResponse(response);
                }
                throw new Exception("HTTP request failed: " + response.ReasonPhrase);
            }
        }
            public APIResponse HttpGet<T>(string url) {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, url)) {
                setupRequestHeaders(requestMessage);
                var response = HttpClient.SendAsync(requestMessage).Result;
                if (response.IsSuccessStatusCode) {
                    updateSecurityHeaders(response);
                    return APIResponse.ReadAndCreateApiResponse<T>(response);
                }
                throw new Exception("HTTP request failed: " + response.ReasonPhrase);
            }
        }

        internal APIResponse HttpPostXML(string url, string xml) {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)) {
                setupRequestHeaders(requestMessage);
                requestMessage.Content = new StringContent(xml, Encoding.UTF8, "application/xml");
                var response = HttpClient.SendAsync(requestMessage).Result;
                if (response.IsSuccessStatusCode) {
                    updateSecurityHeaders(response);
                    return APIResponse.ReadAndCreateApiResponse(response);
                }
                throw new Exception("HTTP request failed: " + response.ReasonPhrase);

            }
        }
    }

}
