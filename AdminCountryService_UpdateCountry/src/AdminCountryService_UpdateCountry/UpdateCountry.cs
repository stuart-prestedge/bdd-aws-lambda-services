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

namespace AdminCountryService_UpdateCountry
{
    public class Endpoint_UpdateCountry
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

                return await UpdateCountry(dataStores, requestHeaders, requestBody);
            });
        }

        /**
         * Get the countries.
         */
        public async Task<APIGatewayProxyResponse> UpdateCountry(IDataStores dataStores, IDictionary<string, string> requestHeaders, JObject requestBody)
        {
            Debug.Tested();
            Debug.AssertValid(dataStores);
            Debug.AssertValid(requestHeaders);
            Debug.AssertValidOrNull(requestBody);

            try {
                // Log call
                LoggingHelper.LogMessage($"AdminCountryService::UpdateCountry()");

                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Check inputs
                Country country = CountryServiceLogicLayer.CheckValidUpdateCountryRequest(requestBody);

                // Check security
                User user = await APIHelper.CheckLoggedInAsAdmin(dbClient, requestHeaders);
                Debug.AssertValid(user);

                // Perform logic
                bool created = await CountryServiceLogicLayer.UpdateCountry(dbClient, loggedInUserId, country);

                // Return response
                return new APIGatewayProxyResponse
                    {
                        StatusCode = created ? APIHelper.STATUS_CODE_CREATED : APIHelper.STATUS_CODE_NO_CONTENT
                    };
            } catch (Exception exception) {
                Debug.Unreachable();
                return APIHelper.ResponseFromException(exception);
            }
        }

    }   // Endpoint_UpdateCountry

}   // AdminCountryService_UpdateCountry
