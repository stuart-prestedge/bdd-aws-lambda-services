using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BDDReferenceService.Contracts;
using BDDReferenceService.Model;
using Newtonsoft.Json.Linq;

namespace BDDReferenceService.Logic
{

    /**
     * Logic layer with helper methods.
     */
    public static class CountryServiceLogicLayer
    {
        
        /**
         * Country service dataset and field names.
         */
        #region Country service dataset names
        internal const string DATASET_COUNTRIES = "Countries";
        internal const string FIELD_COUNTRIES_CODE = "Code";
        internal const string FIELD_COUNTRIES_NAME = "Name";
        internal const string FIELD_COUNTRIES_CURRENCIES = "Currencies";
        internal const string FIELD_COUNTRIES_AVAILABLE = "Available";
        internal const string DATASET_COUNTRY_AUDIT = "CountryAudit";
        internal const string FIELD_COUNTRY_AUDIT_ID = "Id";
        internal const string FIELD_COUNTRY_AUDIT_TIMESTAMP = "Timestamp";
        internal const string FIELD_COUNTRY_AUDIT_ADMINISTRATOR_ID = "AdministratorId";
        internal const string FIELD_COUNTRY_AUDIT_CHANGE_TYPE = "ChangeType";
        internal const string FIELD_COUNTRY_AUDIT_CODE = "Code";
        internal const string FIELD_COUNTRY_AUDIT_NAME = "Name";
        internal const string FIELD_COUNTRY_AUDIT_CURRENCIES = "Currencies";
        internal const string FIELD_COUNTRY_AUDIT_AVAILABLE = "Available";
        #endregion Country service dataset names

        /**
         * Helper methods
         */
        #region Country service helper methods

        /**
         * Class constructor.
         * Sets up the test data
         */
        static CountryServiceLogicLayer() {
            Debug.Tested();

            SetupTestData();
        }

        /**
         * Reset the data.
         */
        internal static void Reset() {
            Debug.Tested();

            ClearTestData();
            SetupTestData();
        }

        /**
         * Clear the data.
         */
        private static void ClearTestData() {
            Debug.Tested();

        }

        /**
         * Setup the test data.
         */
        internal static void SetupTestData() {
            Debug.Tested();

            SetupTestCurrencies();
        }

        /**
         * Setup the test currencies.
         */
        internal static void SetupTestCurrencies() {
            Debug.Tested();

            //??++ Countries.Add(new Country {
            //     Code = "gb",
            //     Name = "United Kingdom",
            //     Currencies = new List<string> { "GBP" },
            //     Available = true
            // });
        }

        #endregion Country service helper methods

        /**
         * Admin country service methods.
         */
        #region Admin country service

        /**
         * Get countries.
         */
        public static async Task<List<Country>> GetCountries(AmazonDynamoDBClient dbClient) {
            Debug.Untested();
            Debug.AssertValid(dbClient);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            List<string> attributes = new List<string>();
            attributes.Add(FIELD_COUNTRIES_CODE);
            attributes.Add(FIELD_COUNTRIES_NAME);
            attributes.Add(FIELD_COUNTRIES_CURRENCIES);
            attributes.Add(FIELD_COUNTRIES_AVAILABLE);
            ScanResponse response = await dbClient.ScanAsync(DATASET_COUNTRIES, attributes);
            Debug.AssertValid(response);
            //??++CHECK RESPONSE?
            LoggingHelper.LogMessage($"Get countries: {response.Count} items");
            List<Country> countries = new List<Country>();
            foreach (var item in response.Items)
            {
                Debug.AssertValid(item);
                countries.Add(new Country {
                    Code = item[FIELD_COUNTRIES_CODE].S,
                    Name = item[FIELD_COUNTRIES_NAME].S,
                    Currencies = item[FIELD_COUNTRIES_CURRENCIES].SS,
                    Available = item[FIELD_COUNTRIES_AVAILABLE].BOOL
                });
            }
            return countries;
        }

