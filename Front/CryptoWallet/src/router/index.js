import { createRouter, createWebHistory } from 'vue-router';
import AltaTransaccion from '../components/AltaTransaccion.vue';
import Historial from '../components/Historial.vue';
import Editar from '../components/EditarTransaccion.vue';
import Estado from '../components/EstadoActual.vue';

const routes = [
  { path: '/', component: Historial },
  { path: '/alta', component: AltaTransaccion },
  { path: '/editar/:id', component: Editar },
  { path: '/estado', component: Estado }
];

export default createRouter({
  history: createWebHistory(),
  routes
});
