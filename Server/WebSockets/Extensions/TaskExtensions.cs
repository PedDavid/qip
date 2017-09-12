using API.Interfaces.ServicesExceptions;
using System;
using System.Threading.Tasks;

namespace WebSockets.Extensions {
    public static class TaskExtensions {
        public static Exception GetServiceFault(this Task task) {
            try {
                task.Wait();
            }
            catch(ServiceException e) {
                return e;
            }
            return null;
        }
    }
}