        /**
         * Check validity of update country request inputs.
         */
        public static Country CheckValidUpdateCountryRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                if (!string.IsNullOrEmpty((string)requestBody["name"])) {
                    Debug.Tested();
                    if (Helper.IsValidCountryCode((string)requestBody["code"])) {
                        Debug.Tested();
                        bool? available = null;
                        if (APIHelper.RequestBodyContainsField(requestBody, "available", out JToken availableField) && (availableField.Type == JTokenType.Boolean)) {
                            available = (bool)availableField;
                        }
                        if (available != null) {
                            if (APIHelper.RequestBodyContainsField(requestBody, "currencies", out JToken currenciesField) && (currenciesField.Type == JTokenType.Array)) {
                                Debug.Tested();
                                List<string> currencies = new List<string>();
                                foreach (string currency in currenciesField) {
                                    Debug.Tested();
                                    if (!Helper.IsValidCurrencyCode(currency)) {
                                        // Invalid currency
                                        Debug.Tested();
                                        throw APIHelper.CreateInvalidInputParameterException(error);
                                    }
                                    currencies.Add(currency);
                                }
                                if (currencies.Count > 0) {
                                    Debug.Tested();
                                    if (Helper.AllFieldsRecognized(requestBody,
                                                                    new List<string>(new String[]{
                                                                        "code",
                                                                        "name",
                                                                        "available",
                                                                        "currencies"
                                                                        }))) {
                                        // Valid request
                                        Debug.Tested();
                                        return new Country {
                                            Code = (string)requestBody["code"],
                                            Name = (string)requestBody["name"],
                                            Currencies = currencies,
                                            Available = (bool)available
                                        };
                                    } else {
                                        // Unrecognised field(s)
                                        Debug.Tested();
                                        error = APIHelper.UNRECOGNISED_FIELD;
                                    }
                                } else {
                                    // Empty currencies
                                    Debug.Tested();
                                }
                            } else {
                                // Missing/invalid currencies
                                Debug.Tested();
                            }
                        } else {
                            // Missing/invalid available
                            Debug.Untested();
                        }
                    } else {
                        // Missing/invalid country code
                        Debug.Tested();
                    }
                } else {
                    // Missing/invalid name
                    Debug.Tested();
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Add a new country or update an existing one.
         */
        public static async Task<bool> UpdateCountry(AmazonDynamoDBClient dbClient, string loggedInUserId, Country country) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.Assert(Helper.IsValidCountryCode(country.Code));
            Debug.Assert(country.Currencies.Count > 0);

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            // Get the existing country
            bool created = false;
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>();
            key.Add(FIELD_COUNTRIES_CODE, new AttributeValue(country.Code));
            GetItemResponse getResponse = await dbClient.GetItemAsync(DATASET_COUNTRIES, key);
            Debug.AssertValid(getResponse);
            Debug.AssertValidOrNull(getResponse.Item);
            if (getResponse.Item != null)
            {
                // The country exists so update it.
                Dictionary<string, AttributeValueUpdate> attributeUpdates = new Dictionary<string, AttributeValueUpdate>();
                attributeUpdates.Add(FIELD_COUNTRIES_NAME, new AttributeValueUpdate(new AttributeValue(country.Name), AttributeAction.PUT));
                attributeUpdates.Add(FIELD_COUNTRIES_CURRENCIES, new AttributeValueUpdate(new AttributeValue(country.Currencies), AttributeAction.PUT));
                attributeUpdates.Add(FIELD_COUNTRIES_AVAILABLE, new AttributeValueUpdate(new AttributeValue { BOOL = country.Available }, AttributeAction.PUT));
                UpdateItemResponse updateResponse = await dbClient.UpdateItemAsync(DATASET_COUNTRIES, key, attributeUpdates);
                Debug.AssertValid(updateResponse);
                //??++CHECK RESPONSE?
            }
            else
            {
                // The country does not exist so create it.
                Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue>();
                item.Add(FIELD_COUNTRIES_CODE, new AttributeValue(country.Code));
                item.Add(FIELD_COUNTRIES_NAME, new AttributeValue(country.Name));
                item.Add(FIELD_COUNTRIES_CURRENCIES, new AttributeValue(country.Currencies));
                item.Add(FIELD_COUNTRIES_AVAILABLE, new AttributeValue { BOOL = country.Available });
                PutItemResponse putResponse = await dbClient.PutItemAsync(DATASET_COUNTRIES, item);
                Debug.AssertValid(putResponse);
                //??++CHECK RESPONSE?
                created = true;
            }
            AddCountryAudit(dbClient, loggedInUserId, CountryAuditRecord.AuditChangeType.create, country);
            return created;
        }

