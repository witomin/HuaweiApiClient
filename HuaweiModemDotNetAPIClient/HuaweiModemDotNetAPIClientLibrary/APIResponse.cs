﻿using System.Net.Mime;
using System.Xml;
using System.Xml.Serialization;

namespace HuaweiApiClient {

    public class APIResponse {

        public enum ApiResponseType {
            XML,
            TEXT,
            JSON,
            HTML,
            UNDEFINED
        }


        private APIResponse() {
            Response = new Dictionary<string, object>();
        }

        public Dictionary<string, object> Response {
            get; private set;
        }

        public object ResponseData { get; private set; }

        public ApiResponseType Type {
            get; private set;
        }

        public ContentType ContentType {
            get; private set;
        }

        private void ParseXmlResponse(string xml) {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes) {
                if (node.NodeType == XmlNodeType.Element) {
                    Response[node.Name] = node.InnerText;
                }
            }
        }

        private void ParseXmlResponse<T>(string xml){
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml)) {
                T responseData = (T)serializer.Deserialize(reader);
                ResponseData = responseData;
            }
        }

        private void ParseTextResponse(string text) {

            Response["data"] = text;
        }

        public static APIResponse ReadAndCreateApiResponse(HttpResponseMessage response) {
            APIResponse ret = new APIResponse();
            SetResponseType(response, ret);

            switch (ret.Type) {
                case ApiResponseType.XML:
                    ret.ParseXmlResponse(response.Content.ReadAsStringAsync().Result);
                    break;
                case ApiResponseType.TEXT:
                    ret.ParseTextResponse(response.Content.ReadAsStringAsync().Result);
                    break;
                default:
                    throw new Exception("Response Type: " + ret.Type + " currently not supported");
            }

            return ret;
        }

        private static void SetResponseType(HttpResponseMessage response, APIResponse ret) {
            IEnumerable<string> contentTypes;
            if (response.Headers.TryGetValues("Content-Type", out contentTypes)) {
                var contentType = new ContentType(contentTypes.First());
                ret.ContentType = contentType;
                if (contentType.MediaType.Contains("text/xml")) {
                    ret.Type = ApiResponseType.XML;
                }
                else if (contentType.MediaType.Contains("text/plain")) {
                    ret.Type = ApiResponseType.TEXT;
                }
                else if (contentType.MediaType.Contains("text/html")) {
                    ret.Type = ApiResponseType.HTML;
                }
                else if (contentType.MediaType.Contains("application/json")) {
                    ret.Type = ApiResponseType.JSON;
                }
            }
            else {
                // if Content-Type not set, assume XML
                ret.ContentType = new ContentType("text/xml; charset=utf-8");
                ret.Type = ApiResponseType.XML;
            }
        }

        public static APIResponse ReadAndCreateApiResponse<T>(HttpResponseMessage response) {
            APIResponse ret = new APIResponse();
            SetResponseType(response, ret);

            switch (ret.Type) {
                case ApiResponseType.XML:
                    ret.ParseXmlResponse(response.Content.ReadAsStringAsync().Result);
                    ret.ParseXmlResponse<T>(response.Content.ReadAsStringAsync().Result);
                    break;
                case ApiResponseType.TEXT:
                    ret.ParseTextResponse(response.Content.ReadAsStringAsync().Result);
                    break;
                default:
                    throw new Exception("Response Type: " + ret.Type + " currently not supported");
            }

            return ret;
        }
        public APIResponse(HttpContent httpContent) {

        }


    }

}