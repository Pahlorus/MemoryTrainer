using System;
using System.Collections.Generic;

namespace Utility
{
    public interface IConstructedFrom<T> { }

    public class Creator { }
    public class Creator<S, T> : Creator where S : IConstructedFrom<T>
    {
        private Func<T, S> _factory;

        public Creator(Func<T, S> factory)
        {
            _factory = factory;
        }

        public S Create(T value)
        {
            return _factory.Invoke(value);
        }
    }

    public class TypeFactory : Dictionary<Type,Creator>
    {
        public void Add<S,T>(Func<T,S> factory) where S : IConstructedFrom<T>
        {
            Add(typeof(S), new Creator<S, T>(factory));
        }

        public Creator<S,T> GetCreator<S, T>() where S : IConstructedFrom<T>
        {
            return this[typeof(S)] as Creator<S,T>;
        }

        /// <summary>
        /// If you want create multiple instances of same type - use <see cref="GetCreator{S, T}"/>
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public S Create<S,T>(T value) where S : IConstructedFrom<T>
        {
            return GetCreator<S, T>().Create(value);
        }
    }
}
