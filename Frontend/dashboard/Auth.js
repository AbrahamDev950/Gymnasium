// auth-utils.js - Funciones centralizadas para autenticación

const API_URL = '/api';

/**
 * Obtiene el token del localStorage
 * @returns {string|null} Token o null si no existe
 */
function getToken() {
    const token = localStorage.getItem('token');
    console.log('🔑 Token recuperado:', token ? `${token.substring(0, 20)}...` : 'No existe');
    return token;
}

/**
 * Verifica si el token existe y es válido
 * @returns {boolean}
 */
function isTokenValid() {
    const token = getToken();
    return token && token.length > 0 && token !== 'undefined' && token !== 'null';
}

/**
 * Realiza un fetch con autenticación automática
 * @param {string} endpoint - Ruta de la API (ej: '/socios')
 * @param {object} options - Opciones del fetch
 * @returns {Promise<Response>}
 */
async function fetchWithAuth(endpoint, options = {}) {
    const token = getToken();

    // Verificar si el token existe
    if (!token || token === 'undefined' || token === 'null') {
        console.error('❌ No hay token disponible. Redirigiendo a login...');
        localStorage.clear();
        window.location.href = 'login.html';
        return null;
    }

    // Configurar headers con autenticación
    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        ...options.headers
    };

    try {
        console.log(`📤 Requesting: ${API_URL}${endpoint}`);
        console.log('📋 Headers:', {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token.substring(0, 20)}...`
        });

        const response = await fetch(`${API_URL}${endpoint}`, {
            ...options,
            headers
        });

        // Si el token expiró o no es válido
        if (response.status === 401 || response.status === 403) {
            console.error('❌ Token no válido o expirado');
            localStorage.clear();
            window.location.href = 'login.html';
            return null;
        }

        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));
            throw new Error(errorData.mensaje || `HTTP ${response.status}: ${response.statusText}`);
        }

        return response;
    } catch (error) {
        console.error('❌ Error en fetchWithAuth:', error);
        throw error;
    }
}

/**
 * Hacer logout seguro
 */
function logout() {
    console.log('🚪 Realizando logout...');
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('user');
    sessionStorage.clear();
    window.location.href = 'login.html';
}

/**
 * Verificar sesión al cargar la página
 */
function verifySession() {
    if (!isTokenValid()) {
        console.warn('⚠️ Sesión inválida. Redirigiendo a login...');
        logout();
    }
}

// Exportar para uso en otros archivos
window.AuthUtils = {
    getToken,
    isTokenValid,
    fetchWithAuth,
    logout,
    verifySession
};