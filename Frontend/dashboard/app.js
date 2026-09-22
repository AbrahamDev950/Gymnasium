// dashboard.js


// -------------------------------- Elementos del DOM -----------------------------------

const dashboard = document.getElementById('dashboard');
const loading = document.getElementById('loading');
const error = document.getElementById('error');

const refreshBtn = document.getElementById('refreshBtn');
const UsuariosTarjetaBtn = document.getElementById('UsuariosTarjetaBtn');
const logoutBtn = document.getElementById('logoutBtn');

// -------------------------------- Estadísticas -----------------------------------

const asistenciasHoy = document.getElementById('asistenciasHoy');
const membresíasVigentes = document.getElementById('membresíasVigentes');
const membresíasProximasAVencer = document.getElementById('membresíasProximasAVencer');
const ingresosDelMes = document.getElementById('ingresosDelMes');
const lastUpdate = document.getElementById('lastUpdate');

// -------------------------------- Interfaz -----------------------------------

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

// -------------------------------- Formateadores -----------------------------------

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

// -------------------------------- Dashboard -----------------------------------

async function fetchDashboard() {
    showLoading();

    try {
        const response = await AuthUtils.fetchWithAuth('/dashboard');

        // Si AuthUtils detectó una sesión inválida,
        // ya se encargó de redirigir.
        if (!response) {
            return;
        }

        const data = await response.json();

        // Actualizar estadísticas
        asistenciasHoy.textContent = data.asistenciasHoy ?? 0;

        membresíasVigentes.textContent =
            data.membresíasVigentes ?? 0;

        membresíasProximasAVencer.textContent =
            data.membresíasProximasAVencer ?? 0;

        ingresosDelMes.textContent =
            formatCurrency(data.ingresosDelMes ?? 0);

        lastUpdate.textContent =
            data.fechaConsulta
                ? formatTime(data.fechaConsulta)
                : '--:--:--';

        showDashboard();

    } catch (err) {
        console.error('Error obteniendo dashboard:', err);

        showError(
            err.message || 'No se pudo conectar con la API'
        );
    }
}

// -------------------------------- Eventos -----------------------------------

logoutBtn.addEventListener('click', () => {
    AuthUtils.logout();
});

refreshBtn.addEventListener('click', fetchDashboard);

UsuariosTarjetaBtn.addEventListener('click', () => {
    window.location.href = 'users.html';
});

// -------------------------------- Inicialización -----------------------------------

document.addEventListener('DOMContentLoaded', async () => {
    console.log('🚀 Inicializando dashboard...');

    // Verificar sesión
    AuthUtils.verifySession();

    // Si no hay token, verifySession() redirige.
    if (!AuthUtils.isTokenValid()) {
        return;
    }

    await fetchDashboard();
});

function verificarAutenticacionEstricta() {
    AuthUtils.verifySession();
}
window.addEventListener('pageshow', () => {
    verificarAutenticacionEstricta();
});


verificarAutenticacionEstricta();