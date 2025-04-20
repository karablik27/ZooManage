using System;
using ZooDomain.Enums;

namespace ZooPresentation.Models
{
    public class CreateAnimalRequest
    {
        public string Species { get; set; } = default!;
        public string Name { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string FavoriteFoodName { get; set; } = default!;
        public int Calories { get; set; }
    }
}

