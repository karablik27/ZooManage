using System;
namespace ZooDomain
{
    /// <summary>
    /// Базовое исключение доменного уровня.
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}

