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

namespace Dynatrace.OpenKit
{
    /// <summary>
    /// Concrete builder that creates an <code>IOpenKit</code> instance for Dynatrace SaaS/Managed
    /// </summary>
    public class DynatraceOpenKitBuilder : AbstractOpenKitBuilder
    {
        /// <summary>
        /// The default ID of the server to communicate with
        /// </summary>
        public const int DefaultServerIdValue = 1;

        /// <summary>
        /// Identifies the type of OpenKit for which this builder is made.
        /// </summary>
        public const string Type = "DynatraceOpenKit";


        private string applicationName = null;

        /// <summary>
        /// Creates a new instance of type DynatraceOpenKitBuilder
        /// </summary>
        /// <param name="endPointUrl">endpoint OpenKit connects to</param>
        /// <param name="applicationId">unique application id</param>
        /// <param name="deviceId">unique device id</param>
        public DynatraceOpenKitBuilder(string endPointUrl, string applicationId, long deviceId)
            : base(endPointUrl, deviceId)
        {
            ApplicationId = applicationId;
        }

        /// <summary>
        /// Creates a new instance of type DynatraceOpenKitBuilder
        /// </summary>
        /// <param name="endPointUrl">endpoint OpenKit connects to</param>
        /// <param name="applicationId">unique application id</param>
        /// <param name="deviceId">unique device id</param>
        [Obsolete("use DynatraceOpenKitBuilder(string string, long) instead")]
        public DynatraceOpenKitBuilder(string endPointUrl, string applicationId, string deviceId)
            : base(endPointUrl, deviceId)
        {
            ApplicationId = applicationId;
        }

        /// <summary>
        /// Sets the application name. The value is only set if it is not null.
        /// </summary>
        /// <param name="applicationName">name of the application</param>
        /// <returns><code>this</code></returns>
        [Obsolete("set in Dynatrace UI when custom application is created")]
        public AbstractOpenKitBuilder WithApplicationName(string applicationName)
        {
            this.applicationName = applicationName;
            return this;
        }

        public override int DefaultServerId => DefaultServerIdValue;

        public override string OpenKitType => Type;

        public override string ApplicationId { get; }

        public override string ApplicationName => applicationName;


    }
}
