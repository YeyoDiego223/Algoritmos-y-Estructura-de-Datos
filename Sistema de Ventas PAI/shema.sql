PRAGMA foreign_keys = ON;

-- Tabla Clientes
CREATE TABLE IF NOT EXISTS Clientes (
    ID_Cliente INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre TEXT NOT NULL,
    Apellido TEXT NOT NULL,
    Email TEXT,
    Telefono TEXT,
    Direccion TEXT
);

-- Tabla Proveedores
CREATE TABLE IF NOT EXISTS Proveedores (
    ID_Proveedor INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre_Proveedor TEXT NOT NULL,
    Contacto TEXT,
    Telefono TEXT,
    Email TEXT
);

-- Tabla Productos
CREATE TABLE IF NOT EXISTS Productos (
    ID_Producto INTEGER PRIMARY KEY AUTOINCREMENT,
    Nombre_Producto TEXT NOT NULL,
    Descripcion TEXT,
    Precio REAL NOT NULL,
    Stock INTEGER NOT NULL,
    Categoria TEXT
);

-- Tabla Compras
CREATE TABLE IF NOT EXISTS Compras (
    ID_Compra INTEGER PRIMARY KEY AUTOINCREMENT,
    ID_Proveedor INTEGER,
    Fecha_Compra TEXT NOT NULL, -- SQLite usa TEXT para fechas (formato ISO: YYYY-MM-DD)
    Total_Compra REAL NOT NULL,
    FOREIGN KEY (ID_Proveedor) REFERENCES Proveedores(ID_Proveedor)
);

-- Tabla Detalles_Compra
CREATE TABLE IF NOT EXISTS Detalles_Compra (
    ID_Detalle_Compra INTEGER PRIMARY KEY AUTOINCREMENT,
    ID_Compra INTEGER,
    ID_Producto INTEGER,
    Cantidad INTEGER NOT NULL,
    Precio_Compra REAL NOT NULL,
    FOREIGN KEY (ID_Compra) REFERENCES Compras(ID_Compra),
    FOREIGN KEY (ID_Producto) REFERENCES Productos(ID_Producto)
);

-- Tabla Ventas
CREATE TABLE IF NOT EXISTS Ventas (
    ID_Venta INTEGER PRIMARY KEY AUTOINCREMENT,
    Fecha_Venta TEXT NOT NULL,
    ID_Cliente INTEGER,
    Total_Venta REAL NOT NULL,
    FOREIGN KEY (ID_Cliente) REFERENCES Clientes(ID_Cliente)
);

-- Tabla Detalles_Venta
CREATE TABLE IF NOT EXISTS Detalles_Venta (
    ID_Detalle INTEGER PRIMARY KEY AUTOINCREMENT,
    ID_Venta INTEGER,
    ID_Producto INTEGER,
    Cantidad INTEGER NOT NULL,
    Precio REAL NOT NULL,
    FOREIGN KEY (ID_Venta) REFERENCES Ventas(ID_Venta),
    FOREIGN KEY (ID_Producto) REFERENCES Productos(ID_Producto)
);

-- Tabla Usuarios (con usuario inicial)
CREATE TABLE IF NOT EXISTS Usuarios (
    Cuenta INTEGER PRIMARY KEY AUTOINCREMENT,
    Cargo TEXT,
    Usuario TEXT UNIQUE,
    Contraseña TEXT
);

-- Insertar usuario admin si no existe
INSERT INTO Usuarios (Cargo, Usuario, Contraseña)
SELECT 'Administrador', 'admin', '12345'
WHERE NOT EXISTS (SELECT 1 FROM Usuarios WHERE Usuario = 'admin');