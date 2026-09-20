namespace Domain.Entities
{
    public class Cuenta
    {
        public Guid Id { get; private set; } // porque private set ? porque voy a usar el patron domain driven desing 
        public string Nombre { get; private set; }
        public decimal SaldoDisponible { get; private set; }
        public string TipoDeCuenta { get; private set; }
        public string TipoDeMoneda { get; private set; }
        public Guid ClienteId { get; private set; }

        private readonly List<Movimiento> _movimientos = new();
        public IReadOnlyList<Movimiento> Movimientos => _movimientos;
        private Cuenta() { } // Constructor privado para EF Core

        // Constructor publico para crear una nueva cuenta
        public Cuenta(string nombre, decimal saldoDisponible, string tipoDeCuenta, string tipoDeMoneda, Guid clienteId)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la cuenta no puede estar vacío.", nameof(nombre));

            if (saldoDisponible < 0)
                throw new ArgumentException("El saldo disponible no puede ser negativo.", nameof(saldoDisponible));

            if (tipoDeCuenta != "Banco" && tipoDeCuenta != "Billetera Virtual" && tipoDeCuenta != "Broker" && tipoDeCuenta != "Efectivo")
                throw new ArgumentException("El tipo de cuenta debe ser 'Banco' o 'Billetera Virtual' o 'Broker' o 'Efectivo'.", nameof(tipoDeCuenta));

            if (tipoDeMoneda != "USD" && tipoDeMoneda != "ARS")
                throw new ArgumentException("El tipo de moneda debe ser 'USD' o 'ARS'.", nameof(tipoDeMoneda));
            

            Id = Guid.NewGuid();
            Nombre = nombre;
            SaldoDisponible = saldoDisponible;
            TipoDeCuenta = tipoDeCuenta;
            TipoDeMoneda = tipoDeMoneda;
            ClienteId = clienteId;
        }

        public Movimiento RegistrarIngreso(decimal monto, string descripcion, DateTime fecha, string tipoDeMoneda)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción del movimiento no puede estar vacía.", nameof(descripcion));
            if (fecha > DateTime.Now)
                throw new ArgumentException("La fecha del movimiento no puede ser futura.", nameof(fecha));
            if (tipoDeMoneda != "USD" && tipoDeMoneda != "ARS")
                throw new ArgumentException("El tipo de moneda debe ser 'USD' o 'ARS'.", nameof(tipoDeMoneda));
            if (monto <= 0)
                throw new ArgumentException("El monto del movimiento debe ser mayor a cero.", nameof(monto));
            if (tipoDeMoneda != TipoDeMoneda)
                throw new ArgumentException("El tipo de moneda del movimiento debe coincidir con el tipo de moneda de la cuenta.", nameof(tipoDeMoneda));

            var movimiento = new Movimiento("Ingreso", descripcion, monto, fecha, tipoDeMoneda, this.Id);
            SaldoDisponible += monto;
            _movimientos.Add(movimiento);
            return movimiento;
        }

        public Movimiento RegistrarEgreso(decimal monto, string descripcion, DateTime fecha, string tipoDeMoneda)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("La descripción del movimiento no puede estar vacía.", nameof(descripcion));
            if (fecha > DateTime.Now)
                throw new ArgumentException("La fecha del movimiento no puede ser futura.", nameof(fecha));
            if (tipoDeMoneda != "USD" && tipoDeMoneda != "ARS")
                throw new ArgumentException("El tipo de moneda debe ser 'USD' o 'ARS'.", nameof(tipoDeMoneda));
            if (monto <= 0)
                throw new ArgumentException("El monto del movimiento debe ser mayor a cero.", nameof(monto));
            if (tipoDeMoneda != TipoDeMoneda)
                throw new ArgumentException("El tipo de moneda del movimiento debe coincidir con el tipo de moneda de la cuenta.", nameof(tipoDeMoneda));   
            if (monto > SaldoDisponible)
                throw new InvalidOperationException("No se puede registrar un egreso mayor al saldo disponible.");

            var movimiento = new Movimiento("Egreso", descripcion, monto, fecha, tipoDeMoneda, this.Id);
            SaldoDisponible -= monto;
            _movimientos.Add(movimiento);
            return movimiento;
        }


        public void CancelarMovimiento(Guid idMovimiento)
        {
            var movimiento = _movimientos.FirstOrDefault(m => m.Id == idMovimiento)
                ?? throw new InvalidOperationException("No se encontró el movimiento en esta cuenta.");
            
            movimiento.Cancelar();
            
            if (movimiento.TipoDeMovimiento == "Ingreso")
                SaldoDisponible -= movimiento.Monto; 
            else
                SaldoDisponible += movimiento.Monto;

        }
    }

}