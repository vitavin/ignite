/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Portable;
using Apache.Ignite.ExamplesDll.Portable;

namespace Apache.Ignite.Examples.Datagrid
{
    /// <summary>
    /// This example demonstrates several put-get operations on Ignite cache
    /// with portable values. Note that portable object can be retrieved in
    /// fully-deserialized form or in portable object format using special
    /// cache projection.
    /// <para />
    /// 1) Build the project Apache.Ignite.ExamplesDll (select it -> right-click -> Build).
    ///    Apache.Ignite.ExamplesDll.dll must appear in %IGNITE_HOME%/platforms/dotnet/Examples/Apache.Ignite.ExamplesDll/bin/${Platform]/${Configuration} folder.
    /// 2) Set this class as startup object (Apache.Ignite.Examples project -> right-click -> Properties ->
    ///     Application -> Startup object);
    /// 3) Start example (F5 or Ctrl+F5).
    /// <para />
    /// This example can be run with standalone Apache Ignite .Net node:
    /// 1) Run %IGNITE_HOME%/platforms/dotnet/Apache.Ignite/bin/${Platform]/${Configuration}/Apache.Ignite.exe:
    /// Apache.Ignite.exe -IgniteHome="%IGNITE_HOME%" -springConfigUrl=platforms\dotnet\examples\config\example-cache.xml -assembly=[path_to_Apache.Ignite.ExamplesDll.dll]
    /// 2) Start example.
    /// </summary>
    public class PutGetExample
    {
        /// <summary>Cache name.</summary>
        private const string CacheName = "cache_put_get";

        /// <summary>
        /// Runs the example.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var cfg = new IgniteConfiguration
            {
                SpringConfigUrl = @"platforms\dotnet\examples\config\example-cache.xml",
                JvmOptions = new List<string> { "-Xms512m", "-Xmx1024m" }
            };

            using (var ignite = Ignition.Start(cfg))
            {
                Console.WriteLine();
                Console.WriteLine(">>> Cache put-get example started.");

                // Clean up caches on all nodes before run.
                ignite.GetOrCreateCache<object, object>(CacheName).Clear();

                PutGet(ignite);
                PutGetPortable(ignite);
                PutAllGetAll(ignite);
                PutAllGetAllPortable(ignite);

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine(">>> Example finished, press any key to exit ...");
            Console.ReadKey();
        }

        /// <summary>
        /// Execute individual Put and Get.
        /// </summary>
        /// <param name="ignite">Ignite instance.</param>
        private static void PutGet(IIgnite ignite)
        {
            var cache = ignite.GetCache<int, Organization>(CacheName);

            // Create new Organization to store in cache.
            Organization org = new Organization(
                "Microsoft",
                new Address("1096 Eddy Street, San Francisco, CA", 94109),
                OrganizationType.Private,
                DateTime.Now
            );

            // Put created data entry to cache.
            cache.Put(1, org);

            // Get recently created employee as a strongly-typed fully de-serialized instance.
            Organization orgFromCache = cache.Get(1);

            Console.WriteLine();
            Console.WriteLine(">>> Retrieved organization instance from cache: " + orgFromCache);
        }

        /// <summary>
        /// Execute individual Put and Get, getting value in portable format, without de-serializing it.
        /// </summary>
        /// <param name="ignite">Ignite instance.</param>
        private static void PutGetPortable(IIgnite ignite)
        {
            var cache = ignite.GetCache<int, Organization>(CacheName);

            // Create new Organization to store in cache.
            Organization org = new Organization(
                "Microsoft",
                new Address("1096 Eddy Street, San Francisco, CA", 94109),
                OrganizationType.Private,
                DateTime.Now
            );

            // Put created data entry to cache.
            cache.Put(1, org);

            // Create projection that will get values as portable objects.
            var portableCache = cache.WithKeepPortable<int, IPortableObject>();

            // Get recently created organization as a portable object.
            var portableOrg = portableCache.Get(1);

            // Get organization's name from portable object (note that  object doesn't need to be fully deserialized).
            string name = portableOrg.GetField<string>("name");

            Console.WriteLine();
            Console.WriteLine(">>> Retrieved organization name from portable object: " + name);
        }

        /// <summary>
        /// Execute bulk Put and Get operations.
        /// </summary>
        /// <param name="ignite">Ignite instance.</param>
        private static void PutAllGetAll(IIgnite ignite)
        {
            var cache = ignite.GetCache<int, Organization>(CacheName);

            // Create new Organizations to store in cache.
            Organization org1 = new Organization(
                "Microsoft",
                new Address("1096 Eddy Street, San Francisco, CA", 94109),
                OrganizationType.Private,
                DateTime.Now
            );

            Organization org2 = new Organization(
                "Red Cross",
                new Address("184 Fidler Drive, San Antonio, TX", 78205),
                OrganizationType.NonProfit,
                DateTime.Now
            );

            var map = new Dictionary<int, Organization> { { 1, org1 }, { 2, org2 } };

            // Put created data entries to cache.
            cache.PutAll(map);

            // Get recently created organizations as a strongly-typed fully de-serialized instances.
            IDictionary<int, Organization> mapFromCache = cache.GetAll(new List<int> { 1, 2 });

            Console.WriteLine();
            Console.WriteLine(">>> Retrieved organization instances from cache:");

            foreach (Organization org in mapFromCache.Values)
                Console.WriteLine(">>>     " + org);
        }

        /// <summary>
        /// Execute bulk Put and Get operations getting values in portable format, without de-serializing it.
        /// </summary>
        /// <param name="ignite">Ignite instance.</param>
        private static void PutAllGetAllPortable(IIgnite ignite)
        {
            var cache = ignite.GetCache<int, Organization>(CacheName);

            // Create new Organizations to store in cache.
            Organization org1 = new Organization(
                "Microsoft",
                new Address("1096 Eddy Street, San Francisco, CA", 94109),
                OrganizationType.Private,
                DateTime.Now
            );

            Organization org2 = new Organization(
                "Red Cross",
                new Address("184 Fidler Drive, San Antonio, TX", 78205),
                OrganizationType.NonProfit,
                DateTime.Now
            );

            var map = new Dictionary<int, Organization> { { 1, org1 }, { 2, org2 } };

            // Put created data entries to cache.
            cache.PutAll(map);

            // Create projection that will get values as portable objects.
            var portableCache = cache.WithKeepPortable<int, IPortableObject>();

            // Get recently created organizations as portable objects.
            IDictionary<int, IPortableObject> portableMap =
                portableCache.GetAll(new List<int> { 1, 2 });

            Console.WriteLine();
            Console.WriteLine(">>> Retrieved organization names from portable objects:");

            foreach (IPortableObject poratbleOrg in portableMap.Values)
                Console.WriteLine(">>>     " + poratbleOrg.GetField<string>("name"));
        }
    }
}
