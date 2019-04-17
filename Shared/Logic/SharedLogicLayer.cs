using System;
using System.Collections.Generic;
using BDDReferenceService.Contracts;
using BDDReferenceService.Logic;
using BDDReferenceService.Model;

namespace BDDReferenceService {

    /**
     * Logic layer with helper methods.
     */
    internal static class SharedLogicLayer {

        /**
         * Game service errors.
         */
        internal const string ERROR_INVALID_INPUT_PARAMETER = "INVALID_INPUT_PARAMETER";
        internal const string ERROR_NOT_LOGGED_IN = "NOT_LOGGED_IN";
        internal const string ERROR_NOT_AN_ADMIN = "NOT_AN_ADMIN";
        internal const string ERROR_NOT_FOUND = "NOT_AN_ADMIN";
        internal const string ERROR_NO_PERMISSION = "NO_PERMISSION";
        internal const string ERROR_FROM_GREATER_THAN_TO = "FROM_GREATER_THAN_TO";
        internal const string ERROR_SYSTEM_LOCKED = "SYSTEM_LOCKED";
        internal const string ERROR_SYSTEM_ALREADY_LOCKED = "SYSTEM_ALREADY_LOCKED";
        internal const string ERROR_INVALID_LINK = "INVALID_LINK";
        internal const string ERROR_INVALID_LINK_USER = "INVALID_LINK_USER";
        internal const string ERROR_UNRECOGNIZED_EMAIL_ADDRESS = "UNRECOGNIZED_EMAIL_ADDRESS";

    }   // SharedLogicLayer

}   // BDDReferenceService
