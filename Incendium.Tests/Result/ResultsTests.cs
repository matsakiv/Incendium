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
                var result = new Result<TestObject>((Error)null!);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                Result<TestObject> result = (Error)null!;
            });
        }

        [Fact]
        public void Test_Result_DeconstructWhenSuccess()
        {
            // act
            var (result, error) = GetResult(withError: false, intField: 12, stringField: "Test");

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
            var (result, error) = GetResult(withError: true, errorCode: 100, errorMessage: "Error");

            // asserts
            Assert.Null(result);
            Assert.NotNull(error);
            Assert.Equal(100, error.Code);
            Assert.Equal("Error", error.Message);
        }

        [Fact]
        public void Test_Result_TapIfSuccess()
        {
            var tapped = false;

            // act
            GetResult(withError: false, intField: 10, stringField: "test")
                .Tap(obj => tapped = true);

            // asserts
            Assert.True(tapped);
        }

        [Fact]
        public void Test_Result_NoTapIfFailure()
        {
            var tapped = false;

            // act
            GetResult(withError: true, errorCode: 1)
                .Tap(obj => tapped = true);

            // asserts
            Assert.False(tapped);
        }

        [Fact]
        public void Test_Result_MapIfSuccess() {
            // act
            var (str, error) = GetResult(withError: false, intField: 10, stringField: "test")
                .Map(obj => $"{obj.StringField!} {obj.IntField}");

            // asserts
            Assert.Equal("test 10", str);
            Assert.Null(error);
        }

        [Fact]
        public void Test_Result_NoMapIfFailure() {
            // act
            var (str, error) = GetResult(withError: true, errorCode: 1)
                .Map(obj => $"{obj.StringField!} {obj.IntField}");

            // asserts
            Assert.NotNull(error);
            Assert.Equal(1, error.Code);
            Assert.Null(str);
        }

        [Fact]
        public void Test_Result_BindIfSuccess() {
            // act
            var (str, error) = GetResult(withError: false, intField: 10, stringField: "test")
                .Bind(obj => Result<string>.Success($"{obj.StringField!} {obj.IntField}"));

            // asserts
            Assert.Equal("test 10", str);
            Assert.Null(error);
        }


        [Fact]
        public void Test_Result_NoBindIfFailure() {
            // act
            var (str, error) = GetResult(withError: true, errorCode: 1)
                .Bind(obj => Result<string>.Success($"{obj.StringField!} {obj.IntField}"));

            // asserts
            Assert.NotNull(error);
            Assert.Equal(1, error.Code);
            Assert.Null(str);
        }

        [Fact]
        public void Test_Result_MatchIfSuccess() {
            // act
            var result = GetResult(withError: false, intField: 10, stringField: "test")
                .Match(obj => true, error => false);

            // asserts
            Assert.True(result);
        }

        [Fact]
        public void Test_Result_MatchIfFailure() {
            // act
            var result = GetResult(withError: true, errorCode: 1)
                .Match(obj => false, error => true);

            // asserts
            Assert.True(result);
        }

        [Fact]
        public void Test_Result_UseChain()
        {
            string logMessage = "";

            // act
            GetResult(withError: false, intField: 10, stringField: "test")
                .Tap(obj => logMessage = obj.StringField!)
                .Map(obj => $"{obj.StringField!} {obj.IntField}")
                .Bind<bool>(s =>
                {
                    if (s == "test 10")
                        return true;
                    else
                        return new Error();
                })
                .Match(
                    success => Assert.True(success),
                    error => Assert.Fail());
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
                var result = new NullableResult<TestObject>((Error)null!);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                NullableResult<TestObject> result = (Error)null!;
            });
        }

        [Fact]
        public void Test_NullableResult_DeconstructWhenSuccess()
        {
            // act
            var (result, error) = GetNullableResult(
                withError: false,
                returnNull: false,
                intField: 12,
                stringField: "Test");


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
            var (result, error) = GetNullableResult(
                withError: true,
                returnNull: false,
                errorCode: 100,
                errorMessage: "Error");


            // asserts
            Assert.Null(result);
            Assert.NotNull(error);
            Assert.Equal(100, error.Code);
            Assert.Equal("Error", error.Message);
        }

        [Fact]
        public void Test_NullableResult_TapIfSuccess()
        {
            var tapped = false;

            // act
            GetNullableResult(withError: false, returnNull: false)
                .Tap(obj => tapped = true);

            // asserts
            Assert.True(tapped);
        }

        [Fact]
        public void Test_NullableResult_NoTapIfFailure()
        {
            var tapped = false;

            // act
            GetNullableResult(withError: true, returnNull: false)
                .Tap(obj => tapped = true);

            // asserts
            Assert.False(tapped);
        }

        [Fact]
        public void Test_NullableResult_TapIfNull()
        {
            var tapped = false;

            // act
            GetNullableResult(withError: false, returnNull: true)
                .Tap(obj => tapped = true);

            // asserts
            Assert.True(tapped);
        }

        [Fact]
        public void Test_NullableResult_MapIfSuccess()
        {
            // act
            var (str, error) = GetNullableResult(
                    withError: false,
                    returnNull: false,
                    intField: 12,
                    stringField: "Test")
                .Map(obj => $"{obj!.StringField!} {obj.IntField}");

            // asserts
            Assert.Equal("Test 12", str);
            Assert.Null(error);
        }

        [Fact]
        public void Test_NullableResult_MapPreservesErrorState()
        {
            // act
            var (str, error) = GetNullableResult(
                    withError: true,
                    returnNull: false,
                    errorCode: 100,
                    errorMessage: "Error")
                .Map(obj => $"{obj!.StringField!} {obj.IntField}");

            // asserts
            Assert.Null(str);
            Assert.NotNull(error);
            Assert.Equal(100, error.Code);
        }

        [Fact]
        public void Test_NullableResult_MapIfNull()
        {
            // act
            var (str, error) = GetNullableResult(withError: false, returnNull: true)
                .Map(obj => obj == null ? "null" : $"{obj.StringField!} {obj.IntField}");

            // asserts
            Assert.Equal("null", str);
            Assert.Null(error);
        }

        [Fact]
        public void Test_NullableResult_BindIfSuccess()
        {
            // act
            var (str, error) = GetNullableResult(
                    withError: false,
                    returnNull: false,
                    intField: 12,
                    stringField: "Test")
                .Bind(obj => NullableResult<string>.Success($"{obj!.StringField!} {obj.IntField}"));

            // asserts
            Assert.Equal("Test 12", str);
            Assert.Null(error);
        }

        [Fact]
        public void Test_NullableResult_BindPreservesErrorState()
        {
            // act
            var (str, error) = GetNullableResult(
                    withError: true,
                    returnNull: false,
                    errorCode: 100,
                    errorMessage: "Error")
                .Bind(obj => NullableResult<string>.Success($"{obj!.StringField!} {obj.IntField}"));

            // asserts
            Assert.Null(str);
            Assert.NotNull(error);
            Assert.Equal(100, error.Code);
        }

        [Fact]
        public void Test_NullableResult_BindIfNull()
        {
            // act
            var (str, error) = GetNullableResult(withError: false, returnNull: true)
                .Bind(obj => obj == null
                    ? NullableResult<string>.Success("null")
                    : NullableResult<string>.Success($"{obj.StringField!} {obj.IntField}"));

            // asserts
            Assert.Equal("null", str);
            Assert.Null(error);
        }

        [Fact]
        public void Test_NullableResult_MatchIfSuccess()
        {
            // act
            var result = GetNullableResult(withError: false, returnNull: false)
                .Match(
                    obj => true,
                    error => false);

            // asserts
            Assert.True(result);
        }

        [Fact]
        public void Test_NullableResult_MatchIfFailure()
        {
            // act
            var result = GetNullableResult(withError: true, returnNull: false)
                .Match(
                    obj => false,
                    error => true);

            // asserts
            Assert.True(result);
        }

        [Fact]
        public void Test_NullableResult_MatchIfNull()
        {
            // act
            var result = GetNullableResult(withError: false, returnNull: true)
                .Match(
                    obj => obj == null,
                    error => false);

            // asserts
            Assert.True(result);
        }

        [Fact]
        public void Test_NullableResult_UseChain()
        {
            string logMessage = "";

            // act
            GetNullableResult(
                    withError: false,
                    returnNull: false,
                    intField: 12,
                    stringField: "Test")
                .Tap(obj => logMessage = obj?.StringField ?? "null")
                .Map(obj => obj == null ? "null" : $"{obj.StringField!} {obj.IntField}")

                .Bind<bool>(s =>
                {
                    if (s == "Test 12")
                        return true;
                    else
                        return new Error();
                })
                .Match(
                    success => Assert.True(success),
                    error => Assert.Fail());
        }

        [Fact]
        public void Test_Result_ThrowIfError_WhenSuccess()
        {
            // arrange
            var result = GetResult(withError: false, intField: 12, stringField: "Test");

            // act
            var value = result.ThrowIfError();

            // assert
            Assert.Equal(12, value.IntField);
            Assert.Equal("Test", value.StringField);
        }

        [Fact]
        public void Test_Result_ThrowIfError_WhenError()
        {
            // arrange
            var result = GetResult(withError: true, errorCode: 100, errorMessage: "Test error");

            // assert
            var exception = Assert.Throws<InvalidOperationException>(() => result.ThrowIfError());
            Assert.Contains("Test error", exception.Message);
        }

        [Fact]
        public void Test_NullableResult_ThrowIfError_WhenSuccess()
        {
            // arrange
            var result = GetNullableResult(withError: false, returnNull: false, intField: 12, stringField: "Test");

            // act
            var value = result.ThrowIfError();

            // assert
            Assert.NotNull(value);
            Assert.Equal(12, value.IntField);
            Assert.Equal("Test", value.StringField);
        }

        [Fact]
        public void Test_NullableResult_ThrowIfError_WhenNull()
        {
            // arrange
            var result = GetNullableResult(withError: false, returnNull: true);

            // act
            var value = result.ThrowIfError();

            // assert
            Assert.Null(value);
        }

        [Fact]
        public void Test_NullableResult_ThrowIfError_WhenError()
        {
            // arrange
            var result = GetNullableResult(withError: true, returnNull: false, errorCode: 100, errorMessage: "Test error");

            // assert
            var exception = Assert.Throws<InvalidOperationException>(() => result.ThrowIfError());
            Assert.Contains("Test error", exception.Message);
        }

        private static Result<TestObject> GetResult(
            bool withError,
            int intField = 0,
            string? stringField = null,
            int errorCode = 0,
            string? errorMessage = null)
        {
            if (withError)
                return new Error(errorCode, errorMessage!);

            return new TestObject(intField, stringField);
        }

        private static NullableResult<TestObject> GetNullableResult(
            bool withError,
            bool returnNull,
            int intField = 0,
            string? stringField = null,
            int errorCode = 0,
            string? errorMessage = null)
        {
            if (withError)
                return new Error(errorCode, errorMessage!);

            if (returnNull)
                return (TestObject?)null;

            return new TestObject(intField, stringField);
        }
    }
}

