**ChanchitoAPI**

API REST para llevar las finanzas personales. La API permite crear cuentas en pesos o dólares, movimientos, categorías y metas de ahorro.

Qué permite hacer la API?
-Registrarse e iniciar sesión.
-Crear cuentas (banco, billetera virtual, broker o efectivo) en ARS o USD.
-Registrar ingresos y gastos.
-Armar categorías propias, además de las que define el sistema.
-Crear metas de ahorro y aportarles desde una cuenta de la misma moneda.
-Consultar la cotización del dólar (oficial y MEP) desde DolarAPI.
-Como administrador: ver clientes, suspender y reactivar usuarios (clientes y otros admins) y gestionar las categorías del sistema.

Extras:
-Cada movimiento en USD guarda la cotización oficial del momento, así el historial no cambia cuando se mueve el dólar. Si DolarAPI no responde, se usa la última guardada.
-Un aporte a una meta de ahorro es un gasto de una cuenta.
-Cancelar no borra: las transacciones y las metas se marcan como canceladas y el saldo se revierte a la cuenta.
-Suspender es inmediato: el token se revalida contra la base en cada request, así que un usuario suspendido queda afuera aunque su token no haya vencido. Un admin no puede suspenderse a sí mismo.
-Cada customer solo ve y toca lo suyo.

Cómo está creado?
Esta creado bajo el enfoque de diseño de software "Clean Architecture", junto a el stack: .NET 10, EF Core, SQL Server, JWT, BCrypt, HttpClient.

Deploy y Azure
La API corre en Azure, dentro de un único grupo de recursos (rg-sanchez-chanchito).
App Service (plan F1)
Azure SQL Database + servidor SQL

CI/CD
GitHub Actions (.github/workflows/deploy.yml):
En cada PR a main: restaura, compila y publica.
Solo en push a main: instala dotnet-ef, aplica las migraciones a la base de Azure (con reintentos) y despliega al App Service.

Diagrama de clases UML

<img width="1432" height="2894" alt="image" src="https://github.com/user-attachments/assets/07e1ede0-0cc6-425d-8ce2-b80e67eec8db" />


