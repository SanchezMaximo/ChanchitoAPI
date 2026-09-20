namespace Domain.Entities
{
    public class Movimiento
    {
        public Guid Id { get; private set; }
        public string TipoDeMovimiento { get; private set; }
        public string Descripcion { get; private set; }
        public decimal Monto { get; private set; }
        public DateTime Fecha { get; private set; }
        public string TipoDeMoneda { get; private set; }
        public bool Cancelado { get; private set; }
        public Guid CuentaId { get; private set; }
        
        
        private Movimiento()  {}// Constructor privado para EF Core

        public Movimiento(string tipoDeMovimiento, string descripcion, decimal monto, DateTime fecha, string tipoDeMoneda, Guid cuentaId)
        {
            if (string.IsNullOrWhiteSpace(tipoDeMovimiento))
                throw new ArgumentException("El tipo de movimiento no puede estar vacío.", nameof(tipoDeMovimiento));
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción del movimiento no puede estar vacía.", nameof(descripcion));
            if (monto <= 0)
                throw new ArgumentException("El monto del movimiento debe ser mayor a cero.", nameof(monto));
            if (fecha > DateTime.Now)
                throw new ArgumentException("La fecha del movimiento no puede ser futura.", nameof(fecha));
            if (tipoDeMoneda != "USD" && tipoDeMoneda != "ARS")
                throw new ArgumentException("El tipo de moneda debe ser 'USD' o 'ARS'.", nameof(tipoDeMoneda));
            if (cuentaId == Guid.Empty)
                throw new ArgumentException("El Id de la cuenta no puede estar vacío.", nameof(cuentaId));
            
            Id = Guid.NewGuid();
            TipoDeMovimiento = tipoDeMovimiento;
            Descripcion = descripcion;
            Monto = monto;
            Fecha = fecha;
            TipoDeMoneda = tipoDeMoneda;
            Cancelado = false;
            CuentaId = cuentaId;
        }

        public void Cancelar()
        {
            if (Cancelado)
                throw new InvalidOperationException("El movimiento ya se encuentra cancelado.");

            Cancelado = true;
        }
    }

}
