# 📘 PSP Console – Guía de uso

Este proyecto es una **aplicación de consola en C#** que implementa el **PSP (Proceso Personal de Software)** con las fases:  
1. **Planificación**  
2. **Desarrollo**  
3. **Post-Mortem**  

Permite registrar estimaciones de líneas de código (LOC), medir métricas reales y exportar reportes en **CSV**.

---

## 🚀 Requisitos previos
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download) instalado  
- Sistema operativo: Windows, macOS o Linux  
- Editor recomendado: Visual Studio Code  

---

## 📂 Estructura del proyecto

```
PSP/
├─ Program.cs
├─ Common/
├─ Domain/
├─ Services/
├─ Persistence/
├─ Presentation/
└─ data.json   ← aquí se guardan las tareas y métricas
```

---

## ▶️ Ejecución

1. Abrir terminal en la carpeta del proyecto:

```bash
cd PSP
dotnet run
```

2. Se mostrará el menú:

```
=== PSP Console ===
1) Planificación (crear nueva tarea)
2) Desarrollo (registrar métricas reales)
3) Post-Mortem (comparar y cerrar tarea)
4) Ver historial
5) Exportar CSV
0) Salir
```

---

## 📝 Flujo de uso

### 1. Planificación
Registrar una nueva tarea con nombre, estimación de LOC y referencias.  
Ejemplo:
```
Nombre: Calculadora básica
Estimación: 120
Referencias: Proyecto clase; Ejemplo libro PSP
```

### 2. Desarrollo
Registrar métricas reales:  
- Opción **1** → Manual (tú escribes LOCs, LOcm y LOccs).  
- Opción **2** → Automática: el programa cuenta líneas en una carpeta con archivos `.cs`.  

Ejemplo:
```
Ruta de carpeta: Archivos
```

El sistema calculará automáticamente:
- LOCs (líneas de código fuente)
- LOccs (líneas comentadas)
- Blancas (no cuentan)

Después pedirá **LOcm** (líneas modificadas), que se ingresa manualmente.

### 3. Post-Mortem
Cierra la tarea y genera un análisis comparando **estimado vs real**.  
Muestra:
- Error absoluto
- Error porcentual
- Observación sobre la calidad de la estimación

### 4. Ver historial
Lista todas las tareas registradas, indicando si están abiertas o cerradas.

### 5. Exportar CSV
Genera un archivo con todas las métricas registradas.  
El archivo se guarda en la **carpeta raíz del proyecto**:

```
PSP/psp_export.csv
```

Puedes abrirlo con **Excel, Google Sheets o Numbers**.

---

## 📊 Ejemplo de salida Post-Mortem

```
=== Resumen Post-Mortem ===
Tarea     : Calculadora básica
Estimado  : 120 LOC
Real LOCs : 85 | LOcm: 0 | LOccs: 15
Error abs : 35 LOC
Error %   : 29.17%
Observación: Buena, pero puede afinarse.
```

---

## 📑 Ejemplo del CSV exportado (`psp_export.csv`)

```csv
Id,Name,EstimatedLOC,RealLOCs,LOcm,LOccs,CreatedAt,ClosedAt,Refs
A1B2C3D4,"Calculadora básica",120,85,0,15,2025-09-08T22:15:00.0000000Z,2025-09-08T22:45:00.0000000Z,"Proyecto clase | Ejemplo libro PSP"
```

---

## 📌 Notas
- El archivo `data.json` guarda el historial interno del sistema.  
- Los reportes se generan siempre en la raíz del proyecto (`psp_export.csv`).  
- LOcm (líneas modificadas) debe ingresarse manualmente en esta versión.  
