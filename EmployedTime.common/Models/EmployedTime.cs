using System;

namespace EmployedTime.common.Models
{
    public class EmployedTime
    {
        public int IdEmployeed { get; set; }

        public DateTime Fecha { get; set; }

        public int Tipo { get; set; }

        public bool Consolidado { get; set; }

    }
}
