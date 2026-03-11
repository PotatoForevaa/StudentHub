from fastapi import FastAPI
from pydantic import BaseModel
import requests
from tasks import toxic_task
from celery_app import celery_app

app = FastAPI(
    title="Toxic Filter API",
    root_path="/toxic_filter"
)

MODEL_URL = "http://model_service:8001/predict"


class Comment(BaseModel):
    text: str

@app.post("/predict")
def predict(comment: Comment):
    r = requests.post(MODEL_URL, json={"text": comment.text})
    return r.json()

@app.post("/predict_async")
def predict_async(comment: Comment):
    task = toxic_task.delay(comment.text)
    return {"task_id": task.id}

@app.get("/task_result/{task_id}")
def get_task_result(task_id: str):
    task = celery_app.AsyncResult(task_id)
    if task.state == "PENDING":
        return {"status": "pending"}
    elif task.state == "FAILURE":
        return {"status": "failure", "error": str(task.result)}
    else:  # SUCCESS
        return {"status": "success", "result": task.result}