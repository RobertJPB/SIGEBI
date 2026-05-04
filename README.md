# SIGEBI - Sistema de Gestión Bibliotecaria

Sistema integral para la administración de recursos bibliográficos, gestión de préstamos y control de usuarios. El sistema está diseñado bajo una arquitectura modular que separa las responsabilidades de consulta pública de las operaciones administrativas avanzadas.

## Arquitectura del Sistema

El proyecto implementa una **Arquitectura Limpia (Clean Architecture)** estructurada en las siguientes capas:

*   **SIGEBI.Domain**: Contiene las entidades centrales (Recurso, Usuario, Préstamo, Auditoría), interfaces base y reglas de negocio puras.
*   **SIGEBI.Business**: Capa de aplicación que gestiona la lógica de los casos de uso, mapeo de datos y contratos de servicios.
*   **SIGEBI.Infrastructure**: Implementación de persistencia mediante Entity Framework Core, configuración de repositorios y servicios externos. Incluye interceptores para la auditoría automática de cambios.
*   **SIGEBI.API**: Punto de entrada de servicios backend.
*   **SIGEBI.Desktop**: Interfaz administrativa desarrollada en WPF (Windows Presentation Foundation) con patrón MVVM.
*   **SIGEBI.Web**: Interfaz de consulta y catálogo para usuarios finales.

## Funcionalidades del Sistema

### Gestión para Usuarios Finales
El portal de usuario permite la consulta dinámica del catálogo, visualización de disponibilidad en tiempo real y seguimiento de préstamos personales.

![Catálogo de Recursos](assets/img3.png)
![Visualización del Catálogo](assets/img4.png)

Los usuarios pueden acceder a detalles específicos de cada recurso, incluyendo metadatos técnicos y valoraciones de otros lectores.

![Detalle de Recurso](assets/img5.png)
![Valoraciones y Comentarios](assets/img6.png)

Cada usuario dispone de un perfil centralizado para gestionar sus credenciales y consultar su historial de préstamos activo y devuelto.

![Perfil de Usuario](assets/img8.png)
![Historial de Préstamos](assets/img7.png)

---

### Panel Administrativo
El panel de control proporciona herramientas avanzadas para la gestión del inventario bibliográfico, permitiendo la categorización y registro detallado de libros, revistas y documentos.

![Gestión Bibliográfica](assets/img9.png)
![Registro de Recursos](assets/img10.png)

### Control de Préstamos y Usuarios
El sistema automatiza el flujo de préstamos, devoluciones y la gestión de cuentas de usuario, incluyendo el control de roles y estados de cuenta.

![Gestión de Préstamos](assets/img11.png)
![Registro de Préstamo](assets/img12.png)
![Administración de Usuarios](assets/img13.png)
![Registro de Nuevo Usuario](assets/img15.png)

### Sistema de Sanciones y Notificaciones
Incluye un módulo de penalizaciones para gestionar infracciones por retrasos o mal comportamiento, junto con un sistema de notificaciones para eventos críticos del sistema.

![Gestión de Penalizaciones](assets/img16.png)
![Aplicación de Sanción](assets/img17.png)
![Bandeja de Notificaciones](assets/img14.png)

### Auditoría y Analítica
Para garantizar la integridad de los datos, el sistema registra cada operación mediante un log de auditoría detallado. Además, ofrece un generador de reportes personalizable con exportación a formatos externos.

![Auditoría de Actividad](assets/img18.png)
![Reportes y Estadísticas](assets/img19.png)

## Seguridad y Acceso
El acceso está restringido mediante un sistema de autenticación que distingue entre el portal público y el acceso administrativo.

![Acceso General](assets/img1.png)
![Acceso Administrativo](assets/img2.png)
