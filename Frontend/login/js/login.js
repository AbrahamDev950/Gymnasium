// ------------------------- Referencias al DOM --------------------------------------------------------
const form = document.getElementById("loginForm");
const message = document.getElementById("message");
// ------------------- Event listener para el evento de envío del formulario ---------------------------
form.addEventListener("submit", handleLoginSubmit);


// ----------------------- Funciones ---------------------------

// Función para manejar el envío del formulario de inicio de sesión
async function handleLoginSubmit(event) {
    event.preventDefault();

    clearMessage();
    // Obtener los datos del formulario y crear el objeto LoginRequest
    const LoginRequest = getLoginRequest();

    try {
        const data = await authenticateUser(LoginRequest);
        // guardar token en localStorage
        localStorage.setItem('token', data.token);
        localStorage.setItem('role', data.role);
        window.location.href ="dashboard.html";
        
    } catch (error) {
        showError(error.message);
    }
}

// Función para obtener los datos del formulario y crear el objeto LoginRequest
function getLoginRequest() {
    const nombreUsuario = document.getElementById("nombreUsuario").value.trim();
    const password = document.getElementById("password").value;

    return {
        nombreUsuario,
        password
    };
}

// Función para autenticar al usuario usando
async function authenticateUser(LoginRequest) {
    const response = await fetch(
        "/api/auth/login",
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(LoginRequest)
        }
    );

    if (response.status === 401) {
        throw new Error("Correo o contraseña incorrectos.");
    }

    if (!response.ok) {
        throw new Error("No fue posible iniciar sesión.");
    }
    
    return await response.json();
}

// Función para mostrar un mensaje de error en el DOM
function showError(errorMessage) {
    message.textContent = errorMessage;
}

// Función para limpiar el mensaje de error en el DOM
function clearMessage() {
    message.textContent = "";
}