namespace HuaweiApiClient {
    public class APIContext {

        private Queue<string> requestVerificationTokens;

        public APIContext(APIConfig config) {
            this.Config = config;
            this.LoggedIn = false;
            this.requestVerificationTokens = new Queue<string>();
            this.Session = new HTTPSession(this);
        }

        public APIConfig Config {
            get; private set;
        }

        public HTTPSession Session {
            get; private set;
        }

        public string SessionId {
            get; set;
        }

        public bool LoggedIn {
            get; set;
        }

        public void AddRequestVerificationToken(string verificationToken) {
            requestVerificationTokens.Enqueue(verificationToken);
        }

        public void AddRequestVerificationToken(string[] verificationTokens) {
            foreach (string verificationToken in verificationTokens) {
                AddRequestVerificationToken(verificationToken);
            }
        }

        public string NextRequestVerificationToken() {
            return requestVerificationTokens.Dequeue();
        }

        public string CurrentRequestVerificationToken() {
            return requestVerificationTokens.Peek();
        }

        public int VerificationTokensCount {
            get {
                return requestVerificationTokens.Count;
            }
        }

    }

}