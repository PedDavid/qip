using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebSockets.Operations {
    public class OperationUtils {
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
                //TODO Rever o que fazer caso mais tarde dê excepção
                //task.ContinueWith()
            }
        }
    }
}
