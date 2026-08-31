const form = document.getElementById("loginForm");
const message = document.getElementById("message");
const loginSection = document.getElementById("loginSection");
const dashboardSection = document.getElementById("dashboardSection");
const welcomeMessage = document.getElementById("welcomeMessage");
const roleMessage = document.getElementById("roleMessage");

form.addEventListener("submit", handleLoginSubmit);

async function handleLoginSubmit(event) {
    event.preventDefault();

    clearMessage();

    const loginRequest = getLoginRequest();

    try {
        const data = await authenticateUser(loginRequest);
        // guardar token en localStorage
        localStorage.setItem('token', data.token);
        localStorage.setItem('role', data.role);
        window.location.href ="index.html";
        
    } catch (error) {
        showError(error.message);
    }
}

function getLoginRequest() {
    const nombreUsuario = document.getElementById("nombreUsuario").value.trim();
    const password = document.getElementById("password").value;

    return {
        email: nombreUsuario,
        password
    };
}

async function authenticateUser(loginRequest) {
    const response = await fetch(
        "/api/auth/login",
        {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(loginRequest)
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


function showError(errorMessage) {
    message.textContent = errorMessage;
}

function clearMessage() {
    message.textContent = "";
}