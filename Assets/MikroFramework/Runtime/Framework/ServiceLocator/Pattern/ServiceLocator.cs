using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MikroFramework.ServiceLocator
{
    public class ServiceLocator {
        private readonly Cache cache = new Cache();
        private readonly AbstractInitialContext context;

        public ServiceLocator(AbstractInitialContext context) {
            this.context = context;
        }

        /// <summary>
        /// Get a service from cache or create and get the service by the InitialContext
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IService GetService(string name) {
            IService service = cache.GetService(name);

            if (service == null) {
                service = context.LookUp(name);
                cache.AddService(service);
            }

            if (service == null) {
                throw new Exception($"Service: {name} does not exist!");
            }

            return service;
        }
    }
}
