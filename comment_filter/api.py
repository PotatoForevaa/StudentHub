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


class LinkedComment(BaseModel):
    text: str
    comment_id: str

@app.post("/predict")
def predict(comment: Comment):
    r = requests.post(MODEL_URL, json={"text": comment.text})
    return r.json()

@app.post("/predict_async")
def predict_async(comment: Comment):
    task = toxic_task.delay(comment.text)
    return {"task_id": task.id}


@app.post("/predict_async_linked")
def predict_async_linked(comment: LinkedComment):
    """Submit an async task linked to a comment_id. Returns a task_id (used as cancellation token)."""
    task = toxic_task.delay(comment.text, comment.comment_id)
    return {"task_id": task.id, "comment_id": comment.comment_id}


@app.post("/cancel/{task_id}")
def cancel_task(task_id: str):
    """Soft-cancel a task: revoke it if it's pending or retrying. Do not terminate running tasks."""
    task = celery_app.AsyncResult(task_id)
    if task.state in ["PENDING", "RETRY"]:
        # revoke without terminate (soft cancel)
        task.revoke(terminate=False)
        return {"status": "cancelled", "task_id": task_id}
    else:
        return {"status": "cannot_cancel", "current_state": task.state}


@app.get("/health")
def health():
    return {"status": "healthy"}

@app.get("/task_result/{task_id}")
def get_task_result(task_id: str):
    task = celery_app.AsyncResult(task_id)
    if task.state == "PENDING":
        return {"status": "pending"}
    elif task.state == "FAILURE":
        return {"status": "failure", "error": str(task.result)}
    else:  # SUCCESS
        return {"status": "success", "result": task.result}