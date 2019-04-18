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

namespace UserIdentityService_SetUserEmail
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

                return await SetUserEmail(dataStores, request.Headers, requestBody);
            });
        }

        /**
         * Set user email.
         */
        internal async Task<APIGatewayProxyResponse> SetUserEmail(IDataStores dataStores,
                                                                  IDictionary<string, string> requestHeaders,
                                                                  JObject requestBody)
        {
            Debug.Untested();
            Debug.AssertValid(dataStores);
            Debug.AssertValid(requestHeaders);
            Debug.AssertValidOrNull(requestBody);

            try {
                // Log call
                LoggingHelper.LogMessage($"UserIdentityService::SetUserEmail()");

                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = (AmazonDynamoDBClient)dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Check inputs
                SetUserEmailRequest setUserEmailRequest = UserIdentityService_SetUserEmail_LogicLayer.CheckValidSetUserEmailRequest(requestBody);
                Debug.AssertValid(setUserEmailRequest);

                // Check authenticated endpoint security
                string loggedInUserId = await APIHelper.CheckLoggedIn(dbClient, requestHeaders);
                Debug.AssertID(loggedInUserId);

                // Perform logic
                await UserIdentityService_SetUserEmail_LogicLayer.SetUserEmail(dbClient, loggedInUserId, setUserEmailRequest);

                // Respond
                return new APIGatewayProxyResponse {
                    StatusCode = APIHelper.STATUS_CODE_NO_CONTENT
                };
            } catch (Exception exception) {
                Debug.Tested();
                if ((exception.Message == IdentityServiceLogicLayer.ERROR_EMAIL_IN_USE) ||
                    (exception.Message == IdentityServiceLogicLayer.ERROR_EMAIL_ALREADY_BEING_CHANGED)) {
                    Debug.Untested();
                    GeneralErrorResponse response = new GeneralErrorResponse();
                    if (exception.Message == IdentityServiceLogicLayer.ERROR_EMAIL_IN_USE) {
                        Debug.Untested();
                        response.error = IdentityServiceLogicLayer.EMAIL_IN_USE;
                    } else if (exception.Message == IdentityServiceLogicLayer.ERROR_EMAIL_ALREADY_BEING_CHANGED) {
                        Debug.Untested();
                        response.error = IdentityServiceLogicLayer.EMAIL_ALREADY_BEING_CHANGED;
                    }
                    //??--ObjectResult result = new ObjectResult(response);
                    //??--result.StatusCode = APIHelper.STATUS_CODE_UNAUTHORIZED;
                    //??--return StatusCode(APIHelper.STATUS_CODE_UNAUTHORIZED, response);
                    return new APIGatewayProxyResponse {
                        StatusCode = APIHelper.STATUS_CODE_UNAUTHORIZED,
                        Body = JsonConvert.SerializeObject(response)
                    };
                } else {
                    Debug.Tested();
                    return APIHelper.ResponseFromException(exception);
                }
            }
        }
    }   // LambdaHandler

}   // UserIdentityService_SetUserEmail
