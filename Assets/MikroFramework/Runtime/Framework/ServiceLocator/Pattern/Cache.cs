using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    public class Cache {
        private List<IService> services = new List<IService>();

        public IService GetService(string serviceName) {
            return services.SingleOrDefault(s => s.Name == serviceName);
        }

        public void AddService(IService service) {
            services.Add(service);
        }
    }
}
