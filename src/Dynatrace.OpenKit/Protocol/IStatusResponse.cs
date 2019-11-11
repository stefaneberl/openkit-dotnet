//
// Copyright 2018-2019 Dynatrace LLC
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

namespace Dynatrace.OpenKit.Protocol
{
    /// <summary>
    /// Defines a status response which is sent back for the request types status check and beacon send.
    /// </summary>
    public interface IStatusResponse : IResponse
    {
        bool Capture { get; }

        int SendInterval { get; }

        int ServerId { get; }

        int MaxBeaconSize { get; }

        bool CaptureErrors { get; }

        bool CaptureCrashes { get; }

        int Multiplicity { get; }
    }
}