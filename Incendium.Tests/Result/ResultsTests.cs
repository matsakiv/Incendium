using Incendium.Extensions;

namespace Incendium.Result
{
    public class ResultsTests
    {
        private record TestObject(int IntField, string? StringField);

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
        public void Test_NullableResult_DeconstructWhenSuccess()
        {
            // act
            var (result, error) = GetNullableResult(withError: false);

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
            var (result, error) = GetNullableResult(withError: true);

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

        private static NullableResult<TestObject> GetNullableResult(bool withError)
        {
            if (withError)
                return new Error(code: 100, message: "Error");

            return new TestObject(12, "Test");
        }
    }
}
