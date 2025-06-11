// js/auth.js

function salvarUsuario(usuario) {
    let users = JSON.parse(localStorage.getItem("usuarios")) || [];
    users.push(usuario);
    localStorage.setItem("usuarios", JSON.stringify(users));
}

function buscarUsuario(username) {
    let users = JSON.parse(localStorage.getItem("usuarios")) || [];
    return users.find(u => u.username === username);
}

function registrar(event) {
    event.preventDefault();
    const username = document.getElementById("reg-username").value;
    const email = document.getElementById("reg-email").value;
    const senha = document.getElementById("reg-password").value;

    if (buscarUsuario(username)) {
        alert("Usuário já existe!");
        return;
    }

    salvarUsuario({ username, email, senha });
    alert("Cadastro realizado com sucesso!");
    window.location.href = "index.html";
}

function logar(event) {
    event.preventDefault();
    const username = document.getElementById("login-username").value;
    const senha = document.getElementById("login-password").value;

    const usuario = buscarUsuario(username);
    if (!usuario || usuario.senha !== senha) {
        alert("Usuário ou senha incorretos.");
        return;
    }

    localStorage.setItem("usuarioLogado", JSON.stringify(usuario));
    window.location.href = "menu.html";
}
