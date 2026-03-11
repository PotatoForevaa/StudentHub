import torch
from transformers import AutoTokenizer, AutoModelForSequenceClassification
from fastapi import FastAPI
from pydantic import BaseModel

MODEL_PATH = "toxic_model_balanced"

device = "cuda" if torch.cuda.is_available() else "cpu"

tokenizer = AutoTokenizer.from_pretrained(MODEL_PATH)
model = AutoModelForSequenceClassification.from_pretrained(MODEL_PATH)

model.to(device)
model.eval()

app = FastAPI()


class Comment(BaseModel):
    text: str


@app.post("/predict")
def predict(comment: Comment):

    inputs = tokenizer(
        comment.text,
        return_tensors="pt",
        truncation=True,
        padding=True,
        max_length=128
    )

    inputs = {k: v.to(device) for k, v in inputs.items()}

    with torch.no_grad():
        outputs = model(**inputs)
        logits = outputs.logits
        probs = torch.softmax(logits, dim=1)
        pred = torch.argmax(probs, dim=1).item()

    return {
        "prediction": "toxic" if pred == 1 else "not toxic",
        "toxic_probability": float(probs[0][1])
    }