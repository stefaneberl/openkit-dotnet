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

namespace Dynatrace.OpenKit.Providers
{
    /// <summary>
    /// Default implementation of PRNGenerator providing random numbers
    /// </summary>
    internal class DefaultPrnGenerator : IPrnGenerator
    {
        private readonly Random random = new Random();

        public int NextPositiveInt()
        {
            return random.Next(int.MaxValue);
        }

        public long NextPositiveLong()
        {
            return (long)(random.NextDouble() * long.MaxValue);
        }
    }
}
