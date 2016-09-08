namespace Genesis.Repository.UnitTests.Disambiguate.System
{
    using global::System;
    using Xunit;

    public sealed class DateTimeExtensionsFixture
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData("2013-02-13 03:31:26", 1360726286L)]
        [InlineData("2004-12-31 14:38:59", 1104503939L)]
        [InlineData("1978-12-04 18:09:12", 281642952L)]
        [InlineData("1979-10-26 02:12:01", 309751921L)]
        public void to_unix_time_calculates_correctly(string dateTimeString, long? expectedResult)
        {
            var result = dateTimeString
                .ToDateTime()
                .ToUnixTime();

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(1360726286L, "2013-02-13 03:31:26")]
        [InlineData(1104503939L, "2004-12-31 14:38:59")]
        [InlineData(281642952L, "1978-12-04 18:09:12")]
        [InlineData(309751921L, "1979-10-26 02:12:01")]
        public void to_date_time_calculates_correctly(long? unixTime, string expectedDateTimeString)
        {
            var expectedDateTime = expectedDateTimeString.ToDateTime();
            var result = unixTime.ToDateTime();

            Assert.Equal(expectedDateTime, result);
        }
    }
}