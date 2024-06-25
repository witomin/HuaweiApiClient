namespace HuaweiApiClient {

    public abstract class API {
        public API(APIContext context) {
            this.Context = context;
        }

        public APIContext Context {
            get; private set;
        }

    }
}