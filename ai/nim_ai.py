import sys
import json
from functools import reduce
from operator import xor

def jogada_ia(pilhas):
    xor_total = reduce(xor, pilhas)
    if xor_total == 0:
        for i, qtd in enumerate(pilhas):
            if qtd > 0:
                return i, 1
    for i, qtd in enumerate(pilhas):
        alvo = qtd ^ xor_total
        if alvo < qtd:
            return i, qtd - alvo
    for i, qtd in enumerate(pilhas):
        if qtd > 0:
            return i, 1
    return -1, 0

if __name__ == "__main__":
    entrada = sys.stdin.read()
    pilhas = json.loads(entrada)
    pilha, remover = jogada_ia(pilhas)
    print(json.dumps({"pilha": pilha, "remover": remover}))
