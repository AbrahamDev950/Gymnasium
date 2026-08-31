const UsuariosTarjetaInicioBtn = document.getElementById("UsuariosTarjetaInicioBtn");
const UsuariosTarjetaPlanesBtn = document.getElementById("UsuariosTarjetaPlanesBtn");
const UsuariosTarjetaUsuariosConMembresiaBtn = document.getElementById("UsuariosTarjetaUsuariosConMembresiaBtn");
const UsuariosTarjetaUsuarioRegistroBtn = document.getElementById("UsuariosTarjetaUsuarioRegistroBtn");
const UsuariosTarjetaVigentesBtn = document.getElementById("UsuariosTarjetaVigentesBtn");
const UsuariosTarjetaBuscarBtn = document.getElementById("UsuariosTarjetaBuscarBtn");
const infoUsuariosConMembresia = document.getElementById("infoUsuariosConMembresia");
const buscadorUsuarios = document.getElementById("buscadorUsuarios");
const infoMembersiasVigentes = document.getElementById("infoMembersiasVigentes");
const infoPlanes = document.getElementById("infoPlanes");
const infoRegistroNuevo = document.getElementById("infoRegistroNuevo");
const totalMembresiasVigentes = document.getElementById("totalMembresiasVigentes");

const API_URL = '';

// ----------------   Event Listeners para las tarjetas de navegación ---------------
mostrarMembresiasVigentes();
UsuariosTarjetaPlanesBtn.addEventListener("click", () => {
    if (infoPlanes.style.display === 'none') {
        infoPlanes.style.display = "block";
    } else {
        infoPlanes.style.display = "none";
    }
});

UsuariosTarjetaUsuariosConMembresiaBtn.addEventListener("click", () => {
    if (infoUsuariosConMembresia.style.display === 'none') {
        infoUsuariosConMembresia.style.display = "block";
    } else {
        infoUsuariosConMembresia.style.display = "none";
    }
});

UsuariosTarjetaInicioBtn.addEventListener("click", () => {
    window.location.href = "index.html";
});

UsuariosTarjetaBuscarBtn.addEventListener("click", () => {
    if (buscadorUsuarios.style.display === 'none') {
        buscadorUsuarios.style.display = "block";
    } else {
        buscadorUsuarios.style.display = "none";
    }
});

UsuariosTarjetaUsuarioRegistroBtn.addEventListener("click", () => {
    if (infoRegistroNuevo.style.display === 'none') {
        infoRegistroNuevo.style.display = "block";
    } else {
        infoRegistroNuevo.style.display = "none";
    }
})

UsuariosTarjetaVigentesBtn.addEventListener("click", () => {
    if (infoMembersiasVigentes.style.display === 'none') {
        infoMembersiasVigentes.style.display = "block";
    } else {
        infoMembersiasVigentes.style.display = "none";
    }
})

// ------------------- FIN EVENT LISTENERS -------------------

//-------------------   Funciones para mostrar info de acuerdo a la tarjeta de navegacion ---------------
// async function mostrarInfoUsuariosConMembresia() {
//     // Acceder al endpoint /api/socios para obtener la información de los usuarios con membresía
//     try {
//         const response = await fetch(`${API_URL}/api/socios`, {
//             method: 'GET',
//             headers: {
//                 'Content-Type': 'application/json',
//                 // 'Authorization': `Bearer ${localStorage.getItem('token')}`
//             }
//         });
//
//         if (!response.ok) {
//             throw new Error(`HTTP ${response.status}: ${response.statusText}`);
//         }
//
//         const data = await response.json();
//         console.log('Usuarios con membresía:', data);
//         // agregar la información al DOM, por ejemplo, mostrar el total de membresías vigentes
//         totalMembresiasVigentes.textContent = data.length;
//     } catch (error) {
//         console.error('Error al obtener la información de los usuarios con membresía:', error);
//     }
// }

// Llamar a la función para mostrar la información al cargar la página


// --------------------  Obtener y mostrar planes ----------------------


