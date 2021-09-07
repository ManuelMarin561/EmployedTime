using EmployedTime.common.Models;
using EmployedTime.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace EmployedTime.Test.Helpers
{
    public class TestFactory
    {
        public static EmployedTimeEntity GetEmployedTimeEntity()
        {
            return new EmployedTimeEntity
            {

                ETag = "*",
                PartitionKey = "EmployedTime",
                RowKey = Guid.NewGuid().ToString(),
                IdEmployeed = 1,
                Fecha = DateTime.UtcNow,
                Consolidado = false,
                Tipo = 0

            };
        }

        public static List<EmployedTimeEntity> GetTimeRecordsEntity()
        {
            return new List<EmployedTimeEntity> {
            new EmployedTimeEntity
                {

                ETag = "*",
                PartitionKey = "EmployedTime",
                RowKey = Guid.NewGuid().ToString(),
                IdEmployeed = 1,
                Fecha = DateTime.UtcNow,
                Consolidado = false,
                Tipo = 0
                }

            };
        }
        public static List<ConsolitedTimeEntity> GetConsolidatedListEntity()
        {
            return new List<ConsolitedTimeEntity> {
            new ConsolitedTimeEntity
                {

                ETag = "*",
                PartitionKey = "ConsolitedTime",
                RowKey = Guid.NewGuid().ToString(),
                IdEmployeed = 1,
                Fecha = DateTime.UtcNow,
                MinTrabajados=60
                }

            };
        }
        public static ConsolitedTimeEntity GetConsolidatedEntity()
        {
            return new ConsolitedTimeEntity
            {

                ETag = "*",
                PartitionKey = "ConsolitedTime",
                RowKey = Guid.NewGuid().ToString(),
                IdEmployeed = 1,
                Fecha = DateTime.UtcNow,
                MinTrabajados=60
            };
        }
        public static DefaultHttpRequest CreateHttpRequest(Guid employeekey, EmployedTimeEntity employedTimeEntity)
        {
            string request = JsonConvert.SerializeObject(employedTimeEntity);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/${employeekey}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(DateTime date)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/${date}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid employeekey)
        {

            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/${employeekey}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(EmployedTimeEntity employedTimeEntity)
        {

            string request = JsonConvert.SerializeObject(employedTimeEntity);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {

            return new DefaultHttpRequest(new DefaultHttpContext());

        }

        public static EmployedTimeEntity GetTimeRecordRequest()
        {
            return new EmployedTimeEntity
            {
                IdEmployeed = 1,
                Fecha = DateTime.UtcNow,
                Consolidado = false,
                Tipo = 0

            };
        }
        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }

    }
}
