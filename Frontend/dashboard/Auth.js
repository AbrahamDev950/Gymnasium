// auth-utils.js
// Funciones centralizadas para autenticación

const API_URL = '/api';

/**
 * Obtiene el token almacenado.
 * @returns {string|null}
 */
function getToken() {
    const token = localStorage.getItem('token');

    if (!token || token === 'undefined' || token === 'null') {
        return null;
    }

    return token;
}

/**
 * Verifica si existe un token válido en localStorage.
 * @returns {boolean}
 */
function isTokenValid() {
    return !!getToken();
}

/**
 * Realiza una petición HTTP autenticada.
 *
 * @param {string} endpoint - Ruta de la API, por ejemplo "/socios"
 * @param {object} options - Opciones de fetch
 * @returns {Promise<Response|null>}
 */
async function fetchWithAuth(endpoint, options = {}) {
    const token = getToken();

    // No existe token
    if (!token) {
        logout();
        return null;
    }

    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        ...options.headers
    };

    try {
        const response = await fetch(`${API_URL}${endpoint}`, {
            ...options,
            headers
        });

        // Token inválido o expirado
        if (response.status === 401 || response.status === 403) {
            console.warn('Sesión expirada o no autorizada.');
            logout();
            return null;
        }

        // Otros errores HTTP
        if (!response.ok) {
            const errorData = await response.json().catch(() => ({}));

            throw new Error(
                errorData.mensaje ||
                `HTTP ${response.status}: ${response.statusText}`
            );
        }

        return response;

    } catch (error) {
        console.error('Error en la petición:', error);
        throw error;
    }
}

/**
 * Cierra la sesión.
 */
function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('user');
    localStorage.removeItem('nombreUsuario');

    sessionStorage.clear();

    window.location.href = 'index.html';
}

/**
 * Verifica que exista una sesión activa.
 */
function verifySession() {
    if (!isTokenValid()) {
        logout();
    }
}

// Exponer funciones globalmente
window.AuthUtils = {
    getToken,
    isTokenValid,
    fetchWithAuth,
    logout,
    verifySession
};