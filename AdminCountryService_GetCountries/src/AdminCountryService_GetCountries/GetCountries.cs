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

namespace AdminCountryService_GetCountries
{
    /**
     * Endpoint Lambda function handler.
     */
    public class Endpoint_GetCountries
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

            return await LambdaHelper.ExecuteGetAsync(request, async (IDataStores dataStores) => {
                Debug.AssertValid(dataStores);

                return await GetCountries(dataStores, request);
            });
        }

        /**
         * Get the countries.
         */
        public async Task<APIGatewayProxyResponse> GetCountries(IDataStores dataStores, APIGatewayProxyRequest request)
        {
            Debug.Tested();
            Debug.AssertValid(dataStores);
            Debug.AssertValid(request);

            try {
                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = (AmazonDynamoDBClient)dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Security check
                User user = await APIHelper.CheckLoggedInAsAdmin(dbClient, request.Headers);
                Debug.AssertValid(user);

                // Get response information
                IEnumerable<Country> countries = await CountryServiceLogicLayer.GetCountries(dbClient);
                Debug.AssertValid(countries);

                // Return response
                return new APIGatewayProxyResponse
                    {
                        StatusCode = APIHelper.STATUS_CODE_OK,
                        Body = JsonConvert.SerializeObject(countries)
                    };
            } catch (Exception exception) {
                Debug.Unreachable();
                return APIHelper.ResponseFromException(exception);
            }
        }

    }   // Endpoint_GetCountries

}   // AdminCountryService_GetCountries
