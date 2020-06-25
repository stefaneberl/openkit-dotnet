﻿//
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
using Dynatrace.OpenKit.Protocol;

namespace Dynatrace.OpenKit.Core.Communication
{
    /// <summary>
    /// Initial state for beacon sending.
    ///
    /// The initial state is used to retrieve the configuration from the server and update the configuration.
    ///
    /// Transitions to:
    /// <ul>
    ///     <li><see cref="BeaconSendingTerminalState"/> upon shutdown request</li>
    ///     <li><see cref="BeaconSendingCaptureOnState"/> if initial status request succeeded and capturing is enabled.</li>
    ///     <li><see cref="BeaconSendingCaptureOffState"/> if initial status request succeeded and capturing is disabled.</li>
    /// </ul>
    /// </summary>
    internal class BeaconSendingInitState : AbstractBeaconSendingState
    {
        private const int MaxInitialStatusRequestRetries = 5;
        public const int InitialRetrySleepTimeMilliseconds = 1000;

        // delays between consecutive initial status requests
        internal static readonly int[] ReInitDelayMilliseconds =
        {
            1 * 60 * 1000, // 1 minute in milliseconds
            5 * 60 * 1000, // 5 minutes in milliseconds
            15 * 60 * 1000, // 15 minutes in milliseconds
            1 * 60 * 60 * 1000, // 1 hour in milliseconds
            2 * 60 * 60 * 1000 // 2 hours in milliseconds
        };

        // current index into RE_INIT_DELAY_MILLISECONDS
        private int reinitializeDelayIndex = 0;

        internal BeaconSendingInitState() : base(false) { }

        internal override AbstractBeaconSendingState ShutdownState => new BeaconSendingTerminalState();

        internal override void OnInterrupted(IBeaconSendingContext context)
        {
            context.InitCompleted(false);
        }

        protected override void DoExecute(IBeaconSendingContext context)
        {
            var statusResponse = ExecuteStatusRequest(context);

            if (context.IsShutdownRequested)
            {
                // shutdown was requested -> abort init with failure
                // transition to shutdown state is handled by base class
                context.InitCompleted(false);
            }
            else if (BeaconSendingResponseUtil.IsSuccessfulResponse(statusResponse))
            {
                // success -> continue with capture on/capture off
                context.HandleStatusResponse(statusResponse);
                if (context.IsCaptureOn)
                {
                    context.NextState = new BeaconSendingCaptureOnState();
                }
                else
                {
                    context.NextState = new BeaconSendingCaptureOffState();
                }
                context.InitCompleted(true);
            }
        }

        public override string ToString()
        {
            return "Initial";
        }

        /// <summary>
        /// Execute status requests, until a successful response was received or shutdown was requested.
        /// </summary>
        /// <param name="context">The state's context</param>
        /// <returns>The last received status response, which might be erroneous if shutdown has been requested.</returns>
        private IStatusResponse ExecuteStatusRequest(IBeaconSendingContext context)
        {
            IStatusResponse statusResponse;

            while (true)
            {
                var currentTimestamp = context.CurrentTimestamp;
                context.LastOpenSessionBeaconSendTime = currentTimestamp;
                context.LastStatusCheckTime = currentTimestamp;

                statusResponse = BeaconSendingRequestUtil.SendStatusRequest(context, MaxInitialStatusRequestRetries, InitialRetrySleepTimeMilliseconds);
                if (context.IsShutdownRequested || BeaconSendingResponseUtil.IsSuccessfulResponse(statusResponse))
                {
                    // shutdown was requested or a successful status response was received
                    break;
                }

                var sleepTime = ReInitDelayMilliseconds[reinitializeDelayIndex];
                if (BeaconSendingResponseUtil.IsTooManyRequestsResponse(statusResponse))
                {
                    // in case of too many requests the server might send us a retry-after
                    sleepTime = statusResponse.GetRetryAfterInMilliseconds();
                    // also temporarily disable capturing to avoid further server overloading
                    context.DisableCaptureAndClear();
                }

                // status request needs to be sent again after some delay
                context.Sleep(sleepTime);
                reinitializeDelayIndex = Math.Min(reinitializeDelayIndex + 1, ReInitDelayMilliseconds.Length - 1); // ensure no out of bounds
            }

            return statusResponse;
        }
    }
}
