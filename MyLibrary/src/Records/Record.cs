using System;

namespace MyLibrary.Records
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class RecordAttribute : Attribute
    {
        public Type TupleType { get; }

        public RecordAttribute(Type tupleType)
        {
            this.TupleType = tupleType;
        }
    }
}