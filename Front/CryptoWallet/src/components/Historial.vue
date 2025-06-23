<template>
  <div>
    <h2>Historial de Transacciones</h2>
    <table>
      <thead>
        <tr>
          <th>Cripto</th>
          <th>Acción</th>
          <th>Cantidad</th>
          <th>ARS</th>
          <th>Fecha</th>
          <th>Acciones</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="t in transacciones" :key="t.id">
          <td>{{ t.criptomonedaNombre }}</td>
          <td>{{ t.tipo }}</td>
          <td>{{ t.cantCrypto }}</td>
          <td>${{ t.cantARS.toFixed(2) }}</td>
          <td>{{ new Date(t.fecha).toLocaleString() }}</td>
          <td>
            <button @click="$router.push('/editar/' + t.id)">Editar</button>
            <button @click="borrar(t.id)">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue';

const transacciones = ref([]);

const cargarTransacciones = async () => {
    const res = await fetch('http://localhost:7028/api/transacciones');
    transacciones.value = await res.json();
};

const borrar = async (id) => {
    if (confirm('¿Estás seguro de eliminar esta transacción?')) {
        await fetch(`http://localhost:7028/api/transacciones/${id}`, {
            method: 'DELETE'
        });
        cargarTransacciones();
    }
};

onMounted(cargarTransacciones);
</script>
