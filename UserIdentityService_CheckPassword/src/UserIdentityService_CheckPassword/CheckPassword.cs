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

namespace UserIdentityService_CheckPassword
{
    public class Endpoint_CheckPassword
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> LambdaFunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            Debug.Untested();
            Debug.AssertValid(request);
            Debug.AssertValid(request.Headers);
            Debug.AssertValid(context);

            return await LambdaHelper.ExecuteAsync(request, APIHelper.REQUEST_METHOD_PUT, async (IDataStores dataStores, JObject requestBody) => {
                Debug.AssertValid(dataStores);
                Debug.AssertValid(requestBody);

                return await CheckPassword(dataStores, request.Headers, requestBody);
            });
        }

        /**
         * Check password.
         */
        public async Task<APIGatewayProxyResponse> CheckPassword(IDataStores dataStores,
                                                                 IDictionary<string, string> requestHeaders,
                                                                 JObject requestBody)
        {
            Debug.Untested();
            Debug.AssertValid(dataStores);
            Debug.AssertValid(requestHeaders);
            Debug.AssertValid(requestBody);

            try {
                // Log call
                LoggingHelper.LogMessage($"UserIdentityService::CheckPassword()");

                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = (AmazonDynamoDBClient)dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Check inputs
                CheckPasswordRequest checkPasswordRequest = UserIdentityService_CheckPassword_LogicLayer.CheckValidCheckPasswordRequest(requestBody);
                Debug.AssertValid(checkPasswordRequest);

                // Check authenticated endpoint security
                string loggedInUserId = await APIHelper.CheckLoggedIn(dbClient, requestHeaders);
                Debug.AssertID(loggedInUserId);

                // Perform logic
                await UserIdentityService_CheckPassword_LogicLayer.CheckPassword(dbClient, checkPasswordRequest, loggedInUserId);

                // Respond
                return new APIGatewayProxyResponse {
                    StatusCode = APIHelper.STATUS_CODE_OK
                };
            } catch (Exception exception) {
                Debug.Tested();
                if ((exception.Message == IdentityServiceLogicLayer.ERROR_INCORRECT_PASSWORD) ||
                    (exception.Message == IdentityServiceLogicLayer.ERROR_USER_BLOCKED) ||
                    (exception.Message == IdentityServiceLogicLayer.ERROR_USER_LOCKED)) {
                    Debug.Untested();
                    //??--GeneralErrorResponse response = new GeneralErrorResponse();
                    string error = null;
                    if (exception.Message == IdentityServiceLogicLayer.ERROR_INCORRECT_PASSWORD) {
                        Debug.Tested();
                        error = IdentityServiceLogicLayer.INCORRECT_PASSWORD;
                    } else if (exception.Message == IdentityServiceLogicLayer.ERROR_USER_BLOCKED) {
                        Debug.Tested();
                        error = IdentityServiceLogicLayer.USER_BLOCKED;
                    } else if (exception.Message == IdentityServiceLogicLayer.ERROR_USER_LOCKED) {
                        Debug.Tested();
                        error = IdentityServiceLogicLayer.USER_LOCKED;
                    }
                    //??-- ObjectResult result = new ObjectResult(response);
                    // result.StatusCode = APIHelper.STATUS_CODE_UNAUTHORIZED;
                    // return result;
                    return new APIGatewayProxyResponse {
                        StatusCode = APIHelper.STATUS_CODE_UNAUTHORIZED,
                        Body = $"{{ error = \"{error}\"}}"
                    };
                } else {
                    Debug.Tested();
                    return APIHelper.ResponseFromException(exception);
                }
            }
        }

    }   // Endpoint_CheckPassword
    
}   // UserIdentityService_CheckPassword
