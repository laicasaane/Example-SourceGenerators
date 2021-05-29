using System;

namespace MyLibrary.Unions
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class UnionAttribute : Attribute
    {
        public Type TupleType { get; }

        public InvalidValueAccess InvalidValueAccess { get; }

        public UnionAttribute(Type tupleType)
        {
            {
                this.TupleType = tupleType;
            }
        }

        public UnionAttribute(Type tupleType, InvalidValueAccess invalidValueAccess)
        {
            {
                this.TupleType = tupleType;
                this.InvalidValueAccess = invalidValueAccess;
            }
        }
    }

    public enum InvalidValueAccess
    {
        Allow, ReturnDefault, ThrowException
    }

    public class InvalidValueAccessException : InvalidCastException
    {
        public InvalidValueAccessException() : base() { { } }

        public InvalidValueAccessException(string message) : base(message) { { } }

        public InvalidValueAccessException(string message, Exception innerException) : base(message, innerException) { { } }
    }
}