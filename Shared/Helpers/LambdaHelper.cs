using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json.Linq;

namespace BDDReferenceService
{

    internal class AWSDataStores : IDataStores
    {
        public INoSQLDataStore GetNoSQLDataStore()
        {
            return new DynamoDBDataStore();
        }

        public ISQLDataStore GetSQLDataStore()
        {
            throw new NotImplementedException();
        }
    }

    /**
        * NoSQL data store encapsulating DynamoDB.
        */
    internal class DynamoDBDataStore : INoSQLDataStore
    {
        private AmazonDynamoDBClient amazonDynamoDBClient;

        internal DynamoDBDataStore()
        {
            amazonDynamoDBClient = new AmazonDynamoDBClient();
        }

        public object GetDBClient()
        {
            return amazonDynamoDBClient;
        }

    }   // DynamoDBDataStore

    public static class LambdaHelper
    {

        /**
         * Lambda logic callback.
         */
        //??--public delegate APIGatewayProxyResponse ExecuteLogic<in T, out TResult>(T arg)

        /**
         * Execute a GET HTTP method request.
         * GET requests should not have a body. Return an error if this is not the case.
         */
        public static async Task<APIGatewayProxyResponse> ExecuteGetAsync(APIGatewayProxyRequest request, Func<IDataStores, Task<APIGatewayProxyResponse>> logic)
        {
            Debug.Untested();
            Debug.AssertValidOrNull(request);

            LoggingHelper.LogMessage("LambdaHelper.ExecuteGetAsync()");
            if (request != null)
            {
                // Check that the method is GET
                Debug.AssertValid(request.HttpMethod);
                APIHelper.CheckRequestMethod(request.HttpMethod, APIHelper.REQUEST_METHOD_GET);

                string requestBodyString = request.Body;
                LoggingHelper.LogMessage(requestBodyString);
                if (string.IsNullOrEmpty(requestBodyString))
                {
                    LoggingHelper.LogMessage("Empty requestBodyString");
                    return await logic(new AWSDataStores());
                }
                else
                {
                    LoggingHelper.LogMessage("Non-empty bodyString");
                    return new APIGatewayProxyResponse
                        {
                            StatusCode = APIHelper.STATUS_CODE_BAD_REQUEST,
                            Body = $"{{ error = \"{APIHelper.NON_EMPTY_BODY}\" }}"
                        };
                }
            }
            else
            {
                return new APIGatewayProxyResponse
                    {
                        StatusCode = APIHelper.STATUS_CODE_INTERNAL_ERROR
                    };
            }
        }

        /**
         * Execute a non-GET HTTP method request.
         */
        public static async Task<APIGatewayProxyResponse> ExecuteAsync(APIGatewayProxyRequest request, string requiredHttpMethod, Func<IDataStores, JObject, Task<APIGatewayProxyResponse>> logic)
        {
            Debug.Untested();
            Debug.AssertValidOrNull(request);
            Debug.AssertString(requiredHttpMethod);

            LoggingHelper.LogMessage("LambdaHelper.ExecuteAsync()");
            if (request != null)
            {
                // Check the method type
                Debug.AssertValid(request.HttpMethod);
                APIHelper.CheckRequestMethod(request.HttpMethod, requiredHttpMethod);

                LoggingHelper.LogMessage("LambdaHelper.ExecuteAsync() - request is non-null");
                Debug.AssertValidOrNull(request.Body);
                LoggingHelper.LogMessage(request.Body);
                JObject requestBody = null;
                if (!string.IsNullOrEmpty(request.Body))
                {
                    requestBody = JObject.Parse(request.Body);
                }
                LoggingHelper.LogMessage($"LambdaHelper.ExecuteAsync() - requestBody: {requestBody}");
                //??--AmazonDynamoDBClient dbClient = new AmazonDynamoDBClient();
                return await logic(new AWSDataStores(), requestBody);
            }
            else
            {
                return new APIGatewayProxyResponse
                    {
                        StatusCode = APIHelper.STATUS_CODE_INTERNAL_ERROR
                    };
            }
        }

    }   // LambdaHelper

}   // BDDReferenceService
