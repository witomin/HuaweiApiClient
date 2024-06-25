namespace HuaweiApiClient {

    public abstract class API {
        public API(APIContext ctx) {
            this.Ctx = ctx;
        }

        public APIContext Ctx {
            get; private set;
        }

    }
}