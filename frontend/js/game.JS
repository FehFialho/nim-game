// Jogo.js
let columns = [5, 5, 5, 5, 5];
let isPlayerTurn = true;
let timer;
let countdown = 30;
let selected = {
    column: null,
    indices: []
};

function renderColumns() {
    const columnsDiv = document.getElementById('columns');
    columnsDiv.innerHTML = '';

    columns.forEach((count, colIndex) => {
        const col = document.createElement('div');
        col.classList.add('column');

        for (let i = 0; i < count; i++) {
            const stick = document.createElement('div');
            stick.classList.add('stick');
            stick.dataset.col = colIndex;
            stick.dataset.index = i;

            if (selected.column === colIndex && selected.indices.includes(i)) {
                stick.classList.add('selected');
            }

            stick.onclick = () => toggleStick(colIndex, i);
            col.appendChild(stick);
        }
        columnsDiv.appendChild(col);
    });
}

function toggleStick(column, index) {
    if (!isPlayerTurn) return;

    if (selected.column === null) {
        selected.column = column;
    }

    if (selected.column !== column) {
        alert("Você só pode selecionar palitos de uma única linha.");
        return;
    }

    const i = selected.indices.indexOf(index);
    if (i > -1) {
        selected.indices.splice(i, 1);
    } else {
        selected.indices.push(index);
    }

    renderColumns();
}

function confirmRemove() {
    if (!isPlayerTurn || selected.indices.length === 0) return;

    columns[selected.column] -= selected.indices.length;
    selected.column = null;
    selected.indices = [];

    renderColumns();
    checkGameEnd();
    switchTurn();
}

function checkGameEnd() {
    if (columns.reduce((a, b) => a + b) === 0) {
        document.getElementById('game-result').classList.remove('hidden');
        document.getElementById('game-result').classList.add('show');
        document.getElementById('status').innerText = '';
        document.getElementById('result-message').innerText = isPlayerTurn ? 'Você perdeu!' : 'Você venceu!';
        clearInterval(timer);
    }
}

function switchTurn() {
    isPlayerTurn = !isPlayerTurn;
    selected.column = null;
    selected.indices = [];

    document.getElementById('status').innerText = isPlayerTurn ? 'Sua vez!' : 'Vez da IA...';
    resetTimer();

    if (!isPlayerTurn) {
        setTimeout(aiPlay, 2000);
    }
}

function aiPlay() {
    for (let i = 0; i < columns.length; i++) {
        if (columns[i] > 0) {
            const removeCount = Math.floor(Math.random() * columns[i]) + 1;
            columns[i] -= removeCount;
            break;
        }
    }
    renderColumns();
    checkGameEnd();
    switchTurn();
}

function resetTimer() {
    clearInterval(timer);
    countdown = 30;
    document.getElementById('timer').innerText = countdown;
    timer = setInterval(() => {
        countdown--;
        document.getElementById('timer').innerText = countdown;
        if (countdown <= 0) {
            clearInterval(timer);
            document.getElementById('game-result').classList.remove('hidden');
            document.getElementById('game-result').classList.add('show');
            document.getElementById('result-message').innerText = isPlayerTurn ? 'Você perdeu por tempo!' : 'IA perdeu por tempo!';
        }
    }, 1000);
}

window.onload = () => {
    renderColumns();
    resetTimer();

    const removeButton = document.createElement('button');
    removeButton.innerText = "Remover Palitos Selecionados";
    removeButton.onclick = confirmRemove;
    document.body.appendChild(removeButton);
};

// auth.js
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
