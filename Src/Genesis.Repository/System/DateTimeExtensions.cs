namespace System
{
    /// <summary>
    /// Adds extension methods to <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Defines the Unix epoch, which is January 1st 1970 at midnight.
        /// </summary>
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts from <c>DateTime</c> to Unix time.
        /// </summary>
        /// <param name="this">
        /// The <c>DateTime</c> to convert.
        /// </param>
        /// <returns>
        /// The number of seconds elapsed since <see cref="UnixEpoch"/>.
        /// </returns>
        public static long ToUnixTime(this DateTime @this) =>
            (long)@this.Subtract(UnixEpoch).TotalSeconds;

        /// <summary>
        /// Converts from Unix time to a <c>DateTime</c>.
        /// </summary>
        /// <param name="unixTime">
        /// The Unix time.
        /// </param>
        /// <returns>
        /// The corresponding <c>DateTime</c>.
        /// </returns>
        public static DateTime ToDateTime(this long unixTime) =>
            UnixEpoch.AddSeconds(unixTime);

        /// <summary>
        /// Converts from <c>DateTime?</c> to Unix time.
        /// </summary>
        /// <param name="this">
        /// The <c>DateTime</c> to convert.
        /// </param>
        /// <returns>
        /// The number of seconds elapsed since <see cref="UnixEpoch"/>, or <see langword="null"/>.
        /// </returns>
        public static long? ToUnixTime(this DateTime? @this) =>
            @this.HasValue ? (long?)@this.Value.ToUnixTime() : null;

        /// <summary>
        /// Converts from a nullable Unix time to a <c>DateTime?</c>.
        /// </summary>
        /// <param name="unixTime">
        /// The Unix time.
        /// </param>
        /// <returns>
        /// The corresponding <c>DateTime</c>, or <see langword="null"/>.
        /// </returns>
        public static DateTime? ToDateTime(this long? unixTime) =>
            unixTime.HasValue ? (DateTime?)unixTime.Value.ToDateTime() : null;
    }
}