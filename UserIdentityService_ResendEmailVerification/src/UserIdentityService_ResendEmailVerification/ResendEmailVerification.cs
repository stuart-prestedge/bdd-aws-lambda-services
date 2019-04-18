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

namespace UserIdentityService_ResendEmailVerification
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
            Debug.AssertValid(context);

            return await LambdaHelper.ExecuteAsync(request, APIHelper.REQUEST_METHOD_PUT, async (IDataStores dataStores, JObject requestBody) => {
                Debug.AssertValid(dataStores);
                Debug.AssertValid(requestBody);

                return await ResendEmailVerification(dataStores, requestBody);
            });
        }

        /**
         * Set user address.
         */
        private async Task<APIGatewayProxyResponse> ResendEmailVerification(IDataStores dataStores,
                                                                            JObject requestBody)
        {
            Debug.Untested();
            Debug.AssertValid(dataStores);
            Debug.AssertValidOrNull(requestBody);

            try {
                // Log call
                LoggingHelper.LogMessage($"UserIdentityService::ResendEmailVerification()");

                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = (AmazonDynamoDBClient)dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Check inputs
                ResendEmailVerificationRequest resendEmailVerificationRequest = UserIdentityService_ResendEmailVerification_LogicLayer.CheckValidResendEmailVerificationRequest(requestBody);
                Debug.AssertValid(resendEmailVerificationRequest);

                // Perform logic
                await UserIdentityService_ResendEmailVerification_LogicLayer.ResendEmailVerification(dbClient, resendEmailVerificationRequest);

                // Respond
                return new APIGatewayProxyResponse {
                    StatusCode = APIHelper.STATUS_CODE_NO_CONTENT
                };
            } catch (Exception exception) {
                Debug.Tested();
                return APIHelper.ResponseFromException(exception);
            }
        }

    }   // LambdaHandler

}   // UserIdentityService_ResendEmailVerification
