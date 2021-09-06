using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployedTime.Functions.Entities
{
    public class ConsolitedTime : TableEntity
    {

        public int IdEmployeed { get; set; }
        public DateTime Fecha { get; set; }

        public double MinTrabajados { get; set; }

    }
}
