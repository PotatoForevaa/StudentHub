Run these integration tests after `docker-compose up --build` completes and the services are running.

Prerequisites
- Docker Compose running the project's stack (start from repository root):

```bash
docker-compose up --build
```

Run tests

```bash
pip install pytest requests
pytest -q tests/test_comment_filter.py
```

Notes
- The tests assume the `toxic_filter` service is reachable at `http://localhost:7272/toxic_filter` as defined in `docker-compose.yml`.
- Tests accept either `success`, `failure`, or `cancelled` as terminal task states so they work even if the model service is unavailable.
