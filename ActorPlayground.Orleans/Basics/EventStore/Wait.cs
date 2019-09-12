using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ActorPlayground.Orleans.Basics.EventStore
{
    public static class Wait
    {

        public static async Task Until(Func<bool> condition, TimeSpan timeout, int frequency = 200)
        {

            var waitTask = Task.Run(async () =>
            {
                while (!condition())
                {
                    await Task.Delay(frequency);
                }
            });


            if (waitTask == await Task.WhenAny(waitTask, Task.Delay(timeout)))
            {
                return;
            }
            
            throw new TimeoutException();

        }

    }
}
