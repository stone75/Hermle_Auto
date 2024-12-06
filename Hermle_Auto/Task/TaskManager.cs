using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hermle_Auto.Task
{
    public class TaskManager
    {
        private readonly static TaskManager __instance = new TaskManager ();

        private TaskManager () 
        {
            init ();
        }

        private void init ()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
        }
    }
}
