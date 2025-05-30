﻿using System;

namespace Singletons
{
    /// <summary>
    /// <para>Designed for decoupling Singleton pattern and user classes.</para>
    /// <para>Usage: <see cref="Singleton"/>.Of&lt;MyClass&gt;()</para>
    /// </summary>
    public static class Singleton
    {
        public static T Of<T>() where T : class, new()
            => Single<T>.GetInstance(() => new T());

        public static T Of<T>(Func<T> instantiator) where T : class
        {
            if (instantiator == null)
                throw new ArgumentNullException(nameof(instantiator));

            return Single<T>.GetInstance(instantiator);
        }

        private static class Single<T> where T : class
        {
            private static T s_instance;

            public static T GetInstance(Func<T> instantiator)
            {
                if (s_instance == null)
                {
                    s_instance = instantiator();
                }

                return s_instance;
            }
        }
    }
}
