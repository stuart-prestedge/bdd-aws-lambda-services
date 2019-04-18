using System;
using System.Collections.Generic;
using BDDReferenceService.Contracts;
using BDDReferenceService.Logic;
using BDDReferenceService.Model;

namespace BDDReferenceService {

    /**
     * Logic layer with helper methods.
     */
    public static class SharedLogicLayer {

        /**
         * Game service errors.
         */
        public const string ERROR_INVALID_HTTP_METHOD = "INVALID_HTTP_METHOD";
        public const string ERROR_INVALID_INPUT_PARAMETER = "INVALID_INPUT_PARAMETER";
        public const string ERROR_NOT_LOGGED_IN = "NOT_LOGGED_IN";
        public const string ERROR_NOT_AN_ADMIN = "NOT_AN_ADMIN";
        public const string ERROR_NOT_FOUND = "NOT_AN_ADMIN";
        public const string ERROR_NO_PERMISSION = "NO_PERMISSION";
        public const string ERROR_FROM_GREATER_THAN_TO = "FROM_GREATER_THAN_TO";
        public const string ERROR_SYSTEM_LOCKED = "SYSTEM_LOCKED";
        public const string ERROR_SYSTEM_ALREADY_LOCKED = "SYSTEM_ALREADY_LOCKED";
        public const string ERROR_INVALID_LINK = "INVALID_LINK";
        public const string ERROR_INVALID_LINK_USER = "INVALID_LINK_USER";
        public const string ERROR_UNRECOGNIZED_EMAIL_ADDRESS = "UNRECOGNIZED_EMAIL_ADDRESS";

    }   // SharedLogicLayer

}   // BDDReferenceService