        /**
         * Add a ticket price audit record.
         */
        private static async void AddCountryAudit(AmazonDynamoDBClient dbClient,
                                                  string loggedInUserId,
                                                  CountryAuditRecord.AuditChangeType changeType,
                                                  Country country) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertID(loggedInUserId);
            Debug.AssertValid(country);

            Dictionary<string, AttributeValue> item = new Dictionary<string, AttributeValue>();
            string id = RandomHelper.Next();
            item.Add(FIELD_COUNTRY_AUDIT_ID, new AttributeValue(id));
            item.Add(FIELD_COUNTRY_AUDIT_TIMESTAMP, new AttributeValue(APIHelper.APITimestampStringFromDateTime(DateTime.Now)));
            item.Add(FIELD_COUNTRY_AUDIT_ADMINISTRATOR_ID, new AttributeValue(loggedInUserId));
            item.Add(FIELD_COUNTRY_AUDIT_CHANGE_TYPE, new AttributeValue { N = changeType.ToString() });
            item.Add(FIELD_COUNTRY_AUDIT_CODE, new AttributeValue(country.Code));
            item.Add(FIELD_COUNTRY_AUDIT_NAME, new AttributeValue(country.Name));
            item.Add(FIELD_COUNTRY_AUDIT_CURRENCIES, new AttributeValue(country.Currencies));
            item.Add(FIELD_COUNTRY_AUDIT_AVAILABLE, new AttributeValue { BOOL = country.Available });
            PutItemResponse response = await dbClient.PutItemAsync(DATASET_COUNTRY_AUDIT, item);
            Debug.AssertValid(response);
            //??++CHECK RESPONSE?
        }

        /**
         * Check validity of get ticket price audits request inputs.
         */
        internal static GetCountryAuditsRequest CheckValidGetCountryAuditsRequest(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            string error = null;
            if (requestBody != null) {
                Debug.Tested();
                bool validFrom = false;
                string from = null;
                if (APIHelper.RequestBodyContainsField(requestBody, "from", out JToken fromField)) {
                    if (fromField.Type == JTokenType.Date) {
                        validFrom = true;
                        from = APIHelper.APIDateTimeStringFromDateTime((DateTime)fromField);
                    } else if (APIHelper.IsValidAPIDateTimeString((string)fromField)) {
                        validFrom = true;
                        from = (string)fromField;
                    }
                }
                if (validFrom) {
                    Debug.Tested();
                    bool validTo = false;
                    string to = null;
                    if (APIHelper.RequestBodyContainsField(requestBody, "to", out JToken toField)) {
                        if (toField.Type == JTokenType.Date) {
                            validTo = true;
                            to = APIHelper.APIDateTimeStringFromDateTime((DateTime)toField);
                        } else if (APIHelper.IsValidAPIDateTimeString((string)toField)) {
                            validTo = true;
                            to = (string)toField;
                        }
                    }
                    if (validTo) {
                        Debug.Tested();
                        if (Helper.AllFieldsRecognized(requestBody,
                                                        new List<string>(new String[]{
                                                            "from",
                                                            "to"
                                                            }))) {
                            // Valid request
                            Debug.Tested();
                            return new GetCountryAuditsRequest {
                                from = from,
                                to = to
                            };
                        } else {
                            // Unrecognised field(s)
                            Debug.Tested();
                            error = APIHelper.UNRECOGNISED_FIELD;
                        }
                    } else {
                        // Missing/invalid to
                        Debug.Tested();
                    }
                } else {
                    // Missing/invalid from
                    Debug.Tested();
                }
            } else {
                // No body
                Debug.Tested();
                error = APIHelper.NO_REQUEST_BODY;
            }
            throw APIHelper.CreateInvalidInputParameterException(error);
        }

