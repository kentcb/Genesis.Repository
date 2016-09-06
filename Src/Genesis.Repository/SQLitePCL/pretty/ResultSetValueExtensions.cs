namespace SQLitePCL.pretty
{
    using Genesis.Ensure;

    /// <summary>
    /// Defines extension methods against the <see cref="IResultSetValue"/> interface.
    /// </summary>
    public static class IResultSetValueExtensions
    {
        /// <summary>
        /// Converts a value to a nullable <c>string</c>.
        /// </summary>
        /// <param name="this">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The <c>string</c>, which may be <see langword="null"/>.
        /// </returns>
        public static string ToNullableString(this IResultSetValue @this)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            return @this.SQLiteType == SQLiteType.Null ? null : @this.ToString();
        }

        /// <summary>
        /// Converts a value to a nullable <c>bool</c>.
        /// </summary>
        /// <param name="this">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The <c>bool</c>, which may be <see langword="null"/>.
        /// </returns>
        public static bool? ToNullableBool(this IResultSetValue @this)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            return @this.SQLiteType == SQLiteType.Null ? (bool?)null : @this.ToBool();
        }

        /// <summary>
        /// Converts a value to a nullable <c>byte</c>.
        /// </summary>
        /// <param name="this">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The <c>byte</c>, which may be <see langword="null"/>.
        /// </returns>
        public static byte? ToNullableByte(this IResultSetValue @this)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            return @this.SQLiteType == SQLiteType.Null ? (byte?)null : @this.ToByte();
        }

        /// <summary>
        /// Converts a value to a nullable <c>decimal</c>.
        /// </summary>
        /// <param name="this">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The <c>decimal</c>, which may be <see langword="null"/>.
        /// </returns>
        public static decimal? ToNullableDecimal(this IResultSetValue @this)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            return @this.SQLiteType == SQLiteType.Null ? (decimal?)null : @this.ToDecimal();
        }

        /// <summary>
        /// Converts a value to a nullable <c>double</c>.
        /// </summary>
        /// <param name="this">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The <c>double</c>, which may be <see langword="null"/>.
        /// </returns>
        public static double? ToNullableDouble(this IResultSetValue @this)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            return @this.SQLiteType == SQLiteType.Null ? (double?)null : @this.ToDouble();
        }

        /// <summary>
        /// Converts a value to a nullable <c>float</c>.
        /// </summary>
        /// <param name="this">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The <c>float</c>, which may be <see langword="null"/>.
        /// </returns>
        public static float? ToNullableFloat(this IResultSetValue @this)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            return @this.SQLiteType == SQLiteType.Null ? (float?)null : @this.ToFloat();
        }

        /// <summary>
        /// Converts a value to a nullable <c>int</c>.
        /// </summary>
        /// <param name="this">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The <c>int</c>, which may be <see langword="null"/>.
        /// </returns>
        public static int? ToNullableInt(this IResultSetValue @this)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            return @this.SQLiteType == SQLiteType.Null ? (int?)null : @this.ToInt();
        }

        /// <summary>
        /// Converts a value to a nullable <c>long</c>.
        /// </summary>
        /// <param name="this">
        /// The value to convert.
        /// </param>
        /// <returns>
        /// The <c>long</c>, which may be <see langword="null"/>.
        /// </returns>
        public static long? ToNullableInt64(this IResultSetValue @this)
        {
            Ensure.ArgumentNotNull(@this, nameof(@this));
            return @this.SQLiteType == SQLiteType.Null ? (long?)null : @this.ToInt64();
        }
    }
}