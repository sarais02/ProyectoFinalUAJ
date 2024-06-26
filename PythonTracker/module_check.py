import importlib.util
import sys

modulos = ["json", "pandas", "numpy", "matplotlib"]

modulos_faltan = []

for modulo in modulos:
    if importlib.util.find_spec(modulo) is None:
        modulos_faltan.append(modulo)

if modulos_faltan:
    print("Faltan los siguientes módulos:")
    for modulo in modulos_faltan:
        print(f"- {modulo}")
    print("\nInstálalos ejecutando este comando:")
    print(f"pip install {' '.join(modulos_faltan)}")
    sys.exit(1)
else:
    sys.exit(0)