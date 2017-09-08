using API.Services;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebSockets.Operations {
    public static class OperationUtils {
        public static void ResolveTaskContinuation(Task task) {
            if(task.IsCompleted) {//Testa se a Task foi completada sincronamente
                try {//Tratando o resultado imediatamente nesse caso
                    task.Wait();
                }
                catch(Exception) {
                    return;//TODO Rever
                }
            }
            else {
                //TODO Ver se não devia esperar que ela termine, pq embora prenda a thread garante que a db está consiste na altura
                //TODO Rever o que fazer caso mais tarde dê excepção
                //task.ContinueWith()
            }
        }
    }
}
