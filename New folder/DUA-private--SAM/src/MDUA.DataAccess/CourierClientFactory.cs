using MDUA.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MDUA.DataAccess
{
    public static class CourierClientFactory
    {
        private static readonly Dictionary<string, ICourierClient> _clients;

        static CourierClientFactory()
        {
            _clients = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    typeof(ICourierClient).IsAssignableFrom(t) &&
                    !t.IsInterface &&
                    !t.IsAbstract)
                .Select(t => (ICourierClient)Activator.CreateInstance(t))
                .ToDictionary(
                    c => c.CarrierName,
                    c => c,
                    StringComparer.OrdinalIgnoreCase
                );
        }

        public static ICourierClient Resolve(string carrierName)
        {
            if (!_clients.TryGetValue(carrierName, out var client))
                throw new Exception($"Courier not supported: {carrierName}");

            return client;
        }
    }
}
