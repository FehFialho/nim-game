from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from typing import List
from nim_ai import jogada_ia

app = FastAPI()

class EstadoJogo(BaseModel):
    pilhas: List[int]

class Jogada(BaseModel):
    pilha: int
    remover: int

@app.post("/jogada", response_model=Jogada)
def obter_jogada(estado: EstadoJogo):
    try:
        pilha, remover = jogada_ia(estado.pilhas)
        return {"pilha": pilha, "remover": remover}
    except Exception as e:
        raise HTTPException(status_code=400, detail=str(e))