// Crea nuevo plan en la base de datos usando el btn "agregarPlanBtn" y activa el form del html "nuevoPlanForm"
async function crearNuevoPlan() {
    const agregarPlanBtn = document.getElementById('agregarPlanBtn');
    const crearPlanForm = document.getElementById('crearPlanForm');

    agregarPlanBtn.addEventListener('click', () => {
        if (crearPlanForm.style.display === 'none' || crearPlanForm.style.display === '') {
            crearPlanForm.style.display = 'block';
        } else {
            crearPlanForm.style.display = 'none';
        }
    });

    nuevoPlanForm.addEventListener('submit', async (event) => {
        event.preventDefault();

        const response = await fetch(`${API_URL}/api/planes`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify({
                nombre: document.getElementById('nombrePlan').value,
                precio: document.getElementById('precioPlan').value,
                duracion: document.getElementById('duracionPlan').value
            })
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();
        alert('Plan creado con éxito');
        cargarPlanes(); // Recargar la lista de planes después de crear uno nuevo
    });
}

// Asegúrate de llamar a la función cuando el DOM esté cargado
document.addEventListener('DOMContentLoaded', crearNuevoPlan);

async function cargarPlanes() {
    try {
        const infoPlanes = document.getElementById('infoPlanes');
        if (infoPlanes.style.display === 'none') {
            infoPlanes.style.display = 'block';
        } else {
            infoPlanes.style.display = 'none';
        }
        const response = await fetch(`${API_URL}/api/planes`);

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        const planes = await response.json();
        const contenedor = document.getElementById('contenedorPlanes');

        if (!planes || planes.length === 0) {
            contenedor.innerHTML = '<p>No hay planes disponibles</p>';
            return;
        }

        planes.sort((a,b) => Number(a.precio) - Number(b.precio));

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

        // Agregar eventos a los botones
        document.querySelectorAll('.btn-desactivar-plan').forEach(btn => {
            btn.addEventListener('click', async (e) => {
                const planId = e.target.dataset.planId;
                const plan = planes.find(p => p.id == planId);
                const etiquetaBoton = e.target.textContent.trim();

                // Si la etiqueta del botón es "Desactivar", preguntar si desea desactivar
                if (etiquetaBoton === 'Desactivar' && confirm(`¿Desactivar el plan "${plan.nombre}"?`)) {
                    await desactivarPlan(planId);
                    await cargarPlanes();
                }
                // Obtener la etiqueta del botón y preguntar si desea reactivar
                else if (etiquetaBoton === 'Reactivar' && confirm(`¿Reactivar el plan "${plan.nombre}"?`)) {
                    // Color del boton cambia a verde y se muestra un mensaje de confirmación
                    await reactivarPlan(planId);
                    await cargarPlanes();
                }
            });
        });

    } catch (error) {
        console.error('Error al cargar planes:', error);
        document.getElementById('contenedorPlanes').innerHTML =
            `<p>Error: ${error.message}</p>`;
    }
}
async function reactivarPlan(planId) {
    try {
        const response = await fetch(`${API_URL}/api/planes/${planId}/reactivar`, {
            method: 'PATCH',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        if (!response.ok) {
            const error = await response.json();
            alert(`Error: ${error.mensaje}`);
            return;
        }

        const data = await response.json();
        alert(data.mensaje);
    } catch (error) {
        console.error('Error al reactivar plan:', error);
        alert('Error al reactivar el plan');
    }
}
async function desactivarPlan(planId) {
    try {
        const response = await fetch(`${API_URL}/api/planes/${planId}/desactivar`, {
            method: 'PATCH',
            headers: {
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        if (!response.ok) {
            const error = await response.json();
            alert(`Error: ${error.mensaje}`);
            return;
        }

        const data = await response.json();
        alert(data.mensaje);
    } catch (error) {
        console.error('Error al desactivar plan:', error);
        alert('Error al desactivar el plan');
    }
}
// Se encarga de registrar un nuevo socio al enviar el formulario
document.getElementById('socioForm').addEventListener('submit', async function(event) {
    event.preventDefault();
    const token = localStorage.getItem('token');
    if (!token) {
        alert('No se encontró token de autenticación. Por favor, inicia sesión nuevamente.');
        window.location.href = 'login.html';
        return;
    }

    const crearSocioDto = {
        nombre: document.getElementById('nombre').value.trim(),
        apellido: document.getElementById('apellido').value.trim(),
        email: document.getElementById('email').value.trim(),
        telefono: document.getElementById('telefono').value.trim(),
    };

    console.log("Socio DTO a enviar:", crearSocioDto);

    try {
        const response = await fetch(`${API_URL}/api/socios`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(crearSocioDto)
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();
        console.log('Socio registrado:', data);
        alert('Socio registrado con éxito');
        document.getElementById('socioForm').reset();
        infoRegistroNuevo.style.display = 'none'; // Ocultar el formulario después del éxito

    } catch (error) {
        console.error('Error al registrar el socio:', error);
        alert(`Error: ${error.message}`);
    }
});

document.addEventListener('DOMContentLoaded', cargarPlanes);

// Asignar membresia a un socio existente
document.getElementById('membresiaForm').addEventListener('submit', async function(event) {
    event.preventDefault();
    
    const mensajeConfirmacion = `¿Estás seguro de que los datos son correctos?\n\n- ID del Plan: ${document.getElementById('planIdM').value.trim()}\n- ID del Socio: ${document.getElementById('socioIdM').value.trim()}`;
    const usuarioConfirmo = confirm(mensajeConfirmacion);

    if (!usuarioConfirmo) {
        return;
    }    
    
    // Llamada a la API para asignar una membresía
    // Ruta: api/membresias
    
    // recibe socioId y planId
    const asignarMembresiaDto = {
        planId: parseInt(document.getElementById('planIdM').value.trim(), 10),
        socioId: parseInt(document.getElementById('socioIdM').value.trim(), 10)
    };

    console.log("Asignar Membresía DTO a enviar:", asignarMembresiaDto);

    try {
        const response = await fetch(`${API_URL}/api/membresias`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify(asignarMembresiaDto)
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();
        console.log('Membresía asignada:', data);
        alert('Membresía asignada con éxito al usuario: ' + data.socioId);
        document.getElementById('membresiaForm').reset();
        infoUsuariosConMembresia.style.display = 'none'; // Ocultar el formulario después del éxito

    } catch (error) {
        console.error('Error al asignar membresía:', error);
        alert(`Error al registrar la membresía, revise que el ID del socio y el ID del plan sean correctos.`);
    }
});

// Registrar visita de socio
document.getElementById('visitaForm').addEventListener('submit', async function(event) {
    event.preventDefault();
    // [Route("api/asistencias")]
    const socioId = parseInt(document.getElementById('socioIdVisita').value.trim(), 10);
    
    // Crear el objeto DTO para registrar la visita
    const registrarVisitaDto = {
        socioId: socioId
    };

    console.log("Registrar Visita DTO a enviar:", registrarVisitaDto);

    try {
        const response = await fetch(`${API_URL}/api/asistencias`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify(registrarVisitaDto)
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();
        console.log('Visita registrada:', data);
        alert('Visita registrada con éxito');
        document.getElementById('visitaForm').reset();
    } catch (error) {
        console.error('Error al registrar la visita:', error);
        alert(`Error al registrar la visita, revise que el ID del socio sea correcto y que tenga una membresía vigente.`);
    }
});

// Mostrar las membresías vigentes en la tarjeta correspondiente en totalMembresiasVigentes
async function mostrarMembresiasVigentes() {
    try {
        const response = await fetch(`${API_URL}/api/membresias/totales/vigentes`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }
        const data = await response.json();
        console.log('Membresías vigentes:', data);
        // Parsear el total de membresías vigentes y mostrarlo en el DOM
        const total = parseInt(data.total, 10);
        // Mostrar el total de membresías vigentes en el DOM
        document.getElementById('totalMembresiasVigentes').textContent = total;
        // console.log ('Total de membresías vigentes:', data.totalMembresiasVigentes);
    } catch (error) {
        console.error('Error al obtener las membresías vigentes:', error);
    }
}

// Buscar socio por nombre, apellido, correo o teléfono
document.getElementById('buscarForm').addEventListener('submit', async function(event) {
    event.preventDefault();
    
    const botonBuscar = document.getElementById('buscarBtn');

    if (botonBuscar.disabled) return;

    // Deshabilitar el botón INMEDIATAMENTE
    botonBuscar.disabled = true;
    const textoOriginal = botonBuscar.textContent;
    botonBuscar.textContent = 'Buscando...';

    const query = document.getElementById('buscador').value.trim();

    if (!query) {
        alert('Por favor ingresa un término valido de búsqueda');
        // Si la validación falla, reactivamos el botón antes de salir
        botonBuscar.disabled = false;
        botonBuscar.textContent = textoOriginal;
        return;
    }

    try {
        const response = await fetch(`${API_URL}/api/socios/buscar?termino=${encodeURIComponent(query)}`,
            {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                }
            });

        if (!response.ok) {
            throw new Error('Error en la búsqueda');
        }

        const socios = await response.json();
        mostrarResultadosEnTabla(socios);

    } catch (error) {
        console.error('Error al buscar:', error);
        alert('Error al realizar la búsqueda');
    } finally {
        botonBuscar.disabled = false;
        botonBuscar.textContent = textoOriginal;
    }
});

// Función para mostrar resultados en la tabla
function mostrarResultadosEnTabla(socios) {
    const tablaCuerpo = document.getElementById('tablaCuerpo');
    const tabla = document.getElementById('miembros-totales');

    tablaCuerpo.innerHTML = '';

    if (socios.length === 0) {
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
                <button onclick="toggleEstadoSocio(${socio.id}, ${socio.activo}, this)" 
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

// Función para desactivar/activar socio
async function toggleEstadoSocio(id, estaActivo) {
    const endpoint = estaActivo ? 'desactivar' : 'activar';
    const accion = estaActivo ? 'desactiva' : 'activa';
    
    // Confimar con el usuario antes de hacer la petición
    const confirmacion = confirm(`¿Estás seguro de que deseas ${accion} al socio con ID ${id}?`);

    if (!confirmacion) {
        return;
    }

    try {
        const boton = event.target; // Obtener el botón que fue clickeado
        boton.disabled = true;
        boton.style.opacity = '0.6';
        boton.style.cursor = 'not-allowed';
        const textoOriginal = boton.textContent;
        boton.textContent = 'Procesando...';
        const response = await fetch(`${API_URL}/api/socios/${id}/${endpoint}`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            }
        });

        if (!response.ok) {
            if (response.status === 404) {
                throw new Error('Socio no encontrado.');
            } else if (response.status === 400) {
                const errorData = await response.json();
                throw new Error(errorData.mensaje || 'Error al cambiar el estado.');
            } else {
                throw new Error(`Error HTTP ${response.status}`);
            }
        }

        alert(`Socio ${accion}do exitosamente`);
        document.getElementById('buscarForm').dispatchEvent(new Event('submit'));

    } catch (error) {
        console.error('Error:', error);
        alert(`Error: ${error.message}`);

    } finally {
        // ✅ Reactivar el botón (se ejecuta siempre, éxito o error)
        boton.disabled = false;
        boton.style.opacity = '1';
        boton.style.cursor = 'pointer';
        boton.textContent = textoOriginal;
    }
}
// Actualizar la funcion de membresias vigentes para que se ejecute después de asignar una membresía o cambiar el estado de un socio
    
// Función para editar (puedes expandir esto)
function editarSocio(id) {
    alert(`Editar socio ${id}`);
    // Aquí implementarías la lógica de edición
}
