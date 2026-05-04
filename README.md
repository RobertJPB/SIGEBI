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

<img width="1600" height="738" alt="image" src="https://github.com/user-attachments/assets/6dc2aa4a-6157-4cb4-a467-14f6711fce21" />
<img width="1600" height="736" alt="image" src="https://github.com/user-attachments/assets/506d45af-46eb-48c7-9554-34eed296ff42" />

Los usuarios pueden acceder a detalles específicos de cada recurso, incluyendo metadatos técnicos y valoraciones de otros lectores.

<img width="1600" height="752" alt="image" src="https://github.com/user-attachments/assets/97a77651-91e3-4ca6-9803-f4b9d79f8145" />
<img width="1600" height="736" alt="image" src="https://github.com/user-attachments/assets/90acacc5-6e6f-4440-8e98-8f68fc9556bb" />

Cada usuario dispone de un perfil centralizado para gestionar sus credenciales y consultar su historial de préstamos activo y devuelto.

<img width="1600" height="741" alt="image" src="https://github.com/user-attachments/assets/80fc2a32-950a-43d7-97af-5a99cde9a561" />
<img width="1600" height="742" alt="image" src="https://github.com/user-attachments/assets/521a0494-0f98-4bbb-a93e-ca7c1a2d82d0" />

---

### Panel Administrativo
El panel de control proporciona herramientas avanzadas para la gestión del inventario bibliográfico, permitiendo la categorización y registro detallado de libros, revistas y documentos.

<img width="1600" height="903" alt="image" src="https://github.com/user-attachments/assets/61f6497b-d41c-4266-8e40-80b22422411d" />


### Control de Préstamos y Usuarios
El sistema automatiza el flujo de préstamos, devoluciones y la gestión de cuentas de usuario, incluyendo el control de roles y estados de cuenta.

<img width="1600" height="895" alt="image" src="https://github.com/user-attachments/assets/08b14089-1af3-464e-845a-e36c1027d411" />
<img width="1600" height="907" alt="image" src="https://github.com/user-attachments/assets/a6832204-9e18-4548-8931-151d43f60b3c" />


### Sistema de Sanciones y Notificaciones
Incluye un módulo de penalizaciones para gestionar infracciones por retrasos o mal comportamiento, junto con un sistema de notificaciones para eventos críticos del sistema.

<img width="1600" height="911" alt="image" src="https://github.com/user-attachments/assets/b5dda9ec-6e2a-4586-ac7a-86fb878ccb26" />
<img width="1600" height="895" alt="image" src="https://github.com/user-attachments/assets/61c48846-8672-4c01-ab8f-6a7a7eb245f6" />

### Auditoría y Analítica
Para garantizar la integridad de los datos, el sistema registra cada operación mediante un log de auditoría detallado. Además, ofrece un generador de reportes personalizable con exportación a formatos externos como Excel.

<img width="1600" height="896" alt="image" src="https://github.com/user-attachments/assets/2f849800-bd53-4d40-bf4f-0864b8577bf1" />
<img width="1600" height="893" alt="image" src="https://github.com/user-attachments/assets/913a1c65-b679-48a0-8a10-739635884ba3" />


## Seguridad y Acceso
El acceso está restringido mediante un sistema de autenticación que distingue entre el portal público y el acceso administrativo.

<img width="1600" height="733" alt="image" src="https://github.com/user-attachments/assets/eb1ea5e3-1d80-439e-80a8-787c04c088be" />
<img width="722" height="863" alt="image" src="https://github.com/user-attachments/assets/e7f3a8f4-3cdf-4b26-a3d9-4df104f07efc" />

