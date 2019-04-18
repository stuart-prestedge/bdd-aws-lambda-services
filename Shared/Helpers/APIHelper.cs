using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using BDDReferenceService.Contracts;
using BDDReferenceService.Logic;
using BDDReferenceService.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BDDReferenceService
{
    public static class APIHelper {

        /*
         * Request methods
         */
        public const string REQUEST_METHOD_GET = "GET";
        public const string REQUEST_METHOD_PUT = "PUT";
        public const string REQUEST_METHOD_POST = "POST";
        public const string REQUEST_METHOD_DELETE = "DELETE";

        /*
         * Response status codes
         */
        public const int STATUS_CODE_OK = 200;
        public const int STATUS_CODE_CREATED = 201;
        public const int STATUS_CODE_NO_CONTENT = 204;
        public const int STATUS_CODE_NOT_MODIFIED = 304;
        public const int STATUS_CODE_BAD_REQUEST = 400;
        public const int STATUS_CODE_UNAUTHORIZED = 401;
        public const int STATUS_CODE_FORBIDDEN = 403;
        public const int STATUS_CODE_NOT_FOUND = 404;
        public const int STATUS_CODE_INTERNAL_ERROR = 500;

        /* 
         * Error message constants
         */
        internal const string NO_REQUEST_BODY = "NO_REQUEST_BODY";
        internal const string NON_EMPTY_BODY = "NON_EMPTY_BODY";
        internal const string UNRECOGNISED_FIELD = "UNRECOGNISED_FIELD";
        internal const string INVALID_EMAIL_ADDRESS = "INVALID_EMAIL_ADDRESS";
        internal const string INVALID_PASSWORD = "INVALID_PASSWORD";
        internal const string INVALID_USER_ID = "INVALID_USER_ID";
        internal const string INVALID_NAME = "INVALID_NAME";
        internal const string INVALID_LINK_ID = "INVALID_LINK_ID";
        internal const string INVALID_MESSAGE = "INVALID_MESSAGE";
        internal const string INVALID_RECAPTCHA = "INVALID_RECAPTCHA";

        /**
         * Is the specified date string valid? Must be a non-empty string in "YYYY-MM-DD" format.
         * Does not check the semantics of the date (e.g. valid month or day, in the past etc.).
         */
        internal static bool IsValidAPIDateString(string date) {
            Debug.Tested();
            Debug.AssertValidOrNull(date);

            bool retVal = false;
            if (date != null) {
                if (date.Length == 10) {
                    if ((date[0] >= '0') && (date[0] <= '9')) {
                        if ((date[1] >= '0') && (date[1] <= '9')) {
                            if ((date[2] >= '0') && (date[2] <= '9')) {
                                if ((date[3] >= '0') && (date[3] <= '9')) {
                                    if (date[4] == '-') {
                                        if ((date[5] >= '0') && (date[5] <= '9')) {
                                            if ((date[6] >= '0') && (date[6] <= '9')) {
                                                if (date[7] == '-') {
                                                    if ((date[8] >= '0') && (date[8] <= '9')) {
                                                        if ((date[9] >= '0') && (date[9] <= '9')) {
                                                            retVal = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return retVal;
        }

        /**
         * Is the specified date/time string valid? Must be a non-empty string in "YYYY-MM-DDTHH:MM:SSZ" format.
         * Does not check the semantics of the date (e.g. valid month or day, in the past etc.).
         */
        internal static bool IsValidAPIDateTimeString(string date) {
            Debug.Tested();
            Debug.AssertValidOrNull(date);

            bool retVal = false;
            if (date != null) {
                if (date.Length == 20) {
                    if ((date[0] >= '0') && (date[0] <= '9')) {
                        if ((date[1] >= '0') && (date[1] <= '9')) {
                            if ((date[2] >= '0') && (date[2] <= '9')) {
                                if ((date[3] >= '0') && (date[3] <= '9')) {
                                    if (date[4] == '-') {
                                        if ((date[5] >= '0') && (date[5] <= '9')) {
                                            if ((date[6] >= '0') && (date[6] <= '9')) {
                                                if (date[7] == '-') {
                                                    if ((date[8] >= '0') && (date[8] <= '9')) {
                                                        if ((date[9] >= '0') && (date[9] <= '9')) {
                                                            if (date[10] == 'T') {
                                                                if ((date[11] >= '0') && (date[11] <= '9')) {
                                                                    if ((date[12] >= '0') && (date[12] <= '9')) {
                                                                        if (date[13] == ':') {
                                                                            if ((date[14] >= '0') && (date[14] <= '9')) {
                                                                                if ((date[15] >= '0') && (date[15] <= '9')) {
                                                                                    if (date[16] == ':') {
                                                                                        if ((date[17] >= '0') && (date[17] <= '9')) {
                                                                                            if ((date[18] >= '0') && (date[18] <= '9')) {
                                                                                                if (date[19] == 'Z') {
                                                                                                    retVal = true;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return retVal;
        }

        /**
         * Is the specified date/time string valid? Must be a non-empty string in "YYYY-MM-DDTHH:MM:SS.MMMZ" format.
         * Does not check the semantics of the date (e.g. valid month or day, in the past etc.).
         */
        internal static bool IsValidAPITimestampString(string date) {
            Debug.Untested();
            Debug.AssertValidOrNull(date);

            bool retVal = false;
            if (date != null) {
                if (date.Length == 24) {
                    if ((date[0] >= '0') && (date[0] <= '9')) {
                        if ((date[1] >= '0') && (date[1] <= '9')) {
                            if ((date[2] >= '0') && (date[2] <= '9')) {
                                if ((date[3] >= '0') && (date[3] <= '9')) {
                                    if (date[4] == '-') {
                                        if ((date[5] >= '0') && (date[5] <= '9')) {
                                            if ((date[6] >= '0') && (date[6] <= '9')) {
                                                if (date[7] == '-') {
                                                    if ((date[8] >= '0') && (date[8] <= '9')) {
                                                        if ((date[9] >= '0') && (date[9] <= '9')) {
                                                            if (date[10] == 'T') {
                                                                if ((date[11] >= '0') && (date[11] <= '9')) {
                                                                    if ((date[12] >= '0') && (date[12] <= '9')) {
                                                                        if (date[13] == ':') {
                                                                            if ((date[14] >= '0') && (date[14] <= '9')) {
                                                                                if ((date[15] >= '0') && (date[15] <= '9')) {
                                                                                    if (date[16] == ':') {
                                                                                        if ((date[17] >= '0') && (date[17] <= '9')) {
                                                                                            if ((date[18] >= '0') && (date[18] <= '9')) {
                                                                                                if (date[19] == '.') {
                                                                                                    if ((date[20] >= '0') && (date[17] <= '9')) {
                                                                                                        if ((date[21] >= '0') && (date[18] <= '9')) {
                                                                                                            if ((date[22] >= '0') && (date[18] <= '9')) {
                                                                                                                if (date[23] == 'Z') {
                                                                                                                    retVal = true;
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return retVal;
        }

        /**
         * Convert a string in "YYYY-MM-DD" format to a date/time object.
         */
        internal static DateTime? DateFromAPIDateString(string date) {
            Debug.Tested();
            Debug.AssertValidOrNull(date);

            DateTime? retVal = null;
            if (IsValidAPIDateString(date)) {
                Debug.Tested();
                int year = int.Parse(date.Substring(0, 4));
                int month = int.Parse(date.Substring(5, 2));
                int day = int.Parse(date.Substring(8, 2));
                retVal = new DateTime(year, month, day);
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Convert a string in "YYYY-MM-DDTHH:MM:SSZ" format to a date/time object.
         */
        internal static DateTime? DateTimeFromAPIDateTimeString(string date) {
            Debug.Tested();
            Debug.AssertValidOrNull(date);

            DateTime? retVal = null;
            if (IsValidAPIDateTimeString(date)) {
                Debug.Tested();
                int year = int.Parse(date.Substring(0, 4));
                int month = int.Parse(date.Substring(5, 2));
                int day = int.Parse(date.Substring(8, 2));
                int hour = int.Parse(date.Substring(11, 2));
                int minute = int.Parse(date.Substring(14, 2));
                int second = int.Parse(date.Substring(17, 2));
                retVal = new DateTime(year, month, day, hour, minute, second);
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Convert a string in "YYYY-MM-DDTHH:MM:SS.MMMZ" format to a date/time object.
         */
        internal static DateTime? DateTimeFromAPITimestampString(string date) {
            Debug.Untested();
            Debug.AssertValidOrNull(date);

            DateTime? retVal = null;
            if (IsValidAPIDateTimeString(date)) {
                Debug.Tested();
                int year = int.Parse(date.Substring(0, 4));
                int month = int.Parse(date.Substring(5, 2));
                int day = int.Parse(date.Substring(8, 2));
                int hour = int.Parse(date.Substring(11, 2));
                int minute = int.Parse(date.Substring(14, 2));
                int second = int.Parse(date.Substring(17, 2));
                int millisecond = int.Parse(date.Substring(20, 3));
                retVal = new DateTime(year, month, day, hour, minute, second, millisecond);
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Convert a date/time object to a date string in "YYYY-MM-DD" format.
         */
        internal static string APIDateStringFromDate(DateTime? dt) {
            Debug.Tested();
            Debug.AssertValidOrNull(dt);

            string retVal = null;
            if (dt != null) {
                Debug.Tested();
                retVal = $"{dt?.Year:D4}-{dt?.Month:D2}-{dt?.Day:D2}";
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Convert a date/time object to a date/time string in "YYYY-MM-DDTHH:MM::SSZ" format.
         */
        public static string APIDateTimeStringFromDateTime(DateTime? dt) {
            Debug.Tested();
            Debug.AssertValidOrNull(dt);

            string retVal = null;
            if (dt != null) {
                Debug.Tested();
                retVal = $"{dt?.Year:D4}-{dt?.Month:D2}-{dt?.Day:D2}T{dt?.Hour:D2}:{dt?.Minute:D2}:{dt?.Second:D2}Z";
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Convert a date/time object to a date/time string in "YYYY-MM-DDTHH:MM::SS.MMMZ" format.
         */
        internal static string APITimestampStringFromDateTime(DateTime? dt) {
            Debug.Untested();
            Debug.AssertValidOrNull(dt);

            string retVal = null;
            if (dt != null) {
                Debug.Tested();
                retVal = $"{dt?.Year:D4}-{dt?.Month:D2}-{dt?.Day:D2}T{dt?.Hour:D2}:{dt?.Minute:D2}:{dt?.Second:D2}.{dt?.Millisecond:D3}Z";
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Check that the correct HTTP method is used.
         */
        public static void CheckRequestMethod(string httpMethod, string requiredHttpMethod)
        {
            Debug.Untested();
            Debug.AssertString(httpMethod);
            Debug.AssertString(requiredHttpMethod);
            Debug.Assert((requiredHttpMethod == REQUEST_METHOD_GET) ||
                         (requiredHttpMethod == REQUEST_METHOD_PUT) ||
                         (requiredHttpMethod == REQUEST_METHOD_POST) ||
                         (requiredHttpMethod == REQUEST_METHOD_DELETE));

            if (httpMethod != requiredHttpMethod)
            {
                Debug.Untested();
                throw new Exception(SharedLogicLayer.ERROR_INVALID_HTTP_METHOD, new Exception(SharedLogicLayer.ERROR_INVALID_HTTP_METHOD));
            }
        }

        /**
         * Check that the request body is null.
         */
        public static void CheckEmptyRequestBody(Stream requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            if (requestBody == null) {
                return;
            } else if (requestBody.ReadByte() == -1) {
                return;
            }
            throw APIHelper.CreateInvalidInputParameterException(NON_EMPTY_BODY);
        }

        /**
         * Check that the request body is empty (or null).
         */
        public static void CheckEmptyRequestBody(JObject requestBody) {
            Debug.Tested();
            Debug.AssertValidOrNull(requestBody);

            if ((requestBody == null) || Helper.AllFieldsRecognized(requestBody)) {
                return;
            }
            throw APIHelper.CreateInvalidInputParameterException(NON_EMPTY_BODY);
        }

        /**
         * Check that the request body is empty (or null).
         */
        internal static bool RequestBodyContainsField(JObject requestBody, string fieldName, out JToken field) {
            Debug.Tested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            if (requestBody.ContainsKey(fieldName)) {
                Debug.Tested();
                retVal = true;
                field = requestBody[fieldName];
                Debug.AssertValid(field);
            } else {
                Debug.Tested();
                field = null;
            }
            return retVal;
        }

        /**
         * Gets the value of an ID from the passed in request body object.
         */
        internal static bool GetIDFromRequestBody(JObject requestBody, string fieldName, out string id) {
            Debug.Tested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            id = Helper.INVALID_ID;
            if (APIHelper.RequestBodyContainsField(requestBody, fieldName, out JToken field)) {
                Debug.Tested();
                if (GetIDFromRequestField(field, out string id_)) {
                    retVal = true;
                    id = id_;
                }
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Gets the value of an ID from the passed in request body object.
         */
        internal static bool GetIDFromRequestField(JToken field, out string id) {
            Debug.Tested();
            Debug.AssertValid(field);

            bool retVal = false;
            id = Helper.INVALID_ID;
            if (field.Type == JTokenType.Integer) {
                Debug.Tested();
                string id_ = field.ToString();
                if (Helper.IsValidID(id_)) {
                    retVal = true;
                    id = id_;
                }
            } else if (field.Type == JTokenType.String) {
                Debug.Untested();
                if (Helper.IsValidID((string)field)) {
                    retVal = true;
                    id = (string)field;
                }
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Gets the value of an array of IDs from the passed in request body object.
         */
        internal static bool GetIDArrayFromRequestBody(JObject requestBody, string fieldName, out string[] ids) {
            Debug.Untested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            ids = null;
            if (APIHelper.RequestBodyContainsField(requestBody, fieldName, out JToken field)) {
                Debug.Tested();
                if (field.Type == JTokenType.Array) {
                    Debug.Tested();
                    retVal = true;
                    JArray jArray = (JArray)field;
                    ids = new string[jArray.Count];
                    int index = 0;
                    foreach (JToken token in jArray) {
                        Debug.Tested();
                        Debug.AssertValid(token);
                        if (GetIDFromRequestField(token, out string id_)) {
                            Debug.Tested();
                            ids[index++] = id_;
                        } else {
                            Debug.Untested();
                            retVal = false;
                            break;
                        }
                    }
                } else {
                    Debug.Untested();
                }
            } else {
                Debug.Untested();
            }
            return retVal;
        }

        /**
         * Gets the value of an optional ID from the passed in request body object.
         */
        internal static bool GetOptionalIDFromRequestBody(JObject requestBody, string fieldName, out string id) {
            Debug.Tested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            id = Helper.INVALID_ID;
            if (!APIHelper.RequestBodyContainsField(requestBody, fieldName, out JToken field) || (field.Type == JTokenType.Null)) {
                Debug.Tested();
                retVal = true;
            } else {
                Debug.Tested();
                if (GetIDFromRequestBody(requestBody, fieldName, out string id_)) {
                    Debug.Tested();
                    retVal = true;
                    id = id_;
                } else {
                    Debug.Tested();
                }
            }
            return retVal;
        }

        /**
         * Gets the value of a boolean from the passed in request body object.
         */
        internal static bool GetBooleanFromRequestBody(JObject requestBody, string fieldName, out bool? val) {
            Debug.Tested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            val = null;
            if (APIHelper.RequestBodyContainsField(requestBody, fieldName, out JToken field)) {
                Debug.Tested();
                if (field.Type == JTokenType.Boolean) {
                    Debug.Tested();
                    retVal = true;
                    val = (bool)field;
                } else if (field.Type == JTokenType.String) {
                    Debug.Tested();
                    if (bool.TryParse((string)field, out bool autoDraw_)) {
                        Debug.Untested();
                        retVal = true;
                        val = autoDraw_;
                    } else {
                        Debug.Tested();
                    }
                } else {
                    Debug.Tested();
                }
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Gets the value of an optional boolean from the passed in request body object.
         */
        internal static bool GetOptionalBooleanFromRequestBody(JObject requestBody, string fieldName, out bool? val) {
            Debug.Tested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            if (!APIHelper.RequestBodyContainsField(requestBody, fieldName, out JToken field)) {
                Debug.Tested();
                retVal = true;
                val = null;
            } else {
                Debug.Tested();
                retVal = GetBooleanFromRequestBody(requestBody, fieldName, out val);
            }
            return retVal;
        }

        /**
         * Gets the value of an ID from the passed in request body object.
         */
        internal static bool GetNumberFromRequestBody(JObject requestBody, string fieldName, out int? val) {
            Debug.Tested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            val = null;
            if (APIHelper.RequestBodyContainsField(requestBody, fieldName, out JToken field)) {
                Debug.Tested();
                if (field.Type == JTokenType.Integer) {
                    Debug.Tested();
                    retVal = true;
                    val = (int)field;
                } else if ((field.Type == JTokenType.String) && int.TryParse((string)field, out int val_)) {
                    Debug.Untested();
                    retVal = true;
                    val = val_;
                } else {
                    Debug.Tested();
                }
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Gets the value of an optional ID from the passed in request body object.
         */
        internal static bool GetOptionalNumberFromRequestBody(JObject requestBody, string fieldName, out int? val) {
            Debug.Tested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            val = null;
            if (!APIHelper.RequestBodyContainsField(requestBody, fieldName, out JToken field) || (field.Type == JTokenType.Null)) {
                Debug.Tested();
                retVal = true;
            } else {
                Debug.Tested();
                retVal = GetNumberFromRequestBody(requestBody, fieldName, out val);
            }
            return retVal;
        }

        /**
         * Gets the value of a string from the passed in request body object.
         * Must exist. Cannot be null. Can be empty.
         */
        internal static bool GetStringFromRequestBody(JObject requestBody, string fieldName, out string val, bool failIfEmpty = false) {
            Debug.Tested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            val = null;
            if (APIHelper.RequestBodyContainsField(requestBody, fieldName, out JToken field)) {
                Debug.Tested();
                if (field.Type == JTokenType.String) {
                    Debug.Tested();
                    Debug.Assert(field != null);
                    val = (string)field;
                    if (!failIfEmpty) {
                        Debug.Untested();
                        retVal = true;
                    } else if (val != "") {
                        Debug.Untested();
                        retVal = true;
                    } else {
                        Debug.Untested();
                    }
                } else {
                    Debug.Tested();
                }
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Gets the value of an optional string from the passed in request body object.
         * If does not exist or is null then true is returned with val set to null.
         */
        internal static bool GetOptionalStringFromRequestBody(JObject requestBody, string fieldName, out string val) {
            Debug.Tested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            val = null;
            if (!APIHelper.RequestBodyContainsField(requestBody, fieldName, out JToken field) || (field.Type == JTokenType.Null)) {
                Debug.Tested();
                retVal = true;
            } else {
                Debug.Tested();
                retVal = GetStringFromRequestBody(requestBody, fieldName, out val);
            }
            return retVal;
        }

        /**
         * Gets the value of a date/time (timestamp) string from the passed in request body object.
         */
        internal static bool GetDateTimeFromRequestBody(JObject requestBody, string fieldName, out string dt) {
            Debug.Tested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            dt = null;
            if (APIHelper.RequestBodyContainsField(requestBody, fieldName, out JToken field)) {
                Debug.Tested();
                if (field.Type == JTokenType.Date) {
                    Debug.Tested();
                    retVal = true;
                    dt = APIHelper.APIDateTimeStringFromDateTime((DateTime)field);
                } else if (APIHelper.IsValidAPIDateTimeString((string)field)) {
                    Debug.Untested();
                    retVal = true;
                    dt = (string)field;
                } else {
                    Debug.Tested();
                }
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Gets the value of an optional date/time (timestamp) string from the passed in request body object.
         */
        internal static bool GetOptionalDateTimeFromRequestBody(JObject requestBody, string fieldName, out string dt) {
            Debug.Tested();
            Debug.AssertValid(requestBody);
            Debug.AssertString(fieldName);

            bool retVal = false;
            dt = null;
            if (!APIHelper.RequestBodyContainsField(requestBody, fieldName, out JToken field) || (field.Type == JTokenType.Null)) {
                Debug.Tested();
                retVal = true;
            } else {
                Debug.Tested();
                retVal = APIHelper.GetDateTimeFromRequestBody(requestBody, fieldName, out dt);
            }
            return retVal;
        }

        /**
         * Get the ID of the logged in user from the supplied access token in the Bearer token.
         * Returns null if there is an issue of any sort.
         */
        private static async Task<string> GetAuthorization(AmazonDynamoDBClient dbClient, IDictionary<string, string> headers) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValidOrNull(headers);

            //??--bool retVal = false;
            //??--userId = Helper.INVALID_ID;
            string retVal = null;
            if (headers != null) {
                string authorization = (string)headers["Authorization"];
                if (!string.IsNullOrEmpty(authorization)) {
                    Debug.Tested();
                    string[] parts = authorization.Split(' ');
                    Debug.AssertValid(parts);
                    if (parts.Length == 2) {
                        Debug.Tested();
                        if (parts[0].Equals("BEARER", StringComparison.InvariantCultureIgnoreCase)) {
                            Debug.Tested();
                            string accessToken = parts[1];
                            Debug.AssertString(accessToken);
                            string userId = await IdentityServiceLogicLayer.UserIDFromAccessToken(dbClient, accessToken);
                            if (Helper.IsValidID(userId)) {
                                Debug.Tested();
                                retVal = userId;
                            } else {
                                Debug.Tested();
                            }
                        } else {
                            Debug.Tested();
                        }
                    } else {
                        Debug.Tested();
                    }
                } else {
                    Debug.Tested();
                }
            } else {
                Debug.Tested();
            }
            return retVal;
        }

        /**
         * Is a user logged in and are they an administrator?
         */
        //??? internal static bool IsLoggedInAsAdmin_(IDictionary<string, string> requestHeaders, out string loggedInUserId) {
        //     Debug.Tested();
        //     Debug.AssertValid(requestHeaders);

        //     bool retVal = false;
        //     loggedInUserId = Helper.INVALID_ID;
        //     if (GetAuthorization(requestHeaders, out string userId)) {
        //         Debug.Tested();
        //         Debug.AssertID(userId);
        //         if (IdentityServiceLogicLayer.UserHasPermission(userId, IdentityServiceLogicLayer.PERMISSION_IS_ADMIN)) {
        //             Debug.Tested();
        //             loggedInUserId = userId;
        //             retVal = true;
        //         } else {
        //             Debug.Untested();
        //         }
        //     } else {
        //         Debug.Tested();
        //     }
        //     return retVal;
        // }

        /**
         * Is a user logged in and are they an administrator?
         * If so, returns the user object.
         * If not, returns null.
         */
        public static async Task<User> CheckLoggedInAsAdmin(AmazonDynamoDBClient dbClient,
                                                            IDictionary<string, string> requestHeaders) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(requestHeaders);

            string userId = await GetAuthorization(dbClient, requestHeaders);
            Debug.AssertIDOrNull(userId);
            if (userId != null)
            {
                // A valid user ID exists.
                Debug.Tested();
                User user = await IdentityServiceLogicLayer.FindUserByID(dbClient, userId);
                Debug.AssertValidOrNull(user);
                if (user != null)
                {
                    if (IdentityServiceLogicLayer.UserHasPermission(user, IdentityServiceLogicLayer.PERMISSION_IS_ADMIN))
                    {
                        Debug.Tested();
                        //??--loggedInUserId = userId;
                        return user;
                    }
                    else
                    {
                        Debug.Tested();
                        throw new Exception(SharedLogicLayer.ERROR_NOT_AN_ADMIN, new Exception(SharedLogicLayer.ERROR_NOT_AN_ADMIN));
                    }
                }
                else
                {
                    Debug.Untested();
                    throw new Exception(IdentityServiceLogicLayer.ERROR_USER_NOT_FOUND, new Exception(SharedLogicLayer.ERROR_NOT_LOGGED_IN));
                }
            }
            else
            {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_NOT_LOGGED_IN, new Exception(SharedLogicLayer.ERROR_NOT_LOGGED_IN));
            }
        }

        /**
         * Is a user logged in and are they an administrator?
         */
        //??? internal static void CheckIsLoggedInAsAdminWithPermissions(IDictionary<string, string> requestHeaders, out string loggedInUserId, string permission) {
        //     Debug.Tested();
        //     Debug.AssertValid(requestHeaders);
        //     Debug.AssertString(permission);

        //     CheckLoggedInAsAdmin(requestHeaders, out loggedInUserId);
        //     Debug.AssertID(loggedInUserId);
        //     if (!IdentityServiceLogicLayer.UserHasPermission(loggedInUserId, permission)) {
        //         Debug.Tested();
        //         throw new Exception(SharedLogicLayer.ERROR_NO_PERMISSION);
        //     } else {
        //         Debug.Tested();
        //     }
        // }

        /**
         * Is a user logged in?
         * Decode the Authorization header to check.
         * Returns the logged in user ID.
         */
        public static async Task<string> CheckLoggedIn(AmazonDynamoDBClient dbClient, IDictionary<string, string> requestHeaders) {
            Debug.Untested();
            Debug.AssertValid(dbClient);
            Debug.AssertValid(requestHeaders);

            string userId = await GetAuthorization(dbClient, requestHeaders);
            Debug.AssertIDOrNull(userId);
            if (userId != null)
            {
                // A valid user ID exists.
                Debug.Tested();
                return userId;
            }
            else
            {
                Debug.Tested();
                throw new Exception(SharedLogicLayer.ERROR_NOT_LOGGED_IN, new Exception(SharedLogicLayer.ERROR_NOT_LOGGED_IN));
            }
        }

        /**
         * Create an invalid parameter exception.
         */
        internal static Exception CreateInvalidInputParameterException(string error) {
            Debug.Tested();
            Debug.AssertStringOrNull(error);

            if (error == null) {
                return new Exception(SharedLogicLayer.ERROR_INVALID_INPUT_PARAMETER);
            } else {
                return new Exception(SharedLogicLayer.ERROR_INVALID_INPUT_PARAMETER, new Exception(error));
            }
        }

        /**
         * Creates an action result from an exception.
         */
        public static APIGatewayProxyResponse ResponseFromException(Exception exception) {
            Debug.Tested();
            Debug.AssertValid(exception);

            int statusCode = STATUS_CODE_INTERNAL_ERROR;
            if (exception.Message == SharedLogicLayer.ERROR_SYSTEM_ALREADY_LOCKED)
            {
                statusCode = STATUS_CODE_NOT_MODIFIED;   // Not modified
            }
            else if ((exception.Message == GameServiceLogicLayer.ERROR_GAME_NOT_FOUND) ||
                (exception.Message == GameServiceLogicLayer.ERROR_DRAW_NOT_FOUND) ||
                (exception.Message == GameServiceLogicLayer.ERROR_DRAW_NOT_IN_GAME) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_TICKET_NOT_FOUND) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_TICKET_NUMBER_NOT_FOUND) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_TICKET_GIFT_OFFER_NOT_FOUND) ||
                (exception.Message == SyndicateServiceLogicLayer.ERROR_SYNDICATE_NOT_FOUND)) 
            {
                statusCode = STATUS_CODE_NOT_FOUND;
            }
            else if ((exception.Message == SharedLogicLayer.ERROR_SYSTEM_LOCKED) ||
                (exception.Message == SharedLogicLayer.ERROR_NOT_LOGGED_IN) ||
                (exception.Message == SharedLogicLayer.ERROR_NOT_AN_ADMIN) ||
                (exception.Message == SharedLogicLayer.ERROR_NO_PERMISSION) ||
                (exception.Message == SharedLogicLayer.ERROR_INVALID_LINK) ||
                (exception.Message == SharedLogicLayer.ERROR_INVALID_LINK_USER) ||
                (exception.Message == SharedLogicLayer.ERROR_UNRECOGNIZED_EMAIL_ADDRESS) ||
                (exception.Message == IdentityServiceLogicLayer.ERROR_USER_NOT_FOUND) ||
                (exception.Message == IdentityServiceLogicLayer.ERROR_USER_ACCOUNT_CLOSED) ||
                (exception.Message == IdentityServiceLogicLayer.ERROR_EMAIL_ALREADY_VERIFIED) ||
                (exception.Message == GameServiceLogicLayer.ERROR_GAME_LOCKED) ||
                (exception.Message == GameServiceLogicLayer.ERROR_GAME_NOT_LOCKED) ||
                (exception.Message == GameServiceLogicLayer.ERROR_GAME_PUBLISHED) ||
                (exception.Message == GameServiceLogicLayer.ERROR_GAME_NOT_PUBLISHED) ||
                (exception.Message == GameServiceLogicLayer.ERROR_GAME_FROZEN) ||
                (exception.Message == GameServiceLogicLayer.ERROR_GAME_NOT_FROZEN) ||
                (exception.Message == GameServiceLogicLayer.ERROR_GAME_DOES_NOT_MATCH) ||
                (exception.Message == GameServiceLogicLayer.ERROR_DRAW_DATE_ALREADY_SET) ||
                (exception.Message == GameServiceLogicLayer.ERROR_DRAW_ALREADY_DRAWN) ||
                (exception.Message == GameServiceLogicLayer.ERROR_DRAW_NOT_DRAWN) ||
                (exception.Message == GameServiceLogicLayer.ERROR_NOT_TIME_FOR_DRAW) ||
                (exception.Message == GameServiceLogicLayer.ERROR_DRAW_IS_AUTO_DRAW) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_USER_NOT_TICKET_OWNER_OR_OFFEREE) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_TICKET_NOT_RESERVED_OR_OWNED) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_TICKET_NOT_OWNED) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_TICKET_NOT_OFFERED_TO_USER) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_TICKET_NOT_OFFERED_TO_EMAIL) ||
                (exception.Message == SyndicateServiceLogicLayer.ERROR_USER_NOT_SYNDICATE_MEMBER))
            {
                statusCode = STATUS_CODE_UNAUTHORIZED;
            }
            else if ((exception.Message == SharedLogicLayer.ERROR_INVALID_HTTP_METHOD) ||
                (exception.Message == SharedLogicLayer.ERROR_INVALID_INPUT_PARAMETER) ||
                (exception.Message == IdentityServiceLogicLayer.ERROR_NO_PHONE_NUMBER_SET) ||
                (exception.Message == IdentityServiceLogicLayer.ERROR_PHONE_NUMBER_VERIFIED) ||
                (exception.Message == GameServiceLogicLayer.ERROR_GAME_NAME_ALREADY_IN_USE) ||
                (exception.Message == GameServiceLogicLayer.ERROR_GAME_HAS_NO_DRAWS) ||
                (exception.Message == GameServiceLogicLayer.ERROR_GAME_OPEN_DATE_NOT_BEFORE_CLOSE_DATE) ||
                (exception.Message == GameServiceLogicLayer.ERROR_INVALID_AUDIT_DATA_TYPE) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_TICKET_ALREADY_RESERVED_OR_OWNED) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_TICKET_ALREADY_OFFERED) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_TICKET_NOT_OFFERED) ||
                (exception.Message == TicketingServiceLogicLayer.ERROR_TICKET_NUMBER_TOO_LARGE) ||
                (exception.Message == SharedLogicLayer.ERROR_FROM_GREATER_THAN_TO))
            {
                statusCode = STATUS_CODE_BAD_REQUEST;
            }
            APIGatewayProxyResponse response = new APIGatewayProxyResponse { StatusCode = statusCode };
            if (exception.InnerException != null)
            {
                response.Body = $"{{ error = \"{exception.InnerException.Message}\" }}";
            }
            return response;
        }

        /**
         * Test the APIHelper class.
         */    
        internal static void Test() {
            Debug.Tested();

            TestDateMethods();
            TestGetAuthorization();
        }

        /**
         * Test the APIHelper date conversion methods.
         */    
        internal static void TestDateMethods() {
            Debug.Tested();

            // IsValidAPIDateString
            Debug.Assert(!IsValidAPIDateString(null));
            Debug.Assert(!IsValidAPIDateString(""));
            Debug.Assert(!IsValidAPIDateString("x"));
            Debug.Assert(IsValidAPIDateString("2000-01-01"));

            // IsValidAPIDateTimeString
            Debug.Assert(!IsValidAPIDateTimeString(null));
            Debug.Assert(!IsValidAPIDateTimeString(""));
            Debug.Assert(!IsValidAPIDateTimeString("x"));
            Debug.Assert(!IsValidAPIDateTimeString("2000-01-01"));
            Debug.Assert(IsValidAPIDateTimeString("2000-01-01T00:00:00Z"));

            // DateFromAPIDateString / APIDateStringFromDate
            Debug.Assert(DateFromAPIDateString(null) == null);
            Debug.Assert(APIDateStringFromDate(null) == null);
            Debug.Assert(APIDateStringFromDate(DateFromAPIDateString("2000-01-01")) == "2000-01-01");
            Debug.Assert(APIDateStringFromDate(DateFromAPIDateString("2000-12-31")) == "2000-12-31");

            // DateTimeFromAPIDateTimeString / APIDateTimeStringFromDateTime
            Debug.Assert(DateTimeFromAPIDateTimeString(null) == null);
            Debug.Assert(APIDateTimeStringFromDateTime(null) == null);
            Debug.Assert(APIDateTimeStringFromDateTime(DateTimeFromAPIDateTimeString("2000-01-01T00:00:00Z")) == "2000-01-01T00:00:00Z");
            Debug.Assert(APIDateTimeStringFromDateTime(DateTimeFromAPIDateTimeString("2020-12-31T23:13:53Z")) == "2020-12-31T23:13:53Z");
        }

        /**
         * Test the APIHelper date conversion methods.
         */    
        internal static void TestGetAuthorization() {
            Debug.Tested();
            
            //??++ Debug.Assert(!GetAuthorization(null, out string userId));
            // Debug.Assert(userId == Helper.INVALID_ID);
            // Dictionary<string, string> headers = new Dictionary<string, string>();
            // Debug.Assert(!GetAuthorization(headers, out userId));
            // Debug.Assert(userId == Helper.INVALID_ID);
            // headers["Authorization"] = "";
            // Debug.Assert(!GetAuthorization(headers, out userId));
            // Debug.Assert(userId == Helper.INVALID_ID);
            // headers["Authorization"] = "x";
            // Debug.Assert(!GetAuthorization(headers, out userId));
            // Debug.Assert(userId == Helper.INVALID_ID);
            // headers["Authorization"] = "x x";
            // Debug.Assert(!GetAuthorization(headers, out userId));
            // Debug.Assert(userId == Helper.INVALID_ID);
            // headers["Authorization"] = "bearer x";
            // Debug.Assert(!GetAuthorization(headers, out userId));
            // Debug.Assert(userId == Helper.INVALID_ID);
        }

    }   // APIHelper

}   // BDDReferenceService
