using System;
using System.Diagnostics;

namespace WolvenKit.RED4.Types
{
    public interface IRedType
    { 
        public object GetValue();
        public void Read(Red4Reader file, uint size);
    }

    public interface IRedType<T> : IRedType
    {
        public T Value { get; set; }
    }

    public interface IRedGenericType : IRedType
    {
    }

    public interface IRedGenericType<T> : IRedGenericType
    {
    }
}
