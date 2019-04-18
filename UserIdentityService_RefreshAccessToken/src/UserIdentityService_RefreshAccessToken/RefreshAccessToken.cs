using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BDDReferenceService;
using BDDReferenceService.Model;
using BDDReferenceService.Logic;
using Amazon.DynamoDBv2;
using BDDReferenceService.Contracts;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace UserIdentityService_RefreshAccessToken
{
    public class LambdaHandler
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> handleRequest(APIGatewayProxyRequest request, ILambdaContext context)
        {
            Debug.Untested();
            Debug.AssertValid(request);
            Debug.AssertValid(request.Headers);
            Debug.AssertValid(context);

            return await LambdaHelper.ExecuteAsync(request, APIHelper.REQUEST_METHOD_PUT, async (IDataStores dataStores, JObject requestBody) => {
                Debug.AssertValid(dataStores);
                Debug.AssertValid(requestBody);

                return await RefreshAccessToken(dataStores, request.Headers, requestBody);
            });
        }

        /**
         * Refresh access token.
         */
        public async Task<APIGatewayProxyResponse> RefreshAccessToken(IDataStores dataStores,
                                                                      IDictionary<string, string> requestHeaders,
                                                                      JObject requestBody)
        {
            Debug.Untested();
            Debug.AssertValid(dataStores);
            Debug.AssertValid(requestHeaders);
            Debug.AssertValid(requestBody);

            try {
                // Log call
                LoggingHelper.LogMessage($"UserIdentityService::RefreshAccessToken()");

                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = (AmazonDynamoDBClient)dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Check inputs
                APIHelper.CheckEmptyRequestBody(requestBody);

                // Check authenticated endpoint security
                string loggedInUserId = await APIHelper.CheckLoggedIn(dbClient, requestHeaders);
                Debug.AssertID(loggedInUserId);

                // Perform logic
                DateTime? expiryTime = await UserIdentityService_RefreshAccessToken_LogicLayer.RefreshAccessToken(dbClient, loggedInUserId);
                Debug.AssertValid(expiryTime);

                // Respond
                RefreshAccessTokenResponse response = new RefreshAccessTokenResponse {
                    expiryTime = APIHelper.APIDateTimeStringFromDateTime(expiryTime)
                };
                return new APIGatewayProxyResponse {
                    StatusCode = APIHelper.STATUS_CODE_OK,
                    Body = JsonConvert.SerializeObject(response)
                };
                //??--return Ok(response);
            } catch (Exception exception) {
                Debug.Tested();
                if (exception.Message == IdentityServiceLogicLayer.ERROR_CANNOT_EXTEND_ACCESS_TOKEN) {
                    Debug.Untested();
                    return new APIGatewayProxyResponse {
                        StatusCode = APIHelper.STATUS_CODE_FORBIDDEN,
                        Body = $"{{ error = {IdentityServiceLogicLayer.CANNOT_EXTEND_ACCESS_TOKEN} }}"
                    };
                    //??--return StatusCode(APIHelper.STATUS_CODE_FORBIDDEN, new GeneralErrorResponse { error = IdentityServiceLogicLayer.CANNOT_EXTEND_ACCESS_TOKEN });
                } else {
                    Debug.Tested();
                    return APIHelper.ResponseFromException(exception);
                }
            }
        }

    }   // Endpoint_RefreshAccessToken

}   // UserIdentityService_RefreshAccessToken
