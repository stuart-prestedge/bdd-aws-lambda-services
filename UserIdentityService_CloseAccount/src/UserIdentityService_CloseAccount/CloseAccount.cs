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

namespace UserIdentityService_CloseAccount
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

                return await CloseAccount(dataStores, request.Headers, requestBody);
            });
        }

        /**
         * Login
         */
        internal async Task<APIGatewayProxyResponse> CloseAccount(IDataStores dataStores,
                                                                  IDictionary<string, string> requestHeaders,
                                                                  JObject requestBody)
        {
            Debug.Untested();
            Debug.AssertValid(dataStores);
            Debug.AssertValid(requestHeaders);
            Debug.AssertValidOrNull(requestBody);

            try {
                // Log call
                LoggingHelper.LogMessage($"UserIdentityService::CloseAccount()");

                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = (AmazonDynamoDBClient)dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Check inputs
                CloseAccountRequest closeAccountRequest = UserIdentityService_CloseAccount_LogicLayer.CheckValidCloseAccountRequest(requestBody);
                Debug.AssertValid(closeAccountRequest);

                // Check authenticated endpoint security
                string loggedInUserId = await APIHelper.CheckLoggedIn(dbClient, requestHeaders);
                Debug.AssertID(loggedInUserId);

                // Perform logic
                await UserIdentityService_CloseAccount_LogicLayer.CloseAccount(dbClient, loggedInUserId, closeAccountRequest);

                // Respond
                return new APIGatewayProxyResponse {
                    StatusCode = APIHelper.STATUS_CODE_NO_CONTENT
                };
            } catch (Exception exception) {
                Debug.Tested();
                if (exception.Message == IdentityServiceLogicLayer.ERROR_INCORRECT_PASSWORD) {
                    Debug.Untested();
                    //??--return StatusCode(APIHelper.STATUS_CODE_UNAUTHORIZED, new GeneralErrorResponse { error = IdentityServiceLogicLayer.INCORRECT_PASSWORD });
                    //??--return Unauthorized();
                    return new APIGatewayProxyResponse {
                        StatusCode = APIHelper.STATUS_CODE_UNAUTHORIZED,
                        Body = $"{{ error = \"{IdentityServiceLogicLayer.INCORRECT_PASSWORD}\"}}"
                    };
                } else {
                    Debug.Tested();
                    return APIHelper.ResponseFromException(exception);
                }
            }
        }

    }   // LambdaHandler

}   // UserIdentityService_CloseAccount
