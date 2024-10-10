import numpy as np
import matplotlib.pyplot as plt
from scipy import constants

# Constantes
h = constants.h  # Constante de Planck
c = constants.c  # Velocidad de la luz
e = constants.e  # Carga del electrón
m_e = constants.m_e  # Masa del electrón
epsilon_0 = constants.epsilon_0  # Permitividad del vacío

def energia_nivel(n):
    return -13.6 * (1 / n**2)  # en eV

def longitud_onda_transicion(n1, n2):
    E1 = energia_nivel(n1)
    E2 = energia_nivel(n2)
    delta_E = E2 - E1  # La transición es de n2 a n1
    return (h * c) / (abs(delta_E) * e) * 1e9  # Convertir a nm

# Calcular y graficar niveles de energía
n_levels = 5
n = np.arange(1, n_levels + 1)
energias = [energia_nivel(i) for i in n]

plt.figure(figsize=(10, 6))
plt.bar(n, energias, align='center', alpha=0.8)
plt.title('Niveles de energía en el átomo de hidrógeno')
plt.xlabel('Número cuántico principal (n)')
plt.ylabel('Energía (eV)')
plt.grid(True)
plt.show()

# Calcular y graficar longitudes de onda de las transiciones
transiciones = []
longitudes_onda = []

for n2 in range(2, n_levels + 1):
    for n1 in range(1, n2):
        transiciones.append(f'{n2}->{n1}')
        longitudes_onda.append(longitud_onda_transicion(n1, n2))

plt.figure(figsize=(12, 6))
plt.bar(transiciones, longitudes_onda, align='center', alpha=0.8)
plt.title('Longitudes de onda de las transiciones en el átomo de hidrógeno')
plt.xlabel('Transición')
plt.ylabel('Longitud de onda (nm)')
plt.xticks(rotation=45)
plt.grid(True)
plt.tight_layout()
plt.show()

# Imprimir resultados
print("Niveles de energía (eV):")
for i, e in zip(n, energias):
    print(f"n = {i}: {e:.2f} eV")

print("\nLongitudes de onda de las transiciones (nm):")
for t, l in zip(transiciones, longitudes_onda):
    print(f"Transición {t}: {l:.2f} nm")

# Calcular y graficar el espectro de emisión
plt.figure(figsize=(12, 6))
for t, l in zip(transiciones, longitudes_onda):
    plt.axvline(x=l, color='r', alpha=0.5, linewidth=2)
plt.title('Espectro de emisión del átomo de hidrógeno')
plt.xlabel('Longitud de onda (nm)')
plt.ylabel('Intensidad relativa (unidades arbitrarias)')
plt.xlim(0, 700)
plt.grid(True)
plt.show()