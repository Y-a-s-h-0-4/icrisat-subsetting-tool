from fastapi import FastAPI
from pydantic import BaseModel
from typing import Dict, Optional
from code import extract_traits_with_ranges

app = FastAPI()

class TraitRequest(BaseModel):
    text: str

class TraitResponse(BaseModel):
    traits: Dict[str, Dict[str, Optional[float]]]

@app.post("/extract-traits", response_model=TraitResponse)
async def extract_traits(request: TraitRequest):
    """
    Extract traits and their values from the given text.
    Returns a dictionary of traits with their min, max, and unit values.
    """
    traits = extract_traits_with_ranges(request.text)
    return TraitResponse(traits=traits)

@app.get("/")
async def root():
    return {"message": "Trait Extraction API is running. Use /extract-traits endpoint to extract traits from text."} 