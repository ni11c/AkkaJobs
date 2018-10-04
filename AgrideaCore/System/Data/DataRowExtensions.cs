namespace System.Data
{
    public static class DataRowExtensions
    {
        /// <summary>
        /// Easily get a strongly-typed value from a datarow, that is more robust than DataRow.Field.
        /// </summary>
        /// <exception cref="InvalidCastException">if getting a value of type T was not possible, throw InvalidCastException.</exception>
        public static T GetValue<T>(this DataRow x, string columnName)
        {
            var type = x.Table.Columns[columnName].DataType;
            var typeOfT = typeof(T);
            if (type == typeOfT)
                return x.Field<T>(columnName);

            IConvertible convertible = x[columnName] as IConvertible;
            object value = x[columnName];
            if (convertible != null)
                try
                {
                    return (T)Convert.ChangeType(value, typeOfT);
                }
                catch (InvalidCastException) { }

            if (typeOfT.IsNullableType())
            {
                if (value is DBNull || (value is string && string.IsNullOrEmpty((string)value)))
                    return default(T);

                var nonNullable = typeOfT.GetNonNullableType();
                try
                {
                    return (T)Convert.ChangeType(value, nonNullable);
                }
                catch (InvalidCastException) { }
            }

            throw new InvalidCastException(string.Format("Cannot convert datarow type {0} to {1}", type.Name, typeOfT.Name));
        }
    }
}
