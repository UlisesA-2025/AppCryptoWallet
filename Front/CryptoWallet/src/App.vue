<template>
  <div>
    <nav>
      <router-link to="/">Historial</router-link> |
      <router-link to="/alta">Alta</router-link> |
      <router-link to="/estado">Estado actual</router-link>
    </nav>
    <hr />
    <router-view></router-view>
  </div>
</template>

<script setup>

</script>

<style scoped>
nav {
  display: flex;
  gap: 10px;
  margin-bottom: 10px;
}

nav a {
  text-decoration: none;
  padding: 5px 10px;
  background-color: #eee;
  border-radius: 5px;
}

nav a.router-link-exact-active {
  background-color: #3498db;
  color: white;
}
</style>
