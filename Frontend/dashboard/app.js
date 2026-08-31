const API_URL = '/api';

// Elementos del DOM
const dashboard = document.getElementById('dashboard');
const loading = document.getElementById('loading');
const error = document.getElementById('error');
const refreshBtn = document.getElementById('refreshBtn');
const UsuariosTarjetaBtn = document.getElementById('UsuariosTarjetaBtn');

// Elementos de Estadísticas
const asistenciasHoy = document.getElementById('asistenciasHoy');
const membresíasVigentes = document.getElementById('membresíasVigentes');
const membresíasProximasAVencer = document.getElementById('membresíasProximasAVencer');
const ingresosDelMes = document.getElementById('ingresosDelMes');
const lastUpdate = document.getElementById('lastUpdate');
const logoutBtn = document.getElementById('logoutBtn');

// --- Autenticación y Tokens ---
function obtenerToken() {
    const storedData = localStorage.getItem('token');
    if (!storedData) return null;

    if (storedData.startsWith('ey')) {
        return storedData;
    }

    try {
        const parsed = JSON.parse(storedData);
        return parsed.token || parsed;
    } catch (e) {
        console.error('Error parsing token from localStorage:', e);
        return null;
    }
}

function getAuthToken() {
    const token = obtenerToken();
    if (!token || token === 'undefined' || token === 'null') {
        return null;
    }
    return token;
}

function verificarAutenticacionEstricta() {
    const token = getAuthToken();
    if (!token) {
        window.location.replace('login.html');
    }
}

function logOut() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('nombreUsuario');
    window.location.replace('login.html');
}

// --- Control de Interfaz ---
function showLoading() {
    loading.style.display = 'flex';
    dashboard.style.display = 'none';
    error.style.display = 'none';
}

function showDashboard() {
    loading.style.display = 'none';
    dashboard.style.display = 'block';
    error.style.display = 'none';
}

function showError(message) {
    error.textContent = `❌ Error: ${message}`;
    error.style.display = 'block';
    dashboard.style.display = 'none';
    loading.style.display = 'none';
}

// --- Formateadores ---
function formatCurrency(value) {
    return new Intl.NumberFormat('es-MX', {
        style: 'currency',
        currency: 'MXN'
    }).format(value);
}

function formatTime(isoString) {
    const date = new Date(isoString);
    return date.toLocaleTimeString('es-MX', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
    });
}

// --- Peticiones API ---
async function fetchDashboard() {
    showLoading();

    try {
        const token = obtenerToken();

        const response = await fetch(`${API_URL}/dashboard`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();

        // Actualizar estadísticas del DOM
        asistenciasHoy.textContent = data.asistenciasHoy ?? 0;
        membresíasVigentes.textContent = data.membresíasVigentes ?? 0;
        membresíasProximasAVencer.textContent = data.membresíasProximasAVencer ?? 0;
        ingresosDelMes.textContent = formatCurrency(data.ingresosDelMes ?? 0);
        lastUpdate.textContent = data.fechaConsulta ? formatTime(data.fechaConsulta) : '--:--:--';

        showDashboard();
    } catch (err) {
        console.error('Error fetching dashboard:', err);
        showError(err.message || 'No se pudo conectar con la API');
    }
}

// --- Listeners de Eventos ---
logoutBtn.addEventListener('click', logOut);
refreshBtn.addEventListener('click', fetchDashboard);

UsuariosTarjetaBtn.addEventListener('click', () => {
    window.location.href = 'users.html';
});

window.addEventListener('pageshow', () => {
    verificarAutenticacionEstricta();
});

// --- Inicialización ---
verificarAutenticacionEstricta();

document.addEventListener('DOMContentLoaded', async () => {
    console.log('🚀 Inicializando aplicación...');

    const token = getAuthToken();
    if (!token) {
        console.error('❌ No hay token de autenticación');
        window.location.href = 'login.html';
        return;
    }

    await fetchDashboard();
});

// Verificación periódica del token (cada 5 min)
setInterval(() => {
    const token = getAuthToken();
    if (!token) {
        console.warn('⚠️ Sesión expirada');
        logOut();
    }
}, 5 * 60 * 1000);