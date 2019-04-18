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

namespace UserIdentityService_ResetUserPassword
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

                return await ResetUserPassword(dataStores, request.Headers, requestBody);
            });
        }

        /**
         * Set user phone number.
         */
        private async Task<APIGatewayProxyResponse> ResetUserPassword(IDataStores dataStores,
                                                                    IDictionary<string, string> requestHeaders,
                                                                    JObject requestBody)
        {
            Debug.Untested();
            Debug.AssertValid(dataStores);
            Debug.AssertValid(requestHeaders);
            Debug.AssertValidOrNull(requestBody);

            try {
                // Log call
                LoggingHelper.LogMessage($"UserIdentityService::ResetUserPassword()");

                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = (AmazonDynamoDBClient)dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Check inputs
                ResetUserPasswordRequest resetUserPasswordRequest = UserIdentityService_ResetUserPassword_LogicLayer.CheckValidResetUserPasswordRequest(requestBody);
                Debug.AssertValid(resetUserPasswordRequest);

                // Check reCaptcha
                ReCaptchaHelper.CheckReCaptchaToken(resetUserPasswordRequest.reCaptcha);

                // Perform logic
                await UserIdentityService_ResetUserPassword_LogicLayer.ResetUserPassword(dbClient, resetUserPasswordRequest);

                // Respond
                return new APIGatewayProxyResponse {
                    StatusCode = APIHelper.STATUS_CODE_NO_CONTENT
                };
            } catch (Exception exception) {
                Debug.Tested();
                if ((exception.Message == SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS) ||
                    (exception.Message == IdentityServiceLogicLayer.ERROR_EMAIL_NOT_VERIFIED) ||
                    (exception.Message == ReCaptchaHelper.ERROR_INVALID_RECAPTCHA_TOKEN)) {
                    Debug.Tested();
                    // Note tht no error code is returned for security reasons.
                    //??--return StatusCode(APIHelper.STATUS_CODE_FORBIDDEN);
                    return new APIGatewayProxyResponse {
                        StatusCode = APIHelper.STATUS_CODE_FORBIDDEN
                    };
                }
                else
                {
                    Debug.Untested();
                    return APIHelper.ResponseFromException(exception);
                }
            }
        }

    }   // LambdaHandler

}   // UserIdentityService_ResetUserPassword