        /**
         * Get a list of all the audit records in a time range.
         * The response is limited to a 1000 records.
         */
        internal static async Task<List<CountryAuditRecord>> GetCountryAuditRecords(AmazonDynamoDBClient dbClient, GetCountryAuditsRequest getCountryAuditsRequest) {
            Debug.Tested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(getCountryAuditsRequest);
            Debug.Assert(APIHelper.IsValidAPIDateTimeString(getCountryAuditsRequest.from));
            Debug.Assert(APIHelper.IsValidAPIDateTimeString(getCountryAuditsRequest.to));

            // Check that the system is not locked.
            await SystemHelper.CheckSystemNotLocked(dbClient);

            //
            DateTime from = (DateTime)APIHelper.DateTimeFromAPIDateTimeString(getCountryAuditsRequest.from);
            DateTime to = (DateTime)APIHelper.DateTimeFromAPIDateTimeString(getCountryAuditsRequest.to);
            if (from <= to) {
                Debug.Tested();
                bool limitResults = ((to - from).TotalSeconds > 60);
                List<string> attributes = new List<string>();
                attributes.Add(FIELD_COUNTRY_AUDIT_ID);
                attributes.Add(FIELD_COUNTRY_AUDIT_TIMESTAMP);
                attributes.Add(FIELD_COUNTRY_AUDIT_NAME);
                attributes.Add(FIELD_COUNTRY_AUDIT_ADMINISTRATOR_ID);
                attributes.Add(FIELD_COUNTRY_AUDIT_CHANGE_TYPE);
                attributes.Add(FIELD_COUNTRY_AUDIT_CODE);
                attributes.Add(FIELD_COUNTRY_AUDIT_NAME);
                attributes.Add(FIELD_COUNTRY_AUDIT_CURRENCIES);
                attributes.Add(FIELD_COUNTRY_AUDIT_AVAILABLE);
                ScanResponse response = await dbClient.ScanAsync(DATASET_COUNTRY_AUDIT, attributes);
                Debug.AssertValid(response);
                //??++CHECK RESPONSE?
                List<CountryAuditRecord> retVal = new List<CountryAuditRecord>();
                    //     Code = item[FIELD_COUNTRIES_CODE].S,
                    //     Name = item[FIELD_COUNTRIES_NAME].S,
                    //     Currencies = item[FIELD_COUNTRIES_CURRENCIES].SS,
                    //     Available = item[FIELD_COUNTRIES_AVAILABLE].BOOL
                    // });
                foreach (var item in response.Items)
                {
                    Debug.Untested();
                    Debug.AssertValid(item);

                    DateTime? timestamp = APIHelper.DateTimeFromAPITimestampString(item[FIELD_COUNTRY_AUDIT_TIMESTAMP].S);
                    Debug.AssertValid(timestamp);
                    if (from <= timestamp) {
                        Debug.Tested();
                        if (timestamp < to) {
                            Debug.Tested();
                            CountryAuditRecord countryAuditRecord = new CountryAuditRecord {
                                ID = item[FIELD_COUNTRY_AUDIT_ID].S,
                                Timestamp = (DateTime)timestamp,
                                AdministratorID = item[FIELD_COUNTRY_AUDIT_ADMINISTRATOR_ID].S,
                                ChangeType = (CountryAuditRecord.AuditChangeType)int.Parse(item[FIELD_COUNTRY_AUDIT_CHANGE_TYPE].N),
                                Code = item[FIELD_COUNTRY_AUDIT_CODE].S,
                                Name = item[FIELD_COUNTRY_AUDIT_NAME].S,
                                Currencies = item[FIELD_COUNTRY_AUDIT_CURRENCIES].SS,
                                Available = item[FIELD_COUNTRY_AUDIT_AVAILABLE].BOOL
                            };
                            retVal.Add(countryAuditRecord);
                            if (limitResults) {
                                Debug.Tested();
                                if (retVal.Count == 1000) {
                                    Debug.Untested();
                                    break;
                                } else {
                                    Debug.Tested();
                                }
                            } else {
                                Debug.Untested();
                            }
                        } else {
                            Debug.Untested();
                        }
                    } else {
                        Debug.Untested();
                    }
                }
                return retVal;
            } else {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_FROM_GREATER_THAN_TO);
            }
        }

        #endregion Admin country service

        /**
         * User country service methods.
         */
        #region User country service

        #endregion User country service

        /*
         * Worker methods
         */
        #region Worker methods

        // There are no workers in the country service.
        
        #endregion Worker methods

        /*
         * The test methods
         */
        #region Test methods
        
        /*
         * The main test entry point.
         */
        internal static void Test() {
            Debug.Tested();
            
        }

        #endregion Test methods

    }   // CountryServiceLogicLayer

}   // BDDReferenceService.Logic
