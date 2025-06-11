

const response = await fetch('http://localhost:8000/', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ pilhas: columns }),
});



let columns = [5, 5, 5, 5, 5];
let isPlayerTurn = true;
let selected = { column: null, indices: [] };

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

            stick.onclick = () => {
                if (!isPlayerTurn) return;
                if (selected.column === null) {
                    selected.column = colIndex;
                }
                if (selected.column !== colIndex) {
                    alert('Você só pode selecionar palitos de uma única linha.');
                    return;
                }
                const idx = selected.indices.indexOf(i);
                if (idx > -1) {
                    selected.indices.splice(idx, 1);
                } else {
                    selected.indices.push(i);
                }
                renderColumns();
            };

            col.appendChild(stick);
        }

        columnsDiv.appendChild(col);
    });
}

function checkGameEnd() {
    if (columns.reduce((a, b) => a + b) === 0) {
        document.getElementById('game-result').classList.remove('hidden');
        document.getElementById('result-message').innerText = isPlayerTurn ? 'Você perdeu!' : 'Você venceu!';
        document.getElementById('status').innerText = '';
        return true;
    }
    return false;
}

function confirmRemove() {
    if (!isPlayerTurn || selected.indices.length === 0) return;

    columns[selected.column] -= selected.indices.length;
    selected = { column: null, indices: [] };
    renderColumns();

    if (checkGameEnd()) return;

    isPlayerTurn = false;
    document.getElementById('status').innerText = 'Vez da IA...';

    // Chama a IA para jogar
    jogarIA();
}

async function jogarIA() {
    try {
        const response = await fetch('http://localhost:8000/jogada', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ pilhas: columns }),
        });

        if (!response.ok) throw new Error('Erro ao chamar a IA');

        const data = await response.json();

        columns[data.pilha] -= data.remover;
        renderColumns();

        if (checkGameEnd()) return;

        isPlayerTurn = true;
        document.getElementById('status').innerText = 'Sua vez!';
    } catch (error) {
        alert('Erro na comunicação com a IA: ' + error.message);
    }
}

function restartGame() {
    columns = [5, 5, 5, 5, 5];
    isPlayerTurn = true;
    selected = { column: null, indices: [] };
    renderColumns();
    document.getElementById('status').innerText = 'Sua vez!';
    document.getElementById('game-result').classList.add('hidden');
}

window.onload = () => {
    renderColumns();
    document.getElementById('status').innerText = 'Sua vez!';

    document.getElementById('remove-btn').onclick = confirmRemove;
    document.getElementById('restart-btn').onclick = restartGame;
};
