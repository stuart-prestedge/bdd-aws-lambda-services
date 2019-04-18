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

namespace UserIdentityService_Login
{
    public class Endpoint_Login
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
            Debug.AssertValid(context);

            return await LambdaHelper.ExecuteAsync(request, APIHelper.REQUEST_METHOD_PUT, async (IDataStores dataStores, JObject requestBody) => {
                Debug.AssertValid(dataStores);
                Debug.AssertValid(requestBody);

                return await Login(dataStores, requestBody);
            });
        }

        /**
         * Login
         */
        internal async Task<APIGatewayProxyResponse> Login(IDataStores dataStores, JObject requestBody)
        {
            Debug.Untested();
            Debug.AssertValidOrNull(requestBody);

            try {
                // Log call
                LoggingHelper.LogMessage($"UserIdentityService::Login()");

                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = (AmazonDynamoDBClient)dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Check inputs
                LoginRequest loginRequest = UserIdentityService_Login_LogicLayer.CheckValidLoginRequest(requestBody);
                Debug.AssertValid(loginRequest);

                // Perform logic
                LoginResponse response = new LoginResponse();
                Tuple<string, DateTime> result = await UserIdentityService_Login_LogicLayer.Login(dbClient, loginRequest);
                Debug.AssertValid(result);
                response.accessToken = result.Item1;
                response.expiryTime = APIHelper.APIDateTimeStringFromDateTime(result.Item2);

                // Respond
                return new APIGatewayProxyResponse {
                    StatusCode = APIHelper.STATUS_CODE_OK,
                    Body = JsonConvert.SerializeObject(response)
                };
            } catch (Exception exception) {
                Debug.Tested();
                if ((exception.Message == SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS) ||
                    (exception.Message == IdentityServiceLogicLayer.ERROR_INCORRECT_PASSWORD) ||
                    (exception.Message == IdentityServiceLogicLayer.ERROR_USER_ACCOUNT_CLOSED) ||
                    (exception.Message == IdentityServiceLogicLayer.ERROR_USER_BLOCKED) ||
                    (exception.Message == IdentityServiceLogicLayer.ERROR_USER_LOCKED)) {
                    Debug.Tested();
                    string error = null;
                    if (exception.Message == SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS) {
                        Debug.Tested();
                        error = IdentityServiceLogicLayer.USER_NOT_FOUND;
                    } else if (exception.Message == IdentityServiceLogicLayer.ERROR_INCORRECT_PASSWORD) {
                        Debug.Tested();
                        error = IdentityServiceLogicLayer.USER_NOT_FOUND;
                    } else if (exception.Message == IdentityServiceLogicLayer.ERROR_USER_ACCOUNT_CLOSED) {
                        Debug.Tested();
                        error = IdentityServiceLogicLayer.ACCOUNT_CLOSED;
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

    }   // Endpoint_Login

}   // UserIdentityService_Login
