using System.Collections.Generic;
using System.Linq;
using AldursLab.WurmApi.PersistentObjects;
using AldursLab.WurmApi.Tests.Tests.PersistentCollectionsLibraryTests.Data;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Tests.PersistentCollectionsLibraryTests.Tests
{
    [TestFixture]
    class DeserializationErrorHandlingTests : TestsBase
    {
        const string ObjectId = "id";
        const string SomeValue = "SomeValue";

        /// <summary>
        /// Corrupted data store file should trigger strategy handler
        /// and subsystem should correctly react to "ignore" choice of the handler
        /// </summary>
        [Test]
        public void DeserializationErrors_CorruptedData_HandledInStrategy()
        {
            CreateObject();
            
            string newContent = BreakPersistedData();

            string rawData = null;
            IEnumerable<DeserializationErrorDetails> details = null;

            var lib = CreateLibrary(new CustomDeserializationErrorHandler(context =>
            {
                rawData = context.RawSerializedData;
                details = context.DeserializationErrorDetails;
                context.Decision = Decision.IgnoreErrorsAndReturnDefaultsForMissingData;
            }));
            var obj = lib.DefaultCollection.GetObject<DtoWithDefaults>(ObjectId);

            Expect(obj.Entity.Data, EqualTo(TestValues.Default));
            Expect(rawData, EqualTo(newContent));
            var singleErrorDetails = details.FirstOrDefault();
            Expect(singleErrorDetails, TypeOf<DeserializationErrorDetails>());
            Expect(singleErrorDetails.DeserializationErrorKind, EqualTo(DeserializationErrorKind.DeserializationImpossible));
        }

        /// <summary>
        /// Corrupted data store file should trigger strategy handler
        /// After raw data is fixed, subsystem should correctly react to "retry" choice of the handler,
        /// by loading the correct values of the fixed object.
        /// </summary>
        [Test]
        public void DeserializationErrors_CorruptedData_FixAndRetry()
        {
            CreateObject();

            BreakPersistedData();

            string rawData;
            IEnumerable<DeserializationErrorDetails> details = null;
            var lib = CreateLibrary(new CustomDeserializationErrorHandler(context =>
            {
                rawData = context.RawSerializedData;
                details = context.DeserializationErrorDetails;
                context.OverwriteSerializedData("{ " + rawData + " }");
                context.Decision = Decision.RetryDeserialization;
            }));

            var obj = lib.DefaultCollection.GetObject<DtoWithDefaults>(ObjectId);

            var singleErrorDetails = details.FirstOrDefault();
            Expect(singleErrorDetails, TypeOf<DeserializationErrorDetails>());
            Expect(singleErrorDetails.DeserializationErrorKind, EqualTo(DeserializationErrorKind.DeserializationImpossible));

            // After retrying, value should be as saved previously
            Expect(obj.Entity.Data, EqualTo(SomeValue));
        }

        /// <summary>
        /// Corrupted data store file should trigger strategy handler
        /// and subsystem should correctly react to "rethrow" choice of the handler,
        /// by rethrowing the exception at the source of invocation.
        /// </summary>
        [Test]
        public void DeserializationErrors_CorruptedData_Rethrow()
        {
            CreateObject();
            BreakPersistedData();
            var lib = CreateLibrary(new CustomDeserializationErrorHandler(context =>
            {
            }));
            Assert.Throws<DeserializationErrorsException<DtoWithDefaults>>(() => lib.DefaultCollection.GetObject<DtoWithDefaults>(ObjectId));
        }

        /// <summary>
        /// If data store file has incompatible value in some member, 
        /// strategy handler should be triggered, with appropriate context.
        /// Subsystem should correctly react to "ignore" choice of the handler,
        /// restoring default for incompatible member only.
        /// </summary>
        [Test]
        public void DeserializationErrors_IncompatibleMember_Handles()
        {
            CreateObject();

            BreakNumericFieldInData();

            string rawData = null;
            IEnumerable<DeserializationErrorDetails> details = null;
            var lib = CreateLibrary(new CustomDeserializationErrorHandler(context =>
            {
                rawData = context.RawSerializedData;
                details = context.DeserializationErrorDetails;
                context.Decision = Decision.IgnoreErrorsAndReturnDefaultsForMissingData;
            }));

            var obj = lib.DefaultCollection.GetObject<DtoWithDefaults>(ObjectId);

            var singleErrorDetails = details.FirstOrDefault();
            Expect(singleErrorDetails, TypeOf<DeserializationErrorDetails>());
            Expect(singleErrorDetails.DeserializationErrorKind, EqualTo(DeserializationErrorKind.DeserializationOfSomeMembersFailed));

            Expect(obj.Entity.Data, EqualTo(SomeValue));
            Expect(obj.Entity.NumericValue, EqualTo(default(int)));
        }

        void CreateObject()
        {
            var lib = CreateLibrary();
            var obj = lib.DefaultCollection.GetObject<DtoWithDefaults>(ObjectId);
            obj.Entity.Data = SomeValue;
            obj.Entity.NumericValue = 123;
            lib.SaveAll();
        }

        string BreakPersistedData()
        {
            var lib = CreateLibrary();
            var fileContents = lib.PersistenceStrategy.TryLoad(ObjectId, PersistentCollectionsLibrary.DefaultCollectionId);
            // creating malformed json
            var newContent = fileContents.Replace("{", "").Replace("}", "");
            lib.PersistenceStrategy.Save(ObjectId, PersistentCollectionsLibrary.DefaultCollectionId, newContent);
            return newContent;
        }

        string BreakNumericFieldInData()
        {
            var lib = CreateLibrary();
            var fileContents = lib.PersistenceStrategy.TryLoad(ObjectId, PersistentCollectionsLibrary.DefaultCollectionId);
            // creating malformed json
            var newContent = fileContents.Replace("\"NumericValue\": 123,", "\"NumericValue\": \"dfg34tdfg\",");
            lib.PersistenceStrategy.Save(ObjectId, PersistentCollectionsLibrary.DefaultCollectionId, newContent);
            return newContent;
        }
    }
}