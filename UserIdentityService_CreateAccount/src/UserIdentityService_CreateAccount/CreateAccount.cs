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

namespace UserIdentityService_CreateAccount
{
    public class Endpoint_CreateAccount
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

            return await LambdaHelper.ExecuteAsync(request, async (IDataStores dataStores, IDictionary<string, string> requestHeaders, JObject requestBody) => {
                Debug.AssertValid(dataStores);
                Debug.AssertValid(requestHeaders);
                Debug.AssertValid(requestBody);

                return await CreateAccount(dataStores, requestBody);
            });
        }

        /**
         * Create a new account.
         */
        internal async Task<APIGatewayProxyResponse> CreateAccount(IDataStores dataStores, JObject requestBody) {
            Debug.Tested();
            Debug.AssertValid(dataStores);
            Debug.AssertValidOrNull(requestBody);

            try {
                // Log call
                LoggingHelper.LogMessage($"UserIdentityService::CreateAccount()");

                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = (AmazonDynamoDBClient)dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Check inputs
                CreateAccountRequest createAccountRequest = IdentityServiceLogicLayer.CheckValidCreateAccountRequest(requestBody);
                Debug.AssertValid(createAccountRequest);

                // Perform logic
                Tuple<User, bool> result = await IdentityServiceLogicLayer.CreateAccount(dbClient, createAccountRequest);
                Debug.AssertValid(result);
                Debug.AssertValid(result.Item1);
                if (!result.Item2) {
                    Debug.Tested();
                    // New account
                    //???string location = "/identity/self/user-details";
                    CreateAccountResponse response = new CreateAccountResponse();
                    return new APIGatewayProxyResponse {
                        StatusCode = APIHelper.STATUS_CODE_CREATED,
                        Headers = null,
                        Body = JsonConvert.SerializeObject(response)
                    };//??--Created(location, response);
                } else {
                    // Account re-opened
                    Debug.Tested();
                    return new APIGatewayProxyResponse { StatusCode = APIHelper.STATUS_CODE_OK };//??--Ok();
                }
            } catch (Exception exception) {
                Debug.Tested();
                if ((exception.Message == IdentityServiceLogicLayer.ERROR_EMAIL_IN_USE) ||
                    (exception.Message == IdentityServiceLogicLayer.ERROR_USER_ACCOUNT_CLOSED)) {
                    Debug.Untested();
                    return new APIGatewayProxyResponse {
                        StatusCode = APIHelper.STATUS_CODE_FORBIDDEN,
                        Body = $"{{ body = \"{exception.Message}\"}}"
                    };//??--StatusCode(APIHelper.STATUS_CODE_FORBIDDEN, new GeneralErrorResponse { error = exception.Message });
                } else {
                    Debug.Tested();
                    return APIHelper.ResponseFromException(exception);
                }
            }
        }

    }   // Endpoint_CreateAccount

}   // UserIdentityService_CreateAccount
