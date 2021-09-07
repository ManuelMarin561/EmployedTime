using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EmployedTime.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace EmployedTime.Functions.Funtions
{
    public static class ScheduleConsolidateTime
    {
        [FunctionName("ScheduleConsolidateTime")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
        [Table("EmployedTime", Connection = "AzureWebJobsStorage")] CloudTable EmployedTimeTable,
        [Table("ConsolitedTime", Connection = "AzureWebJobsStorage")] CloudTable ConsolitedTimeTable,
        ILogger log)
        {
            log.LogInformation($"Processing time consolidation at: {DateTime.Now}");


            string filter = TableQuery.GenerateFilterConditionForBool("Consolidado", QueryComparisons.Equal, false);

            TableQuery<EmployedTimeEntity> query = new TableQuery<EmployedTimeEntity>().Where(filter);
            TableQuerySegment<EmployedTimeEntity> UnConsolitedEmployed = await EmployedTimeTable.ExecuteQuerySegmentedAsync(query, null);


            DataTable DT = ToDataTable(UnConsolitedEmployed.ToList());

            DataView DtV = DT.DefaultView;
            DtV.Sort = "IdEmployeed, Fecha ASC";

            DT = DtV.ToTable();

            DateTime FechaIni;
            DateTime FechaFin;
            double Min;

            for (int i = 0; i < DT.Rows.Count; i++)
            {
                if ((i + 1) < DT.Rows.Count)
                {

                    if ((Convert.ToInt32(DT.Rows[i]["IdEmployeed"]) == Convert.ToInt32(DT.Rows[i + 1]["IdEmployeed"])) && (Convert.ToInt32(DT.Rows[i]["Tipo"]) != Convert.ToInt32(DT.Rows[i + 1]["Tipo"])) && (Convert.ToBoolean(DT.Rows[i]["Consolidado"]) == false && Convert.ToBoolean(DT.Rows[i + 1]["Consolidado"]) == false))
                    {
                        FechaIni = (DateTime)DT.Rows[i]["Fecha"];

                        FechaFin = (DateTime)DT.Rows[i + 1]["Fecha"];

                        Min = (FechaFin - FechaIni).TotalMinutes;


                        ConsolitedTimeEntity consolitedTimeEntity = new ConsolitedTimeEntity
                        {
                            IdEmployeed = Convert.ToInt32(DT.Rows[i]["IdEmployeed"]),
                            Fecha = Convert.ToDateTime(DT.Rows[i]["Fecha"]),
                            MinTrabajados = Min,
                            ETag = "*",
                            PartitionKey = "EmployedTime",
                            RowKey = Guid.NewGuid().ToString()
                        };

                        TableOperation addOperation = TableOperation.Insert(consolitedTimeEntity);
                        await ConsolitedTimeTable.ExecuteAsync(addOperation);

                        DT.Rows[i]["Consolidado"] = "True";

                        DT.Rows[i + 1]["Consolidado"] = "True";

                        for (int j = i; j < (i + 2); j++)
                        {

                            TableOperation findOperation = TableOperation.Retrieve<EmployedTimeEntity>("EmployedTime", DT.Rows[j]["RowKey"].ToString());
                            TableResult findResult = await EmployedTimeTable.ExecuteAsync(findOperation);


                            EmployedTimeEntity employedTimeEntity = (EmployedTimeEntity)findResult.Result;

                            employedTimeEntity.Consolidado = true;

                            TableOperation addOperation2 = TableOperation.Replace(employedTimeEntity);
                            await EmployedTimeTable.ExecuteAsync(addOperation2);

                        }



                    }


                }
            }


        }


        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


    }
}
