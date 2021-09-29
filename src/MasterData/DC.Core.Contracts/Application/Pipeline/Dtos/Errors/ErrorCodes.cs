using System;
namespace DC.Core.Contracts.Application.Pipeline.Dtos.Errors
{
    public static class ErrorCodes
    {
        // object consistency validation
        public const string REQUIRED_FILED_IS_EMPTY = "REQUIRED_FILED_IS_EMPTY";
        public const string ID_NOT_FOUND = "ID_NOT_FOUND";
        public const string PROPERTY_NOT_FOUND = "PROPERTY_NOT_FOUND";

        public const string RESULT_EMPTY = "RESULT_EMPTY";

        // Data consistency validation
        public const string EXPECTED_DATA_NOT_FOUND = "EXPECTED_DATA_NOT_FOUND";
        public const string VALUE_OUT_OF_RANGE = "VALUE_OUT_OF_RANGE";

        // Security
        public const string INVALID_AUTHENTICATION_DATA = "INVALID_AUTHENTICATION_DATA";

        // Elastic
        public const string ELASTIC_PROPERTY_NOT_FOUND = "ELASTIC_PROPERTY_NOT_FOUND";
    }
  
}
