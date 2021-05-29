namespace MySourceGenerators
{
    public static class BuiltInTypeHelper
    {
        private const string _object  = "object";
        private const string _bool    = "bool";
        private const string _byte    = "byte";
        private const string _sbyte   = "sbyte";
        private const string _short   = "short";
        private const string _ushort  = "ushort";
        private const string _int     = "int";
        private const string _uint    = "uint";
        private const string _long    = "long";
        private const string _ulong   = "ulong";
        private const string _float   = "float";
        private const string _double  = "double";
        private const string _char    = "char";
        private const string _decimal = "decimal";
        private const string _nint    = "nint";
        private const string _nuint   = "nuint";

        public static string Cast(string value)
            => $"({value})";

        public static bool IsUlong(string value)
            => string.Equals(value, _ulong);

        public static bool IsFloat(string value)
            => string.Equals(value, _float);

        public static bool IsDouble(string value)
            => string.Equals(value, _double);

        public static string GetPrimitiveCast(string type)
        {
            var cast = string.Empty;

            switch (type)
            {
                case _bool   : cast = Cast(_bool); break;
                case _byte   : cast = Cast(_byte); break;
                case _sbyte  : cast = Cast(_sbyte); break;
                case _short  : cast = Cast(_short); break;
                case _ushort : cast = Cast(_ushort); break;
                case _int    : cast = Cast(_int); break;
                case _uint   : cast = Cast(_uint); break;
                case _long   : cast = Cast(_long); break;
                case _ulong  : cast = Cast(_ulong); break;
                case _float  : cast = Cast(_float); break;
                case _double : cast = Cast(_double); break;
                case _decimal: cast = Cast(_decimal); break;
                case _char   : cast = Cast(_char); break;
                case _nint   : cast = Cast(_nint); break;
                case _nuint  : cast = Cast(_nuint); break;
            }

            return cast;
        }

        public static void GetPrimitiveCast(string lhs, string rhs, out string lhsCast, out string rhsCast)
        {
            lhsCast = rhsCast = string.Empty;

            switch (lhs)
            {
                case _object:
                {
                    switch (rhs)
                    {
                        case _bool   : lhsCast = Cast(_bool); break;
                        case _byte   : lhsCast = Cast(_byte); break;
                        case _sbyte  : lhsCast = Cast(_sbyte); break;
                        case _short  : lhsCast = Cast(_short); break;
                        case _ushort : lhsCast = Cast(_ushort); break;
                        case _int    : lhsCast = Cast(_int); break;
                        case _uint   : lhsCast = Cast(_uint); break;
                        case _long   : lhsCast = Cast(_long); break;
                        case _ulong  : lhsCast = Cast(_ulong); break;
                        case _float  : lhsCast = Cast(_float); break;
                        case _double : lhsCast = Cast(_double); break;
                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _bool:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_bool); break;
                    }

                    break;
                }

                case _sbyte:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_sbyte); break;

                        case _byte   : lhsCast = Cast(_byte); break;
                        case _short  : lhsCast = Cast(_short); break;
                        case _ushort : lhsCast = Cast(_ushort); break;
                        case _int    : lhsCast = Cast(_int); break;
                        case _uint   : lhsCast = Cast(_uint); break;
                        case _long   : lhsCast = Cast(_long); break;
                        case _ulong  : lhsCast = Cast(_ulong); break;
                        case _float  : lhsCast = Cast(_float); break;
                        case _double : lhsCast = Cast(_double); break;
                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }
                    break;
                }

                case _byte:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_byte); break;
                        case _sbyte  : rhsCast = Cast(_byte); break;

                        case _short  : lhsCast = Cast(_short); break;
                        case _ushort : lhsCast = Cast(_ushort); break;
                        case _int    : lhsCast = Cast(_int); break;
                        case _uint   : lhsCast = Cast(_uint); break;
                        case _long   : lhsCast = Cast(_long); break;
                        case _ulong  : lhsCast = Cast(_ulong); break;
                        case _float  : lhsCast = Cast(_float); break;
                        case _double : lhsCast = Cast(_double); break;
                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _short:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_short); break;
                        case _byte   : rhsCast = Cast(_short); break;
                        case _sbyte  : rhsCast = Cast(_short); break;

                        case _ushort : lhsCast = Cast(_ushort); break;
                        case _int    : lhsCast = Cast(_int); break;
                        case _uint   : lhsCast = Cast(_uint); break;
                        case _long   : lhsCast = Cast(_long); break;
                        case _ulong  : lhsCast = Cast(_ulong); break;
                        case _float  : lhsCast = Cast(_float); break;
                        case _double : lhsCast = Cast(_double); break;
                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _ushort:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_ushort); break;
                        case _byte   : rhsCast = Cast(_ushort); break;
                        case _sbyte  : rhsCast = Cast(_ushort); break;
                        case _short  : rhsCast = Cast(_ushort); break;

                        case _int    : lhsCast = Cast(_int); break;
                        case _uint   : lhsCast = Cast(_uint); break;
                        case _long   : lhsCast = Cast(_long); break;
                        case _ulong  : lhsCast = Cast(_ulong); break;
                        case _float  : lhsCast = Cast(_float); break;
                        case _double : lhsCast = Cast(_double); break;
                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _int:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_int); break;
                        case _byte   : rhsCast = Cast(_int); break;
                        case _sbyte  : rhsCast = Cast(_int); break;
                        case _short  : rhsCast = Cast(_int); break;
                        case _ushort : rhsCast = Cast(_int); break;

                        case _uint   : lhsCast = Cast(_uint); break;
                        case _long   : lhsCast = Cast(_long); break;
                        case _ulong  : lhsCast = Cast(_ulong); break;
                        case _float  : lhsCast = Cast(_float); break;
                        case _double : lhsCast = Cast(_double); break;
                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _uint:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_uint); break;
                        case _byte   : rhsCast = Cast(_uint); break;
                        case _sbyte  : rhsCast = Cast(_uint); break;
                        case _short  : rhsCast = Cast(_uint); break;
                        case _ushort : rhsCast = Cast(_uint); break;
                        case _int    : rhsCast = Cast(_uint); break;

                        case _long   : lhsCast = Cast(_long); break;
                        case _ulong  : lhsCast = Cast(_ulong); break;
                        case _float  : lhsCast = Cast(_float); break;
                        case _double : lhsCast = Cast(_double); break;
                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _long:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_long); break;
                        case _byte   : rhsCast = Cast(_long); break;
                        case _sbyte  : rhsCast = Cast(_long); break;
                        case _short  : rhsCast = Cast(_long); break;
                        case _ushort : rhsCast = Cast(_long); break;
                        case _int    : rhsCast = Cast(_long); break;
                        case _uint   : rhsCast = Cast(_long); break;

                        case _ulong  : lhsCast = Cast(_ulong); break;
                        case _float  : lhsCast = Cast(_double); break;
                        case _double : lhsCast = Cast(_double); break;
                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _ulong:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_ulong); break;
                        case _byte   : rhsCast = Cast(_ulong); break;
                        case _sbyte  : rhsCast = Cast(_ulong); break;
                        case _short  : rhsCast = Cast(_ulong); break;
                        case _ushort : rhsCast = Cast(_ulong); break;
                        case _int    : rhsCast = Cast(_ulong); break;
                        case _uint   : rhsCast = Cast(_ulong); break;
                        case _long   : rhsCast = Cast(_ulong); break;

                        case _float  : lhsCast = Cast(_double); break;
                        case _double : lhsCast = Cast(_double); break;
                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _float:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_float); break;
                        case _byte   : rhsCast = Cast(_float); break;
                        case _sbyte  : rhsCast = Cast(_float); break;
                        case _short  : rhsCast = Cast(_float); break;
                        case _ushort : rhsCast = Cast(_float); break;
                        case _int    : rhsCast = Cast(_float); break;
                        case _uint   : rhsCast = Cast(_float); break;
                        case _long   : rhsCast = Cast(_double); break;
                        case _ulong  : rhsCast = Cast(_double); break;

                        case _double : lhsCast = Cast(_double); break;
                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _double:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_double); break;
                        case _byte   : rhsCast = Cast(_double); break;
                        case _sbyte  : rhsCast = Cast(_double); break;
                        case _short  : rhsCast = Cast(_double); break;
                        case _ushort : rhsCast = Cast(_double); break;
                        case _int    : rhsCast = Cast(_double); break;
                        case _uint   : rhsCast = Cast(_double); break;
                        case _long   : rhsCast = Cast(_double); break;
                        case _ulong  : rhsCast = Cast(_double); break;
                        case _float  : rhsCast = Cast(_double); break;

                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _decimal:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_decimal); break;
                        case _byte   : rhsCast = Cast(_decimal); break;
                        case _sbyte  : rhsCast = Cast(_decimal); break;
                        case _short  : rhsCast = Cast(_decimal); break;
                        case _ushort : rhsCast = Cast(_decimal); break;
                        case _int    : rhsCast = Cast(_decimal); break;
                        case _uint   : rhsCast = Cast(_decimal); break;
                        case _long   : rhsCast = Cast(_decimal); break;
                        case _ulong  : rhsCast = Cast(_decimal); break;
                        case _float  : rhsCast = Cast(_decimal); break;
                        case _double : rhsCast = Cast(_decimal); break;

                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _char:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_char); break;
                        case _byte   : rhsCast = Cast(_char); break;
                        case _sbyte  : rhsCast = Cast(_char); break;
                        case _short  : rhsCast = Cast(_char); break;
                        case _ushort : rhsCast = Cast(_char); break;
                        case _int    : rhsCast = Cast(_char); break;
                        case _uint   : rhsCast = Cast(_char); break;
                        case _long   : rhsCast = Cast(_char); break;
                        case _ulong  : rhsCast = Cast(_char); break;
                        case _float  : rhsCast = Cast(_char); break;
                        case _double : rhsCast = Cast(_char); break;
                        case _decimal: rhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _nint:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_nint); break;
                        case _byte   : rhsCast = Cast(_nint); break;
                        case _sbyte  : rhsCast = Cast(_nint); break;
                        case _short  : rhsCast = Cast(_nint); break;
                        case _ushort : rhsCast = Cast(_nint); break;
                        case _int    : rhsCast = Cast(_nint); break;
                        case _uint   : rhsCast = Cast(_nint); break;
                        case _long   : rhsCast = Cast(_nint); break;
                        case _ulong  : rhsCast = Cast(_nint); break;
                        case _float  : rhsCast = Cast(_nint); break;
                        case _double : rhsCast = Cast(_nint); break;
                        case _decimal: rhsCast = Cast(_nint); break;
                        case _char   : rhsCast = Cast(_nint); break;

                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _nuint:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_nuint); break;
                        case _byte   : rhsCast = Cast(_nuint); break;
                        case _sbyte  : rhsCast = Cast(_nuint); break;
                        case _short  : rhsCast = Cast(_nuint); break;
                        case _ushort : rhsCast = Cast(_nuint); break;
                        case _int    : rhsCast = Cast(_nuint); break;
                        case _uint   : rhsCast = Cast(_nuint); break;
                        case _long   : rhsCast = Cast(_nuint); break;
                        case _ulong  : rhsCast = Cast(_nuint); break;
                        case _float  : rhsCast = Cast(_nuint); break;
                        case _double : rhsCast = Cast(_nuint); break;
                        case _decimal: rhsCast = Cast(_nuint); break;
                        case _char   : rhsCast = Cast(_nuint); break;
                        case _nint   : rhsCast = Cast(_nuint); break;
                    }

                    break;
                }
            }
        }

        public static void GetPrimitiveCast(string lhs, string rhs, out string lhsCast, out string rhsCast, out string cast)
        {
            lhsCast = rhsCast = cast = string.Empty;

            switch (lhs)
            {
                case _object:
                {
                    switch (rhs)
                    {
                        case _bool   : lhsCast = Cast(_bool); break;
                        case _byte   : lhsCast = Cast(_byte); break;
                        case _sbyte  : lhsCast = Cast(_sbyte); break;
                        case _short  : lhsCast = Cast(_short); break;
                        case _ushort : lhsCast = Cast(_ushort); break;
                        case _int    : lhsCast = Cast(_int); break;
                        case _uint   : lhsCast = Cast(_uint); break;
                        case _long   : lhsCast = Cast(_long); break;
                        case _ulong  : lhsCast = Cast(_ulong); break;
                        case _float  : lhsCast = Cast(_float); break;
                        case _double : lhsCast = Cast(_double); break;
                        case _decimal: lhsCast = Cast(_decimal); break;
                        case _char   : lhsCast = Cast(_char); break;
                        case _nint   : lhsCast = Cast(_nint); break;
                        case _nuint  : lhsCast = Cast(_nuint); break;
                    }

                    break;
                }

                case _bool:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = Cast(_bool); break;
                    }

                    break;
                }

                case _sbyte:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_sbyte); break;
                        case _sbyte  : cast = Cast(_sbyte); break;
                        case _byte   : lhsCast = cast = Cast(_byte); break;
                        case _short  : lhsCast = cast = Cast(_short); break;
                        case _ushort : lhsCast = cast = Cast(_ushort); break;
                        case _int    : lhsCast = cast = Cast(_int); break;
                        case _uint   : lhsCast = cast = Cast(_uint); break;
                        case _long   : lhsCast = cast = Cast(_long); break;
                        case _ulong  : lhsCast = cast = Cast(_ulong); break;
                        case _float  : lhsCast = cast = Cast(_float); break;
                        case _double : lhsCast = cast = Cast(_double); break;
                        case _decimal: lhsCast = cast = Cast(_decimal); break;
                        case _char   : lhsCast = cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }
                    break;
                }

                case _byte:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_byte); break;
                        case _sbyte  : rhsCast = cast = Cast(_byte); break;
                        case _byte   : cast = Cast(_byte); break;
                        case _short  : lhsCast = cast = Cast(_short); break;
                        case _ushort : lhsCast = cast = Cast(_ushort); break;
                        case _int    : lhsCast = cast = Cast(_int); break;
                        case _uint   : lhsCast = cast = Cast(_uint); break;
                        case _long   : lhsCast = cast = Cast(_long); break;
                        case _ulong  : lhsCast = cast = Cast(_ulong); break;
                        case _float  : lhsCast = cast = Cast(_float); break;
                        case _double : lhsCast = cast = Cast(_double); break;
                        case _decimal: lhsCast = cast = Cast(_decimal); break;
                        case _char   : lhsCast = cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _short:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_short); break;
                        case _byte   : rhsCast = cast = Cast(_short); break;
                        case _sbyte  : rhsCast = cast = Cast(_short); break;
                        case _short  : cast = Cast(_short); break;
                        case _ushort : lhsCast = cast = Cast(_ushort); break;
                        case _int    : lhsCast = cast = Cast(_int); break;
                        case _uint   : lhsCast = cast = Cast(_uint); break;
                        case _long   : lhsCast = cast = Cast(_long); break;
                        case _ulong  : lhsCast = cast = Cast(_ulong); break;
                        case _float  : lhsCast = cast = Cast(_float); break;
                        case _double : lhsCast = cast = Cast(_double); break;
                        case _decimal: lhsCast = cast = Cast(_decimal); break;
                        case _char   : lhsCast = cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _ushort:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_ushort); break;
                        case _byte   : rhsCast = cast = Cast(_ushort); break;
                        case _sbyte  : rhsCast = cast = Cast(_ushort); break;
                        case _short  : rhsCast = cast = Cast(_ushort); break;
                        case _ushort : cast = Cast(_ushort); break;
                        case _int    : lhsCast = cast = Cast(_int); break;
                        case _uint   : lhsCast = cast = Cast(_uint); break;
                        case _long   : lhsCast = cast = Cast(_long); break;
                        case _ulong  : lhsCast = cast = Cast(_ulong); break;
                        case _float  : lhsCast = cast = Cast(_float); break;
                        case _double : lhsCast = cast = Cast(_double); break;
                        case _decimal: lhsCast = cast = Cast(_decimal); break;
                        case _char   : lhsCast = cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _int:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_int); break;
                        case _byte   : rhsCast = cast = Cast(_int); break;
                        case _sbyte  : rhsCast = cast = Cast(_int); break;
                        case _short  : rhsCast = cast = Cast(_int); break;
                        case _ushort : rhsCast = cast = Cast(_int); break;
                        case _int    : cast = Cast(_int); break;
                        case _uint   : lhsCast = cast = Cast(_uint); break;
                        case _long   : lhsCast = cast = Cast(_long); break;
                        case _ulong  : lhsCast = cast = Cast(_ulong); break;
                        case _float  : lhsCast = cast = Cast(_float); break;
                        case _double : lhsCast = cast = Cast(_double); break;
                        case _decimal: lhsCast = cast = Cast(_decimal); break;
                        case _char   : lhsCast = cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _uint:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_uint); break;
                        case _byte   : rhsCast = cast = Cast(_uint); break;
                        case _sbyte  : rhsCast = cast = Cast(_uint); break;
                        case _short  : rhsCast = cast = Cast(_uint); break;
                        case _ushort : rhsCast = cast = Cast(_uint); break;
                        case _int    : rhsCast = cast = Cast(_uint); break;
                        case _uint   : cast = Cast(_uint); break;
                        case _long   : lhsCast = cast = Cast(_long); break;
                        case _ulong  : lhsCast = cast = Cast(_ulong); break;
                        case _float  : lhsCast = cast = Cast(_float); break;
                        case _double : lhsCast = cast = Cast(_double); break;
                        case _decimal: lhsCast = cast = Cast(_decimal); break;
                        case _char   : lhsCast = cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _long:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_long); break;
                        case _byte   : rhsCast = cast = Cast(_long); break;
                        case _sbyte  : rhsCast = cast = Cast(_long); break;
                        case _short  : rhsCast = cast = Cast(_long); break;
                        case _ushort : rhsCast = cast = Cast(_long); break;
                        case _int    : rhsCast = cast = Cast(_long); break;
                        case _uint   : rhsCast = cast = Cast(_long); break;
                        case _long   : cast = Cast(_long); break;
                        case _ulong  : lhsCast = cast = Cast(_ulong); break;
                        case _float  : lhsCast = cast = Cast(_double); break;
                        case _double : lhsCast = cast = Cast(_double); break;
                        case _decimal: lhsCast = cast = Cast(_decimal); break;
                        case _char   : lhsCast = cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _ulong:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_ulong); break;
                        case _byte   : rhsCast = cast = Cast(_ulong); break;
                        case _sbyte  : rhsCast = cast = Cast(_ulong); break;
                        case _short  : rhsCast = cast = Cast(_ulong); break;
                        case _ushort : rhsCast = cast = Cast(_ulong); break;
                        case _int    : rhsCast = cast = Cast(_ulong); break;
                        case _uint   : rhsCast = cast = Cast(_ulong); break;
                        case _long   : rhsCast = cast = Cast(_ulong); break;
                        case _ulong  : cast = Cast(_ulong); break;
                        case _float  : lhsCast = cast = Cast(_double); break;
                        case _double : lhsCast = cast = Cast(_double); break;
                        case _decimal: lhsCast = cast = Cast(_decimal); break;
                        case _char   : lhsCast = cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _float:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_float); break;
                        case _byte   : rhsCast = cast = Cast(_float); break;
                        case _sbyte  : rhsCast = cast = Cast(_float); break;
                        case _short  : rhsCast = cast = Cast(_float); break;
                        case _ushort : rhsCast = cast = Cast(_float); break;
                        case _int    : rhsCast = cast = Cast(_float); break;
                        case _uint   : rhsCast = cast = Cast(_float); break;
                        case _long   : rhsCast = cast = Cast(_double); break;
                        case _ulong  : rhsCast = cast = Cast(_double); break;
                        case _float  : cast = Cast(_float); break;
                        case _double : lhsCast = cast = Cast(_double); break;
                        case _decimal: lhsCast = cast = Cast(_decimal); break;
                        case _char   : lhsCast = cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _double:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_double); break;
                        case _byte   : rhsCast = cast = Cast(_double); break;
                        case _sbyte  : rhsCast = cast = Cast(_double); break;
                        case _short  : rhsCast = cast = Cast(_double); break;
                        case _ushort : rhsCast = cast = Cast(_double); break;
                        case _int    : rhsCast = cast = Cast(_double); break;
                        case _uint   : rhsCast = cast = Cast(_double); break;
                        case _long   : rhsCast = cast = Cast(_double); break;
                        case _ulong  : rhsCast = cast = Cast(_double); break;
                        case _float  : rhsCast = cast = Cast(_double); break;
                        case _double : cast = Cast(_double); break;
                        case _decimal: lhsCast = cast = Cast(_decimal); break;
                        case _char   : lhsCast = cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _decimal:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_decimal); break;
                        case _byte   : rhsCast = cast = Cast(_decimal); break;
                        case _sbyte  : rhsCast = cast = Cast(_decimal); break;
                        case _short  : rhsCast = cast = Cast(_decimal); break;
                        case _ushort : rhsCast = cast = Cast(_decimal); break;
                        case _int    : rhsCast = cast = Cast(_decimal); break;
                        case _uint   : rhsCast = cast = Cast(_decimal); break;
                        case _long   : rhsCast = cast = Cast(_decimal); break;
                        case _ulong  : rhsCast = cast = Cast(_decimal); break;
                        case _float  : rhsCast = cast = Cast(_decimal); break;
                        case _double : rhsCast = cast = Cast(_decimal); break;
                        case _decimal: cast = Cast(_decimal); break;
                        case _char   : lhsCast = cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _char:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_char); break;
                        case _byte   : rhsCast = cast = Cast(_char); break;
                        case _sbyte  : rhsCast = cast = Cast(_char); break;
                        case _short  : rhsCast = cast = Cast(_char); break;
                        case _ushort : rhsCast = cast = Cast(_char); break;
                        case _int    : rhsCast = cast = Cast(_char); break;
                        case _uint   : rhsCast = cast = Cast(_char); break;
                        case _long   : rhsCast = cast = Cast(_char); break;
                        case _ulong  : rhsCast = cast = Cast(_char); break;
                        case _float  : rhsCast = cast = Cast(_char); break;
                        case _double : rhsCast = cast = Cast(_char); break;
                        case _decimal: rhsCast = cast = Cast(_char); break;
                        case _char   : cast = Cast(_char); break;
                        case _nint   : lhsCast = cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _nint:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_nint); break;
                        case _byte   : rhsCast = cast = Cast(_nint); break;
                        case _sbyte  : rhsCast = cast = Cast(_nint); break;
                        case _short  : rhsCast = cast = Cast(_nint); break;
                        case _ushort : rhsCast = cast = Cast(_nint); break;
                        case _int    : rhsCast = cast = Cast(_nint); break;
                        case _uint   : rhsCast = cast = Cast(_nint); break;
                        case _long   : rhsCast = cast = Cast(_nint); break;
                        case _ulong  : rhsCast = cast = Cast(_nint); break;
                        case _float  : rhsCast = cast = Cast(_nint); break;
                        case _double : rhsCast = cast = Cast(_nint); break;
                        case _decimal: rhsCast = cast = Cast(_nint); break;
                        case _char   : rhsCast = cast = Cast(_nint); break;
                        case _nint   : cast = Cast(_nint); break;
                        case _nuint  : lhsCast = cast = Cast(_nuint); break;
                    }

                    break;
                }

                case _nuint:
                {
                    switch (rhs)
                    {
                        case _object : rhsCast = cast = Cast(_nuint); break;
                        case _byte   : rhsCast = cast = Cast(_nuint); break;
                        case _sbyte  : rhsCast = cast = Cast(_nuint); break;
                        case _short  : rhsCast = cast = Cast(_nuint); break;
                        case _ushort : rhsCast = cast = Cast(_nuint); break;
                        case _int    : rhsCast = cast = Cast(_nuint); break;
                        case _uint   : rhsCast = cast = Cast(_nuint); break;
                        case _long   : rhsCast = cast = Cast(_nuint); break;
                        case _ulong  : rhsCast = cast = Cast(_nuint); break;
                        case _float  : rhsCast = cast = Cast(_nuint); break;
                        case _double : rhsCast = cast = Cast(_nuint); break;
                        case _decimal: rhsCast = cast = Cast(_nuint); break;
                        case _char   : rhsCast = cast = Cast(_nuint); break;
                        case _nint   : rhsCast = cast = Cast(_nuint); break;
                        case _nuint  : cast = Cast(_nuint); break;
                    }

                    break;
                }
            }
        }
    }
}