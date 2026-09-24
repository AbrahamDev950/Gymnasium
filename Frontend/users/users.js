const API_URL_S = '';

// ==========================================
// 1. UTILIDADES Y MANEJO CENTRALIZADO DE ERRORES
// ==========================================

// Abstracción para alertas o notificaciones UI
const notificar = (mensaje, tipo = 'error') => {
    // Si usas una librería como Toastify/SweetAlert, la cambias aquí centralizadamente:
    // Toastify({ text: mensaje, className: tipo }).showToast();
    alert(`${tipo === 'error' ? '❌ Error: ' : '✅ '}${mensaje}`);
};

// Cliente HTTP centralizado que maneja errores y tokens automáticamente
async function apiFetch(endpoint, options = {}) {
    const token = localStorage.getItem('token');

    const headers = {
        'Content-Type': 'application/json',
        ...(token ? {'Authorization': `Bearer ${token}`} : {}),
        ...options.headers
    };

    try {
        const response = await fetch(`${API_URL_S}${endpoint}`, {
            ...options,
            headers
        });

        // Caso especial: Sesión expirada
        if (response.status === 401) {
            notificar('Sesión expirada o no válida. Inicia sesión nuevamente.');
            window.location.href = 'index.html';
            throw new Error('No autorizado');
        }

        // Leer el cuerpo de la respuesta según el tipo de contenido
        let body;
        const contentType = response.headers.get('content-type');

        if (contentType && contentType.includes('application/json')) {
            body = await response.json();
        } else {
            body = await response.text();
        }

        // Si la respuesta NO es OK, extraer el error
        if (!response.ok) {
            let errorMsg = `Error HTTP ${response.status}: ${response.statusText}`;

            // Si el body es un objeto con mensaje
            if (typeof body === 'object' && body !== null) {
                errorMsg = body.mensaje || body.message || errorMsg;
            }
            // Si es string/HTML, usar como está
            else if (typeof body === 'string') {
                errorMsg = body.substring(0, 200); // Limitar a 200 caracteres
            }

            throw new Error(errorMsg);
        }

        // Si es OK, retornar el body
        return body || {};

    } catch (error) {
        console.error(`[API Error] ${options.method || 'GET'} ${endpoint}:`, error);
        throw error;
    }
}

// ==========================================
// 2. LÓGICA DE UI Y NAVEGACIÓN
// ==========================================

const seccionesMap = {
    UsuariosTarjetaPlanesBtn: 'infoPlanes',
    UsuariosTarjetaUsuariosConMembresiaBtn: 'infoUsuariosConMembresia',
    UsuariosTarjetaBuscarBtn: 'buscadorUsuarios',
    UsuariosTarjetaUsuarioRegistroBtn: 'infoRegistroNuevo',
    UsuariosTarjetaVigentesBtn: 'infoMembersiasVigentes'
};

const toggleVisibilidad = (idElemento) => {
    const el = document.getElementById(idElemento);
    if (el) {
        el.style.display = (el.style.display === 'none') ? 'block' : 'none';
    }
};

Object.entries(seccionesMap).forEach(([btnId, seccionId]) => {
    document.getElementById(btnId)?.addEventListener('click', () => toggleVisibilidad(seccionId));
});

document.getElementById('UsuariosTarjetaInicioBtn')?.addEventListener('click', () => {
    window.location.href = 'dashboard.html';
});

// ==========================================
// 3. FUNCIONES DE NEGOCIO REFACTORIZADAS
// ==========================================

async function crearNuevoPlan() {
    const agregarPlanBtn = document.getElementById('agregarPlanBtn');
    const crearPlanForm = document.getElementById('crearPlanForm');
    const nuevoPlanForm = document.getElementById('nuevoPlanForm');

    agregarPlanBtn?.addEventListener('click', () => {
        crearPlanForm.style.display = (crearPlanForm.style.display === 'none' || crearPlanForm.style.display === '') ? 'block' : 'none';
    });

    nuevoPlanForm?.addEventListener('submit', async (event) => {
        event.preventDefault();
        try {
            await apiFetch('/api/planes', {
                method: 'POST',
                body: JSON.stringify({
                    nombre: document.getElementById('nombrePlan').value,
                    precio: document.getElementById('precioPlan').value,
                    duracion: document.getElementById('duracionPlan').value
                })
            });
            notificar('Plan creado con éxito', 'exito');
            cargarPlanes();
        } catch (error) {
            notificar(error.message);
        }
    });
}

