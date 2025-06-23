<template>
  <form @submit.prevent="guardarTransaccion" class="formulario">
    <h2>Registrar Compra/Venta</h2>

    <label for="exchange">Exchange</label>
    <select id="exchange" v-model="form.exchange">
      <option value="binance">Binance</option>
      <option value="ripio">Ripio</option>
      <option value="lemon">Lemon</option>
    </select>

    <label for="crypto_code">Criptomoneda</label>
    <select id="crypto_code" v-model="form.crypto_code">
      <option value="btc">Bitcoin</option>
      <option value="eth">Ethereum</option>
      <option value="usdc">USDC</option>
    </select>

    <label for="tipo">Tipo de operación</label>
    <select id="tipo" v-model="form.tipo">
      <option value="purchase">Compra</option>
      <option value="sale">Venta</option>
    </select>

    <label for="crypto_amount">Cantidad</label>
    <input id="crypto_amount" type="number" v-model="form.crypto_amount" placeholder="Cantidad" step="0.0001" />

    <label for="datetime">Fecha y hora</label>
    <input id="datetime" type="datetime-local" v-model="form.datetime" />

    <label for="precio_ars">Precio en ARS</label>
    <input id="precio_ars" type="text" :value="precioEnARS" readonly placeholder="Precio en ARS" />

    <button type="submit">Guardar</button>

    <p v-if="mensaje">{{ mensaje }}</p>
  </form>
</template>

<script setup>
import { ref, watch, onMounted, computed } from 'vue';
import axios from 'axios';

const form = ref({
  crypto_code: 'btc',
  tipo: 'purchase',
  exchange: 'binance',
  crypto_amount: 0.01,
  datetime: new Date().toISOString().slice(0, 16)
});

const mensaje = ref('');
const precioActual = ref(0);


const obtenerPrecio = async () => {
  try {
    const res = await fetch(`https://criptoya.com/api/${form.value.exchange}/${form.value.crypto_code}/ars`);
    const data = await res.json();

    
    precioActual.value =
      form.value.tipo === 'purchase'
        ? data.totalAsk ?? data.ask ?? 0
        : data.totalBid ?? data.bid ?? 0;
  } catch (e) {
    precioActual.value = 0;
  }
};


watch(
  () => [form.value.crypto_code, form.value.exchange, form.value.tipo],
  obtenerPrecio,
  { immediate: true }
);

onMounted(obtenerPrecio);


const precioEnARS = computed(() =>
  (form.value.crypto_amount * precioActual.value).toLocaleString('es-AR', {
    style: 'currency',
    currency: 'ARS'
  })
);


const guardarTransaccion = async () => {
    try {
        const res = await fetch('http://localhost:7028/api/Transacciones', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                crypto_code: form.value.crypto_code,
                tipo: form.value.tipo,
                exchangeNombre: form.value.exchange,
                cantCrypto: form.value.crypto_amount,
                fecha: form.value.datetime
            })
        });

        if (res.ok) {
            mensaje.value = 'Transacción guardada con éxito ✅';
        } else {
            const errorData = await res.json();
            mensaje.value = 'Error al guardar: ' + (errorData.message || res.statusText);
        }
    } catch (e) {
        mensaje.value = 'Error: ' + e.message;
    }
};
</script>



<style scoped>
.formulario {
  max-width: 500px;
  margin: auto;
  display: flex;
  flex-direction: column;
  gap: 10px;
}
</style>
