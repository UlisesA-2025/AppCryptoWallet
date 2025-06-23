const API_URL = "https://localhost:5022/api/transacciones";

async function handleResponse(res) {
  if (!res.ok) {
    const errorText = await res.text();
    throw new Error(errorText);
  }
  return await res.json();
}

export default {
  async crearTransaccion(data) {
    const res = await fetch(API_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    return await handleResponse(res);
  },

  async obtenerTransacciones() {
    const res = await fetch(API_URL);
    return await handleResponse(res);
  },

  async obtenerTransaccion(id) {
    const res = await fetch(`${API_URL}/${id}`);
    return await handleResponse(res);
  },

  async editarTransaccion(id, data) {
    const res = await fetch(`${API_URL}/${id}`, {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    return await handleResponse(res);
  },

  async eliminarTransaccion(id) {
    const res = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    return await handleResponse(res);
  },

  async obtenerEstado() {
    const res = await fetch(`${API_URL}/estado`);
    return await handleResponse(res);
  },
};
