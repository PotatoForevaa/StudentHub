from celery_app import celery_app
import requests
import logging

LOGGER = logging.getLogger("toxic_tasks")

# Hardcoded internal model URL
MODEL_URL = "http://model_service:8001/predict"


@celery_app.task(bind=True)
def toxic_task(self, text: str, comment_id: str | None = None):
    """Call the external model service. Returns a dict containing comment_id and model result."""
    try:
        r = requests.post(MODEL_URL, json={"text": text}, timeout=30)
        r.raise_for_status()
        result = r.json()
    except requests.RequestException as exc:
        LOGGER.exception("Model request failed")
        raise

    return {"comment_id": comment_id, "result": result}