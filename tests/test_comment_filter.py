import time
import requests

BASE = "http://localhost:7272/toxic_filter"


def wait_for_service(url, timeout=60):
    start = time.time()
    while time.time() - start < timeout:
        try:
            r = requests.get(url, timeout=5)
            if r.status_code == 200:
                return True
        except requests.RequestException:
            time.sleep(1)
    return False


def test_health():
    assert wait_for_service(BASE + "/health"), "service did not become available"
    r = requests.get(BASE + "/health", timeout=5)
    assert r.status_code == 200
    assert r.json().get("status") == "healthy"


def test_predict_async_linked_and_cancel():
    # Submit a linked async job
    payload = {"text": "this is an integration test", "comment_id": "test-1"}
    r = requests.post(BASE + "/predict_async_linked", json=payload, timeout=10)
    assert r.status_code == 200
    data = r.json()
    assert "task_id" in data
    task_id = data["task_id"]

    # Try to cancel immediately (soft cancel)
    r2 = requests.post(f"{BASE}/cancel/{task_id}", timeout=5)
    assert r2.status_code == 200
    assert r2.json().get("status") in ("cancelled", "cannot_cancel")

    # Poll for a final state for up to 20 seconds
    deadline = time.time() + 20
    final = None
    while time.time() < deadline:
        rr = requests.get(f"{BASE}/task_result/{task_id}", timeout=5)
        assert rr.status_code == 200
        js = rr.json()
        status = js.get("status")
        if status in ("success", "failure", "cancelled"):
            final = js
            break
        time.sleep(1)

    assert final is not None, "task did not reach a terminal state in time"
