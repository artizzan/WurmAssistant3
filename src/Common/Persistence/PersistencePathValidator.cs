using System;
using System.Text.RegularExpressions;

namespace AldursLab.Persistence
{
    public static class PersistencePathValidator
    {
        const string RegexPattern = @"^[A-Za-z\d_-][ A-Za-z\d_-]*[A-Za-z\d_-]$";
        const string ShortRegexPattern = @"^[A-Za-z\d_-]$";
        const string ValidationErrorInfo = "String should: " +
                                           "not be empty; " +
                                           "not start or end with whitespace; " +
                                           "be no longer than 50 characters; " +
                                           "contain only numbers, dashes, underscores and english alphabet letters";

        public static void ThrowIfPathInvalid(string pathOrObjectId)
        {
            if (string.IsNullOrWhiteSpace(pathOrObjectId)
                || pathOrObjectId.Length > 50
                || (pathOrObjectId.Length > 1 && !Regex.IsMatch(pathOrObjectId, RegexPattern))
                || (pathOrObjectId.Length == 1 && !Regex.IsMatch(pathOrObjectId, ShortRegexPattern)))
            {
                throw new InvalidOperationException("Object id or Set id is invalid. " + ValidationErrorInfo);
            }
        }

        
    }
}