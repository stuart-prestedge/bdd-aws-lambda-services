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

namespace UserIdentityService_GetUserPermissions
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

            return await LambdaHelper.ExecuteGetAsync(request, async (IDataStores dataStores) => {
                Debug.AssertValid(dataStores);

                return await GetUserPermissions(dataStores, request.Headers);
            });
        }

        /**
         * Get user permissions.
         */
        public async Task<APIGatewayProxyResponse> GetUserPermissions(IDataStores dataStores,
                                                                      IDictionary<string, string> requestHeaders) {
            Debug.Untested();
            Debug.AssertValid(dataStores);
            Debug.AssertValid(requestHeaders);

            try {
                // Log call
                LoggingHelper.LogMessage($"UserIdentityService::GetUserPermissions()");

                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = (AmazonDynamoDBClient)dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Check authenticated endpoint security
                string loggedInUserId = await APIHelper.CheckLoggedIn(dbClient, requestHeaders);
                Debug.AssertID(loggedInUserId);

                // Perform logic
                var permissions = await UserIdentityService_GetUserPermissions_LogicLayer.GetUserPermissions(dbClient, loggedInUserId);
                Debug.AssertValid(permissions);
                GetPermissionsResponse response = new GetPermissionsResponse();
                response.permissions = permissions;
                //??--GetPermissionsResponse response = await UserIdentityService_GetUserPermissions_LogicLayer.GetUserPermissions(dbClient, loggedInUserId);
                //Debug.AssertValid(response);

                // Respond
                return new APIGatewayProxyResponse {
                    StatusCode = APIHelper.STATUS_CODE_OK,
                    Body = JsonConvert.SerializeObject(response)
                };
            } catch (Exception exception) {
                Debug.Tested();
                return APIHelper.ResponseFromException(exception);
            }
        }

    }   // LambdaHandler

}   // UserIdentityService_GetUserPermissions
