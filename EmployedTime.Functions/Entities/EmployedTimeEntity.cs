using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmployedTime.Functions.Entities
{
    public class EmployedTimeEntity : TableEntity
    {

        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public int Tipo { get; set; }

        public bool Consolidado { get; set; }

    }
}
