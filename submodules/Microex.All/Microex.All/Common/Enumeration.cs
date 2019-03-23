using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microex.All.Common
{
    public abstract class Enumeration<T, TImplemention> : ValueObject, IEnumeration<T>, IComparable where T : IComparable where TImplemention : IEnumeration<T>
    {
        public static bool TryParse<T>(T value, out TImplemention result)
        {
            result = GetAll().FirstOrDefault(x => x.Value.CompareTo(value) == 0);
            return result != null;
        }

        public static TImplemention ParseValue<T>(T value)
        {
            var result = GetAll().First(x => x.Value.CompareTo(value) == 0);
            return result;
        }

        public static List<T> PossibleValues { get; } = GetAll().Select(x => x.Value).ToList();
        public static List<TImplemention> PossibleEnumerations { get; } = GetAll();

        private static List<TImplemention> GetAll()
        {
            var type = typeof(TImplemention);
            var typeInfo = type.GetTypeInfo();
            var fields = typeInfo.GetFields(BindingFlags.Public |
                                            BindingFlags.Static |
                                            BindingFlags.DeclaredOnly);
            var all = new List<TImplemention>();
            foreach (var info in fields)
            {
                if (info.GetValue(default(TImplemention)) is TImplemention locatedValue)
                {
                    all.Add(locatedValue);
                }
            }

            return all;
        }

        protected Enumeration()
        {

        }
        public abstract string Name { get; protected set; }
        public abstract T Value { get; protected set; }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator T(Enumeration<T, TImplemention> value)
        {
            return value.Value;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
            yield return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var otherValue = obj as Enumeration<T, TImplemention>;
            if (otherValue == null)
            {
                return false;
            }
            var typeMatches = GetType() == obj.GetType();
            if (!typeMatches)
            {
                return false;
            }
            var valueMatches = Value.Equals(otherValue.Value);
            return valueMatches;
        }

        public int CompareTo(object other)
        {
            return Value.CompareTo(((Enumeration<T, TImplemention>)other).Value);
        }

        // Other utility methods ...
    }

    public interface IEnumeration<T>
    {
        string Name { get; }
        T Value { get; }
    }
}
