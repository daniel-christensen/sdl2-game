﻿using TestApp.Interfaces;
using TestApp.Messages;

namespace TestApp.Registry
{
    internal static class RegistryManager
    {
        private static Dictionary<Enum, Type> _registeredComponents = new Dictionary<Enum, Type>();

        private static Dictionary<Enum, Type> GetRegistry(Enum key)
        {
            switch (key)
            {
                case RegistryKey.Component: return _registeredComponents;
            }
            throw new InvalidOperationException(RegistryExceptionMessage.NonExistentRegistry);
        }

        internal static void Register<T>(Enum key, Enum subKey, Type type)
        {
            if (!typeof(IRegisterable).IsAssignableFrom(type))
                throw new InvalidOperationException(RegistryExceptionMessage.RegisterableInterfaceNotImplemented);
            if (!typeof(T).IsAssignableFrom(type))
                throw new InvalidOperationException(RegistryExceptionMessage.SubKeyInterfaceNotImplemented);
            var registry = GetRegistry(key);
            if (registry.ContainsKey(subKey))
                throw new InvalidOperationException(RegistryExceptionMessage.DoubleRegister);
            registry.Add(subKey, type);
        }

        internal static T CreateInstance<T>(Enum key, Enum subKey, Game game)
        {
            var registry = GetRegistry(key);
            if (!registry.ContainsKey(subKey))
                throw new InvalidOperationException(RegistryExceptionMessage.NonExistentRegistration);
            Type type = registry[subKey];
            var instance = Activator.CreateInstance(type, [game]);
            if (instance is null)
                throw new InvalidOperationException(RegistryExceptionMessage.NullInstance);
            return (T)instance;
        }
    }

    internal enum RegistryKey
    {
        Component
    }

    internal enum ComponentKey
    {
        Graphics,
        Logic,
        EventPoller
    }
}