async function cargarPlanes() {
    const contenedor = document.getElementById('contenedorPlanes');
    try {
        toggleVisibilidad('infoPlanes');

        const planes = await apiFetch('/api/planes');

        if (!planes || planes.length === 0) {
            contenedor.innerHTML = '<p>No hay planes disponibles</p>';
            return;
        }

        planes.sort((a, b) => Number(a.precio) - Number(b.precio));

        contenedor.innerHTML = planes.map(plan => `
            <article class="tarjeta-plan">
                <h3>${plan.nombre}</h3>
                <p class="id">id: ${plan.id}</p>
                <p class="precio">$${plan.precio} <span>/mes</span></p>
                <p class="duracion">Duración: ${plan.duracion} días</p>
                <button 
                    class="btn-desactivar-plan ${plan.activo ? 'btn-activo' : 'btn-inactivo'}" 
                    data-plan-id="${plan.id}">
                    ${plan.activo ? 'Desactivar' : 'Reactivar'}
                </button>
            </article>
        `).join('');

        document.querySelectorAll('.btn-desactivar-plan').forEach(btn => {
            btn.addEventListener('click', async (e) => {
                const planId = e.target.dataset.planId;
                const plan = planes.find(p => p.id == planId);
                const etiquetaBoton = e.target.textContent.trim();

                if (etiquetaBoton === 'Desactivar' && confirm(`¿Desactivar el plan "${plan.nombre}"?`)) {
                    await cambiarEstadoPlan(planId, 'desactivar');
                } else if (etiquetaBoton === 'Reactivar' && confirm(`¿Reactivar el plan "${plan.nombre}"?`)) {
                    await cambiarEstadoPlan(planId, 'reactivar');
                }
            });
        });

    } catch (error) {
        contenedor.innerHTML = `<p class="error">Error al cargar planes: ${error.message}</p>`;
    }
}

// Unificación de reactivarPlan y desactivarPlan
async function cambiarEstadoPlan(planId, accion) {
    try {
        const data = await apiFetch(`/api/planes/${planId}/${accion}`, { method: 'PATCH' });
        notificar(data.mensaje || `Plan ${accion}do exitosamente`, 'exito');
        cargarPlanes();
    } catch (error) {
        notificar(error.message);
    }
}

// Registro de Socio
document.getElementById('socioForm')?.addEventListener('submit', async function(event) {
    event.preventDefault();

    const crearSocioDto = {
        nombre: document.getElementById('nombre').value.trim(),
        apellido: document.getElementById('apellido').value.trim(),
        email: document.getElementById('email').value.trim(),
        telefono: document.getElementById('telefono').value.trim(),
    };

    try {
        await apiFetch('/api/socios', {
            method: 'POST',
            body: JSON.stringify(crearSocioDto)
        });
        notificar('Socio registrado con éxito', 'exito');
        document.getElementById('socioForm').reset();
        document.getElementById('infoRegistroNuevo').style.display = 'none';
    } catch (error) {
        notificar(`Error al registrar el socio: ${error.message}`);
    }
});

// Asignar Membresía
document.getElementById('membresiaForm')?.addEventListener('submit', async function(event) {
    event.preventDefault();

    const planId = document.getElementById('planIdM').value.trim();
    const socioId = document.getElementById('socioIdM').value.trim();

    if (!confirm(`¿Estás seguro de que los datos son correctos?\n\n- ID del Plan: ${planId}\n- ID del Socio: ${socioId}`)) {
        return;
    }

    try {
        const data = await apiFetch('/api/membresias', {
            method: 'POST',
            body: JSON.stringify({
                planId: (planId),
                socioId: (socioId)
            })
        });
        notificar(`Membresía asignada con éxito al usuario: ${data.socioId || socioId}`, 'exito');
        document.getElementById('membresiaForm').reset();
        document.getElementById('infoUsuariosConMembresia').style.display = 'none';
    } catch (error) {
        notificar(`Error al asignar la membresía: ${error.message}`);
    }
});

// Registrar Visita
document.getElementById('visitaForm')?.addEventListener('submit', async function(event) {
    event.preventDefault();
    const socioId = parseInt(document.getElementById('socioIdVisita').value.trim(), 10);

    try {
        await apiFetch('/api/asistencias', {
            method: 'POST',
            // Enviar solo el id del socio para registrar la visita
            body: JSON.stringify({ socioId })
        });
        notificar('Visita registrada con éxito', 'exito');
        document.getElementById('visitaForm').reset();
    } catch (error) {
        notificar(`Error al registrar la visita: ${error.message}`);
    }
});

