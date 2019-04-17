using System;
namespace BDDReferenceService.Contracts
{
    public class SearchUsersResponse
    {
    
        /**
         * The users.
         */
        public SearchUserResponse[] users { get; set; }

    }   // SearchUsersResponse

    public class SearchUserResponse
    {
    
        /**
         * A mandatory string with the user's email address. It must be a valid email address. It must be unique (i.e. not already used in another account, either validated, not validated or new (when a user is in the process of changing their email address).
         */
        public string id { get; set; }

        /**
         * A mandatory string (must exist and must not be null or empty) with the user's given name.
         */
        public string givenName { get; set; }

        /**
         * A mandatory string with the user's family name.
         */
        public string familyName { get; set; }

        /**
         * The optional full name.
         */
        public string fullName { get; set; }

        /**
         * A mandatory string with the user's email address. It must be a valid email address. It must be unique (i.e. not already used in another account, either validated, not validated or new (when a user is in the process of changing their email address).
         */
        public string emailAddress { get; set; }

    }   // SearchUserResponse

}   // BDDReferenceService.Contracts
