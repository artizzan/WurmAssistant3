using System;
using System.Text.RegularExpressions;

namespace AldursLab.WurmApi.PersistentObjects
{
    static class PersistentObjectValidator
    {
        const string ValidIdPattern = @"^[A-Za-z0-9-]+$";
        const int IdMaxLength = 100;

        internal static void ValidateObjectId(string objectId)
        {
            if (objectId == null)
            {
                throw new ArgumentException("objectId cannot be null");
            }
            var valid = Regex.IsMatch(objectId, ValidIdPattern, RegexOptions.Compiled);
            if (!valid)
            {
                throw new ArgumentException(string.Format("objectId {0} is not valid, valid Ids must match Regex pattern {1}", objectId, ValidIdPattern));
            }
            if (objectId.Length > IdMaxLength)
            {
                throw new ArgumentException(string.Format("objectId {0} is too long, maximum length is {1}", objectId, IdMaxLength));
            }
        }

        internal static void ValidateCollectionId(string collectionId)
        {
            if (collectionId == null)
            {
                throw new ArgumentException("collectionId cannot be null");
            }
            var valid = Regex.IsMatch(collectionId, ValidIdPattern, RegexOptions.Compiled);
            if (!valid)
            {
                throw new ArgumentException(string.Format("collectionId {0} is not valid, valid Ids must match Regex pattern {1}", collectionId, ValidIdPattern));
            }
            if (collectionId.Length > IdMaxLength)
            {
                throw new ArgumentException(string.Format("collectionId {0} is too long, maximum length is {1}", collectionId, IdMaxLength));
            }
        }
    }
}