// Métricas Dashboard
async function mostrarMembresiasVigentes() {
    try {
        const data = await apiFetch('/api/dashboard');
        const total = parseInt(data.membresíasVigentes ?? 0, 10);
        document.getElementById('totalMembresiasVigentes').textContent = total;
    } catch (error) {
        console.error('Error al obtener las membresías vigentes:', error);
    }
}

// Búsqueda de Socios
document.getElementById('buscarForm')?.addEventListener('submit', async function(event) {
    event.preventDefault();

    const botonBuscar = document.getElementById('buscarBtn');
    if (botonBuscar.disabled) return;

    const query = document.getElementById('buscador').value.trim();
    if (!query) {
        notificar('Por favor ingresa un término válido de búsqueda');
        return;
    }

    botonBuscar.disabled = true;
    const textoOriginal = botonBuscar.textContent;
    botonBuscar.textContent = 'Buscando...';

    try {
        const socios = await apiFetch(`/api/socios/buscar?termino=${encodeURIComponent(query)}`);
        mostrarResultadosEnTabla(socios);
    } catch (error) {
        notificar(`No se pudo realizar la búsqueda: ${error.message}`);
    } finally {
        botonBuscar.disabled = false;
        botonBuscar.textContent = textoOriginal;
    }
});

function mostrarResultadosEnTabla(socios) {
    const tablaCuerpo = document.getElementById('tablaCuerpo');
    const tabla = document.getElementById('miembros-totales');

    tablaCuerpo.innerHTML = '';

    if (!socios || socios.length === 0) {
        tablaCuerpo.innerHTML = '<tr><td colspan="4" style="text-align: center;">No se encontraron resultados</td></tr>';
        tabla.style.display = 'table';
        return;
    }

    socios.forEach(socio => {
        const fila = document.createElement('tr');
        const estado = socio.activo ?
            '<span class="estado-activo">✓ Activo</span>' :
            '<span class="estado-inactivo">✗ Inactivo</span>';

        const acciones = `
            <div class="acciones-btn">
                <button onclick="editarSocio(${socio.id})" class="btn-editar">Editar</button>
                <button onclick="toggleEstadoSocio(event, ${socio.id}, ${socio.activo})" 
                        class="btn-${socio.activo ? 'desactivar' : 'activar'}">
                    ${socio.activo ? 'Desactivar' : 'Activar'}
                </button>
            </div>
        `;

        fila.innerHTML = `
            <td>${socio.id}</td>
            <td>
                <strong>${socio.nombre} ${socio.apellido}</strong><br>
                <small>${socio.email}</small><br>
                <small>${socio.telefono}</small>
            </td>
            <td>${estado}</td>
            <td>${acciones}</td>
        `;

        tablaCuerpo.appendChild(fila);
    });

    tabla.style.display = 'table';
}

// Activar / Desactivar Socio
async function toggleEstadoSocio(evt, id, estaActivo) {
    const endpoint = estaActivo ? 'desactivar' : 'activar';
    const accion = estaActivo ? 'desactiva' : 'activa';

    if (!confirm(`¿Estás seguro de que deseas ${accion}r al socio con ID ${id}?`)) {
        return;
    }

    const boton = evt.target;
    boton.disabled = true;
    const textoOriginal = boton.textContent;
    boton.textContent = 'Procesando...';

    try {
        await apiFetch(`/api/socios/${id}/${endpoint}`, { method: 'PATCH' });
        notificar(`Socio ${accion}do exitosamente`, 'exito');
        document.getElementById('buscarForm').dispatchEvent(new Event('submit'));
    } catch (error) {
        notificar(error.message);
    } finally {
        boton.disabled = false;
        boton.textContent = textoOriginal;
    }
}

function editarSocio(id) {
    alert(`Editar socio ${id}`);
}

function verificarAutenticacionEstricta() {
    if (typeof AuthUtils !== 'undefined') {
        AuthUtils.verifySession();
    }
}

// ==========================================
// 4. INICIALIZACIÓN
// ==========================================

document.addEventListener('DOMContentLoaded', () => {
    crearNuevoPlan();
    cargarPlanes();
    mostrarMembresiasVigentes();
    verificarAutenticacionEstricta();
});

window.addEventListener('pageshow', verificarAutenticacionEstricta);

// Logout
const logoutBtn = document.getElementById('logoutBtn');
logoutBtn.addEventListener('click', () => {
    AuthUtils.logout();
});