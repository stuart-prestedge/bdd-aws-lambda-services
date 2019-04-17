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

namespace UserCountryService_GetCountries
{

    /**
     * Endpoint Lambda function handler.
     */
    public class Endpoint_GetCountries
    {
        
        /// <summary>
        /// SUMMARY
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

                return await GetCountries(dataStores);
            });
        }

        /**
         * Get the countries.
         */
        public async Task<APIGatewayProxyResponse> GetCountries(IDataStores dataStores)
        {
            Debug.Tested();
            Debug.AssertValid(dataStores);

            try {
                // Get the NoSQL DB client
                AmazonDynamoDBClient dbClient = dataStores.GetNoSQLDataStore().GetDBClient();
                Debug.AssertValid(dbClient);

                // Get response information
                IEnumerable<Country> countries = await CountryServiceLogicLayer.GetCountries(dbClient);
                Debug.AssertValid(countries);
                GetCountriesResponse responseBody = new GetCountriesResponse();
                List<GetCountryResponse> availableCountries = new List<GetCountryResponse>();
                foreach (Country country in countries) {
                    Debug.Tested();
                    Debug.AssertValid(country);
                    if (country.Available) {
                        Debug.Tested();
                        availableCountries.Add(new GetCountryResponse { code = country.Code, name = country.Name, currencies = country.Currencies.ToArray() });
                    } else {
                        Debug.Tested();
                    }
                }
                responseBody.countries = availableCountries.ToArray();

                // Return response
                return new APIGatewayProxyResponse
                    {
                        StatusCode = STATUS_CODE_OK,
                        Body = JsonConvert.SerializeObject(responseBody)
                    };
            } catch (Exception exception) {
                Debug.Unreachable();
                return APIHelper.ResponseFromException(exception);
            }
        }

    }   // Endpoint_GetCountries

}   // UserCountryService_GetCountries
