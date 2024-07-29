API RESTful utilizando .NET 8 que actúe como proxy hacia la API de Frankfurter para manejar tasas de cambio de divisas. Utilizando los conocimientos adquiridos en el curso, se deben obtener tasas de cambio desde la API de Frankfurter, almacenarlas en una base de datos SQL Server utilizando Entity Framework, y exponer datos derivados adicionales. Se deben implementar operaciones CRUD completas para gestionar estos datos y proporcionar endpoints que transformen y analicen la información obtenida.

Requerimientos del Proyecto:

Tabla de Monedas (Currency):
•	Columns:
o	Id (int, Primary Key)
o	Symbol (string)
o	Name (string)

Tabla de Tasas de Cambio (ExchangeRates):
•	Columns:
o	Id (int, Primary Key)
o	BaseCurrency (string, Foreign Key to Currency)
o	TargetCurrency (string, Foreign Key to Currency)
o	Rate (decimal)
o	Date (DateTime)

Creación de la Solución:
Crear una solución en .NET 8 que incluya un proyecto de Web API utilizando Minimal APIs o Controllers (a preferencia de ustedes).

Consulta a la API de Frankfurter:
Implementar endpoints que permitan obtener tasas de cambio desde la API de Frankfurter y almacenarlas en la base de datos.
Los endpoints deben ser capaces de manejar diferentes tipos de consultas (tasas de cambio más recientes, históricas, series temporales y conversiones).

Endpoints CRUD:
Implementar los siguientes endpoints para gestionar las tasas de cambio almacenadas:
Ejemplo:
o	GET /rates: Devuelve la lista de tasas de cambio almacenadas.
o	GET /rates/{id}: Devuelve una tasa de cambio específica por Id.
o	POST /rates: Crea nuevas tasas de cambio.
o	PUT /rates/{id}: Actualiza una tasa de cambio existente por Id.
o	DELETE /rates/{id}: Elimina una tasa de cambio por Id.
o	GET /rates/currency/{baseCurrency}: Devuelve las tasas de cambio por moneda base.
o	PUT /rates/currency/{baseCurrency}: Actualiza las tasas de cambio por moneda base (requiere pasar un body con los datos a actualizar).
o	DELETE /rates/currency/{baseCurrency}: Elimina las tasas de cambio por moneda base.

Transformación de Datos:
Implementar endpoints que realicen cálculos sobre los datos almacenados:
o	GET /rates/average?base={baseCurrency}&target={targetCurrency}&start={startDate}&end={endDate}: Devuelve el valor promedio de la tasa de cambio entre las fechas especificadas.
o	GET /rates/minmax?base={baseCurrency}&target={targetCurrency}&start={startDate}&end={endDate}: Devuelve los valores mínimo y máximo de la tasa de cambio entre las fechas especificadas.

Autenticación:
Implementar un sistema de autenticación utilizando Json Web Tokens (JWT) para proteger los endpoints CRUD.
Caché:
Configurar caché en memoria para mejorar el rendimiento de las solicitudes GET.
Subida a GitHub:
Subir el código fuente a un repositorio de GitHub para su revisión.

FRANKFURTER:
Documentación: https://www.frankfurter.app/docs/
API Host: https://api.frankfurter.app
