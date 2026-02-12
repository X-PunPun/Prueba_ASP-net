# 💱 API de Gestión de Divisas (Frankfurter Proxy)

Este proyecto es una **API RESTful desarrollada en .NET 8** que actúa como un proxy e integrador de la API pública de Frankfurter. Permite sincronizar tasas de cambio históricas, almacenarlas en una base de datos **SQL Server** utilizando **Entity Framework Core**, y exponer endpoints para operaciones CRUD, cálculos estadísticos y análisis de datos.

## 📋 Tabla de Contenidos
- [Características del Proyecto](#-características-del-proyecto)
- [Tecnologías Utilizadas](#-tecnologías-utilizadas)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Documentación de la API](#-documentación-de-la-api)
- [Colección de Postman](#-colección-de-postman)

---

## 🚀 Características del Proyecto

Este proyecto cumple con el 100% de los requerimientos solicitados en la evaluación:

- **Arquitectura N-Capas:** Separación de responsabilidades (Controllers, Services, Data, Entity, DTOs).
- **Base de Datos Relacional:**
  - Tabla `Currency` (Monedas).
  - Tabla `ExchangeRates` (Tasas con relación FK).
- **Integración Externa:** Consumo de la API de Frankfurter para obtener datos en tiempo real y series temporales (históricos).
- **Operaciones CRUD Completas:** Gestión total de las tasas almacenadas.
- **Lógica de Negocio Avanzada:**
  - Cálculo de Promedios en rangos de fechas.
  - Determinación de valores Mínimos y Máximos.
  - Actualización y borrado masivo por moneda base.
- **Seguridad:** Implementación de autenticación mediante **JWT (JSON Web Tokens)**.

---

## 🛠 Tecnologías Utilizadas

- **Framework:** .NET 8 (ASP.NET Core Web API)
- **Lenguaje:** C#
- **ORM:** Entity Framework Core
- **Base de Datos:** SQL Server
- **Documentación:** Swagger / OpenAPI
- **Mapeo:** AutoMapper
- **Autenticación:** JWT Bearer

---

## ⚙️ Requisitos Previos

Asegúrate de tener instalado:
1.  [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2.  [SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) (Express o Developer)
3.  [Postman](https://www.postman.com/) (Para pruebas)

---

## 🔧 Instalación y Configuración

### 1. Clonar el Repositorio
```bash
git clone [https://github.com/TU_USUARIO/TU_REPOSITORIO.git](https://github.com/TU_USUARIO/TU_REPOSITORIO.git)
cd TU_REPOSITORIO
