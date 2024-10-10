import numpy as np
import matplotlib.pyplot as plt
from scipy import constants

# Constantes
g = constants.g  # Aceleración debido a la gravedad
eta = 1.81e-5  # Viscosidad del aire
rho_oil = 920  # Densidad del aceite (kg/m^3)
rho_air = 1.2  # Densidad del aire (kg/m^3)
V = 5000  # Voltaje aplicado (V)
d = 0.01  # Distancia entre placas (m)

def simulate_millikan(num_drops=1000):
    # Simulación de gotas con radios aleatorios
    radii = np.random.uniform(0.1e-6, 1e-6, num_drops)
    
    # Cálculo de la carga de cada gota
    charges = []
    for r in radii:
        m = (4/3) * np.pi * r**3 * rho_oil
        v = (2 * r**2 * g * (rho_oil - rho_air)) / (9 * eta)
        q = (m * g * d) / V
        charges.append(q)
    
    return charges

# Realizar la simulación
simulated_charges = simulate_millikan()

# Calcular el valor medio de la carga
mean_charge = np.mean(simulated_charges)
print(f"Carga media simulada: {mean_charge:.3e} C")
print(f"Carga del electrón (valor real): {constants.e:.3e} C")

# Crear un histograma de las cargas
plt.figure(figsize=(10, 6))
plt.hist(simulated_charges, bins=50, edgecolor='black')
plt.title('Distribución de cargas en el experimento de Millikan')
plt.xlabel('Carga (C)')
plt.ylabel('Frecuencia')
plt.grid(True)
plt.show()

# Calcular y mostrar múltiplos de la carga del electrón
electron_charge = constants.e
multiples = np.round(np.array(simulated_charges) / electron_charge)
unique_multiples, counts = np.unique(multiples, return_counts=True)

plt.figure(figsize=(10, 6))
plt.bar(unique_multiples, counts)
plt.title('Múltiplos de la carga del electrón')
plt.xlabel('Múltiplo de e')
plt.ylabel('Frecuencia')
plt.grid(True)
plt.show()