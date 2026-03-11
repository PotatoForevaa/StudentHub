from celery_app import celery_app
import requests

MODEL_URL = "http://model_service:8001/predict"


@celery_app.task
def toxic_task(text: str):

    r = requests.post(MODEL_URL, json={"text": text})

    return r.json()