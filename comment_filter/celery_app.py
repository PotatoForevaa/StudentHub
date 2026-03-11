from celery import Celery

celery_app = Celery(
    "toxic_tasks",
    broker="redis://redis:6379/0",
    backend="redis://redis:6379/0"
)

celery_app.conf.task_routes = {
    "tasks.predict_toxic": {"queue": "toxic"}
}