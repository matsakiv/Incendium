using Incendium.Extensions;

namespace Incendium.Result
{
    public class ResultsTests
    {
        private record TestObject(int IntField, string? StringField);

        [Fact]
        public void Test_Result_FailCreateFromNullSuccessValue()
        {
            // asserts
            Assert.Throws<ArgumentNullException>(() =>
            {
                var result = new Result<TestObject>((TestObject)null!);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                Result<TestObject> result = (TestObject)null!;
            });
        }

        [Fact]
        public void Test_Result_FailCreateFromNullErrorValue()
        {
            // asserts
            Assert.Throws<ArgumentNullException>(() =>
            {
                var result = new Result<TestObject>((Error?)null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                Result<TestObject> result = (Error?)null;
            });
        }

        [Fact]
        public void Test_Result_DeconstructWhenSuccess()
        {
            // act
            var (result, error) = GetResult(withError: false);

            // asserts
            Assert.Equal(12, result.IntField);
            Assert.Equal("Test", result.StringField);
            Assert.NotNull(result);
            Assert.Null(error);
        }

        [Fact]
        public void Test_Result_DeconstructWhenError()
        {
            // act
            var (result, error) = GetResult(withError: true);

            // asserts
            Assert.Null(result);
            Assert.NotNull(error);
            Assert.Equal(100, error.Value.Code);
            Assert.Equal("Error", error.Value.Message);
            Assert.Equal(100, error.Code());
            Assert.Equal("Error", error.Message());
        }

        [Fact]
        public void Test_NullableResult_CreateFromNullSuccessValue()
        {
            // act
            var result1 = new NullableResult<TestObject>((TestObject)null!);
            NullableResult<TestObject> result2 = (TestObject)null!;

            // asserts
            Assert.Null(result1.Value);
            Assert.Null(result1.Error);
            Assert.Null(result2.Value);
            Assert.Null(result2.Error);
        }

        [Fact]
        public void Test_NullableResult_FailCreateFromNullErrorValue()
        {
            // asserts
            Assert.Throws<ArgumentNullException>(() =>
            {
                var result = new NullableResult<TestObject>((Error?)null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                NullableResult<TestObject> result = (Error?)null;
            });
        }

        [Fact]
        public void Test_NullableResult_DeconstructWhenSuccess()
        {
            // act
            var (result, error) = GetNullableResult(withError: false, returnNull: false);

            // asserts
            Assert.Equal(12, result!.IntField);
            Assert.Equal("Test", result.StringField);
            Assert.NotNull(result);
            Assert.Null(error);
        }

        [Fact]
        public void Test_NullableResult_DeconstructWhenError()
        {
            // act
            var (result, error) = GetNullableResult(withError: true, returnNull: false);

            // asserts
            Assert.Null(result);
            Assert.NotNull(error);
            Assert.Equal(100, error.Value.Code);
            Assert.Equal("Error", error.Value.Message);
            Assert.Equal(100, error.Code());
            Assert.Equal("Error", error.Message());
        }

        private static Result<TestObject> GetResult(bool withError)
        {
            if (withError)
                return new Error(code: 100, message: "Error");

            return new TestObject(12, "Test");
        }

        private static NullableResult<TestObject> GetNullableResult(bool withError, bool returnNull)
        {
            if (withError)
                return new Error(code: 100, message: "Error");

            if (returnNull)
                return (TestObject?)null;

            return new TestObject(12, "Test");
        }
    }
}
