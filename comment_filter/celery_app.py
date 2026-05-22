from celery import Celery

# Hardcoded internal Docker network Redis URLs (per user request)
celery_app = Celery(
    "toxic_tasks",
    broker="redis://redis:6379/0",
    backend="redis://redis:6379/0",
)


celery_app.conf.update(
    task_serializer="json",
    accept_content=["json"],
    result_serializer="json",
    timezone="UTC",
    enable_utc=True,
    task_track_started=True,
    task_time_limit=5 * 60,  # hard limit 5 minutes
    task_soft_time_limit=4 * 60,  # soft limit 4 minutes
    worker_prefetch_multiplier=4,
    worker_max_tasks_per_child=100,
    task_routes={
        "tasks.toxic_task": {"queue": "toxic"},
    },
    task_default_queue="default",
)

# Expose celery_app for AsyncResult, control, etc.