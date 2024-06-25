namespace HuaweiApiClient {
    public class APIConfig {

        public APIConfig() {
            this.BaseURL = "http://192.168.8.1/api";
            this.HttpTimeout = 10000;
        }

        public string BaseURL {
            get; set;
        }

        public int HttpTimeout {
            get; set;
        }

    }

}