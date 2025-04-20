using System;
using ZooDomain.Enums;

namespace ZooPresentation.Models
{
    public class CreateEnclosureRequest
    {
        public EnclosureType Type { get; set; }
        public double Area { get; set; }
        public int Capacity { get; set; }
    }
}

