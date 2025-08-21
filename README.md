# Sistema de Punto de Venta en C#/.NET (Windows Forms)

Una aplicación de escritorio completa para la gestión de un punto de venta, desarrollada con C# y el framework de Windows Forms. El sistema permite administrar inventario, usuarios, clientes, proveedores, compras y ventas, almacenando toda la información en una base de datos local SQLite.

## 📜 Características Principales
* **Gestión de Usuarios:** Creación, edición y eliminación de usuarios con diferentes roles (Gerente, Administrador, Cajero).
* **Módulo de Clientes y Proveedores:** Administración completa (CRUD) de clientes y proveedores.
* **Control de Inventario:** Gestión de productos, incluyendo stock, precios y categorías.
* **Procesamiento de Transacciones:** Registro detallado de ventas y compras.
* **Estadísticas:** Módulo de visualización de estadísticas de ventas.

## 🛠️ Tecnologías Utilizadas
* **Lenguaje:** C#
* **Framework:** .NET / Windows Forms
* **Base de Datos:** SQLite
* **Acceso a Datos:** Microsoft.Data.Sqlite

## 🚀 Cómo Ejecutar el Proyecto

### Prerrequisitos
* **Visual Studio 2019** o superior.
* Tener instalada la carga de trabajo **".NET desktop development"** en Visual Studio.

### Pasos de Instalación y Ejecución

1.  **Clonar el repositorio:**
    ```bash
    git clone https://github.com/YeyoDiego223/Algoritmos-y-Estructura-de-Datos
    ```

2.  **Abrir en Visual Studio:**
    * Abre el archivo de la solución (`.sln`) con Visual Studio.

3.  **Configurar el Script de la Base de Datos (Paso Crucial):**
    * En el "Explorador de Soluciones" de Visual Studio, busca tu archivo `schema.sql`.
    * Haz clic derecho sobre él y ve a **Propiedades (Properties)**.
    * Asegúrate de que la opción **"Copiar en el directorio de salida" (Copy to Output Directory)** esté establecida en **"Copiar si es más reciente" (Copy if newer)**.
    * *Este paso es fundamental para que la aplicación pueda encontrar el archivo y crear la base de datos la primera vez que se ejecuta.*
    

4.  **Restaurar Paquetes y Construir:**
    * El proyecto debería restaurar los paquetes de NuGet automáticamente. Si no, haz clic derecho en la solución en el "Explorador de Soluciones" y selecciona **"Restaurar paquetes NuGet" (Restore NuGet Packages)**.
    * Construye la solución desde el menú **"Compilar" (Build) -> "Compilar solución" (Build Solution)**.

5.  **Ejecutar la Aplicación:**
    * Presiona el botón **"Iniciar" (Start)** (el que tiene el ícono de Play ▶️) o presiona la tecla `F5`.
    * La aplicación se iniciará y creará el archivo de base de datos `BDTIENDA.db` en su primera ejecución. El usuario por defecto es `Gerente` con la contraseña `Gerente`.


## Captura de Pantalla
<img width="864" height="744" alt="image" src="https://github.com/user-attachments/assets/0ed604c4-22e2-4d9e-9701-29532db51300" />
<img width="1155" height="890" alt="image" src="https://github.com/user-attachments/assets/8660edf3-42e0-4de0-bb63-89711287764b" />
<img width="1190" height="692" alt="image" src="https://github.com/user-attachments/assets/ad362d0d-62fa-4c31-8c62-46ce3410af43" />
<img width="482" height="541" alt="image" src="https://github.com/user-attachments/assets/d477c25f-5751-489b-aa0a-fb1dbe51a6b8" />
<img width="1225" height="716" alt="image" src="https://github.com/user-attachments/assets/f959d5e4-778f-410b-8733-d854e510995b" />
<img width="1230" height="1013" alt="image" src="https://github.com/user-attachments/assets/d85fab7e-330d-47f5-a5cf-be0dfbcb74d9" />
<img width="1231" height="1003" alt="image" src="https://github.com/user-attachments/assets/7346a5cb-c952-4461-9be6-1f0e4af9fde9" />
<img width="1278" height="1014" alt="image" src="https://github.com/user-attachments/assets/0fb09e24-de33-4446-9a9e-ee2876951044" />
<img width="1283" height="1005" alt="image" src="https://github.com/user-attachments/assets/7897ea2f-63d6-4590-9f26-f7a92c815367" />
<img width="1306" height="1006" alt="image" src="https://github.com/user-attachments/assets/150f410b-9649-4382-b544-f58a704abb79" />
<img width="1289" height="1002" alt="image" src="https://github.com/user-attachments/assets/818b03b1-180f-4b9d-8e79-aa5d6c485e74" />
<img width="1314" height="1005" alt="image" src="https://github.com/user-attachments/assets/1e99c11c-0b7c-4ead-bdec-0dc05196831e" />
