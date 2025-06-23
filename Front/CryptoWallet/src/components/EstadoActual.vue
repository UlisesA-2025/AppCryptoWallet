<template>
  <div>
    <h2>Estado Actual</h2>
    <ul>
      <li v-for="e in estado.Estado" :key="e.Criptomoneda">
        {{ e.Criptomoneda }}: {{ e.Cantidad }} ≈ ${{ e.ValorEnARS.toFixed(2) }}
      </li>
    </ul>
    <h3>Total: ${{ estado.Total }}</h3>

    <DoughnutChart v-if="estado.Estado.length > 0" :data="chartData" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { Chart as ChartJS, DoughnutController, ArcElement, Tooltip, Legend } from 'chart.js';
import { Doughnut } from 'vue-chartjs';

ChartJS.register(DoughnutController, ArcElement, Tooltip, Legend);

const estado = ref({ Estado: [], Total: 0 });

const chartData = ref({
    labels: [],
    datasets: [{ data: [], backgroundColor: ['#f39c12', '#3498db', '#2ecc71'] }]
});

const cargarEstado = async () => {
    const res = await fetch('http://localhost:7028/api/transacciones/estado');
    const data = await res.json();
    estado.value = data;

    chartData.value.labels = estado.value.Estado.map(e => e.Criptomoneda);
    chartData.value.datasets[0].data = estado.value.Estado.map(e => e.ValorEnARS);
};

const DoughnutChart = Doughnut;

onMounted(cargarEstado);
</script>
