<template>
  <div>
    <h2>Editar Transacción</h2>
    <form @submit.prevent="editar">
      <input type="number" v-model="form.cantCrypto" step="0.0001" />
      <input type="datetime-local" v-model="form.fecha" />
      <select v-model="form.tipo">
        <option value="purchase">Compra</option>
        <option value="sale">Venta</option>
      </select>
      <button type="submit">Guardar cambios</button>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();
const id = route.params.id;

const form = ref({
    cantCrypto: 0,
    fecha: '',
    tipo: 'purchase'
});

onMounted(async () => {
    const res = await fetch(`http://localhost:7028/api/transacciones/${id}`);
    const data = await res.json();
    form.value.cantCrypto = data.cantCrypto;
    form.value.fecha = data.fecha.slice(0, 16);
    form.value.tipo = data.tipo;
});

const editar = async () => {
    await fetch(`http://localhost:7028/api/transacciones/${id}`, {
        method: 'PATCH',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            cantCrypto: form.value.cantCrypto,
            fecha: form.value.fecha,
            tipo: form.value.tipo,
            crypto_code: 'btc' 
        })
    });
    router.push('/');
};
</script>
