//
// Copyright 2018-2020 Dynatrace LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using System.Threading;

namespace Dynatrace.OpenKit.Core.Util
{
    /// <summary>
    ///
    /// </summary>
    public class InterruptibleThreadSuspender : IInterruptibleThreadSuspender
    {
        private readonly ManualResetEvent waitEvent = new ManualResetEvent(false);

        public bool Sleep(int millis)
        {
#if !(NETCOREAPP1_0 || NETCOREAPP1_1 || WINDOWS_UWP || NETSTANDARD1_1)
            try
            {
#endif
                var sleepTime = millis;
                var sleepEnd = DateTime.UtcNow + TimeSpan.FromMilliseconds(millis);
                while (true)
                {
                    if (waitEvent.WaitOne(sleepTime))
                    {
                        return true;
                    }

                    var now = DateTime.UtcNow;
                    if (now >= sleepEnd)
                    {
                        break;
                    }

                    sleepTime = (int)(sleepEnd - now).TotalMilliseconds;
                }
#if !(NETCOREAPP1_0 || NETCOREAPP1_1 || WINDOWS_UWP || NETSTANDARD1_1)
            }
            catch (ThreadInterruptedException)
            {
                return false;
            }
#endif
            return true;
        }

        public void WakeUp()
        {
            waitEvent.Set();
        }
    }
}