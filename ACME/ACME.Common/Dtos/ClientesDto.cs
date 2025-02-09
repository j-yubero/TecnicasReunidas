﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACME.Common.Dtos
{
    public class ClientesDto
    {
        public Guid? Id { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public bool? Activo {  get; set; }
        public string CreatedBy { get; set; }
    }
}
