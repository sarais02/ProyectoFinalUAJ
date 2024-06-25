@echo off

REM Avisos
echo Asegúrate de vaciar la carpeta DataTracker al cambiar de mapa.
echo .
echo Python debe estar instalado en el PATH del sistema. Ten cuidado con accesos directos y alias de Windows.
echo Debe incluir los módulos json, pandas, numpy y matplotlib.
echo .
timeout /t 3 /nobreak >nul

REM ----------------------------------------------------------------------------------

REM Parece que escribir "python" hace lo mismo pero así podemos hacer nuestra propia comprobación
for /f "delims=" %%p in ('where python') do (
    SET PYTHONPATH=%%p
)

REM Verificar si se encontró una ruta válida
if "%PYTHONPATH%"=="" (
    echo No se encontró una instalación de Python en el PATH del sistema.
    pause
    exit /b 1
)

echo Se encontró Python en: %PYTHONPATH%

REM ----------------------------------------------------------------------------------

REM Comprobar módulos instalados
%PYTHONPATH% "%~dp0\PythonTracker\module_check.py"
if errorlevel 1 (
    echo Intenta de nuevo tras instalar los módulos necesarios.
    pause
    exit /b 1
)

REM ----------------------------------------------------------------------------------

REM Correr proyecto Unity
start /wait "" "%~dp0\Ejecutable\ProyectoFinalUAJ_G5.exe"

REM ----------------------------------------------------------------------------------

REM ¿Comprobar que existe DataTracker para copiarla?
if not exist "%~dp0\DataTracker\" (
    echo La carpeta DataTracker con los resultados de las pruebas no existe.
    pause
    exit /b 1
)

REM copiar DataTracker a PythonTracker
REM Crear DataTracker de destino si no existe
mkdir "%~dp0\PythonTracker\DataTracker" 2>nul
robocopy "%~dp0\DataTracker" "%~dp0\PythonTracker\DataTracker" /E /NFL /NDL /NJH /NJS /nc /ns /np >nul
set "copyErrorLevel=%errorlevel%"

REM Comprobar errores de copia
if %copyerrorlevel% geq 8 (
    echo La carpeta DataTracker no se ha podido copiar correctamente.
    pause
    exit /b 1
)

REM ----------------------------------------------------------------------------------

REM Correr script análisis
%PYTHONPATH% "%~dp0\PythonTracker\analyze.py"

exit